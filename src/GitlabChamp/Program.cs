using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GitlabChamp.SentryProcessors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;

namespace GitlabChamp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseSentry(o =>
        {
            o.TracesSampleRate = 1.0;
            o.DiagnosticLevel = SentryLevel.Debug;
            o.SampleRate = 1;
            o.AddDiagnosticSourceIntegration();
            o.AddTransactionProcessor(new HealthCheckTransactionFilter());
            o.Debug = true;
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("config.json", true, false)
            .AddEnvironmentVariables()
            .Build();

        var rocketchatUrl = config.GetValue<string>("rocketchat:integration_url");
        var gitlabSecretToken = config.GetValue<string>("gitlab:secret_token");
        if (rocketchatUrl == null) throw new Exception("rocketchat:integration_url is not set");

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient("rocketchat", httpClient =>
        {
            httpClient.BaseAddress = new Uri(rocketchatUrl);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("gitlabChamp");
        });

        // Add health checks
        builder.Services.AddHealthChecks()
            .AddUrlGroup(new Uri(rocketchatUrl), "rocketchat", HealthStatus.Unhealthy);

        var app = builder.Build();
        app.Logger.Log(
            LogLevel.Information,
            $"Starting gitlabChamp version: {typeof(Program).Assembly.GetName().Version}"
        );
        app.UseSentryTracing();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpLogging();

        // Map health check endpoint
        app.MapHealthChecks("/-/health");

        // Map gitlab webhook endpoint
        app.MapPost("/webhook", (HttpContext httpContext, [FromBody] MessageBody messageBody) =>
            {
                if (gitlabSecretToken != null && httpContext.Request.Headers["X-Gitlab-Token"] != gitlabSecretToken)
                {
                    httpContext.Response.StatusCode = 403;
                    return Task.CompletedTask;
                }

                var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>()
                    .CreateClient("rocketchat");
                var body = messageBody.Classify();
                var rchatMessage = new RchatMessage(body);

                var rocketchatMessageResponse = rchatMessage.Send(client);
                var response = new Dictionary<string, object>
                {
                    { "success", rocketchatMessageResponse.IsSuccessStatusCode },
                    { "status", rocketchatMessageResponse.StatusCode },
                    { "body", rocketchatMessageResponse.Content.ReadAsStringAsync().Result }
                };

                return httpContext.Response.WriteAsJsonAsync(response);
            })
            .WithDescription("Gitlab webhook endpoint")
            .WithName("Gitlab webhook")
            .WithOpenApi();

        app.Run();
    }
}
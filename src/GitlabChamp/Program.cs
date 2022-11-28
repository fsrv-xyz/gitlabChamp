using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GitlabChamp.Clients;
using GitlabChamp.Models;
using GitlabChamp.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Prometheus;
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
            o.AddTransactionProcessor(new MetricTransactionFilter());
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

        builder.Services.AddSingleton<IRocketChatClient, RocketChatClient>();
        builder.Services.AddHttpClient<IRocketChatClient, RocketChatClient>(client =>
            {
                client.BaseAddress = new Uri(rocketchatUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("gitlabChamp");
            })
            .UseHttpClientMetrics()
            .AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            );


        // Add health checks
        builder.Services.AddHealthChecks()
            .AddUrlGroup(new Uri(rocketchatUrl), "healthcheck-rocketchat", HealthStatus.Unhealthy)
            .ForwardToPrometheus();

        var app = builder.Build();
        app.Logger.Log(
            LogLevel.Information,
            $"Starting gitlabChamp version: {typeof(Program).Assembly.GetName().Version}"
        );
        app.UseSentryTracing();
        app.UseHttpMetrics();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpLogging();

        // Map health check endpoint
        app.MapHealthChecks("/-/health");
        app.MapMetrics("/-/metrics");

        // Map gitlab webhook endpoint
        app.MapPost("/webhook", (HttpContext httpContext, [FromBody] MessageBody messageBody) =>
            {
                if (gitlabSecretToken != null && httpContext.Request.Headers["X-Gitlab-Token"] != gitlabSecretToken)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Task.CompletedTask;
                }

                var rocketChatClient = httpContext.RequestServices.GetRequiredService<IRocketChatClient>();
                var rocketChatMessageResponse = rocketChatClient.SendMessage(messageBody.ToMessage());
                return httpContext.Response.WriteAsJsonAsync(rocketChatMessageResponse);
            })
            .WithDescription("Gitlab webhook endpoint")
            .WithName("Gitlab webhook")
            .WithOpenApi();

        app.Run();
    }
}
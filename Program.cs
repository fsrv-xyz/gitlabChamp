using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace gitlabChamp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpLogging();

        app.UseAuthorization();

        app.MapPost("/webhook",
                (HttpContext httpContext, [FromBody] MessageBody messageBody) =>
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
                    rchatMessage.Send(client);

                    httpContext.Response.StatusCode = 201;
                    return Task.CompletedTask;
                })
            .WithDescription("Gitlab webhook endpoint")
            .WithName("Gitlab webhook")
            .WithOpenApi();

        app.Run();
    }
}
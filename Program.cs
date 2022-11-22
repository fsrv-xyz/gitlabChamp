using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace gitlabChamp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("config.json", true, false)
            .AddEnvironmentVariables()
            .Build();


        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient("rocketchat", httpClient =>
        {
            var rocketchatUrl = config.GetValue<string>("rocketchat:integration_url");
            if (rocketchatUrl == null) throw new Exception("rocketchat:integration_url is not set");
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

        app.UseAuthorization();

        app.MapPost("/webhook",
                (HttpContext httpContext, [FromBody] MessageBody messageBody) =>
                {
                    var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>()
                        .CreateClient("rocketchat");
                    var body = messageBody.Classify();
                    var rchatMessage = new RchatMessage(body);
                    rchatMessage.Send(client);
                })
            .WithDescription("Gitlab webhook endpoint")
            .WithOpenApi();

        app.Run();
    }
}
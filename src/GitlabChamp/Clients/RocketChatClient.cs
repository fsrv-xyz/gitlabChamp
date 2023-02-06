using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using GitlabChamp.Models;
using Sentry;

namespace GitlabChamp.Clients;

public class RocketChatClient : IRocketChatClient
{
    private readonly HttpClient _httpClient;

    public RocketChatClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public WebhookResponseMessage SendMessage(Message message)
    {
        SentrySdk.AddBreadcrumb(
            category: "RchatMessage",
            message: JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true }),
            type: "debug",
            level: BreadcrumbLevel.Info);
        WebhookResponseMessage result;
        try
        {
            var response = _httpClient.PostAsJsonAsync(string.Empty, message).Result;
            result = new WebhookResponseMessage
            {
                Body = response.Content.ReadAsStringAsync().Result,
                Status = response.StatusCode,
                Success = response.IsSuccessStatusCode
            };
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            result = new WebhookResponseMessage
            {
                Body = e.Message,
                Status = HttpStatusCode.InternalServerError,
                Success = false
            };
        }

        return result;
    }
}

public interface IRocketChatClient
{
    public WebhookResponseMessage SendMessage(Message message);
}
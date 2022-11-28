using System.Net;
using System.Text.Json.Serialization;

namespace GitlabChamp.Models;

public record WebhookResponseMessage
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("status")] public HttpStatusCode Status { get; set; }
    [JsonPropertyName("body")] public string Body { get; set; }
}
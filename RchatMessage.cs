using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sentry;

namespace gitlabChamp;

public class RchatMessage
{
    protected Message Message;


    public RchatMessage(Message message)
    {
        Message = message;
    }

    public void Send(HttpClient client)
    {
        SentrySdk.AddBreadcrumb(
            category: "RchatMessage",
            message: JsonSerializer.Serialize(Message, new JsonSerializerOptions
            {
                WriteIndented = true
            }),
            type: "debug",
            level: BreadcrumbLevel.Info);
        client.PostAsJsonAsync(string.Empty, Message).Result.EnsureSuccessStatusCode();
    }
}

public struct Message
{
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon_url")]
    public string IconUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("attachments")]
    public List<Attachment> Attachments { get; set; }

    public Message()
    {
        Attachments = new List<Attachment>();
    }
}

public struct Attachment
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title_link")]
    public string TitleLink { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("collapsed")] public bool Collapsed { get; set; }

    [JsonPropertyName("fields")] public List<Field> Fields { get; set; }
}

public struct Field
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("short")] public bool Short { get; set; }
}
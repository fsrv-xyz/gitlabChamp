using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace gitlabChamp;

public class RchatMessage
{
    protected Message Message;


    public RchatMessage(Message message)
    {
        Message = message;
    }

    public void Send()
    {
        Console.WriteLine(JsonSerializer.Serialize(Message));
    }
}

public struct Message
{
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;

    [JsonPropertyName("attachments")] public List<Attachment> Attachments { get; set; }

    public Message()
    {
        Attachments = new List<Attachment>();
    }
}

public struct Attachment
{
    [JsonPropertyName("title")] public string Title { get; set; }
    [JsonPropertyName("title_link")] public string TitleLink { get; set; }
    [JsonPropertyName("text")] public string Text { get; set; }
    [JsonPropertyName("color")] public string Color { get; set; }
    [JsonPropertyName("image_url")] public string ImageUrl { get; set; }
}
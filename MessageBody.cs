using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using gitlabChamp.Events;

namespace gitlabChamp;

public class MessageBody
{
    public MessageBody()
    {
        DynamicData = new JsonObject();
    }

    [JsonPropertyName("event_name")]
    [Required]
    public string EventName { get; set; } = null!;

    [JsonExtensionData] [Required] public JsonObject DynamicData { get; set; }

    public Message Classify()
    {
        return Switch().Parse(DynamicData);
    }

    private IEvent Switch()
    {
        switch (EventName)
        {
            case "push":
                return new Push();
            case "tag_push":
                return new TagPush();
            default:
                return new GenericEvent();
        }
    }

    public struct Project
    {
        [JsonPropertyName("path_with_namespace")]
        public string PathWithNamespace { get; set; }

        [JsonPropertyName("homepage")] public string Homepage { get; set; }
    }

    public struct UserDetails
    {
        [JsonPropertyName("user_name")] public string Name { get; set; }
        [JsonPropertyName("user_avatar")] public string Avatar { get; set; }
    }

    public struct Commit
    {
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("message")] public string Message { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
        [JsonExtensionData] public JsonObject DynamicData { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using GitlabChamp.Events;

namespace GitlabChamp;

public class MessageBody
{
    public MessageBody()
    {
        DynamicData = new JsonObject();
    }

    [JsonPropertyName("event_name")] public string EventName { get; set; } = null!;

    [JsonPropertyName("event_type")] public string EventType { get; set; } = null!;

    [JsonExtensionData] [Required] public JsonObject DynamicData { get; set; }

    public Message Classify()
    {
        return Switch().Parse(DynamicData);
    }

    private IEvent Switch()
    {
        switch (string.IsNullOrEmpty(EventName) ? EventType : EventName)
        {
            case "push":
                return new Push();
            case "tag_push":
                return new TagPush();
            case "merge_request":
                return new MergeRequest();
            default:
                return new GenericEvent();
        }
    }
}
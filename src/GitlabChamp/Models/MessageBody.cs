using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using GitlabChamp.Events;

namespace GitlabChamp.Models;

public class MessageBody
{
    public MessageBody()
    {
        DynamicData = new JsonObject();
    }

    [JsonPropertyName("event_name")] public string EventName { get; set; } = null!;

    [JsonPropertyName("event_type")] public string EventType { get; set; } = null!;

    [JsonExtensionData] [Required] public JsonObject DynamicData { get; set; }

    public Message ToMessage()
    {
        return Switch().Parse(DynamicData);
    }

    private IEvent Switch()
    {
        switch (DetermineEventIdentifier())
        {
            case "push":
                return new Push();
            case "tag_push":
                return new TagPush();
            case "merge_request":
                return new MergeRequest();
            default:
                return new GenericEvent
                {
                    Identifier = DetermineEventIdentifier()
                };
        }
    }

    private string DetermineEventIdentifier()
    {
        return string.IsNullOrEmpty(EventName) ? EventType : EventName;
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using gitlabChamp.Events;

namespace gitlabChamp;

public class MessageBody
{
    public MessageBody()
    {
        DynamicData = new Dictionary<string, JsonElement>();
    }

    [JsonPropertyName("event_name")]
    [Required]
    public string EventName { get; set; } = null!;

    [JsonExtensionData] [Required] public Dictionary<string, JsonElement> DynamicData { get; set; }

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
            default:
                return new GenericEvent();
        }
    }
}
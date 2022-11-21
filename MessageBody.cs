using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace gitlabChamp;

public interface IEvent
{
    public IEvent Decode(Dictionary<string, dynamic> data);
}

public class MessageBody
{
    [JsonPropertyName("event_name")]
    [Required]
    public string eventNameRaw { get; set; }

    [JsonExtensionData] public Dictionary<string, dynamic> DynamicData { get; set; }

    public IEvent Classify()
    {
        return Switch().Decode(DynamicData);
    }

    private IEvent Switch()
    {
        switch (eventNameRaw)
        {
            case "push":
                return new PushEvent();
            default:
                return new GenericEvent();
        }
    }
}

public class GenericEvent : IEvent
{
    public IEvent Decode(Dictionary<string, dynamic> data)
    {
        return this;
    }
}

public class PushEvent : IEvent
{
    public IEvent Decode(Dictionary<string, dynamic> data)
    {
        return this;
    }
}
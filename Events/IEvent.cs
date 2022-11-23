using System.Collections.Generic;
using System.Text.Json;

namespace gitlabChamp.Events;

public interface IEvent
{
    public Message Parse(Dictionary<string, JsonElement> data);
}

public class GenericEvent : IEvent
{
    public Message Parse(Dictionary<string, JsonElement> data)
    {
        return new Message
        {
            Text = "Generic Event"
        };
    }
}
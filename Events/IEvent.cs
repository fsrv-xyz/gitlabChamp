using System.Text.Json.Nodes;

namespace gitlabChamp.Events;

public interface IEvent
{
    public Message Parse(JsonObject data);
}

public class GenericEvent : IEvent
{
    public Message Parse(JsonObject data)
    {
        return new Message
        {
            Text = "Generic Event"
        };
    }
}
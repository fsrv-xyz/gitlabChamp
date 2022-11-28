using System.Text.Json.Nodes;
using GitlabChamp.Models;

namespace GitlabChamp.Events;

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
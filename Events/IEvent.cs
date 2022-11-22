using System.Collections.Generic;

namespace gitlabChamp.Events;

public interface IEvent
{
    public Message Parse(Dictionary<string, dynamic> data);
}

public class GenericEvent : IEvent
{
    public Message Parse(Dictionary<string, dynamic> data)
    {
        return new Message { Text = "Generic Event" };
    }
}
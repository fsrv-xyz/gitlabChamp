using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Models;

namespace GitlabChamp.Events;

public interface IEvent
{
    public Message Parse(JsonObject data);
}

public class GenericEvent : IEvent
{
    private readonly string _eventIdentifier;

    public GenericEvent(string eventIdentifier)
    {
        _eventIdentifier = eventIdentifier;
    }

    public Message Parse(JsonObject data)
    {
        var dataString = string.Format(
            "```\n{0}\n```",
            JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions(new JsonSerializerOptions { WriteIndented = true })
            )
        );

        return new Message
        {
            Text = $":gear: Generic Event **{_eventIdentifier}**",
            Attachments = new List<Attachment> { new() { Title = "Data", Text = dataString, Collapsed = true } }
        };
    }
}
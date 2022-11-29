using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Models;

namespace GitlabChamp.Events;

public interface IEvent
{
    public Message Parse(JsonObject data);
}

public sealed class GenericEvent : IEvent
{
    public required string Identifier { init; private get; }

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
            Text = $":gear: Generic Event **{Identifier}**",
            Attachments = new List<Attachment> { new() { Title = "Data", Text = dataString, Collapsed = true } }
        };
    }
}
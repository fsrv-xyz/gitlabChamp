using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace gitlabChamp.Events;

public class Push : IEvent
{
    public Message Parse(JsonObject data)
    {
        var project = data["project"].Deserialize<GitlabHookData.Project>();
        var commits = data["commits"].Deserialize<List<GitlabHookData.Commit>>() ?? new List<GitlabHookData.Commit>();
        var user = data.Deserialize<GitlabHookData.InlineUserDetails>();

        // replace underscores with dashes due to https://github.com/RocketChat/Rocket.Chat/issues/15347
        var projectNameLinkable = project.PathWithNamespace.Replace("_", "-");

        var msg = new Message
        {
            Text = $"Push Event @ [{projectNameLinkable}]({project.Homepage})",
            Username = $"{user.Name} @ gitlab",
            IconUrl = user.Avatar
        };

        // add commits details
        commits.ForEach(commit =>
        {
            var fields = new List<Field>();
            foreach (var group in new List<string> { "added", "modified", "removed" })
            {
                var changes = commit.DynamicData[group].Deserialize<List<string>>();
                if (changes?.Count > 0)
                    fields.Add(new Field
                    {
                        Title = group,
                        Value = string.Join("\n", changes)
                    });
            }

            msg.Attachments.Add(new Attachment
            {
                Title = commit.Title,
                Text = commit.Message,
                TitleLink = commit.Url,
                Collapsed = true,
                Fields = fields
            });
        });

        return msg;
    }
}
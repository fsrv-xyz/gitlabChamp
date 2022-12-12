using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Models;

namespace GitlabChamp.Events;

public sealed class TagPush : IEvent
{
    public Message Parse(JsonObject data)
    {
        var project = data["project"].Deserialize<GitlabHookData.Project>();
        var user = data.Deserialize<GitlabHookData.InlineUserDetails>();
        var commits = data["commits"].Deserialize<List<GitlabHookData.Commit>>() ?? new List<GitlabHookData.Commit>();

        // get tag name
        data.TryGetPropertyValue("ref", out var refName);
        var tagName = refName?.ToString().Split('/').Last();

        // replace underscores with dashes due to https://github.com/RocketChat/Rocket.Chat/issues/15347
        var projectNameLinkable = project.PathWithNamespace.Replace("_", "-");

        var timeline = data.Deserialize<GitlabHookData.BeforeAfter>();
        var titleVerb = timeline.After.Equals("0000000000000000000000000000000000000000") ? "Deleted" : "New";

        var msg = new Message
        {
            Text = $":label: {titleVerb} Tag \"{tagName}\" @ [{projectNameLinkable}]({project.Homepage})",
            Username = $"{user.Name} @ gitlab",
            IconUrl = user.Avatar
        };

        // collect commit statistics
        var stats = new Dictionary<string, uint>();
        commits.ForEach(commit =>
        {
            var author = commit.DynamicData["author"].Deserialize<GitlabHookData.User>();
            if (stats.ContainsKey(author.Name))
                stats[author.Name]++;
            else
                stats.Add(author.Name, 1);
        });

        // add commit statistics to message
        msg.Attachments.Add(new Attachment
        {
            Collapsed = true,
            Title = "Authors",
            Fields = stats.Select(x => new Field
            {
                Title = x.Key,
                Value = x.Value + " commit" + (x.Value > 1 ? "s" : "")
            }).ToList()
        });

        return msg;
    }
}
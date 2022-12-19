using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Models;

namespace GitlabChamp.Events;

public class Issue : IEvent
{
    public Message Parse(JsonObject data)
    {
        var user = data["user"].Deserialize<GitlabHookData.User>();
        var project = data["project"].Deserialize<GitlabHookData.Project>();

        // replace underscores with dashes due to https://github.com/RocketChat/Rocket.Chat/issues/15347
        var projectNameLinkable = project.PathWithNamespace.Replace("_", "-");

        var issueId = data["object_attributes"]?["iid"]?.ToString() ?? "unknown";
        var issueUrl = $"https://gitlab.com/{projectNameLinkable}/-/issues/{issueId}";

        var issueTitleLink = $"[#{issueId}]({issueUrl})";
        var projectTitleLink = $"[{projectNameLinkable}]({project.Homepage})";

        var action = data["object_attributes"]?["action"]?.ToString() ?? "unknown";

        var icon = action switch
        {
            "open" => ":new:",
            "reopen" => ":new:",
            "close" => ":closed_book:",
            "update" => ":pencil:",
            "unknown" => ":question:",
            _ => ":question:"
        };

        return new Message
        {
            Username = $"{user.Name} @ gitlab",
            IconUrl = user.AvatarUrl,
            Text = $"{icon} Issue {action} {issueTitleLink} @ {projectTitleLink}",
            Attachments = new List<Attachment>
            {
                new()
                {
                    Collapsed = true,
                    Title = "Details",
                    Fields = new List<Field>
                    {
                        new()
                        {
                            Title = "Title",
                            Value = data["object_attributes"]?["title"]?.ToString() ?? "No title"
                        },
                        new()
                        {
                            Title = "Description",
                            Value = data["object_attributes"]?["description"]?.ToString() ?? "No description"
                        }
                    }
                }
            }
        };
    }
}
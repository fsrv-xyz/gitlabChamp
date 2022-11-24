using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace gitlabChamp.Events;

public class MergeRequest : IEvent
{
    public Message Parse(JsonObject data)
    {
        var project = data["project"].Deserialize<MessageBody.Project>();
        var user = data["user"].Deserialize<MessageBody.User>();
        var mr = data["object_attributes"].Deserialize<MessageBody.MergeRequestAttributes>();

        var fields = new List<Field>
        {
            new() { Title = "Title", Value = mr.Title },
            new() { Title = "Source", Value = mr.SourceBranch },
            new() { Title = "Target", Value = mr.TargetBranch }
        };

        var verb = mr.Action switch
        {
            "open" => ":mailbox_with_mail: New",
            "close" => ":closed_book: Closed",
            "merge" => ":checkered_flag: Merged",
            "approved" => ":white_check_mark: Approved",
            "unapproved" => ":stop_sign: Unapproved",
            _ => ":arrows_counterclockwise: Updated"
        };

        return new Message
        {
            Text = string.Format(
                @"{0} merge request {1} in {2}",
                verb,
                $"[#{mr.Iid}]({mr.Url})",
                $"[{project.PathWithNamespace.Replace("_", "-")}]({project.Homepage})"
            ),
            Username = $"{user.Name} @ gitlab",
            IconUrl = user.AvatarUrl,
            Attachments = new List<Attachment>
            {
                new()
                {
                    Title = "Details",
                    Collapsed = true,
                    Fields = fields
                }
            }
        };
    }
}
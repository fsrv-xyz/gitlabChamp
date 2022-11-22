using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace gitlabChamp.Events;

public class Push : IEvent
{
    public Message Parse(Dictionary<string, JsonElement> data)
    {
        var msg = new Message();

        data["project"].TryGetProperty("path_with_namespace", out var projectName);
        data.TryGetValue("user_name", out var userName);
        data.TryGetValue("user_avatar", out var userAvatar);

        msg.Text = $"Push Event @ {projectName}";
        msg.Username = $"{userName.ToString()} @ gitlab";
        msg.IconUrl = userAvatar.ToString();


        data["commits"].EnumerateArray().ToList().ForEach(commit =>
        {
            var fields = new List<Field>();
            foreach (var group in new[] { "added", "modified", "removed" }.ToList())
            {
                var changes = commit.GetProperty(group).EnumerateArray().ToList();
                if (changes.Count > 0)
                    fields.Add(new Field
                    {
                        Title = group,
                        Value = string.Join("\n", changes)
                    });
            }

            msg.Attachments.Add(new Attachment
            {
                Title = commit.GetProperty("title").ToString(),
                Text = commit.GetProperty("message").ToString(),
                TitleLink = commit.GetProperty("url").ToString(),
                Collapsed = true,
                Fields = fields
            });
        });

        return msg;
    }
}
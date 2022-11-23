using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace gitlabChamp.Events;

public class TagPush : IEvent
{
    public Message Parse(Dictionary<string, JsonElement> data)
    {
        var msg = new Message();

        data["project"].TryGetProperty("path_with_namespace", out var projectName);
        data["project"].TryGetProperty("homepage", out var projectUrl);
        data.TryGetValue("user_name", out var userName);
        data.TryGetValue("user_avatar", out var userAvatar);
        data.TryGetValue("ref", out var refName);

        var tagName = refName.ToString().Split('/').Last();

        msg.Text = $"New Tag \"{tagName}\" @ [{projectName}]({projectUrl})";
        msg.Username = $"{userName.ToString()} @ gitlab";
        msg.IconUrl = userAvatar.ToString();

        var stats = new Dictionary<string, uint>();
        data["commits"].EnumerateArray().ToList().ForEach(commit =>
        {
            var author = commit.GetProperty("author").GetProperty("name").ToString();
            if (stats.ContainsKey(author))
                stats[author]++;
            else
                stats.Add(author, 1);
        });

        msg.Attachments.Add(new Attachment
        {
            Collapsed = true,
            Title = "Authors",
            Fields = stats.Select(x => new Field
            {
                Title = x.Key,
                Value = x.Value.ToString() + " commit" + (x.Value > 1 ? "s" : "")
            }).ToList()
        });

        return msg;
    }
}
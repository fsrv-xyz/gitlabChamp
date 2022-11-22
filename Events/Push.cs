using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        msg.Username = userName.ToString();
        msg.IconUrl = userAvatar.ToString();
        
        
        data["commits"].EnumerateArray().ToList().ForEach(commit =>
        {
            msg.Attachments.Add(new Attachment
            {
                Title = commit.GetProperty("title").ToString(),
                Text = commit.GetProperty("message").ToString(),
                TitleLink = commit.GetProperty("url").ToString()
            });
        });

        return msg;
    }
}
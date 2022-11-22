using System.Collections.Generic;

namespace gitlabChamp.Events;

public class Push : IEvent
{
    public Message Parse(Dictionary<string, dynamic> data)
    {
        var msg = new Message();
        msg.Text = "Push Event";
        foreach (var point in data)
            msg.Attachments.Add(new Attachment
            {
                Title = point.Key,
                Text = point.Value.ToString()
            });

        return msg;
    }
}
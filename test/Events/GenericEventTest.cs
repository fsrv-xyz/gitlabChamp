using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Events;
using GitlabChamp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test.Events;

[TestClass]
public class GenericEventTest
{
    private readonly IEvent _genericEvent = new GenericEvent { Identifier = "test_event" };

    [TestMethod]
    public void GenericEventParse()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(""" {"foo":"bar"} """);
        Assert.IsNotNull(input);

        var result = _genericEvent.Parse(input);
        var expected = new Message
        {
            Text = ":gear: Generic Event **test_event**",
            Attachments = new List<Attachment>
            {
                new()
                {
                    Title = "Data",
                    Text = "```\n{\n  \"foo\": \"bar\"\n}\n```",
                    Collapsed = true
                }
            }
        };
        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
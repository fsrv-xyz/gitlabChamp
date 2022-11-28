using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Events;
using GitlabChamp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test.Events;

[TestClass]
public class GenericEventTest
{
    private readonly IEvent _genericEvent = new GenericEvent();

    [TestMethod]
    public void GenericEventParse()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(""" {"foo":"bar"} """);
        Assert.IsNotNull(input);

        var result = _genericEvent.Parse(input);
        var expected = new Message { Text = "Generic Event" };

        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
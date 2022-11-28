using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Events;
using GitlabChamp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test.Events;

[TestClass]
public class PushEventTest
{
    private readonly Push _pushEvent = new();

    [TestMethod]
    public void PushEventParse()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(
            File.ReadAllText("../../../testdata/push_example_input.json"));
        var expected = JsonSerializer.Deserialize<Message>(
            File.ReadAllText("../../../testdata/push_example_output.json"));

        Assert.IsNotNull(input);
        Assert.IsNotNull(expected);

        var result = _pushEvent.Parse(input);
        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
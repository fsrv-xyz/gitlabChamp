using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp;
using GitlabChamp.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test.Events;

[TestClass]
public class TagPushEventTest
{
    private readonly TagPush _tagPushEvent = new();

    [TestMethod]
    public void TagPushEventParse()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(
            File.ReadAllText("../../../testdata/tagPush_example_input.json"));
        var expected = JsonSerializer.Deserialize<Message>(
            File.ReadAllText("../../../testdata/tagPush_example_output.json"));

        Assert.IsNotNull(input);
        Assert.IsNotNull(expected);

        var result = _tagPushEvent.Parse(input);
        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
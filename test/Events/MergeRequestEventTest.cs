using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp;
using GitlabChamp.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test.Events;

[TestClass]
public class MergeRequestEventTest
{
    private readonly MergeRequest _mergeRequestEvent = new();

    [TestMethod]
    public void MergeRequestEventParse()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(
            File.ReadAllText("../../../testdata/mergeRequest_example_input.json"));
        var expected = JsonSerializer.Deserialize<Message>(
            File.ReadAllText("../../../testdata/mergeRequest_example_output.json"));

        Assert.IsNotNull(input);
        Assert.IsNotNull(expected);

        var result = _mergeRequestEvent.Parse(input);

        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
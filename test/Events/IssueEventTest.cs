using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using GitlabChamp.Events;
using GitlabChamp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test.Events;

[TestClass]
public class IssueEventTest
{
    private readonly Issue _issueEvent = new();

    [TestMethod]
    public void PushEventParseWithDescription()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(
            File.ReadAllText("../../../testdata/issue_example_input_with_description.json"));
        var expected = JsonSerializer.Deserialize<Message>(
            File.ReadAllText("../../../testdata/issue_example_output_with_description.json"));

        Assert.IsNotNull(input);
        Assert.IsNotNull(expected);

        var result = _issueEvent.Parse(input);
        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }

    [TestMethod]
    public void PushEventParseEmptyDescription()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(
            File.ReadAllText("../../../testdata/issue_example_input_empty_description.json"));
        var expected = JsonSerializer.Deserialize<Message>(
            File.ReadAllText("../../../testdata/issue_example_output_empty_description.json"));

        Assert.IsNotNull(input);
        Assert.IsNotNull(expected);

        var result = _issueEvent.Parse(input);
        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
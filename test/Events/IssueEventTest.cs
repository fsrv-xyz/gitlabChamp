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
    public void PushEventParse()
    {
        var input = JsonSerializer.Deserialize<JsonObject>(
            File.ReadAllText("../../../testdata/issue_example_input.json"));
        var expected = JsonSerializer.Deserialize<Message>(
            File.ReadAllText("../../../testdata/issue_example_output.json"));

        Assert.IsNotNull(input);
        Assert.IsNotNull(expected);

        var result = _issueEvent.Parse(input);
        Assert.IsNotNull(result);
        Assert.AreEqual(JsonSerializer.Serialize(result), JsonSerializer.Serialize(expected));
    }
}
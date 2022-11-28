using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using GitlabChamp.Events;
using GitlabChamp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace test;

[TestClass]
public class MessageBodyTest
{
    [TestMethod]
    public void TestSwitch()
    {
        var dict = new Dictionary<string, IEvent>
        {
            { "push", new Push() },
            { "mergeRequest", new MergeRequest() },
            { "tagPush", new TagPush() }
        };

        dict.ToList().ForEach(x =>
            {
                var input = JsonSerializer.Deserialize<MessageBody>(
                    File.ReadAllText($"../../../testdata/{x.Key}_example_input.json")
                );

                var method = typeof(MessageBody).GetMethod("Switch", BindingFlags.NonPublic | BindingFlags.Instance);
                var res = method?.Invoke(input, null);

                Assert.AreEqual(x.Value.GetType(), res?.GetType());
            }
        );
    }
}
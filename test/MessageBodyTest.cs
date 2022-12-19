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
    public void TestDetermineEventIdentifier()
    {
        var assertions = new Dictionary<MessageBody, string>
        {
            {
                new MessageBody { EventName = "test1" },
                "test1"
            },
            {
                new MessageBody { EventType = "test2" },
                "test2"
            },
            {
                new MessageBody { ObjectKind = "test3" },
                "test3"
            },
            {
                new MessageBody { EventName = "test1", EventType = "test2", ObjectKind = "test3" },
                "test1"
            },
            {
                new MessageBody { EventName = "", EventType = "test2", ObjectKind = "test3" },
                "test2"
            },
            {
                new MessageBody { EventName = "", EventType = "", ObjectKind = "test3" },
                "test3"
            }
        };

        foreach (var (input, expected) in assertions)
        {
            var method = input
                .GetType()
                .GetMethod("DetermineEventIdentifier", BindingFlags.NonPublic | BindingFlags.Instance);

            var result = method?.Invoke(input, null);

            Assert.AreEqual(expected, result);
        }
    }

    [TestMethod]
    public void TestSwitch()
    {
        var dict = new Dictionary<string, IEvent>
        {
            { "push", new Push() },
            { "mergeRequest", new MergeRequest() },
            { "tagPush", new TagPush() },
            { "issue", new Issue() }
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
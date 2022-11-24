using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using gitlabChamp.Events;

namespace gitlabChamp;

public class MessageBody
{
    public MessageBody()
    {
        DynamicData = new JsonObject();
    }

    [JsonPropertyName("event_name")] public string EventName { get; set; } = null!;

    [JsonPropertyName("event_type")] public string EventType { get; set; } = null!;

    [JsonExtensionData] [Required] public JsonObject DynamicData { get; set; }

    public Message Classify()
    {
        return Switch().Parse(DynamicData);
    }

    private IEvent Switch()
    {
        var blb = string.IsNullOrEmpty(EventName) ? EventType : EventName;
        switch (blb)
        {
            case "push":
                return new Push();
            case "tag_push":
                return new TagPush();
            case "merge_request":
                return new MergeRequest();
            default:
                return new GenericEvent();
        }
    }

    public struct Project
    {
        [JsonPropertyName("path_with_namespace")]
        public string PathWithNamespace { get; set; }

        [JsonPropertyName("homepage")] public string Homepage { get; set; }
    }

    public struct InlineUserDetails
    {
        [JsonPropertyName("user_name")] public string Name { get; set; }
        [JsonPropertyName("user_avatar")] public string Avatar { get; set; }
    }

    public struct User
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("email")] public string Email { get; set; }
        [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; }
    }

    public struct Commit
    {
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("message")] public string Message { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
        [JsonExtensionData] public JsonObject DynamicData { get; set; }
    }

    public struct MergeRequestAttributes
    {
        [JsonPropertyName("url")] public string Url { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("source_branch")] public string SourceBranch { get; set; }
        [JsonPropertyName("target_branch")] public string TargetBranch { get; set; }

        [JsonPropertyName("iid")] public int Iid { get; set; }
        [JsonPropertyName("action")] public string Action { get; set; }
    }
}
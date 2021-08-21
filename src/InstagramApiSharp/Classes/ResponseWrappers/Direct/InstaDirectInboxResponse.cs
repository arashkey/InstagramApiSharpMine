﻿using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaDirectInboxResponse : InstaDirectInboxThreadContainerResponse
    {
        [JsonProperty("has_older")] public bool HasOlder { get; set; }

        [JsonProperty("unseen_count_ts")] public long UnseenCountTs { get; set; }

        [JsonProperty("unseen_count")] public long UnseenCount { get; set; }

        [JsonProperty("oldest_cursor")] public string OldestCursor { get; set; }

        [JsonProperty("blended_inbox_enabled")] public bool BlendedInboxEnabled { get; set; }
    }
    public class InstaDirectInboxThreadContainerResponse
    {
        [JsonProperty("threads")] public List<InstaDirectInboxThreadResponse> Threads { get; set; }
    }
    public class InstaDirectInboxThreadSingleContainerResponse : InstaDefaultResponse
    {
        [JsonProperty("thread")] public InstaDirectInboxThreadResponse Thread { get; set; }
    }
}
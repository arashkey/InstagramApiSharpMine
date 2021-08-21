﻿using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaHashtagSearchResponse : BaseStatusResponse
    {
        [JsonIgnore] public List<InstaHashtagResponse> Tags { get; set; } = new List<InstaHashtagResponse>();

        [JsonProperty("has_more")] public bool? MoreAvailable { get; set; }

        [JsonProperty("rank_token")] public string RankToken { get; set; }
    }
}
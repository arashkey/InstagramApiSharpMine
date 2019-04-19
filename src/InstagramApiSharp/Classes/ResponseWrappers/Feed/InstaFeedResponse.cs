using System.Collections.Generic;
using System.ComponentModel;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using InstagramApiSharp.Enums;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaFeedResponse : BaseLoadableResponse
    {
        [JsonProperty("is_direct_v2_enabled")] public bool IsDirectV2Enabled { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        public List<InstaMediaItemResponse> Items { get; set; } = new List<InstaMediaItemResponse>();

        //[JsonProperty("suggested_users")]
        [JsonIgnore]
        public List<InstaSuggestionItemResponse> SuggestedUsers { get; set; } = new List<InstaSuggestionItemResponse>();

        [JsonIgnore]
        public List<InstaPostResponse> Posts { get; set; } = new List<InstaPostResponse>();
    }
    public class InstaPostResponse 
    {
        public InstaFeedsType Type { get; set; }
        public InstaMediaItemResponse Media { get; set; }
        public List<InstaStoryResponse> Stories { get; set; } = new List<InstaStoryResponse>();
        public List<InstaSuggestionItemResponse> SuggestedUserItems { get; set; } = new List<InstaSuggestionItemResponse>();
        public InstaAllCatchedUp EndOfFeedDemarcator { get; set; }
        public List<InstaHashtagMedia> Hashtags { get; set; } = new List<InstaHashtagMedia>();

    }
}
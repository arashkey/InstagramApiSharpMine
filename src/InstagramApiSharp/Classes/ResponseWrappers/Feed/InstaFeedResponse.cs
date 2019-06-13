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
        [JsonProperty("view_state_version")] public string ViewStateVersion { get; set; }
        [JsonProperty("feed_pill_text")] public string FeedPillText { get; set; }
        [JsonProperty("client_session_id")] public string ClientSessionId { get; set; }
        [JsonProperty("client_feed_changelist_applied")] public bool? ClientFeedChangelistApplied { get; set; }
    }
    public class InstaPostResponse 
    {
        public InstaFeedsType Type { get; set; }
        public InstaMediaItemResponse Media { get; set; }
        public List<InstaReelFeedResponse> Stories { get; set; } = new List<InstaReelFeedResponse>();
        //public List<InstaStoryResponse> Stories { get; set; } = new List<InstaStoryResponse>();
        public List<InstaSuggestionItemResponse> SuggestedUserItems { get; set; } = new List<InstaSuggestionItemResponse>();
        public List<InstaSuggestionItemResponse> SuggestedUserCardsItems { get; set; } = new List<InstaSuggestionItemResponse>();
        public InstaAllCatchedUp EndOfFeedDemarcator { get; set; }
        public List<InstaHashtagMedia> Hashtags { get; set; } = new List<InstaHashtagMedia>();
        public InstaStoriesNetego StoriesNetego { get; set; }
    }
}
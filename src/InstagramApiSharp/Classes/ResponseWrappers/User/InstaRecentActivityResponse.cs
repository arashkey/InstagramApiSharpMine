using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaRecentActivityResponse : BaseLoadableResponse
    {
        public bool IsOwnActivity { get; set; } = false;

        [JsonProperty("stories")]
        public List<InstaRecentActivityFeedResponse> Stories { get; set; }
            = new List<InstaRecentActivityFeedResponse>();

        [JsonIgnore] public InstaSuggestionItemListResponse SuggestedItems { get; set; } = new InstaSuggestionItemListResponse();
        [JsonIgnore] public InstaActivityCountResponse Counts { get; set; } = new InstaActivityCountResponse();
    }

    public class InstaActivityCountResponse
    {
        [JsonProperty("usertags")]
        public int? Usertags { get; set; }
        [JsonProperty("comments")]
        public int? Comments { get; set; }
        [JsonProperty("comment_likes")]
        public int? CommentLikes { get; set; }
        [JsonProperty("relationships")]
        public int? Relationships { get; set; }
        [JsonProperty("likes")]
        public int? Likes { get; set; }
        [JsonProperty("campaign_notification")]
        public int? CampaignNotification { get; set; }
        [JsonProperty("shopping_notification")]
        public int? ShoppingNotification { get; set; }
        [JsonProperty("photos_of_you")]
        public int? PhotosOfYou { get; set; }
        [JsonProperty("requests")]
        public int? Requests { get; set; }
    }
}
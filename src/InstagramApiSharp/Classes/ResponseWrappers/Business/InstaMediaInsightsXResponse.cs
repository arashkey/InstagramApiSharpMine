/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaMediaInsightsContainerResponse
    {
        [JsonProperty("data")]
        public InstaMediaInsightsDataResponse Data { get; set; }
    }

    public class InstaMediaInsightsDataResponse
    {
        [JsonProperty("instagram_post_by_igid")]
        public InstaMediaInsightsXResponse InstagramPostByIgid { get; set; }
    }

    public class InstaMediaInsightsXResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("creation_time")]
        public long? CreationTime { get; set; }
        [JsonProperty("has_product_tags")]
        public bool HasProductTags { get; set; }
        [JsonProperty("instagram_media_id")]
        public string InstagramMediaId { get; set; }
        [JsonProperty("instagram_media_owner_id")]
        public string InstagramMediaOwnerId { get; set; }
        [JsonProperty("instagram_actor")]
        public InstaInsightActorResponse InstagramActor { get; set; }
        [JsonProperty("inline_insights_node")]
        public InstaInlineInsightsNodeResponse InlineInsightsNode { get; set; }
        [JsonProperty("display_url")]
        public string DisplayUrl { get; set; }
        [JsonProperty("instagram_media_type")]
        public string InstagramMediaType { get; set; }
        [JsonProperty("image")]
        public InstaInsightImageResponse Image { get; set; }
        [JsonProperty("comment_count")]
        public int? CommentCount { get; set; }
        [JsonProperty("like_count")]
        public int? LikeCount { get; set; }
        [JsonProperty("save_count")]
        public int? SaveCount { get; set; }
        //[JsonProperty("ad_media")]
        //public object ad_media { get; set; }
        [JsonProperty("organic_instagram_media_id")]
        public string OrganicInstagramMediaId { get; set; }
        [JsonProperty("shopping_outbound_click_count")]
        public int? ShoppingOutboundClickCount { get; set; }
        [JsonProperty("shopping_product_click_count")]
        public int? ShoppingProductClickCount { get; set; }
        //[JsonProperty("shopping_product_insights")]
        //public Shopping_Product_Insights shopping_product_insights { get; set; }





        // story
        //public object share_count { get; set; }
        //public int impression_count { get; set; }
        //public int reach_count { get; set; }
        [JsonProperty("story_link_navigation_count")]
        public int? StoryLinkNavigationCount { get; set; }
        [JsonProperty("story_replies_count")]
        public int? StoryRepliesCount { get; set; }
        //[JsonProperty("tags_insights")] // No data! comment it for now
        //public InstaInsightsStoryTagsInsightsResponse TagsInsights { get; set; }
        [JsonProperty("story_exits_count")]
        public int? StoryExitsCount { get; set; }
        [JsonProperty("story_swipe_away_count")]
        public int? StorySwipeAwayCount { get; set; }
        //[JsonProperty("owner_account_follows_count")]
        //public object owner_account_follows_count { get; set; }


        [JsonProperty("taps_back_count")]
        public int? TapsBackCount { get; set; }
        [JsonProperty("taps_forward_count")]
        public int? TapsForwardCount { get; set; }
    }

    public class InstaInsightActorResponse
    {
        [JsonProperty("instagram_actor_id")]
        public string InstagramActorId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class InstaInsightImageResponse
    {
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
    }

    //public class Shopping_Product_Insights
    //{
    //    [JsonProperty("")]
    //    public object[] shopping_product_by_tag_click_count { get; set; }
    //    [JsonProperty("")]
    //    public object[] shopping_product_by_tag_outbound_click_count { get; set; }
    //}

    // inline_insights_node

    public class InstaInlineInsightsNodeResponse
    {
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("metrics")]
        public InstaInsightsMetricResponse Metrics { get; set; }
        [JsonProperty("error")]
        public object Error { get; set; }
    }

    public class InstaInsightsMetricResponse
    {
        [JsonProperty("share_count")]
        public InstaInsightsShareCountResponse ShareCount { get; set; }
        [JsonProperty("owner_profile_views_count")]
        public int? OwnerProfileViewsCount { get; set; }
        [JsonProperty("reach_count")]
        public int? ReachCount { get; set; }
        [JsonProperty("profile_actions")]
        public InstaInsightsProfileActionsResponse ProfileActions { get; set; }
        [JsonProperty("impression_count")]
        public int? ImpressionCount { get; set; }
        [JsonProperty("impressions")]
        public InstaInsightsImpressionsResponse Impressions { get; set; }
        [JsonProperty("owner_account_follows_count")]
        public int? OwnerAccountFollowsCount { get; set; }
        [JsonProperty("reach")]
        public InstaInsightsReachResponse Reach { get; set; }
        [JsonProperty("hashtags_impressions")]
        public InstaInsightsHashtagsImpressionsResponse HashtagsImpressions { get; set; }

    }
    //public class InstaInsightsStoryTagsInsightsResponse
    //{
    //    [JsonProperty("mentions")]
    //    public object[] mentions { get; set; }
    //    [JsonProperty("locations")]
    //    public object[] locations { get; set; }
    //    [JsonProperty("hashtags")]
    //    public object[] hashtags { get; set; }
    //    [JsonProperty("product_items")]
    //    public object[] product_items { get; set; }
    //}


    public class InstaInsightsShareCountResponse
    {
        [JsonProperty("tray")]
        public InstaInsightsShareCountItemResponse Tray { get; set; }
        [JsonProperty("post")]
        public InstaInsightsShareCountItemResponse Post { get; set; }
        [JsonProperty("shares")]
        public InstaInsightsShareCountItemResponse Shares { get; set; }
    }

    public class InstaInsightsShareCountItemResponse
    {
        [JsonProperty("nodes")]
        public InstaInsightNodeItemResponse[] Nodes { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class InstaInsightsProfileActionsResponse
    {
        [JsonProperty("actions")]
        public InstaInsightsShareCountItemResponse Actions { get; set; }
    }

    public class InstaInsightsImpressionsResponse
    {
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("surfaces")]
        public InstaInsightsShareCountItemResponse Data { get; set; }
    }

    public class InstaInsightsReachResponse
    {
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("follow_status")]
        public InstaInsightsShareCountItemResponse FollowStatus { get; set; }
    }

    public class InstaInsightsHashtagsImpressionsResponse
    {
        [JsonProperty("organic")]
        public InstaInsightsShareCountItemResponse Organic { get; set; }
        [JsonProperty("hashtags")]
        public InstaInsightsShareCountItemResponse Hashtags { get; set; }
    }
    public class InstaInsightNodeItemResponse
    {
        [JsonProperty("__typename")]
        public string TypeName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("organic")]
        public InstaInsightNodeItemResponse Organic { get; set; }
        //[JsonProperty("strong_id__")]
        //public object StrongId { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using System;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaMediaInsightsX
    {
        public string Id { get; set; }
        public DateTime CreationTime { get; set; }
        public long CreationTimeUnix { get; set; }
        public bool HasProductTags { get; set; }
        public string InstagramMediaId { get; set; }
        public string InstagramMediaOwnerId { get; set; }
        public InstaInsightActor InstagramActor { get; set; }
        public string DisplayUrl { get; set; }
        public string InstagramMediaType { get; set; }
        public InstaInsightImage Image { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
        public int SaveCount { get; set; }
        public int ShareCount { get; set; }
        public string OrganicInstagramMediaId { get; set; }
        public int ShoppingOutboundClickCount { get; set; }
        public int ShoppingProductClickCount { get; set; }


        // added
        public InstaInsightsMetric Metrics { get; set; } = new InstaInsightsMetric();


        // story
        //public object share_count { get; set; } >>> shares
        //public int impression_count { get; set; }
        //public int reach_count { get; set; }
        public int StoryLinkNavigationCount { get; set; }
        public int StoryRepliesCount { get; set; }
        public int StoryExitsCount { get; set; }
        public int StorySwipeAwayCount { get; set; }
        public int TapsBackCount { get; set; }
        public int TapsForwardCount { get; set; }
    }

    public class InstaInsightActor
    {
        public string InstagramActorId { get; set; }
        public string Id { get; set; }
    }

    public class InstaInsightImage
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class InstaInsightsMetric
    {
        public InstaInsightsShareCount ShareCount { get; set; }
        public int OwnerProfileViewsCount { get; set; }
        public int ReachCount { get; set; }
        public int ProfileActionsCount { get; set; }
        public int ImpressionCount { get; set; }
        public int OwnerAccountFollowsCount { get; set; }
        public InstaInsightsImpressions ProfileActions { get; set; }
        public InstaInsightsImpressions ImpressionsSurfaces { get; set; }
        //public object OwnerAccountFollowsCount { get; set; }
        public InstaInsightsImpressions ReachFollowStatus { get; set; }
        public InstaInsightsHashtagsImpressions HashtagsImpressions { get; set; }
    }

    public class InstaInsightsShareCount
    {
        public InstaInsightsShareCountItem Tray { get; set; }
        public InstaInsightsShareCountItem Post { get; set; }
        public InstaInsightsShareCountItem Shares { get; set; }
    }

    public class InstaInsightsShareCountItem
    {
        public List<InstaInsightNodeItem> Nodes { get; set; } = new List<InstaInsightNodeItem>();
        public int Value { get; set; }
        public string Status { get; set; }
    }

    public class InstaInsightsImpressions
    {
        public int Value { get; set; }
        public InstaInsightsShareCountItem Data { get; set; } = new InstaInsightsShareCountItem();
    }


    public class InstaInsightsHashtagsImpressions
    {
        public InstaInsightsShareCountItem Organic { get; set; }
        public InstaInsightsShareCountItem Hashtags { get; set; }
    }

    public class InstaInsightNodeItem
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public InstaInsightNodeItem Organic { get; set; }
    }
}

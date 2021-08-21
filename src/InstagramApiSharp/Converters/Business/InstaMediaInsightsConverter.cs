/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;


namespace InstagramApiSharp.Converters.Business
{
    internal class InstaMediaInsightsConverter : IObjectConverter<InstaMediaInsightsX, InstaMediaInsightsXResponse>
    {
        public InstaMediaInsightsXResponse SourceObject { get; set; }

        public InstaMediaInsightsX Convert()
        {
            var mediaInsightsX = new InstaMediaInsightsX
            {
                Id = SourceObject.Id,
                CommentCount = SourceObject.CommentCount ?? 0,
                SaveCount = SourceObject.SaveCount ?? 0,
                ShoppingOutboundClickCount = SourceObject.ShoppingOutboundClickCount ?? 0,
                ShoppingProductClickCount = SourceObject.ShoppingProductClickCount ?? 0,
                StoryExitsCount = SourceObject.StoryExitsCount ?? 0,
                StoryLinkNavigationCount = SourceObject.StoryLinkNavigationCount ?? 0,
                StoryRepliesCount = SourceObject.StoryRepliesCount ?? 0,
                StorySwipeAwayCount = SourceObject.StorySwipeAwayCount ?? 0,
                //CreationTime = ,
                CreationTimeUnix = SourceObject.CreationTime ?? 0,
                DisplayUrl = SourceObject.DisplayUrl,
                HasProductTags = SourceObject.HasProductTags,
                InstagramMediaId = SourceObject.InstagramMediaId,
                InstagramMediaOwnerId = SourceObject.InstagramMediaOwnerId,
                InstagramMediaType = SourceObject.InstagramMediaType,
                LikeCount = SourceObject.LikeCount ?? 0,
                OrganicInstagramMediaId = SourceObject.OrganicInstagramMediaId,
                TapsBackCount = SourceObject.TapsBackCount ?? 0,
                TapsForwardCount = SourceObject.TapsForwardCount ?? 0,
            };
            if (SourceObject.CreationTime != null)
                mediaInsightsX.CreationTime = SourceObject.CreationTime.Value.FromUnixTimeSeconds();
            if (SourceObject.Image != null)
                mediaInsightsX.Image = new InstaInsightImage { Width = SourceObject.Image.Width, Height = SourceObject.Image.Height };
            if (SourceObject.InstagramActor != null)
                mediaInsightsX.InstagramActor = new InstaInsightActor { Id = SourceObject.InstagramActor.Id, InstagramActorId = SourceObject.InstagramActor.InstagramActorId };

            if (SourceObject.InlineInsightsNode?.Metrics != null)
            {
                var source = SourceObject.InlineInsightsNode.Metrics;

                var metric = new InstaInsightsMetric
                {
                    OwnerProfileViewsCount = source.OwnerProfileViewsCount ?? 0,
                    ReachCount = source.ReachCount ?? 0,
                    ImpressionCount = source.ImpressionCount ?? 0,
                    OwnerAccountFollowsCount = source.OwnerAccountFollowsCount ?? 0
                };

                InstaInsightNodeItem ConvertToNodeItem(InstaInsightNodeItemResponse response)
                {
                    var nodeItem = new InstaInsightNodeItem
                    {
                        Name = response.Name,
                        TypeName = response.TypeName,
                        Value = response.Value
                    };

                    if (response.Organic != null)
                        nodeItem.Organic = ConvertToNodeItem(response.Organic);
                    return nodeItem;
                }
                InstaInsightsShareCountItem ConvertToShareCountItem(InstaInsightsShareCountItemResponse response)
                {
                    var shareCount = new InstaInsightsShareCountItem
                    {
                        Status = response.Status,
                        Value = response.Value
                    };
                    if (response.Nodes?.Length > 0)
                    {
                        foreach (var item in response.Nodes)
                        {
                            try
                            {
                                shareCount.Nodes.Add(ConvertToNodeItem(item));
                            }
                            catch { }
                        }
                    }
                    return shareCount;
                }
                InstaInsightsImpressions ConvertToImpression(InstaInsightsImpressionsResponse response)
                {
                    return new InstaInsightsImpressions
                    {
                        Value = response.Value,
                        Data = ConvertToShareCountItem(response.Data)
                    };
                }
                InstaInsightsImpressions ConvertToImpressionProfile(InstaInsightsProfileActionsResponse response)
                {
                    return new InstaInsightsImpressions
                    {
                        Data = ConvertToShareCountItem(response.Actions)
                    };
                }
                InstaInsightsImpressions ConvertToImpressionReach(InstaInsightsReachResponse response)
                {
                    return new InstaInsightsImpressions
                    {
                        Data = ConvertToShareCountItem(response.FollowStatus)
                    };
                }
                InstaInsightsHashtagsImpressions ConvertToImpressionHashtags(InstaInsightsHashtagsImpressionsResponse response)
                {
                    var impress = new InstaInsightsHashtagsImpressions();
                    if (response.Hashtags != null)
                    {
                        impress.Hashtags = ConvertToShareCountItem(response.Hashtags);
                        impress.Hashtags.Value = response.Hashtags.Count;
                    }
                    if (response.Organic != null)
                        impress.Organic = ConvertToShareCountItem(response.Organic);
                    return impress;
                }
                InstaInsightsShareCount ConvertToShareCount(InstaInsightsShareCountResponse response)
                {
                    var shareCount = new InstaInsightsShareCount();
                    if (response.Post != null)
                        shareCount.Post = ConvertToShareCountItem(response.Post);
                    if (response.Shares != null)
                        shareCount.Shares = ConvertToShareCountItem(response.Shares);
                    if (response.Tray != null)
                        shareCount.Tray = ConvertToShareCountItem(response.Tray);
                    return shareCount;
                }

                if (source.ProfileActions != null)
                {
                    var profileCount = metric.OwnerProfileViewsCount;
                    var profileActions = ConvertToImpressionProfile(source.ProfileActions);
                    if (profileActions != null)
                        profileCount += profileActions.Data.Value;
                    metric.ProfileActionsCount = profileCount;
                    metric.ProfileActions = profileActions;
                }
                if (source.Reach != null)
                    metric.ReachFollowStatus = ConvertToImpressionReach(source.Reach);
                if (source.Impressions != null)
                    metric.ImpressionsSurfaces = ConvertToImpression(source.Impressions);
                if (source.HashtagsImpressions != null)
                    metric.HashtagsImpressions = ConvertToImpressionHashtags(source.HashtagsImpressions);
                if (source.ShareCount != null)
                {
                    try
                    {
                        metric.ShareCount = ConvertToShareCount(source.ShareCount);
                        var shareCount = 0;
                        if (metric.ShareCount?.Tray?.Nodes?.Count > 0)
                            shareCount = metric.ShareCount.Tray.Nodes[0].Value;
                        else if (metric.ShareCount?.Shares?.Nodes?.Count > 0)
                            shareCount = metric.ShareCount.Shares.Nodes[0].Value;
                        mediaInsightsX.ShareCount = shareCount;
                    }
                    catch { }
                }

                mediaInsightsX.Metrics = metric;
            }

            return mediaInsightsX;
        }
    }
}

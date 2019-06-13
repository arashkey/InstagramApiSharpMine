using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.Converters
{
    internal class InstaFeedConverter : IObjectConverter<InstaFeed, InstaFeedResponse>
    {
        public InstaFeedResponse SourceObject { get; set; }

        public InstaFeed Convert()
        {
            if (SourceObject?.Items == null)
                throw new ArgumentNullException("InstaFeedResponse or its Items");
            var feed = new InstaFeed
            {
                MoreAvailable = SourceObject.MoreAvailable,
                NextMaxId = SourceObject.NextMaxId,
                ViewStateVersion = SourceObject.ViewStateVersion,
                FeedPillText = SourceObject.FeedPillText,
                ClientSessionId = SourceObject.ClientSessionId,
                ClientFeedChangelistApplied = SourceObject.ClientFeedChangelistApplied ?? false
            };
            if (SourceObject.Items?.Count > 0)
                foreach (var instaUserFeedItemResponse in SourceObject.Items)
                {
                    if (instaUserFeedItemResponse?.Type != 0) continue;
                    var feedItem = ConvertersFabric.Instance.GetSingleMediaConverter(instaUserFeedItemResponse).Convert();
                    feed.Medias.Add(feedItem);
                }
            if (SourceObject.SuggestedUsers?.Count > 0)
                foreach (var suggestedItemResponse in SourceObject.SuggestedUsers)
                {
                    try
                    {
                        var suggestedItem = ConvertersFabric.Instance.GetSuggestionItemConverter(suggestedItemResponse).Convert();
                        feed.SuggestedUserItems.Add(suggestedItem);
                    }
                    catch { }
                }
            if (SourceObject.Posts?.Count >0)
            {
                foreach (var item in SourceObject.Posts)
                {
                    try
                    {
                        var post = new InstaPost
                        {
                            Type = item.Type
                        };
                        switch (item.Type)
                        {
                            case InstaFeedsType.EndOfFeedDemarcator:
                                if (item.EndOfFeedDemarcator != null)
                                    post.EndOfFeedDemarcator = item.EndOfFeedDemarcator;
                                break;
                            case InstaFeedsType.SuggestedUsers:
                                foreach (var user in item.SuggestedUserItems)
                                    try
                                    {
                                        if (user != null)
                                            post.SuggestedUserItems.Add(ConvertersFabric.Instance.GetSuggestionItemConverter(user).Convert());
                                    }
                                    catch { }
                                break;
                            case InstaFeedsType.Media:
                                //default:
                                if (item.Media != null)
                                    post.Media = ConvertersFabric.Instance.GetSingleMediaConverter(item.Media).Convert();
                                break;
                            case InstaFeedsType.Hashtag:
                                foreach (var hashtag in item.Hashtags)
                                    try
                                    {
                                        if (hashtag != null)
                                            post.Hashtags.Add(hashtag);
                                    }
                                    catch { }
                                break;
                            case InstaFeedsType.StoriesNetego:
                                post.StoriesNetego = item.StoriesNetego;
                                break;
                            case InstaFeedsType.SuggestedUsersCard:
                                foreach (var user in item.SuggestedUserCardsItems)
                                    try
                                    {
                                        if (user != null)
                                            post.SuggestedUserCardsItems.Add(ConvertersFabric.Instance.GetSuggestionItemConverter(user).Convert());
                                    }
                                    catch { }
                                break;
                        }
                        feed.Posts.Add(post);
                    }
                    catch { }
                }
            }
            return feed;
        }
    }
}
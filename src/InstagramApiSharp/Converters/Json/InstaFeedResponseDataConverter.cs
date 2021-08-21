using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
namespace InstagramApiSharp.Converters.Json
{
    internal class InstaFeedResponseDataConverter : JsonConverter
    {
        private readonly bool RemoveAds = false;
        private readonly long CurrentUserId = 0;
        public InstaFeedResponseDataConverter(long userId, bool removeAds = false) : base()
        {
            CurrentUserId = userId;
            RemoveAds = removeAds;
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(InstaFeedResponse);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var feed = token.ToObject<InstaFeedResponse>();
            var items = token["feed_items"];
            if (items != null)
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var item = items.ElementAt(i);
                    if (item["media_or_ad"] != null) //media_or_ad
                    {
                        var mediaOrAd = item["media_or_ad"];
                        if (mediaOrAd == null) continue;
                        var media = mediaOrAd.ToObject<InstaMediaItemResponse>();
                        if (media.User.FriendshipStatus != null && RemoveAds && media.User.Pk != CurrentUserId)
                        {
                            if (media.User.FriendshipStatus.Following)
                            {
                                feed.Items.Add(media);
                                feed.Posts.Add(new InstaPostResponse
                                {
                                    Media = media,
                                    Type = InstaFeedsType.Media
                                });
                            }
                        }
                        else
                        {
                            feed.Items.Add(media);
                            feed.Posts.Add(new InstaPostResponse
                            {
                                Media = media,
                                Type = InstaFeedsType.Media
                            });
                        }
                    }
                    if (item["suggested_users"] != null)
                    {
                        var users = item["suggested_users"]?["suggestions"];
                        if (users != null)
                        {
                            var post = new InstaPostResponse
                            {
                                Type = InstaFeedsType.SuggestedUsers
                            };
                            //foreach (var user in users)
                            for (int j = 0; j < users.Count(); j++)
                            {
                                var user = users.ElementAt(j);
                                if (user == null) continue;
                                var usr = user.ToObject<InstaSuggestionItemResponse>();
                                feed.SuggestedUsers.Add(usr);
                                post.SuggestedUserItems.Add(usr);

                            }
                            feed.Posts.Add(post);
                        }
                        var users2 = item["suggested_users"]?["suggestion_cards"];
                        if (users2 != null)
                        {
                            var post = new InstaPostResponse
                            {
                                Type = InstaFeedsType.SuggestedUsers
                            };
                            //foreach (var user in users2)
                            for (int k = 0; k < users2.Count(); k++)
                            //user_card
                            {
                                var user = users2.ElementAt(k);
                                if (user == null) continue;
                                var usr = user.First.First/*["user_card"]*/.ToObject<InstaSuggestionItemResponse>();
                                feed.SuggestedUsers.Add(usr);
                                post.SuggestedUserItems.Add(usr);
                            }
                            feed.Posts.Add(post);
                        }
                    }
                    if (item["suggested_producers"] != null)
                    {
                        var users = item["suggested_producers"]?["suggestions"];
                        if (users != null)
                        {
                            var post = new InstaPostResponse
                            {
                                Type = InstaFeedsType.SuggestedUsers
                            };
                            //foreach (var user in users)
                            //{
                            for (int l = 0; l < users.Count(); l++)
                            {
                                var user = users.ElementAt(l);
                                if (user == null) continue;
                                var usr = user.ToObject<InstaSuggestionItemResponse>();
                                feed.SuggestedUsers.Add(usr);
                                post.SuggestedUserItems.Add(usr);

                            }
                            feed.Posts.Add(post);
                        }
                    }
                    if (item["end_of_feed_demarcator"] != null)
                    {
                        var endFeed = item["end_of_feed_demarcator"].ToObject<InstaAllCatchedUpResponse>();
                        if (endFeed.GroupSet != null)
                        {
                            if (endFeed.GroupSet.Groups?.Length > 0)
                            {
                                try
                                {
                                    for (int j = 0; j < endFeed.GroupSet.Groups.Length; j++)
                                    {
                                        try
                                        {
                                            var gp = endFeed.GroupSet.Groups[j];
                                            if (gp.Items != null)
                                            {
                                                gp.FeedItems = JsonConvert.DeserializeObject<List<InstaMediaItemResponse>>(gp.Items.ToString(),
                                                    new InstaFeedGroupFeedsResponseDataConverter(CurrentUserId, RemoveAds));
                                            }
                                        }
                                        catch { }
                                    }
                                }
                                catch { }
                            }
                        }
                        feed.Posts.Add(new InstaPostResponse
                        {
                            EndOfFeedDemarcator = endFeed,
                            Type = InstaFeedsType.EndOfFeedDemarcator
                        });
                        //InstaFeedGroupFeedsResponseDataConverter
                    }
                    if (item["suggested_hashtags"] != null)
                    {
                        var hashtags = item["suggested_hashtags"]?["suggestions_with_media"];
                        if (hashtags != null)
                        {
                            var post = new InstaPostResponse
                            {
                                Type = InstaFeedsType.Hashtag
                            };
                            //foreach (var hashtag in hashtags)
                            for (int m = 0; m < hashtags.Count(); m++)
                            {
                                var hashtag = hashtags.ElementAt(m);
                                if (hashtag == null) continue;
                                var tag = hashtag.ToObject<InstaHashtagMedia>();
                                post.Hashtags.Add(tag);

                            }
                            feed.Posts.Add(post);
                        }
                    }
                    if (item["stories_netego"] != null)
                    {
                        var storiesNetego = item["stories_netego"].ToObject<InstaStoriesNetego>();
                        feed.Posts.Add(new InstaPostResponse
                        {
                            StoriesNetego = storiesNetego,
                            Type = InstaFeedsType.StoriesNetego
                        });
                    }
                }
            }
            else
            {
                items = token["items"];
                feed.Items = items.ToObject<List<InstaMediaItemResponse>>();
                if (feed.Items?.Count > 0)
                {
                    for (int n = 0; n < feed.Items.Count(); n++)
                    {
                        var media = feed.Items.ElementAt(n);
                        //foreach (var media in feed.Items)
                        feed.Posts.Add(new InstaPostResponse
                        {
                            Media = media,
                            Type = InstaFeedsType.Media
                        });
                    }
                }
            }

            var suggestedUsers = token["suggested_users"]?["suggestion_cards"];
            if (suggestedUsers != null)
            {
                var post = new InstaPostResponse
                {
                    Type = InstaFeedsType.SuggestedUsersCard
                };
                for (int o = 0; o < suggestedUsers.Count(); o++)

                //foreach (var item in suggestedUsers)
                {
                    var item = suggestedUsers.ElementAt(o);
                    var card = item["user_card"];
                    if (card != null)
                    {
                        if (card == null) continue;
                        var usr = card.ToObject<InstaSuggestionItemResponse>();
                        feed.SuggestedUsers.Add(usr);
                        post.SuggestedUserCardsItems.Add(usr);
                    }
                }
                feed.Posts.Add(post);
            }
            return feed;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    internal class InstaFeedGroupFeedsResponseDataConverter : JsonConverter
    {
        private readonly bool RemoveAds = false;
        private readonly long CurrentUserId = 0;
        public InstaFeedGroupFeedsResponseDataConverter(long userId, bool removeAds = false) : base()
        {
            CurrentUserId = userId;
            RemoveAds = removeAds;
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<InstaMediaItemResponse>);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var feed = new List<InstaMediaItemResponse>();
            var items = token;
            for (int i = 0; i < items.Count(); i++)
            {
                var item = items.ElementAt(i);
                if (item["media_or_ad"] != null)
                {
                    var mediaOrAd = item["media_or_ad"];
                    if (mediaOrAd == null) continue;
                    var media = mediaOrAd.ToObject<InstaMediaItemResponse>();
                    if (media.User.FriendshipStatus != null && RemoveAds && media.User.Pk != CurrentUserId)
                    {
                        if (media.User.FriendshipStatus.Following)
                            feed.Add(media);
                    }
                    else
                        feed.Add(media);
                }
            }

            return feed;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
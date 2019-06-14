using System;
using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.Converters.Json
{
    internal class InstaFeedResponseDataConverter : JsonConverter
    {
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
                foreach (var item in items)
                {
                    if (item["media_or_ad"] != null)
                    {
                        var mediaOrAd = item["media_or_ad"];
                        if (mediaOrAd == null) continue;
                        var media = mediaOrAd.ToObject<InstaMediaItemResponse>();
                        feed.Items.Add(media);
                        feed.Posts.Add(new InstaPostResponse
                        {
                            Media = media,
                            Type = InstaFeedsType.Media
                        });
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
                            foreach (var user in users)
                            {
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
                            foreach (var user in users2)
                            //user_card
                            {
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
                            foreach (var user in users)
                            {
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
                        var endFeed = item["end_of_feed_demarcator"].ToObject<InstaAllCatchedUp>();
                        feed.Posts.Add(new InstaPostResponse
                        {
                            EndOfFeedDemarcator = endFeed,
                            Type = InstaFeedsType.EndOfFeedDemarcator
                        });
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
                            foreach (var hashtag in hashtags)
                            {
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
                    foreach (var media in feed.Items)
                        feed.Posts.Add(new InstaPostResponse
                        {
                            Media = media,
                            Type = InstaFeedsType.Media
                        });
            }

            var suggestedUsers = token["suggested_users"]?["suggestion_cards"];
            if (suggestedUsers != null)
            {
                var post = new InstaPostResponse
                {
                    Type = InstaFeedsType.SuggestedUsersCard
                };

                foreach (var item in suggestedUsers)
                {
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
}
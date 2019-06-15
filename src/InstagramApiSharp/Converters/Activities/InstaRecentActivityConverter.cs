﻿using System.Globalization;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;

namespace InstagramApiSharp.Converters
{
    internal class InstaRecentActivityConverter :
        IObjectConverter<InstaRecentActivityFeed, InstaRecentActivityFeedResponse>
    {
        public InstaRecentActivityFeedResponse SourceObject { get; set; }

        public InstaRecentActivityFeed Convert()
        {
            var activityStory = new InstaRecentActivityFeed
            {
                Pk = SourceObject.Pk,
                ProfileId = SourceObject.Args.ProfileId,
                ProfileImage = SourceObject.Args.ProfileImage,
                Text = SourceObject.Args.Text,
                RichText = SourceObject.Args.RichText,
                TimeStamp = DateTimeHelper.UnixTimestampToDateTime((long)System.Convert.ToDouble(SourceObject.Args.TimeStamp, new NumberFormatInfo { NumberDecimalSeparator = "." })),
                CommentId= SourceObject.Args.CommentId,
                CommentIds = SourceObject.Args.CommentIds,
                Destination = SourceObject.Args.Destination,
                ProfileImageDestination = SourceObject.Args.ProfileImageDestination,
                ProfileName = SourceObject.Args.ProfileName,
                SecondProfileId = SourceObject.Args.SecondProfileId,
                SecondProfileImage = SourceObject.Args.SecondProfileImage,
                SubText = SourceObject.Args.SubText,
                RequestCount = SourceObject.Args.RequestCount
            };
            try
            {
                activityStory.Type = (Enums.InstaActivityFeedType)SourceObject.Type;
            }
            catch { }
            try
            {
                if (SourceObject.Args.HashtagFollow != null)
                    activityStory.HashtagFollow = ConvertersFabric.Instance.GetHashTagConverter(SourceObject.Args.HashtagFollow).Convert();
            }
            catch { }
            try
            {
                activityStory.StoryType = (Enums.InstaActivityFeedStoryType)SourceObject.Args.StoryType;
            }
            catch { }
            if (activityStory.Type == Enums.InstaActivityFeedType.FriendRequest)
                activityStory.StoryType = Enums.InstaActivityFeedStoryType.FriendRequest;

            if (SourceObject.Args.Links != null)
                foreach (var instaLinkResponse in SourceObject.Args.Links)
                    activityStory.Links.Add(new InstaLink
                    {
                        Start = instaLinkResponse.Start,
                        End = instaLinkResponse.End,
                        Id = instaLinkResponse.Id,
                        Type = instaLinkResponse.Type
                    });
            if (SourceObject.Args.InlineFollow != null)
            {
                activityStory.InlineFollow = new InstaInlineFollow
                {
                    IsFollowing = SourceObject.Args.InlineFollow.IsFollowing,
                    IsOutgoingRequest = SourceObject.Args.InlineFollow.IsOutgoingRequest
                };
                if (SourceObject.Args.InlineFollow.UserInfo != null)
                    activityStory.InlineFollow.User =
                        ConvertersFabric.Instance.GetUserShortConverter(SourceObject.Args.InlineFollow.UserInfo)
                            .Convert();
            }
            if (SourceObject.Args.Media != null)
                foreach (var media in SourceObject.Args.Media)
                    activityStory.Medias.Add(new InstaActivityMedia
                    {
                        Id = media.Id,
                        Image = media.Image
                    });
            return activityStory;
        }
    }
}
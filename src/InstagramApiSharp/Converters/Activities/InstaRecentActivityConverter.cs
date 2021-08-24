﻿using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;
using System;
using System.Globalization;

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
                CommentId = SourceObject.Args.CommentId,
                CommentIds = SourceObject.Args.CommentIds,
                Destination = SourceObject.Args.Destination,
                ProfileImageDestination = SourceObject.Args.ProfileImageDestination,
                ProfileName = SourceObject.Args.ProfileName,
                SecondProfileId = SourceObject.Args.SecondProfileId,
                SecondProfileImage = SourceObject.Args.SecondProfileImage,
                SubText = SourceObject.Args.SubText,
                RequestCount = SourceObject.Args.RequestCount,
                IconUrl = SourceObject.Args.IconUrl,
                AfCandidateId = SourceObject.Args.AfCandidateId,
                HasLikedComment = SourceObject.Args.HasLikedComment ?? false,
                DisplayUfi = SourceObject.Args.DisplayUfi ?? false,
                ShouldIconApplyFilter = SourceObject.Args.ShouldIconApplyFilter ?? false,
                IconShouldApplyFilter = SourceObject.Args.IconShouldApplyFilter ?? false,
                Clicked = SourceObject.Args.Clicked ?? false,
                CommentNotifType = SourceObject.Args.CommentNotifType,
                Tuuid = SourceObject.Args.Tuuid,
                LatestReelMedia = SourceObject.Args.LatestReelMedia ?? 0,
                IsRestricted = (SourceObject.Args.FriendshipStatus != null && (SourceObject.Args.FriendshipStatus.IsRestricted ?? false)),
                Actions = SourceObject.Args.Actions,
            };
            if (!string.IsNullOrEmpty(SourceObject.Args.IconUrl))
                activityStory.ProfileImage = SourceObject.Args.IconUrl;
            if (!string.IsNullOrEmpty(SourceObject.Args.SubText))
                activityStory.Text = SourceObject.Args.SubText;
            if (!string.IsNullOrEmpty(SourceObject.Args.RichText))
                activityStory.Text = SourceObject.Args.RichText;

            activityStory.Type = (Enums.InstaActivityFeedType)SourceObject.Type;
 
            try
            {
                if (SourceObject.Args.HashtagFollow != null)
                    activityStory.HashtagFollow = ConvertersFabric.Instance.GetHashTagConverter(SourceObject.Args.HashtagFollow).Convert();
            }
            catch { }

            activityStory.StoryType = (Enums.InstaActivityFeedStoryType)SourceObject.StoryType;
     
            if (activityStory.Type == Enums.InstaActivityFeedType.FriendRequest)
                activityStory.StoryType = Enums.InstaActivityFeedStoryType.FriendRequest;
            
            if (SourceObject.Args.Extra != null)
            {
                activityStory.Extra = new InstaActivityStoryItemExtra
                {
                    Latitude = SourceObject.Args.Extra.Latitude,
                    Longitude = SourceObject.Args.Extra.Longitude
                };
            }

            if (SourceObject.Args.Links != null)
                foreach (var instaLinkResponse in SourceObject.Args.Links)
                {
                    var type = Enums.InstaLinkType.Unknown;
                    try
                    {
                        var tObj = instaLinkResponse.Type.Replace("_", "");
                        type = (Enums.InstaLinkType)Enum.Parse(typeof(Enums.InstaLinkType), tObj, true);
                    }
                    catch { }
                    activityStory.Links.Add(new InstaLink
                    {
                        Start = instaLinkResponse.Start,
                        End = instaLinkResponse.End,
                        Id = instaLinkResponse.Id,
                        Type = type
                    });
                }

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

            if (SourceObject.Args.Media?.Count > 0)
                foreach (var media in SourceObject.Args.Media)
                    activityStory.Medias.Add(new InstaActivityMedia
                    {
                        Id = media.Id,
                        Image = media.Image
                    });
            
            if (SourceObject.Args.Images?.Count > 0)
                foreach (var media in SourceObject.Args.Images)
                    activityStory.Images.Add(new InstaActivityMedia
                    {
                        Id = media.Id,
                        Image = media.Image,
                        CommentThreadingEnabled = media.CommentThreadingEnabled ?? false
                    });

            return activityStory;
        }
    }
}
/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaHashtagConverter : IObjectConverter<InstaHashtag, InstaHashtagResponse>
    {
        public InstaHashtagResponse SourceObject { get; set; }
         
        public InstaHashtag Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var hashtag = new InstaHashtag
            {
                Id = SourceObject.Id,
                Name = SourceObject.Name,
                MediaCount = SourceObject.MediaCount,
                ProfilePicture = SourceObject.ProfilePicture,
                AllowFollowing = SourceObject.AllowFollowing ?? true,
                Following = System.Convert.ToBoolean(SourceObject.Following ?? 0),
                NonViolating = System.Convert.ToBoolean(SourceObject.NonViolating ?? 1),
                FormattedMediaCount = SourceObject.FormattedMediaCount,
                SearchResultSubtitle = SourceObject.SearchResultSubtitle,
                AllowMutingStory = SourceObject.AllowMutingStory ?? false,
                ShowFollowDropDown = SourceObject.ShowFollowDropDown ?? false,
                SocialContext = SourceObject.SocialContext,
                Subtitle = SourceObject.Subtitle
            };
            try
            {
                hashtag.FollowStatus = System.Convert.ToBoolean(SourceObject.FollowStatus ?? 0);
            }
            catch { }
            return hashtag;
        }
    }
}
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
    internal class InstaHashtagShortConverter : IObjectConverter<InstaHashtagShort, InstaHashtagShortResponse>
    {
        public InstaHashtagShortResponse SourceObject { get; set; }

        public InstaHashtagShort Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var hashtag = new InstaHashtagShort
            {
                Id = SourceObject.Id,
                Name = SourceObject.Name,
                MediaCount = SourceObject.MediaCount ?? 0,
                ProfilePicture = SourceObject.ProfilePicture
            };
            return hashtag;
        }
    }
}

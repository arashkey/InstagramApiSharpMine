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
using InstagramApiSharp.Helpers;

namespace InstagramApiSharp.Converters
{
    internal class InstaDiscoverRecentSearchesItemConverter : IObjectConverter<InstaDiscoverRecentSearchesItem, InstaDiscoverRecentSearchesItemResponse>
    {
        public InstaDiscoverRecentSearchesItemResponse SourceObject { get; set; }

        public InstaDiscoverRecentSearchesItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var recentSearches = new InstaDiscoverRecentSearchesItem
            {
                ClientTime = DateTimeHelper.FromUnixTimeSeconds(SourceObject.ClientTime ?? 0),
                Position = SourceObject.Position
            };
            if (SourceObject.Hashtag != null)
            {
                try
                {
                    recentSearches.Hashtag = ConvertersFabric.Instance.GetHashtagShortConverter(SourceObject.Hashtag).Convert();
                }
                catch { }
            }
            if (SourceObject.User != null)
            {
                try
                {
                    recentSearches.User = ConvertersFabric.Instance.GetUserShortFriendshipConverter(SourceObject.User).Convert();
                }
                catch { }
            }
            return recentSearches;
        }
    }
}

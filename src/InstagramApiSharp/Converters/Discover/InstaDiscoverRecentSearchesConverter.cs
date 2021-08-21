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
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaDiscoverRecentSearchesConverter : IObjectConverter<InstaDiscoverRecentSearches, InstaDiscoverRecentSearchesResponse>
    {
        public InstaDiscoverRecentSearchesResponse SourceObject { get; set; }

        public InstaDiscoverRecentSearches Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var recents = new InstaDiscoverRecentSearches();
            if (SourceObject.Recent?.Count > 0)
            {
                foreach (var search in SourceObject.Recent)
                {
                    try
                    {
                        recents.Recent.Add(ConvertersFabric.Instance.GetDiscoverRecentSearchesItemConverter(search).Convert());
                    }
                    catch { }
                }
            }
            return recents;
        }
    }
}

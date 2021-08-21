/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDiscoverSuggestedSearches
    {
        public string RankToken { get; set; }

        public List<InstaDiscoverSearches> Suggested { get; set; } = new List<InstaDiscoverSearches>();
    }
}

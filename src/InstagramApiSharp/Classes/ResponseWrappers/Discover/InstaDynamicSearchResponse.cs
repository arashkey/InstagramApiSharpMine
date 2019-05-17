/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaDynamicSearchResponse : InstaDefault
    {
        [JsonProperty("sections")]
        public List<InstaDynamicSearchSectionResponse> Sections { get; set; }
    }

    public class InstaDynamicSearchSectionResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("items")]
        public List<InstaDiscoverRecentSearchesItemResponse> Items { get; set; }
    }
}

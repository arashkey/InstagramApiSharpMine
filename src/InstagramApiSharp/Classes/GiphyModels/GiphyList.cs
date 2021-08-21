/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes
{
    internal class GiphyListContainer
    {
        [JsonProperty("results")]
        public GiphyList Results { get; set; }
    }
    public class GiphyList
    {
        [JsonProperty("data")]
        public List<GiphyItem> Items { get; set; } = new List<GiphyItem>();
    }
}

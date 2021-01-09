/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

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

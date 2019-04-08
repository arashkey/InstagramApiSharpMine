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
    public class GiphyList
    {
        [JsonProperty("data")]
        public List<GiphyItem> Items { get; set; } = new List<GiphyItem>();
        [JsonProperty("pagination")]
        public GiphyPagination Pagination { get; set; }
        [JsonProperty("meta")]
        public GiphyMeta Meta { get; set; }
    }

    public class GiphyPagination
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
    }

    public class GiphyMeta
    {
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("response_id")]
        public string ResponseId { get; set; }
    } 
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaFeedGroupResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("show_group_text")]
        public string ShowGroupText { get; set; }
        [JsonIgnore()]
        public List<InstaMediaItemResponse> FeedItems { get; set; } = new List<InstaMediaItemResponse>();
        [JsonProperty("feed_items")]
        public JArray Items { get; set; }
        [JsonProperty("next_max_id")]
        public string NextMaxId { get; set; }
        [JsonProperty("pagination_source")]
        public string PaginationSource { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaTVBrowseFeedResponse : InstaDefaultResponse
    {
        //[JsonProperty("badging")]
        //public object Badging { get; set; }
        [JsonProperty("my_channel")]
        public InstaTVSelfChannelResponse MyChannel { get; set; }
        //[JsonProperty("composer")]
        //public object Composer { get; set; }
        [JsonProperty("banner_token")]
        public string BannerToken { get; set; }
        [JsonProperty("browse_items")]
        public List<InstaTVBrowseFeedItemResponse> BrowseItems { get; set; }
        [JsonProperty("max_id")]
        public string MaxId { get; set; }
        //[JsonProperty("seen_state")]
        //public object SeenState { get; set; }
        [JsonProperty("more_available")]
        public bool MoreAvailable { get; set; }
        //[JsonProperty("channels")]
        //public object Channels { get; set; }
    }
}

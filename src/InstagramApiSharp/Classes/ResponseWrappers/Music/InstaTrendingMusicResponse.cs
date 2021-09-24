/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/rmt4006/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaTrendingMusicResponse : InstaDefaultResponse
    {
        [JsonProperty("items")]
        public List<InstaMusicContainerResponse> Items { get; set; }
        [JsonProperty("page_info")]
        public InstaMusicPageInfoResponse PageInfo { get; set; }
        [JsonProperty("alacorn_session_id")]
        public string AlacornSessionId { get; set; }
        [JsonProperty("music_reels")]
        public object MusicReels { get; set; }
        [JsonProperty("dark_banner_message")]
        public object DarkBannerMessage { get; set; }
    }

    public class InstaMusicPageInfoResponse
    {
        [JsonProperty("next_max_id")]
        public string NextMaxId { get; set; }
        [JsonProperty("more_available")]
        public bool? MoreAvailable { get; set; }
        [JsonProperty("auto_load_more_available")]
        public bool? AutoLoadMoreAvailable { get; set; }
    }
}

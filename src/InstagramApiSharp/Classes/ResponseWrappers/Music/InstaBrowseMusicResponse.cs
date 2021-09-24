/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaBrowseMusicResponse : InstaDefaultResponse
    {
        [JsonProperty("items")]
        public List<InstaBrowseMusicItemResponse> Items { get; set; } = new List<InstaBrowseMusicItemResponse>();
    }

    public class InstaBrowseMusicItemResponse
    {
        [JsonProperty("playlist")]
        public InstaMusicPlaylistResponse Playlist { get; set; }
        [JsonProperty("category")]
        public InstaMusicPlaylistResponse Category { get; set; }
    }
}

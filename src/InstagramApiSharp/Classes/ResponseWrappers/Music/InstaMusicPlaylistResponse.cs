/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaMusicPlaylistResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("icon_url")]
        public object IconUrl { get; set; }
        [JsonProperty("preview_items")]
        public List<InstaMusicContainer2Response> PreviewItems { get; set; } = new List<InstaMusicContainer2Response>();
    }

    public class InstaMusicContainer2Response : InstaMusicContainerResponse
    {
        [JsonProperty("playlist")]
        public InstaMusicPlaylistResponse Playlist { get; set; }
    }
}

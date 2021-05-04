/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaPagingInfoResponse
    {
        [JsonProperty("max_id")]
        public string MaxId { get; set; }
        [JsonProperty("more_available")]
        public bool? MoreAvailable { get; set; }
    }
}

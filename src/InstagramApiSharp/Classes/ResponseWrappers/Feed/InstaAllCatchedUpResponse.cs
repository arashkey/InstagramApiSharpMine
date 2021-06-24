/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaAllCatchedUpResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        [JsonProperty("style")]
        public string Style { get; set; }
        [JsonProperty("pause")]
        public bool? Pause { get; set; }
        [JsonProperty("group_set")]
        public InstaFeedGroupSetResponse GroupSet { get; set; }
    }
}
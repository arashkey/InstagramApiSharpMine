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
    public class InstaFeedGroupSetResponse
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("active_group_id")]
        public string ActiveGroupId { get; set; }
        [JsonProperty("connected_group_id")]
        public string ConnectedGroupId { get; set; }
        [JsonProperty("remember_group_choice")]
        public bool? RememberGroupChoice { get; set; }
        [JsonProperty("style")]
        public InstaFeedGroupSetStyle Style { get; set; }
        [JsonProperty("groups")]
        public InstaFeedGroupResponse[] Groups { get; set; }
    }

    public class InstaFeedGroupSetStyle
    {
        [JsonProperty("header_accessory_style")]
        public string HeaderAccessoryStyle { get; set; }
    }

}

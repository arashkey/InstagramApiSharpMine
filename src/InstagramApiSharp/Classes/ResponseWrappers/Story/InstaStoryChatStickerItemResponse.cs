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
    public class InstaStoryChatStickerItemResponse
    {
        [JsonProperty("story_chat_id")]
        public long StoryChatId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("start_background_color")]
        public string StartBackgroundColor { get; set; }
        [JsonProperty("end_background_color")]
        public string EndBackgroundColor { get; set; }
        [JsonProperty("has_started_chat")]
        public bool HasStartedChat { get; set; }
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}

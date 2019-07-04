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
    public class InstaStoryChatItemResponse
    {
        [JsonProperty("x")]
        public float X { get; set; }
        [JsonProperty("y")]
        public float Y { get; set; }
        [JsonProperty("z")]
        public float Z { get; set; }
        [JsonProperty("width")]
        public float Width { get; set; }
        [JsonProperty("height")]
        public float Height { get; set; }
        [JsonProperty("rotation")]
        public float Rotation { get; set; }
        [JsonProperty("is_pinned")]
        public int IsPinned { get; set; }
        [JsonProperty("is_hidden")]
        public int IsHidden { get; set; }
        [JsonProperty("chat_sticker")]
        public InstaStoryChatStickerItemResponse ChatSticker { get; set; }
    }
}

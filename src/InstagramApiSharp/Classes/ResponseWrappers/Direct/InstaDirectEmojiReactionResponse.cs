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
    public class InstaDirectEmojiReactionResponse
    {
        [JsonProperty("sender_id")] public long SenderId { get; set; }
        [JsonProperty("timestamp")] public long Timestamp { get; set; }
        [JsonProperty("client_context")] public string ClientContext { get; set; }
        [JsonProperty("emoji")] public string Emoji { get; set; }
        [JsonProperty("super_react_type")] public string SuperReactType { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaDirectReactionResponse
    {
        [JsonProperty("likes")]
        public List<InstaDirectLikeReactionResponse> Likes { get; set; }
        [JsonProperty("likes_count")]
        public int LikesCount { get; set; }
        [JsonProperty("emojis")]
        public List<InstaDirectEmojiReactionResponse> Emojis { get; set; }
    }
}

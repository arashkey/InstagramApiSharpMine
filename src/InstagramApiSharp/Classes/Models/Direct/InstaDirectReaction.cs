/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectReaction
    {
        public List<InstaDirectEmojiReaction> Emojis { get; set; } = new List<InstaDirectEmojiReaction>();
        public List<InstaDirectLikeReaction> Likes { get; set; } = new List<InstaDirectLikeReaction>();

        public int LikesCount { get; set; }

        public bool Liked { get; set; }

        public bool Visibility { get; set; }
    }
}

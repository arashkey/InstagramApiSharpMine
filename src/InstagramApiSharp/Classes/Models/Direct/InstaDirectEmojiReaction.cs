/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using System;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectEmojiReaction
    {
        public long SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ClientContext { get; set; }
        public string Emoji { get; set; }
        public string SuperReactType { get; set; }
    }
}

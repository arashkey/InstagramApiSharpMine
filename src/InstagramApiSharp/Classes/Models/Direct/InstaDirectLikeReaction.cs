/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectLikeReaction
    {
        public long SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ClientContext { get; set; }
        public InstaUserShort User { get; set; }
    }
}

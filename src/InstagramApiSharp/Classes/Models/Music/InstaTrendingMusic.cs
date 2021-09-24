/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/rmt4006/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTrendingMusic
    {
        public List<InstaMusic> Items { get; set; } = new List<InstaMusic>();
        public string AlacornSessionId { get; set; }
        public object MusicReels { get; set; }
        public object DarkBannerMessage { get; set; }
        public string NextMaxId { get; set; }
        public bool MoreAvailable { get; set; }
        public bool AutoLoadMoreAvailable { get; set; }
    }
}

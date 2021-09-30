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
    public class InstaBrowseMusic
    {
        public List<InstaBrowseMusicItem> Items { get; set; } = new List<InstaBrowseMusicItem>();
    }
    public class InstaBrowseMusicItem
    {
        public InstaMusicPlaylist Playlist { get; set; }
        public InstaMusicPlaylist Category { get; set; }
        public bool IsPlaylist => Playlist != null;
    }
}

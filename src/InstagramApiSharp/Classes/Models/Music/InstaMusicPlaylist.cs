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
    public class InstaMusicPlaylist
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public object IconUrl { get; set; }
        public List<InstaMusicList> PreviewItems { get; set; } = new List<InstaMusicList>();
    }
    public class InstaMusicList
    {
        public InstaMusicPlaylist Playlist { get; set; }
        public InstaMusic Music { get; set; }
    }
}

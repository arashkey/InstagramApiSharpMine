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
    public class InstaReelsMediaList : IInstaBaseList
    {
        public List<InstaMedia> Medias { get; set; } = new List<InstaMedia>();
        public bool MoreAvailable { get; set; }
        public string NextMaxId { get; set; }
    }
}

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
    public class InstaStoryChatRequestInfoItem
    {
        public List<InstaUserShort> Users { get; set; } = new List<InstaUserShort>();

        public string Cursor { get; set; }

        public int TotalThreadParticipants { get; set; }

        public int TotalParticipantRequests { get; set; }
    }
}

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
    public class InstaFeedGroupSet
    {
        public long? Id { get; set; }
        public string Format { get; set; }
        public string ActiveGroupId { get; set; }
        public string ConnectedGroupId { get; set; }
        public bool? RememberGroupChoice { get; set; }
        public string HeaderAccessoryStyle { get; set; }
        public List<InstaFeedGroup> Groups { get; set; } = new List<InstaFeedGroup>();
    }
}

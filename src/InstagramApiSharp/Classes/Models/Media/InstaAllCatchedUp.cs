/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes.Models
{
    public class InstaAllCatchedUp
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Style { get; set; }
        public bool Pause { get; set; }
        public InstaFeedGroupSet GroupSet { get; set; }
    }
}

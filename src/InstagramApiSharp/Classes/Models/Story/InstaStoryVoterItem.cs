/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryVoterItem
    {
        public InstaUserShortFriendship User { get; set; }

        public double Vote { get; set; }
        public string VoteYesText { get; set; }
        public string VoteNoText { get; set; }

        public DateTime Time { get; set; }
    }
}

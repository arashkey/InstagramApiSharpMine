/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaUserShortFriendshipList : List<InstaUserShortFriendship> { }
    public class InstaUserShortFriendship : InstaUserShort
    {
        private InstaFriendshipShortStatus _status;
        public InstaFriendshipShortStatus FriendshipStatus
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("FriendshipStatus"); }
        }
    }
}

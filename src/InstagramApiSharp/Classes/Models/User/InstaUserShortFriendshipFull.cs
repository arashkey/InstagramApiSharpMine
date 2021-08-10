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
    public class InstaUserShortFriendshipFullList : List<InstaUserShortFriendshipFull> { }

    public class InstaUserShortFriendshipFull : InstaUserShort
    {
        private InstaFriendshipFullStatus _status;
        public InstaFriendshipFullStatus FriendshipStatus
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("FriendshipStatus"); }
        }
    }
}

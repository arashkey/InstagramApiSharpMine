/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaUserChaining : InstaUserShort
    {
        public string ChainingInfo { get; set; }

        public string ProfileChainingSecondaryLabel { get; set; }

        private string _socialContext;
        public string SocialContext
        {
            get
            {
                //if(string.IsNullOrEmpty(_socialContext))
                //    return "";
                //else
                //{
                //    if (_socialContext == FullName || _socialContext == UserName)
                //        return "";
                //    else
                //        return _socialContext;
                //}
                return _socialContext;
            }
            set { _socialContext = value; OnPropertyChanged("SocialContext"); }
        }

        private InstaFriendshipShortStatus _status;
        public InstaFriendshipShortStatus FriendshipStatus
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("FriendshipStatus"); }
        }
        private string _followText = "Follow";
        public string FollowText { get { return _followText; } set { _followText = value; OnPropertyChanged("FollowText"); } }
    }

    public class InstaUserChainingList : List<InstaUserChaining>
    {
        internal string Status { get; set; }

        public bool IsBackup { get; set; }
    }
}

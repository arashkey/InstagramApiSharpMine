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
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDiscoverRecentSearches
    {
        public List<InstaDiscoverRecentSearchesItem> Recent { get; set; } = new List<InstaDiscoverRecentSearchesItem>();
    }

    public class InstaDiscoverRecentSearchesItem : INotifyPropertyChanged
    {
        public int Position { get; set; }

        public InstaUserShortFriendship User { get; set; }

        public DateTime ClientTime { get; set; }

        public InstaHashtagShort Hashtag { get; set; }

        public bool IsHashtag => Hashtag != null;

        bool ShowClose_ = true;
        public bool ShowClose { get { return ShowClose_; } set { ShowClose_ = value; OnPropertyChanged("ShowClose"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}

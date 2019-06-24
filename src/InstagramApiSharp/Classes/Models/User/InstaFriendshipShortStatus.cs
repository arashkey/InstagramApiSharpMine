/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using System.ComponentModel;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaFriendshipShortStatusList : List<InstaFriendshipShortStatus> { }

    public class InstaFriendshipShortStatus : INotifyPropertyChanged
    {
        public long Pk { get; set; }
        private bool following_ = false;
        public bool Following { get { return following_; } set { following_ = value; OnPropertyChanged("Following"); } }

        public bool IsPrivate { get; set; }

        public bool IncomingRequest { get; set; }

        public bool OutgoingRequest { get; set; }

        public bool IsBestie { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}

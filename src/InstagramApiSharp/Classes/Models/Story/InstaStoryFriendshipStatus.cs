/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryFriendshipStatus : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public bool Following { get; set; }

        public bool FollowedBy { get; set; }

        public bool Blocking { get; set; }

        public bool Muting { get; set; }

        public bool IsPrivate { get; set; }

        public bool IncomingRequest { get; set; }

        public bool OutgoingRequest { get; set; }

        public bool IsBlockingReel { get; set; }

        public bool IsMutingReel { get; set; }

        public bool IsBestie { get; set; }
    }
}

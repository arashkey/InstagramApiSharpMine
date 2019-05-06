using System;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    [Serializable]
    public class InstaUserShort : INotifyPropertyChanged
    {
        public bool IsVerified { get; set; }
        public bool IsPrivate { get; set; }
        public long Pk { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePicUrl { get; set; }
        public string ProfilePictureId { get; set; } = "unknown";
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool HasAnonymousProfilePicture { get; set; }

        public static InstaUserShort Empty => new InstaUserShort {FullName = string.Empty, UserName = string.Empty};

        public bool Equals(InstaUserShort user)
        {
            return Pk == user?.Pk;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstaUserShort);
        }

        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }

        private bool _selected = false;
        public bool? Selected
        {
            get { return _selected; }
            set { _selected = value ?? false; OnPropertyChanged("Selected"); }
        }

        private bool _closeButton = false;
        public bool? CloseButton
        {
            get { return _closeButton; }
            set { _closeButton = value ?? false; OnPropertyChanged("CloseButton"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
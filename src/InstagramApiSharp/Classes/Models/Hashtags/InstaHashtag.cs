using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaHashtag : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long MediaCount { get; set; }
        public string ProfilePicture { get; set; }

        private bool _followStatus = false;
        public bool FollowStatus { get { return _followStatus; } set { _followStatus = value; OnPropertyChanged("FollowStatus"); } }
        public bool Following { get; set; }
        public bool NonViolating { get; set; }
        public bool AllowFollowing { get; set; }
        public string FormattedMediaCount { get; set; }
        public string SearchResultSubtitle { get; set; }

        public bool ShowFollowDropDown { get; set; }
        public bool AllowMutingStory { get; set; }
        public string SocialContext { get; set; }
        public string Subtitle { get; set; }
    }
}
using InstagramApiSharp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaRecentActivityFeed : INotifyPropertyChanged
    {
        private bool _hasLikedComment = false;
        public long ProfileId { get; set; }

        public string ProfileImage { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Text { get; set; }

        public string RichText { get; set; }

        public List<InstaLink> Links { get; set; } = new List<InstaLink>();
        public InstaInlineFollow InlineFollow { get; set; }
        public InstaActivityFeedType Type { get; set; }

        public string Pk { get; set; }

        public List<InstaActivityMedia> Medias { get; set; } = new List<InstaActivityMedia>();

        public long? SecondProfileId { get; set; }

        public string SecondProfileImage { get; set; }

        public string ProfileName { get; set; }

        public string ProfileImageDestination { get; set; }

        public InstaHashtag HashtagFollow { get; set; }

        public string Destination { get; set; }

        public long? CommentId { get; set; }
        public List<long> CommentIds { get; set; } = new List<long>();

        public InstaActivityFeedStoryType StoryType { get; set; }

        public int RequestCount { get; set; } = 0;
        public string SubText { get; set; }

        public string IconUrl { get; set; }



        public bool HasLikedComment { get => _hasLikedComment; set { _hasLikedComment = value; OnPropertyChanged("HasLikedComment"); } }
        public bool DisplayUfi { get; set; }
        public bool Clicked { get; set; }
        public string CommentNotifType { get; set; }
        public string Tuuid { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
        public long LatestReelMedia { get; set; }
        public List<InstaActivityMedia> Images { get; set; } = new List<InstaActivityMedia>();
        public bool IsRestricted { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
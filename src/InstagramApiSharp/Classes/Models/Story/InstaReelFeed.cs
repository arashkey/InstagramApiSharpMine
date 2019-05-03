﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaReelFeed : INotifyPropertyChanged
    {
        public bool HasBestiesMedia { get; set; }

        public long PrefetchCount { get; set; }

        public bool? CanReshare { get; set; }

        public bool CanReply { get; set; }

        public DateTime ExpiringAt { get; set; }

        public List<InstaStoryItem> Items { get; set; } = new List<InstaStoryItem>();

        public string Id { get; set; }

        public long LatestReelMedia { get; set; }

        long _seen = -1;
        public long Seen { get { return _seen; } set { _seen = value; OnPropertyChanged("Seen"); } }

        public InstaUserShortFriendshipFull User { get; set; }

        public int MediaCount { get; set; }
        public string ReelType { get; set; }
        public bool IsHashtag => /*ReelType?.ToLower() == "mas_reel" && */Id.ToLower().StartsWith("tag:");
        public InstaHashtagOwner Owner { get; set; }
        public bool Muted { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
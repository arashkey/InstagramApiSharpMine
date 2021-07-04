﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectInboxThread : INotifyPropertyChanged
    {
        public bool Muted { get; set; }

        public List<InstaUserShortFriendship> Users { get; set; } = new List<InstaUserShortFriendship>();

        private string _title;
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged("Title"); } }

        public string OldestCursor { get; set; }
        
        public DateTime LastActivity { get; set; }
        public string LastActivityUnix { get; set; }
        private bool _hasUnreadMessage;
        public bool HasUnreadMessage { get { return _hasUnreadMessage; } set { _hasUnreadMessage = value; OnPropertyChanged("HasUnreadMessage"); } }

        public string VieweId { get; set; }

        public string ThreadId { get; set; }

        public bool HasOlder { get; set; }

        public InstaUserShort Inviter { get; set; }

        public bool Named { get; set; }

        public bool Pending { get; set; }

        public bool Canonical { get; set; }

        public bool HasNewer { get; set; }

        public bool IsSpam { get; set; }

        public InstaDirectThreadType ThreadType { get; set; }

        public List<InstaDirectInboxItem> Items { get; set; } = new List<InstaDirectInboxItem>();
        
        public InstaDirectInboxItem LastPermanentItem { get; set; }

        public bool IsPin { get; set; }

        public bool ValuedRequest { get; set; }

        public long PendingScore { get; set; }

        public bool VCMuted { get; set; }

        public bool IsGroup { get; set; }

        public int ReshareSendCount { get; set; }

        public int ReshareReceiveCount { get; set; }

        public int ExpiringMediaSendCount { get; set; }

        public int ExpiringMediaReceiveCount { get; set; }

        public List<InstaLastSeen> LastSeenAt { get; set; } = new List<InstaLastSeen>();

        public List<InstaUserShortFriendship> LeftUsers { get; set; } = new List<InstaUserShortFriendship>();

        public string NewestCursor { get; set; }

        public bool MentionsMuted { get; set; }

        public bool Archived { get; set; }

        public bool ApprovalRequiredForNewMembers { get; set; }

        public int Folder { get; set; }

        public int InputMode { get; set; }

        public int BusinessThreadFolder { get; set; }

        public int ReadState { get; set; }

        public List<InstaDirectInboxItem> DirectStories { get; set; } = new List<InstaDirectInboxItem>();
        public List<long> AdminUserIds { get; set; } = new List<long>();
        public string LastNonSenderItemAt { get; set; }
        public string AssignedAdminId { get; set; }
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
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
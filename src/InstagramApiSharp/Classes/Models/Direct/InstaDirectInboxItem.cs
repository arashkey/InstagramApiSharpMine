using InstagramApiSharp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectInboxItem : INotifyPropertyChanged
    {
        private InstaDirectInboxItemSendingType _sendingType = InstaDirectInboxItemSendingType.Pending;
        public InstaDirectInboxItemSendingType SendingType
        {
            get => _sendingType;
            set
            {
                _sendingType = value;
                OnPropertyChanged("SendingType");
            }
        }

        public string Text { get; set; }

        public long UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUnix { get; set; }

        public string ItemId { get; set; }

        public InstaDirectThreadItemType ItemType { get; set; } = InstaDirectThreadItemType.Text;

        public string ItemTypeOriginalText { get; set; }

        public InstaInboxMedia Media { get; set; }

        public InstaMedia MediaShare { get; set; }

        public string ClientContext { get; set; }

        public InstaStoryShare StoryShare { get; set; }

        public InstaVisualMedia RavenMedia { get; set; }

        public InstaVisualMediaContainer VisualMedia { get; set; }

        // raven media properties
        public InstaViewMode? RavenViewMode { get; set; }

        public List<long> RavenSeenUserIds { get; set; } = new List<long>();

        public int RavenReplayChainCount { get; set; }

        public int RavenSeenCount { get; set; }

        public InstaRavenMediaActionSummary RavenExpiringMediaActionSummary { get; set; }

        public InstaActionLog ActionLog { get; set; }

        public InstaUserShort ProfileMedia { get; set; }

        public List<InstaMedia> PreviewMedias { get; set; } = new List<InstaMedia>();

        public InstaPlaceholder Placeholder { get; set; }

        public InstaWebLink LinkMedia { get; set; }

        public InstaLocation LocationMedia { get; set; }

        public InstaMedia FelixShareMedia { get; set; }

        public InstaReelShare ReelShareMedia { get; set; }

        public InstaVoiceMedia VoiceMedia { get; set; }

        public InstaAnimatedImage AnimatedMedia { get; set; }

        public InstaDirectHashtag HashtagMedia { get; set; }

        public InstaDirectBroadcast LiveViewerInvite { get; set; }

        public InstaVideoCallEvent VideoCallEvent { get; set; }

        public InstaProduct ProductShare { get; set; }

        public InstaDirectArEffect ArEffect { get; set; }

        public bool ShowForwardAttribution { get; set; }
        public bool IsShhMode { get; set; }
        public InstaDirectInboxItem RepliedToMessage { get; set; }

        public InstaDirectReaction Reactions { get; set; } = new InstaDirectReaction();
        public bool HideInThread { get; set; }
        public string RealtimePath { get; set; }
        public string RealtimeOp { get; set; } = "add";


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
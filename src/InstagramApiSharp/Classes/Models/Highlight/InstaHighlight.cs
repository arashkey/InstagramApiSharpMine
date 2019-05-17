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
    public class InstaHighlightFeeds
    {
        public bool ShowEmptyState { get; set; }

        internal string Status { get; set; }

        public List<InstaHighlightFeed> Items { get; set; } = new List<InstaHighlightFeed>();

        public InstaTVSelfChannel TVChannel { get; set; }
    }
    public class InstaHighlightSingleFeed : InstaHighlightFeed { }
    public class InstaHighlightFeed : INotifyPropertyChanged
    {
        public List<InstaStoryItem> Items { get; set; } = new List<InstaStoryItem>();
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public string HighlightId { get; set; }

        public int LatestReelMedia { get; set; }

        object _seen = null;
        public object Seen { get { return _seen; } set { _seen = value; OnPropertyChanged("Seen"); } }
        
        public bool CanReply { get; set; }

        public object CanReshare { get; set; }
        public System.DateTime CreatedAt { get; set; }

        public string ReelType { get; set; }

        public InstaHighlightCoverMedia CoverMedia { get; set; }

        public InstaUserShort User { get; set; }

        public int RankedPosition { get; set; }

        string _title = null;
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged("Title"); } }
        public int SeenRankedPosition { get; set; }

        public int PrefetchCount { get; set; }

        public int MediaCount { get; set; }
    }

    public class InstaHighlightCoverMedia
    {
        public InstaImage CroppedImage { get; set; }

        public float[] CropRect { get; set; }

        public string MediaId { get; set; }

        public InstaImage Image { get; set; }
    }
}
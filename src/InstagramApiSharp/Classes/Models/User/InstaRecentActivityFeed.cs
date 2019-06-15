using System;
using System.Collections.Generic;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaRecentActivityFeed
    {
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
    }
}
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaRecipients
    {
        public List<InstaRankedRecipientThread> Threads { get; set; } = new List<InstaRankedRecipientThread>();

        public List<InstaUserShort> Users { get; set; } = new List<InstaUserShort>();
        public List<InstaDirectInboxThread> Items { get; set; } = new List<InstaDirectInboxThread>();

        public long ExpiresIn { get; set; }

        public bool Filtered { get; set; }

        public string RankToken { get; set; }

        public string RequestId { get; set; }
    }
}
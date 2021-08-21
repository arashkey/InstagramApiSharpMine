using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaBanyanSuggestions
    {
        public List<InstaUserShort> Users { get; set; } = new List<InstaUserShort>();

        public List<InstaDirectInboxThread> Threads { get; set; } = new List<InstaDirectInboxThread>();

        public List<InstaDirectInboxThread> Items { get; set; } = new List<InstaDirectInboxThread>();
    }
}

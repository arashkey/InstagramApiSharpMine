using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaBanyanSuggestions
    {
        public List<InstaUserShort> Users { get; set; } = new List<InstaUserShort>();

        public List<InstaDirectInboxThread> Threads { get; set; } = new List<InstaDirectInboxThread>();

        public List<InstaUserShort> Items { get; set; } = new List<InstaUserShort>();
    }
}

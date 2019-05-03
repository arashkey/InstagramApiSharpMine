﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaBanyanSuggestions
    {
        public List<InstaUserShort> Users { get; set; } = new List<InstaUserShort>();

        public List<InstaDirectInboxThread> Threads { get; set; } = new List<InstaDirectInboxThread>();
    }
}

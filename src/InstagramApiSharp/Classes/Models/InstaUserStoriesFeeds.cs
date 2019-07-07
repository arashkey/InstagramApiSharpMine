using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaUserStoriesFeeds
    {
        public List<InstaReelFeed> Items { get; set; } = new List<InstaReelFeed>();
    }

}

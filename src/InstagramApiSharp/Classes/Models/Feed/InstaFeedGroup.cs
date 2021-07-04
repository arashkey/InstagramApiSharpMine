/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaFeedGroup
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ShowGroupText { get; set; }
        public InstaMediaList FeedItems { get; set; } = new InstaMediaList();
        public string NextMaxId { get; set; }
        public string PaginationSource { get; set; }
    }
}

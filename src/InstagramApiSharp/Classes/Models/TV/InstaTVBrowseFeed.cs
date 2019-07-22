/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaTVBrowseFeed
    {
        public InstaTVSelfChannel MyChannel { get; set; }
        public string BannerToken { get; set; }
        public List<InstaMedia> BrowseItems { get; set; } = new List<InstaMedia>();
        public string MaxId { get; set; }
        public bool MoreAvailable { get; set; }
    }
}

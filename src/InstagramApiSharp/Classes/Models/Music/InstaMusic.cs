/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/rmt4006/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaMusic
    {
        public string AudioClusterId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string DisplayArtist { get; set; }
        public string CoverArtworkUri { get; set; }
        public string CoverArtworkThumbnailUri { get; set; }
        public string ProgressiveDownloadUrl { get; set; }
        public List<TimeSpan> HighlightStartTimes { get; set; } = new List<TimeSpan>();
        public bool IsExplicit { get; set; }
        public string DashManifest { get; set; }
        public bool HasLyrics { get; set; }
        public string AudioAssetId { get; set; }
        public TimeSpan Duration { get; set; }
        public object DarkMessage { get; set; }
        public bool AllowsSaving { get; set; }
        public bool IsBookmarked { get; set; }
    }
}

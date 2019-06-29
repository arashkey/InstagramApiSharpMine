/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTVVideoUpload
    {
        public InstaImage VideoThumbnail { get; set; }
        public string SegmentedFolderPath { get; set; }
        public Dictionary<string, byte[]> SegmentedFilesBytes { get; set; } = new Dictionary<string, byte[]>();
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public double Length { get; set; } = 0;
        public bool IsMuted { get; set; } = false;
    }
}

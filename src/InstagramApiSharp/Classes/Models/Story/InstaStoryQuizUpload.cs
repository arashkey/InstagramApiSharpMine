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
    public class InstaStoryQuizUpload
    {
        public double X { get; set; } = 0.5279019;
        public double Y { get; set; } = 0.658784;
        public double Z { get; set; } = 0;
        public double Width { get; set; } = 0.83689034;
        public double Height { get; set; } = 0.3454722;
        public double Rotation { get; set; } = 0.0;
        public string Question { get; set; }
        public List<InstaStoryQuizAnswer> Options { get; set; } = new List<InstaStoryQuizAnswer>();
        public int CorrectAnswer { get; set; } = 0;
        public bool ViewerCanAnswer { get; set; } = true;
        public string TextColor { get; set; } = "#ffffff";
        public string StartBackgroundColor { get; set; } = "#262626";
        public string EndBackgroundColor { get; set; } = "#262626";
        internal int ViewerAnswer { get; set; } = -1;
        internal bool IsSticker { get; set; } = true;
    }


}

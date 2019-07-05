/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryChatUpload
    {
        public double X { get; set; } = 0.5;
        public double Y { get; set; } = 0.5;
        public double Z { get; set; } = 0;

        public double Width { get; set; } = 0.48333332;
        public double Height { get; set; } = 0.21962096;
        public double Rotation { get; set; } = 0.0;

        public string GroupName { get; set; }
        public string StartBackgroundColor { get; set; } = "#262626";
        public string EndBackgroundColor { get; set; } = "#262626";
        internal bool IsSticker { get; set; } = true;
        internal bool HasChatStarted { get; set; } = false;
    }
}

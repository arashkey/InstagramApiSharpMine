/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes
{
    public class InstaVideoUploadOption
    {
        public bool IsMuted { get; set; } = false;
        public int SourceWidth { get; set; } = 0;
        public int SourceHeight { get; set; } = 0;

        internal static InstaVideoUploadOption Empty => new InstaVideoUploadOption();
    }
}

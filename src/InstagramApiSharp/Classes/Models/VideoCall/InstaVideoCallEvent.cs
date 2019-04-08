/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Enums;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaVideoCallEvent
    {
        public InstaVideoCallActionType Action { get; set; }

        public long VcId { get; set; }

        public string EncodedServerDataInfo { get; set; }

        public string Description { get; set; }

        public object[] TextAttributes { get; set; }
    }
}

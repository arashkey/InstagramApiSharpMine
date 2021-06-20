/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Enums;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTwoFactorTrustedNotification : InstaDefaultResponse
    {
        public Insta2FANotificationReviewStatus ReviewStatus => (Insta2FANotificationReviewStatus)ReviewStatusValue;

        [JsonProperty("review_status")]
        public int ReviewStatusValue { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes
{
    public class InstaAccountRegistrationPhoneNumberNew : InstaDefaultResponse
    {
        [JsonProperty("error_source")]
        internal string ErrorSource { get; set; }
        [JsonProperty("error_type")]
        internal string ErrorType { get; set; }
        [JsonProperty("tos_version")]
        public string TosVersion { get; set; }
        [JsonProperty("gdpr_required")]
        public bool GdprRequired { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS (c) 2021, April
 */

using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes
{
    public class InstaSignupConsentConfig : InstaDefaultResponse
    {
        [JsonProperty("age_required")]
        public bool? AgeRequired { get; set; }

        [JsonProperty("gdpr_required")]
        public bool? GdprRequired { get; set; }

        [JsonProperty("tos_acceptance_not_required")]
        public bool? TosAcceptanceNotRequired { get; set; }
    }
}

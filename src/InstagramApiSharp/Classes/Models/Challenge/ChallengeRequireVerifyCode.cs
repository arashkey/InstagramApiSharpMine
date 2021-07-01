/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes
{
    public class InstaChallengeRequireVerifyCode : InstaDefaultResponse
    {
        [JsonIgnore]
        public bool IsLoggedIn { get { return LoggedInUser != null || IsSucceed; } }
        [JsonProperty("logged_in_user")]
        public /*InstaUserInfoResponse*/InstaUserShortResponse LoggedInUser { get; set; }
        [JsonProperty("action")]
        internal string Action { get; set; }
    }
}

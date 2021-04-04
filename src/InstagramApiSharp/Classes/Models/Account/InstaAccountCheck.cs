/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaAccountCheck : InstaDefaultResponse
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("available")]
        public bool Available { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("existing_user_password")]
        public bool? ExistingUserPassword { get; set; }
        [JsonProperty("error_type")]
        public string ErrorType { get; set; }
    }
}

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
    public class InstaRegistrationConfirmationCode : InstaDefaultResponse
    {
        [JsonProperty("signup_code")]
        public string SignupCode { get; set; }
    }
}

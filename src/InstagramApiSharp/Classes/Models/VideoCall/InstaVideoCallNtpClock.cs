/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaVideoCallNtpClock
    {
        [JsonProperty("client_time")]
        public long ClientTime { get; set; }
        [JsonProperty("request_time")]
        public long requestTime { get; set; }
        [JsonProperty("server_time")]
        public long ServerTime { get; set; }
        [JsonProperty("status")]
        internal string status { get; set; }
    }

}

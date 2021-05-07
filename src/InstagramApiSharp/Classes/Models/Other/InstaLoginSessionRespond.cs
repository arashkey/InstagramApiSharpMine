/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System.Collections.Generic;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaLoginSessionRespond : InstaDefaultResponse
    {
        [JsonProperty("suspicious_logins")]
        public List<InstaLoginSessionSuspiciousLogin> SuspiciousLogins { get; set; } = new List<InstaLoginSessionSuspiciousLogin>();
        [JsonProperty("sessions")]
        public List<InstaLoginSession> Sessions { get; set; } = new List<InstaLoginSession>();
    }

    public class InstaLoginSessionSuspiciousLogin
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        [JsonProperty("device")]
        public string Device { get; set; }
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }

    public class InstaLoginSession
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        [JsonProperty("device")]
        public string Device { get; set; }
        [JsonProperty("timestamp")]
        public double Timestamp { get; set; }
        [JsonProperty("login_timestamp")]
        public double LoginTimestamp { get; set; }
        [JsonProperty("is_current")]
        public bool IsCurrent { get; set; }
        [JsonProperty("login_id")]
        public long LoginId { get; set; }
    }

}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaTrustedDeviceResponse
    {
        [JsonProperty("last_login_time")]
        public long LastLoginTime { get; set; }
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        [JsonProperty("last_login_location")]
        public string LastLoginLocation { get; set; }
        [JsonProperty("device_guid")]
        public string DeviceGuid { get; set; }
        [JsonProperty("device_type")]
        public string DeviceType { get; set; }
        [JsonProperty("device_name")]
        public string DeviceName { get; set; }
        [JsonProperty("is_current")]
        public bool IsCurrent { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTrustedDevice
    {
        public System.DateTime LastLoginTime { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string LastLoginLocation { get; set; }

        public string DeviceGuid { get; set; }

        public string DeviceType { get; set; }

        public string DeviceName { get; set; }

        public bool IsCurrent { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Linq;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaTrustedDeviceConverter : IObjectConverter<InstaTrustedDevice, InstaTrustedDeviceResponse>
    {
        public InstaTrustedDeviceResponse SourceObject { get; set; }

        public InstaTrustedDevice Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var trustedDevice = new InstaTrustedDevice
            {
                DeviceGuid = SourceObject.DeviceGuid,
                DeviceName = SourceObject.DeviceName,
                DeviceType = SourceObject.DeviceType,
                IsCurrent = SourceObject.IsCurrent,
                LastLoginLocation = SourceObject.LastLoginLocation,
                Latitude = SourceObject.Latitude,
                Longitude = SourceObject.Longitude,
                LastLoginTime = SourceObject.LastLoginTime.FromUnixTimeSeconds()
            };

            return trustedDevice;
        }
    }
}

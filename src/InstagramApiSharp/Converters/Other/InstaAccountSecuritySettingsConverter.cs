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
    internal class InstaAccountSecuritySettingsConverter : IObjectConverter<InstaAccountSecuritySettings, InstaAccountSecuritySettingsResponse>
    {
        public InstaAccountSecuritySettingsResponse SourceObject { get; set; }

        public InstaAccountSecuritySettings Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var securitySettings = new InstaAccountSecuritySettings
            {
                CountryCode = SourceObject.CountryCode,
                IsPhoneConfirmed = SourceObject.IsPhoneConfirmed,
                IsTotpTwoFactorEnabled = SourceObject.IsTotpTwoFactorEnabled,
                IsTwoFactorEnabled = SourceObject.IsTwoFactorEnabled,
                NationalNumber = SourceObject.NationalNumber,
                PhoneNumber = SourceObject.PhoneNumber
            };
            if (SourceObject.BackupCodes?.Count > 0)
                securitySettings.BackupCodes = SourceObject.BackupCodes;
            try
            {
                if (SourceObject.TrustedDevices?.Count > 0)
                {
                    foreach (var device in SourceObject.TrustedDevices)
                        securitySettings.TrustedDevices.Add(ConvertersFabric.Instance.GetTrustedDeviceConverter(device).Convert());
                }
            }
            catch { }
            return securitySettings;
        }
    }
}


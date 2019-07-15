/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaAccountSecuritySettings
    {
        public string PhoneNumber { get; set; }

        public int CountryCode { get; set; }

        public long NationalNumber { get; set; }

        public bool IsPhoneConfirmed { get; set; }

        public bool IsTwoFactorEnabled { get; set; }

        public bool IsTotpTwoFactorEnabled { get; set; }

        public List<string> BackupCodes { get; set; } = new List<string>();

        public List<InstaTrustedDevice> TrustedDevices { get; set; } = new List<InstaTrustedDevice>();
    }
}

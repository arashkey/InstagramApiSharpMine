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
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaMerchantConverter : IObjectConverter<InstaMerchant, InstaMerchantResponse>
    {
        public InstaMerchantResponse SourceObject { get; set; }

        public InstaMerchant Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var merchant = new InstaMerchant
            {
                Pk = SourceObject.Pk,
                ProfilePicture = SourceObject.ProfilePicture,
                Username = SourceObject.Username
            };
            return merchant;
        }
    }
}

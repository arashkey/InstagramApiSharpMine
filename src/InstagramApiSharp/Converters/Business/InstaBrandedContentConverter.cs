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
using System.Linq;

namespace InstagramApiSharp.Converters
{
    internal class InstaBrandedContentConverter : IObjectConverter<InstaBrandedContent, InstaBrandedContentResponse>
    {
        public InstaBrandedContentResponse SourceObject { get; set; }

        public InstaBrandedContent Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var brandedContent = new InstaBrandedContent
            {
                RequireApproval = SourceObject.RequireApproval
            };
            if (SourceObject.WhitelistedUsers != null && SourceObject.WhitelistedUsers.Any())
            {
                foreach (var item in SourceObject.WhitelistedUsers)
                {
                    try
                    {
                        brandedContent.WhitelistedUsers.Add(ConvertersFabric.Instance.GetUserShortConverter(item).Convert());
                    }
                    catch { }
                }
            }
            return brandedContent;
        }
    }
}

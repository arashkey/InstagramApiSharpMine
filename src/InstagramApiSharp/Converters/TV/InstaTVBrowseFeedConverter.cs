/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;
using System.Linq;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.ResponseWrappers.Business;

namespace InstagramApiSharp.Converters
{
    internal class InstaTVBrowseFeedConverter : IObjectConverter<InstaTVBrowseFeed, InstaTVBrowseFeedResponse>
    {
        public InstaTVBrowseFeedResponse SourceObject { get; set; }

        public InstaTVBrowseFeed Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var browseFeed = new InstaTVBrowseFeed
            {
                BannerToken = SourceObject.BannerToken,
                MaxId = SourceObject.MaxId,
                MoreAvailable = SourceObject.MoreAvailable
            };
            try
            {
                if (SourceObject.MyChannel != null)
                    browseFeed.MyChannel = ConvertersFabric.Instance.GetTVSelfChannelConverter(SourceObject.MyChannel).Convert();
            }
            catch { }
            try
            {
                if (SourceObject.BrowseItems?.Count > 0)
                {
                    foreach (var item in SourceObject.BrowseItems)
                    {
                        if (item.Item != null)
                            browseFeed.BrowseItems.Add(ConvertersFabric.Instance.GetSingleMediaConverter(item.Item).Convert());
                    }
                }
            }
            catch { }
            return browseFeed;
        }
    }
}

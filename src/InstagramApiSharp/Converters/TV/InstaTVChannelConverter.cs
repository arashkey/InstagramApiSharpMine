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
    internal class InstaTVChannelConverter : IObjectConverter<InstaTVChannel, InstaTVChannelResponse>
    {
        public InstaTVChannelResponse SourceObject { get; set; }

        public InstaTVChannel Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var channel = new InstaTVChannel
            {
                HasMoreAvailable = SourceObject.HasMoreAvailable,
                Id = SourceObject.Id,
                MaxId = SourceObject.MaxId,
                Title = SourceObject.Title,
                Type = SourceObject.Type,
                ApproxTotalVideos = SourceObject.ApproxTotalVideos ?? 0,
                Description = SourceObject.Description
            };

            if (SourceObject.Items != null && SourceObject.Items.Any())
            {
                foreach (var item in SourceObject.Items)
                {
                    try
                    {
                        channel.Items.Add(ConvertersFabric.Instance.GetSingleMediaConverter(item).Convert());
                    }
                    catch { }
                }
            }
            if (SourceObject.UserDetail != null)
            {
                try
                {
                    channel.UserDetail = ConvertersFabric.Instance.GetTVUserConverter(SourceObject.UserDetail).Convert();
                }
                catch { }
            }
            return channel;
        }
    }
}

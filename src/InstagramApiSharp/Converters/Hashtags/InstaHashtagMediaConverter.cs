﻿/*
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


namespace InstagramApiSharp.Converters.Hashtags
{
    internal class InstaHashtagMediaConverter : IObjectConverter<InstaSectionMedia, InstaSectionMediaListResponse>
    {
        public InstaSectionMediaListResponse SourceObject { get; set; }

        public InstaSectionMedia Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var media = new InstaSectionMedia
            {
                AutoLoadMoreEnabled = SourceObject.AutoLoadMoreEnabled ?? false,
                MoreAvailable = SourceObject.MoreAvailable,
                NextMaxId = SourceObject.NextMaxId,
                NextMediaIds = SourceObject.NextMediaIds,
                NextPage = SourceObject.NextPage ?? 0
            };
            if (SourceObject.Sections?.Count > 0)
            {
                foreach (var section in SourceObject.Sections)
                {
                    try
                    {
                        if (section.FeedType == "channel")
                        {
                            if (section.LayoutContent.ChannelContainer?.Channel != null)
                                media.Channel = ConvertersFabric.Instance.GetChannelConverter(section.LayoutContent.ChannelContainer.Channel).Convert();
                            else if (section.LayoutContent.ChannelContainerX2?.Channel != null)
                                media.Channel = ConvertersFabric.Instance.GetChannelConverter(section.LayoutContent.ChannelContainerX2.Channel).Convert();
                        }
                        else if (section.FeedType == "media")
                        {
                            if (section.LayoutContent?.Medias?.Count > 0)
                                foreach (var item in section.LayoutContent.Medias)
                            {
                                try
                                {
                                    media.Medias.Add(ConvertersFabric.Instance.GetSingleMediaConverter(item.Media).Convert());

                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }

            if (SourceObject.PersistentSections?.Count > 0)
            {
                try
                {
                    foreach (var section in SourceObject.PersistentSections)
                    {
                        if (section.LayoutContent?.Related?.Count > 0)
                        {
                            foreach (var related in section.LayoutContent.Related)
                            {
                                try
                                {
                                    media.RelatedHashtags.Add(ConvertersFabric.Instance.GetRelatedHashtagConverter(related).Convert());
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
            return media;
        }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/rmt4006/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaTrendingMusicConverter : IObjectConverter<InstaTrendingMusic, InstaTrendingMusicResponse>
    {
        public InstaTrendingMusicResponse SourceObject { get; set; }

        public InstaTrendingMusic Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var trending = new InstaTrendingMusic
            {
                AlacornSessionId = SourceObject.AlacornSessionId,
                DarkBannerMessage = SourceObject.DarkBannerMessage,
                MusicReels = SourceObject.MusicReels,
            };
            try
            {
                if (SourceObject.PageInfo != null)
                {
                    trending.MoreAvailable = SourceObject.PageInfo.MoreAvailable ?? false;
                    trending.AutoLoadMoreAvailable = SourceObject.PageInfo.AutoLoadMoreAvailable ?? false;
                    trending.NextMaxId = SourceObject.PageInfo.NextMaxId;
                }
                else
                {
                    trending.AutoLoadMoreAvailable = trending.MoreAvailable = false;
                    trending.NextMaxId = null;
                }
            }
            catch { }
            try
            {
                InstaMusic Convert(InstaMusicResponse music, InstaMusicTrackMetadataResponse meta)
                {
                    music.IsBookmarked = meta != null && (meta.IsBookmarked ?? false);
                    return ConvertersFabric.Instance.GetMusicConverter(music).Convert();
                }

                if (SourceObject.Items?.Count > 0)
                    for (int i = 0; i < SourceObject.Items.Count; i++)
                    {
                        try
                        {
                            var item = SourceObject.Items[i];
                            trending.Items.Add(Convert(item.Track, item.Metadata));
                        }
                        catch { }
                    }
            }
            catch { }
            return trending;
        }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaMusicPlaylistConverter : IObjectConverter<InstaMusicPlaylist, InstaMusicPlaylistResponse>
    {
        public InstaMusicPlaylistResponse SourceObject { get; set; }

        public InstaMusicPlaylist Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var playlist = new InstaMusicPlaylist
            {
                IconUrl = SourceObject.IconUrl,
                Id = SourceObject.Id,
                Title = SourceObject.Title,
            };
            if (SourceObject.PreviewItems?.Count > 0)
            {
                try
                {
                    InstaMusic ConvertToMusic(InstaMusicResponse music, InstaMusicTrackMetadataResponse meta)
                    {
                        music.IsBookmarked = meta != null && (meta.IsBookmarked ?? false);
                        return ConvertersFabric.Instance.GetMusicConverter(music).Convert();
                    }
                    InstaMusicPlaylist ConvertToPlaylist(InstaMusicPlaylistResponse playlistResponse)
                    {
                        return ConvertersFabric.Instance.GetMusicPlaylistConverter(playlistResponse).Convert();
                    }
                    for (int i = 0; i < SourceObject.PreviewItems.Count; i++)
                    {
                        try
                        {
                            var item = SourceObject.PreviewItems[i];
                            var music = new InstaMusicList();
                            if (item.Track != null)
                                music.Music = ConvertToMusic(item.Track, item.Metadata);
                            if (item.Playlist != null)
                                music.Playlist = ConvertToPlaylist(item.Playlist);

                            playlist.PreviewItems.Add(music);
                        }
                        catch { }
                    }
                }
                catch { }
            }
            return playlist;
        }
    }
}

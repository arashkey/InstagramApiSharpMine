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
    internal class InstaBrowseMusicConverter : IObjectConverter<InstaBrowseMusic, InstaBrowseMusicResponse>
    {
        public InstaBrowseMusicResponse SourceObject { get; set; }

        public InstaBrowseMusic Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var browse = new InstaBrowseMusic();
            InstaMusicPlaylist ConvertToPlaylist(InstaMusicPlaylistResponse playlistResponse)
            {
                return ConvertersFabric.Instance.GetMusicPlaylistConverter(playlistResponse).Convert();
            }

            if (SourceObject.Items?.Count > 0)
            {
                for (int i = 0; i < SourceObject.Items.Count; i++)
                {
                    try
                    {
                        var item = SourceObject.Items[i];
                        var music = new InstaBrowseMusicItem();

                        if (item.Category != null)
                        {
                            music.Category = ConvertToPlaylist(item.Category);
                        }

                        if (item.Playlist != null)
                        {
                            music.Playlist = ConvertToPlaylist(item.Playlist);
                        }

                        browse.Items.Add(music);
                    }
                    catch { }
                }
            }
            return browse;
        }
    }
}

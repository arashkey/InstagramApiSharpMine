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
    internal class InstaMusicConverter : IObjectConverter<InstaMusic, InstaMusicResponse>
    {
        public InstaMusicResponse SourceObject { get; set; }

        public InstaMusic Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException("SourceObject");

            var music = new InstaMusic
            {
                Subtitle = SourceObject.Subtitle,
                AllowsSaving = SourceObject.AllowsSaving ?? false,
                AudioAssetId = SourceObject.AudioAssetId,
                AudioClusterId = SourceObject.AudioClusterId,
                CoverArtworkThumbnailUri = SourceObject.CoverArtworkThumbnailUri,
                CoverArtworkUri = SourceObject.CoverArtworkUri,
                DarkMessage = SourceObject.DarkMessage,
                DashManifest = SourceObject.DashManifest,
                DisplayArtist = SourceObject.DisplayArtist,
                Duration = TimeSpan.FromMilliseconds(SourceObject.DurationInMs ?? 0),
                HasLyrics = SourceObject.HasLyrics ?? false,
                Id = SourceObject.Id,
                IsBookmarked = SourceObject.IsBookmarked,
                IsExplicit = SourceObject.IsExplicit ?? false,
                ProgressiveDownloadUrl = SourceObject.ProgressiveDownloadUrl,
                Title = SourceObject.Title,
            };
            if (SourceObject.HighlightStartTimesInMs?.Length > 0)
                foreach (var item in SourceObject.HighlightStartTimesInMs)
                    music.HighlightStartTimes.Add(TimeSpan.FromMilliseconds(item ?? 0));
            return music;
        }
    }
}

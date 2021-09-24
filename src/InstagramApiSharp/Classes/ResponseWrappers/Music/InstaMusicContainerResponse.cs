/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/rmt4006/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaMusicContainerResponse
    {
        [JsonProperty("track")]
        public InstaMusicResponse Track { get; set; }
        [JsonProperty("metadata")]
        public InstaMusicTrackMetadataResponse Metadata { get; set; }
    }

    public class InstaMusicResponse
    {
        [JsonProperty("audio_cluster_id")]
        public string AudioClusterId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        [JsonProperty("display_artist")]
        public string DisplayArtist { get; set; }
        [JsonProperty("cover_artwork_uri")]
        public string CoverArtworkUri { get; set; }
        [JsonProperty("cover_artwork_thumbnail_uri")]
        public string CoverArtworkThumbnailUri { get; set; }
        [JsonProperty("progressive_download_url")]
        public string ProgressiveDownloadUrl { get; set; }
        [JsonProperty("highlight_start_times_in_ms")]
        public double?[] HighlightStartTimesInMs { get; set; }
        [JsonProperty("is_explicit")]
        public bool? IsExplicit { get; set; }
        [JsonProperty("dash_manifest")]
        public string DashManifest { get; set; }
        [JsonProperty("has_lyrics")]
        public bool? HasLyrics { get; set; }
        [JsonProperty("audio_asset_id")]
        public string AudioAssetId { get; set; }
        [JsonProperty("duration_in_ms")]
        public double? DurationInMs { get; set; }
        [JsonProperty("dark_message")]
        public object DarkMessage { get; set; }
        [JsonProperty("allows_saving")]
        public bool? AllowsSaving { get; set; }
        [JsonIgnore()]
        public bool IsBookmarked { get; set; }
    }

    public class InstaMusicTrackMetadataResponse
    {
        [JsonProperty("is_bookmarked")]
        public bool? IsBookmarked { get; set; }
    }
}

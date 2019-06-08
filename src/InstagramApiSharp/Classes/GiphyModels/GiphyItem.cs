/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes
{
    public class GiphyItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("bitly_gif_url")]
        public string BitlyGifUrl { get; set; }
        [JsonProperty("bitly_url")]
        public string BitlyUrl { get; set; }
        [JsonProperty("embed_url")]
        public string EmbedUrl { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("rating")]
        public string Rating { get; set; }
        [JsonProperty("source_post_url")]
        public string SourcePostUrl { get; set; }
        [JsonProperty("is_sticker")]
        public int IsSticker { get; set; }
        [JsonProperty("import_datetime")]
        public string ImportDatetime { get; set; }
        [JsonProperty("trending_datetime")]
        public string TrendingDatetime { get; set; }
        [JsonProperty("user")]
        public GiphyUser User { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("images")]
        public GiphyImages Images { get; set; }
    }

    public class GiphyUser
    {
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonProperty("banner_url")]
        public string BannerUrl { get; set; }
        [JsonProperty("banner_image")]
        public string BannerImage { get; set; }
        [JsonProperty("profile_url")]
        public string ProfileUrl { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }
    }




    public class GiphyImages
    {
        [JsonProperty("original")]
        public GiphyFile Original { get; set; }
        [JsonProperty("fixed_height")]
        public GiphyFile FixedHeight { get; set; }
        [JsonProperty("fixed_width")]
        public GiphyFile FixedWidth { get; set; }
    }

    public class GiphyFile
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public string Width { get; set; }
        [JsonProperty("height")]
        public string Height { get; set; }
        [JsonProperty("size")]
        public string Size { get; set; }
        [JsonProperty("frames")]
        public string Frames { get; set; }
        [JsonProperty("mp4")]
        public string Mp4 { get; set; }
        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }
        [JsonProperty("webp")]
        public string Webp { get; set; }
        [JsonProperty("webp_size")]
        public string WebpSize { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }

}

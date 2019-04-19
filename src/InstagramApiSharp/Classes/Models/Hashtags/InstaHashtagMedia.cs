using System;
using System.Collections.Generic;
using System.Text;
using InstagramApiSharp.Classes.ResponseWrappers;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaHashtagMedia
    {
        [JsonProperty("pk")]
        public long Pk { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        [JsonProperty("hashtag")]
        public InstaHashtagMediaHashtag Hashtag { get; set; }
        [JsonProperty("media_infos")]
        public List<InstaHashtagMediaInfo> MediaInfos { get; set; } = new List<InstaHashtagMediaInfo>();
        [JsonProperty("show_profile_pic")]
        public bool ShowProfilePicture { get; set; }
        [JsonProperty("context_type")]
        public string ContextType { get; set; }
    }

    public class InstaHashtagMediaHashtag
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("media_count")]
        public int MediaCount { get; set; }
        [JsonProperty("follow_status")]
        public int FollowStatus { get; set; }
        [JsonProperty("profile_pic_url")]
        public string ProfilePicture { get; set; }
        public Uri ProfilePictureUri => new Uri(ProfilePicture);
    }

    public class InstaHashtagMediaInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("media_type")]
        public InstaMediaType MediaType { get; set; }
        [JsonProperty("image_versions2")] public InstaImageCandidatesResponse Images { get; set; }
        [JsonProperty("original_width")] public string Width { get; set; }
        [JsonProperty("original_height")] public string Height { get; set; }
    }

}

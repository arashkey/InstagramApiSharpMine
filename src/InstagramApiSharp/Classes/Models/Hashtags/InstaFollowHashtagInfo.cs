using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaFollowHashtagInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("profile_pic_url")]
        public string ProfilePicture { get; set; }
    }
}

using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaAllCatchedUp
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
    }
}

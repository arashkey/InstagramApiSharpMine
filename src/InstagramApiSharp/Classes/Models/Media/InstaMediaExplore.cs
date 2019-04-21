using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaMediaExplore
    {
        [JsonProperty("explanation")]
        public string Explanation { get; set; }
        [JsonProperty("actor_id")]
        public long? ActorId { get; set; }
        [JsonProperty("source_token")]
        public string SourceToken { get; set; }
    }
}

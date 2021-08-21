using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaTagFeedResponse : InstaMediaListResponse
    {
        [JsonProperty("ranked_items")]
        public List<InstaMediaItemResponse> RankedItems { get; set; } = new List<InstaMediaItemResponse>();
    }
}
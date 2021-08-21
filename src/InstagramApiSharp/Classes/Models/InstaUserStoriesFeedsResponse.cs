using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaUserStoriesFeedsResponse
    {
        [JsonIgnore()]
        public List<InstaReelFeedResponse> Items = new List<InstaReelFeedResponse>();
    }
}

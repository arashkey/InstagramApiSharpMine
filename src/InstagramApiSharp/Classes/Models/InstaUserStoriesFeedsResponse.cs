using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaUserStoriesFeedsResponse
    {
        [JsonIgnore()]
        public List<InstaReelFeedResponse> Items = new List<InstaReelFeedResponse>();
    }
}

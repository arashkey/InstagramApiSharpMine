using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaLocationSearchResponse
    {
        [JsonProperty("venues")] public List<InstaLocationShortResponse> Locations { get; set; }

        [JsonProperty("request_id")] public string RequestId { get; set; }

        [JsonProperty("status")] public string Status { get; set; }
    }
}
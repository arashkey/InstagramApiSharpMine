using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaMediaLikersResponse : BadStatusResponse
    {
        [JsonProperty("users")] public List<InstaUserShortResponse> Users { get; set; }

        [JsonProperty("user_count")] public int UsersCount { get; set; }
    }
}
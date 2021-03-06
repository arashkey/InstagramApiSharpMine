/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaDiscoverRecentSearchesResponse
    {
        [JsonProperty("recent")]
        public List<InstaDiscoverRecentSearchesItemResponse> Recent { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
    public class InstaDiscoverRecentSearchesItemResponse
    {
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("user")]
        public InstaUserShortFriendshipResponse User { get; set; }
        [JsonProperty("client_time")]
        public int? ClientTime { get; set; }
        [JsonProperty("hashtag")]
        public InstaHashtagShortResponse Hashtag { get; set; }
    }
}

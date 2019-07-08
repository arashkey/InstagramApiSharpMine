/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaPendingRequestResponse
    {
        [JsonProperty("next_max_id")]
        public string NextMaxId { get; set; }
        [JsonProperty("big_list")]
        public bool BigList { get; set; }
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("suggested_users")]
        public InstaPendingSuggestedUsersResponse SuggestedUsers { get; set; }
        [JsonProperty("truncate_follow_requests_at_index")]
        public int TruncateFollowRequestsAtIndex { get; set; }
        [JsonProperty("users")]
        public List<InstaUserShortFriendshipResponse> Users { get; set; }
    }

    public class InstaPendingSuggestedUsersResponse
    {
        [JsonProperty("netego_type")]
        public string NetegoType { get; set; }
        [JsonProperty("suggestions")]
        public InstaSuggestionItemListResponse Suggestions { get; set; }
    }
}

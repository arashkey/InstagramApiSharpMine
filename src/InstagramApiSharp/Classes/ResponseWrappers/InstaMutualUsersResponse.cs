/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaMutualUsersResponse
    {
        [JsonProperty("suggested_users")]
        public List<InstaSuggestionItemResponse> SuggestedUsers { get; set; } = new List<InstaSuggestionItemResponse>();
        [JsonProperty("show_see_all_followers_button")]
        public bool? ShowSeeAllFollowersButton { get; set; }
        [JsonProperty("mutual_followers")]
        public List<InstaUserShortFriendshipResponse> MutualFollowers { get; set; } = new List<InstaUserShortFriendshipResponse>();
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}

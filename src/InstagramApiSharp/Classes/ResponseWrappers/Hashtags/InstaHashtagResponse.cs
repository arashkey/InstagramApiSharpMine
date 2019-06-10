/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaHashtagResponse
    {
        [JsonProperty("id")] public long Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("media_count")] public long MediaCount { get; set; }

        [JsonProperty("profile_pic_url")] public string ProfilePicture { get; set; }

        [JsonProperty("follow_status")] public long? FollowStatus { get; set; }
        [JsonProperty("following")] public long? Following { get; set; }
        [JsonProperty("non_violating")] public long? NonViolating { get; set; }
        [JsonProperty("allow_following")] public bool? AllowFollowing { get; set; }
        [JsonProperty("formatted_media_count")] public string FormattedMediaCount { get; set; }
        [JsonProperty("search_result_subtitle")] public string SearchResultSubtitle { get; set; }
        [JsonProperty("show_follow_drop_down")] public bool? ShowFollowDropDown { get; set; }
        [JsonProperty("allow_muting_story")] public bool? AllowMutingStory { get; set; }
        [JsonProperty("social_context")] public string SocialContext { get; set; }
        [JsonProperty("subtitle")] public string Subtitle { get; set; }
    }
}
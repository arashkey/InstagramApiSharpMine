using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaRecentActivityStoryItemResponse
    {
        [JsonProperty("profile_id")] public long ProfileId { get; set; }

        [JsonProperty("profile_image")] public string ProfileImage { get; set; }

        [JsonProperty("timestamp")] public string TimeStamp { get; set; }

        [JsonProperty("inline_follow")] public InstaInlineFollowResponse InlineFollow { get; set; }

        [JsonProperty("text")] public string Text { get; set; }

        [JsonProperty("rich_text")] public string RichText { get; set; }

        [JsonProperty("links")] public List<InstaLinkResponse> Links { get; set; }

        [JsonProperty("media")] public List<InstaActivityMediaResponse> Media { get; set; }

        [JsonProperty("second_profile_id")] public long? SecondProfileId { get; set; }

        [JsonProperty("second_profile_image")] public string SecondProfileImage { get; set; }

        [JsonProperty("profile_name")] public string ProfileName { get; set; }

        [JsonProperty("profile_image_destination")] public string ProfileImageDestination { get; set; }

        [JsonProperty("hashtag_follow")] public InstaHashtagResponse HashtagFollow { get; set; }
        [JsonProperty("destination")] public string Destination { get; set; }

        [JsonProperty("comment_id")] public long? CommentId { get; set; }
        [JsonProperty("comment_ids")] public List<long> CommentIds { get; set; } = new List<long>();
        [JsonProperty("request_count")] public int RequestCount { get; set; } = 0;
        [JsonProperty("sub_text")] public string SubText { get; set; }
        [JsonProperty("icon_url")] public string IconUrl { get; set; }
        [JsonProperty("has_liked_comment")] public bool? HasLikedComment { get; set; }
        [JsonProperty("display_ufi")] public bool? DisplayUfi { get; set; }
        [JsonProperty("af_candidate_id")] public string AfCandidateId { get; set; }
        [JsonProperty("should_icon_apply_filter")] public bool? ShouldIconApplyFilter { get; set; }
        [JsonProperty("icon_should_apply_filter")] public bool? IconShouldApplyFilter { get; set; }
        [JsonProperty("clicked")] public bool? Clicked { get; set; }
        [JsonProperty("extra")] public InstaActivityStoryItemExtraResponse Extra { get; set; }
        [JsonProperty("comment_notif_type")] public string CommentNotifType { get; set; }
        [JsonProperty("tuuid")] public string Tuuid { get; set; }
        [JsonProperty("actions")] public List<string> Actions { get; set; } = new List<string>();
        [JsonProperty("latest_reel_media")] public long? LatestReelMedia { get; set; }
        [JsonProperty("images")] public List<InstaActivityMediaResponse> Images { get; set; }
        [JsonProperty("friendship_status")] internal InstaRecentActivityStoryItemFriendshipStatusResponse FriendshipStatus { get; set; }

    }
    internal class InstaRecentActivityStoryItemFriendshipStatusResponse
    {
        [JsonProperty("is_restricted")] public bool? IsRestricted { get; set; }
    }
    public class InstaActivityStoryItemExtraResponse
    {
        [JsonProperty("lat")] public long? Latitude { get; set; }
        [JsonProperty("long")] public long? Longitude { get; set; }
    }
}
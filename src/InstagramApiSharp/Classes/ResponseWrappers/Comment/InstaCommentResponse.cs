using Newtonsoft.Json;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaCommentResponse
    {
        [JsonProperty("type")] public int Type { get; set; }

        [JsonProperty("bit_flags")] public int BitFlags { get; set; }

        [JsonProperty("user_id")] public long UserId { get; set; }

        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("created_at_utc")] public string CreatedAtUtc { get; set; }

        [JsonProperty("comment_like_count")] public int LikesCount { get; set; }

        [JsonProperty("created_at")] public string CreatedAt { get; set; }

        [JsonProperty("content_type")] public string ContentType { get; set; }

        [JsonProperty("user")] public InstaUserShortResponse User { get; set; }

        [JsonProperty("pk")] public long Pk { get; set; }

        [JsonProperty("text")] public string Text { get; set; }

        [JsonProperty("did_report_as_spam")] public bool DidReportAsSpam { get; set; }

        [JsonProperty("has_liked_comment")] public bool HasLikedComment { get; set; }

        [JsonProperty("child_comment_count")] public int ChildCommentCount { get; set; }

        [JsonProperty("num_tail_child_comments")] public int? NumTailChildComments { get; set; }

        [JsonProperty("has_more_tail_child_comments")] public bool HasMoreTailChildComments { get; set; }

        [JsonProperty("has_more_head_child_comments")] public bool HasMoreHeadChildComments { get; set; }

        //[JsonProperty("next_max_child_cursor")] public string NextMaxChildCursor { get; set; }

        [JsonProperty("preview_child_comments")] public List<InstaCommentShortResponse> PreviewChildComments { get; set; } 

        [JsonProperty("other_preview_users")] public List<InstaUserShortResponse> OtherPreviewUsers { get; set; }


        [JsonProperty("share_enabled")] public bool? ShareEnabled { get; set; }
        [JsonProperty("comment_index")] public int? CommentIndex { get; set; }
        [JsonProperty("parent_comment_id")] public long? ParentCommentId { get; set; }
        [JsonProperty("has_translation")] public bool? HasTranslation { get; set; }
    }
}
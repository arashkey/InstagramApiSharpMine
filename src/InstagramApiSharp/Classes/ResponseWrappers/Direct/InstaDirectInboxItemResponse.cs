﻿using System.Collections.Generic;
using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaDirectInboxItemContainerResponse
    {
        [JsonProperty("items")] public List<InstaDirectInboxItemResponse> Items { get; set; } = new List<InstaDirectInboxItemResponse>();
    }
    public class InstaDirectInboxItemResponse : BaseStatusResponse
    { 
        [JsonProperty("text")] public string Text { get; set; }

        [JsonProperty("like")] public string Like { get; set; }

        [JsonProperty("user_id")] public long UserId { get; set; }

        [JsonProperty("timestamp")] public string TimeStamp { get; set; }

        [JsonProperty("item_id")] public string ItemId { get; set; }

        [JsonProperty("item_type")] public string ItemType { get; set; }

        [JsonProperty("media_share")] public InstaMediaItemResponse MediaShare { get; set; }

        [JsonProperty("media")] public InstaInboxMediaResponse Media { get; set; }

        [JsonProperty("link")] public InstaWebLinkResponse Link { get; set; }

        [JsonProperty("client_context")] public string ClientContext { get; set; }

        [JsonProperty("story_share")] public InstaStoryShareResponse StoryShare { get; set; }

        [JsonProperty("raven_media")] public InstaVisualMediaResponse RavenMedia { get; set; }

        [JsonProperty("visual_media")] public InstaVisualMediaContainerResponse VisualMedia { get; set; }
        // raven media properties
        [JsonProperty("view_mode")] public string RavenViewMode { get; set; }

        [JsonProperty("seen_user_ids")] public List<long> RavenSeenUserIds { get; set; }

        [JsonProperty("reply_chain_count")] public int? RavenReplayChainCount { get; set; }

        [JsonProperty("seen_count")] public int RavenSeenCount { get; set; }

        [JsonProperty("expiring_media_action_summary")] public InstaRavenMediaActionSummaryResponse RavenExpiringMediaActionSummary { get; set; }
        // end
        [JsonProperty("action_log")] public InstaActionLogResponse ActionLogMedia { get; set; }

        [JsonProperty("profile")] public InstaUserShortResponse ProfileMedia { get; set; }

        [JsonProperty("preview_medias")] public List<InstaMediaItemResponse> PreviewMedias { get; set; }

        [JsonProperty("placeholder")] public InstaPlaceholderResponse Placeholder { get; set; }

        [JsonProperty("location")] public InstaLocationResponse LocationMedia { get; set; }

        [JsonProperty("felix_share")] public InstaFelixShareResponse FelixShareMedia { get; set; }

        [JsonProperty("reel_share")] public InstaReelShareResponse ReelShareMedia { get; set; }

        [JsonProperty("voice_media")] public InstaVoiceMediaResponse VoiceMedia { get; set; }

        [JsonProperty("animated_media")] public InstaAnimatedImageResponse AnimatedMedia { get; set; }

        [JsonProperty("hashtag")] public InstaDirectHashtagResponse HashtagMedia { get; set; }

        [JsonProperty("live_viewer_invite")] public InstaDirectBroadcastResponse LiveViewerInvite { get; set; }

        [JsonProperty("video_call_event")] public InstaVideoCallEventResponse VideoCallEvent { get; set; }

        [JsonProperty("product_share")] public InstaProductContainerResponse ProductShare { get; set; }

        [JsonProperty("ar_effect")] public InstaDirectArEffectResponse ArEffect { get; set; }

        [JsonProperty("show_forward_attribution")] public bool? ShowForwardAttribution { get; set; }

        [JsonProperty("is_shh_mode")] public bool? IsShhMode { get; set; }

        [JsonProperty("replied_to_message")] public InstaDirectInboxItemResponse RepliedToMessage { get; set; }

        [JsonProperty("reactions")] public InstaDirectReactionResponse Reactions { get; set; }

        [JsonProperty("hide_in_thread")] public int? HideInThread { get; set; }

    }
}
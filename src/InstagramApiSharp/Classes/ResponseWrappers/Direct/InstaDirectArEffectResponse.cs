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
    public class InstaDirectArEffectResponse
    {
        [JsonProperty("data")] public InstaDirectArEffectDataResponse Data { get; set; }
        [JsonProperty("preview_video")] public InstaStoryResponse PreviewVideo { get; set; }
    }

    public class InstaDirectArEffectDataResponse
    {
        [JsonProperty("instagram_direct_effects")]
        public InstaDirectInstagramDirectEffectsResponse InstagramDirectEffects { get; set; }
    }

    public class InstaDirectInstagramDirectEffectsResponse
    {
        [JsonProperty("target_effect_preview")]
        public InstaDirectTargetEffectPreviewResponse TargetEffectPreview { get; set; }
    }

    public class InstaDirectTargetEffectPreviewResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("attribution_user")]
        public InstaDirectAttributionUserResponse AttributionUser { get; set; }
        [JsonProperty("thumbnail_image")]
        public InstaDirectProfileThumbnailResponse ThumbnailImage { get; set; }
        //public Effect_Action_Sheet effect_action_sheet { get; set; }
    }

    public class InstaDirectAttributionUserResponse
    {
        [JsonProperty("instagram_user_id")]
        public string InstagramUserId { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("profile_picture")]
        public InstaDirectProfileThumbnailResponse ProfilePicture { get; set; }
    }

    public class InstaDirectProfileThumbnailResponse
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }

    //public class Effect_Action_Sheet
    //{
    //    public string[] primary_actions { get; set; }
    //    public string[] secondary_actions { get; set; }
    //}


}

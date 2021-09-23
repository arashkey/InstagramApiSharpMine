/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Enums;
using Newtonsoft.Json;
namespace InstagramApiSharp.Classes
{
    [System.Serializable]
    public class InstaChallengeRequireVerifyMethod
    {
        [JsonProperty("step_name")]
        public string StepName { get; set; }
        [JsonProperty("step_data")]
        public InstaChallengeRequireStepData StepData { get; set; }
        [JsonProperty("user_id")]
        public long UserId { get; set; }
        [JsonProperty("nonce_code")]
        public string NonceCode { get; set; }
        [JsonProperty("flow_render_type")]
        public string FlowRender { get; set; }  // int
        [JsonProperty("bloks_action")]
        public string BloksAction { get; set; }
        [JsonProperty("cni")]
        public string Cni { get; set; }             // long
        [JsonProperty("challenge_context")]
        public string ChallengeContext { get; set; }
        [JsonProperty("challenge_type_enum_str")]
        public string ChallengeTypeEnumStr { get; set; }

        [JsonProperty("status")]
        internal string Status { get; set; }
        [JsonProperty("message")]
        internal string Message { get; set; }

        public bool SubmitPhoneRequired => StepName == "submit_phone";
        
        public bool IsUnvettedDelta => ChallengeTypeEnumStr == "UNVETTED_DELTA";

        public InstaChallengeFlowRenderType FlowRenderType => (InstaChallengeFlowRenderType)int.Parse(FlowRender.IsEmpty() ? "0": FlowRender);
        // FAKE DATA>

        [JsonProperty("PerfLoggingId")]
        public string PerfLoggingId { get; set; }

    }
    [System.Serializable]
    public class InstaChallengeRequireStepData
    {
        [JsonProperty("choice")]
        public string Choice { get; set; }
        [JsonProperty("fb_access_token")]
        public string FbAccessToken { get; set; }
        [JsonProperty("big_blue_token")]
        public string BigBlueToken { get; set; }
        [JsonProperty("google_oauth_token")]
        public string GoogleOauthToken { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone_number", NullValueHandling = NullValueHandling.Ignore)]
        public string PhoneNumber { get; set; }
        [JsonProperty("vetted_device")]
        public string VettedDevice { get; set; }
        [JsonProperty("delta_enable_new_ui")]
        public string DeltaEnableNewUi { get; set; }

        public bool SubmitPhoneRequired => (PhoneNumber != null);

        public bool IsNewDeltaUI()
        {
            if (bool.TryParse(DeltaEnableNewUi, out bool b))
                return b;
            else
                return false;
        }
    }

}

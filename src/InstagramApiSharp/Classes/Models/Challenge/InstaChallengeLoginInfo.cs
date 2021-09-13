/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System;

namespace InstagramApiSharp.Classes
{
    [Serializable]
    public class InstaChallengeLoginInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("api_path")]
        public string ApiPath { get; set; }
        [JsonProperty("hide_webview_header")]
        public bool HideWebviewHeader { get; set; }
        [JsonProperty("lock")]
        public bool Lock { get; set; }
        [JsonProperty("logout")]
        public bool Logout { get; set; }
        [JsonProperty("native_flow")]
        public bool NativeFlow { get; set; }
        [JsonProperty("challenge_context")]
        public string ChallengeContext { get; set; }
        [JsonProperty("flow_render_type")]
        public long? FlowRenderType { get; set; }

        public InstaChallengeContext ChallengeContextAsObject
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<InstaChallengeContext>(ChallengeContext);
                }
                catch { return null; }
            }
        }
    }
}

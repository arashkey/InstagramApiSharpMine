/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaBloksDeltaChallengeResponse : InstaDefaultResponse
    {
        [JsonProperty("layout")]
        public InstaBloksDeltaChallengeLayoutResponse Layout { get; set; }
    }

    public class InstaBloksDeltaChallengeLayoutResponse
    {
        [JsonProperty("bloks_payload")]
        public InstaBloksDeltaChallengePayloadResponse BloksPayload { get; set; }
    }

    public class InstaBloksDeltaChallengePayloadResponse
    {
        [JsonProperty("tree")]
        public JToken Tree { get; set; }
    }
}

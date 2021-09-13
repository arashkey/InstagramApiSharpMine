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
    public class InstaChallengeContext
    {
        [JsonProperty("step_name")]
        public string StepName { get; set; }
        [JsonProperty("nonce_code")]
        public string NonceCode { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; } // Long and String
        [JsonProperty("is_stateless")]
        public string IsStateless { get; set; }
        /// <summary>
        ///     <para>
        ///         Null or Empty value => Everything is fine
        ///     </para>
        ///     <para>
        ///         UNVETTED_DELTA => Delta challenge
        ///     </para>
        ///     <para>
        ///         UNKNOWN => Unsolvable challenge, open it in browser [don't forget to add x-mid cookie ]
        ///     </para>
        /// </summary>
        [JsonProperty("challenge_type_enum")]
        public string ChallengeTypeEnumStr { get; set; }
        [JsonProperty("cni")]
        public string Cni { get; set; } // long > 17842656572655492
    }
}

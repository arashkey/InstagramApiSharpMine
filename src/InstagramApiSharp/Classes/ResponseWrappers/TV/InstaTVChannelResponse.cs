/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaTVChannelResponse
    {
        [JsonIgnore]
        public InstaTVChannelType Type { get { return PrivateType.GetChannelType(); } }
        [JsonProperty("type")]
        internal string PrivateType { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("approx_total_videos")]
        public int? ApproxTotalVideos { get; set; }
        [JsonProperty("items")]
        public List<InstaMediaItemResponse> Items { get; set; }
        [JsonProperty("more_available")]
        public bool HasMoreAvailable { get; set; }
        [JsonProperty("max_id")]
        public string MaxId { get; set; }
        //public Seen_State1 seen_state { get; set; }

        [JsonProperty("user_dict")]
        public InstaTVUserResponse UserDetail { get; set; }
    }
    public class InstaTVSelfChannelResponse : InstaTVChannelResponse
    {
        [JsonProperty("user_dict")]
        public InstaTVUserResponse User { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaBroadcastJoin : InstaDefaultResponse
    {
        [JsonProperty("sdp_response")]
        public string SdpResponse { get; set; }
        [JsonProperty("server_data_info")]
        public InstaBroadcastJoinServerDataInfo ServerDataInfo { get; set; }
        [JsonProperty("encoded_server_data_info")]
        public string EncodedServerDataInfo { get; set; }
    }

    public class InstaBroadcastJoinServerDataInfo
    {
        [JsonProperty("cluster")]
        public string Cluster { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("conferenceName")]
        public string ConferenceName { get; set; }
    }
}

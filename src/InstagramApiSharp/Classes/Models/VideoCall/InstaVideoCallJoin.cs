/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaVideoCallJoin
    {
        [JsonProperty("video_call_id")]
        public long VideoCallId { get; set; }
        [JsonProperty("sdp_response")]
        public string SdpResponse { get; set; }
        [JsonProperty("server_data_info")]
        public InstaVideoCallJoinServerDataInfo ServerDataInfo { get; set; }
        [JsonProperty("encoded_server_data_info")]
        public string EncodedServerDataInfo { get; set; }
        //public Media_Status media_status { get; set; }//null
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class InstaVideoCallJoinServerDataInfo
    {
        [JsonProperty("cluster")]
        public string Cluster { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("conferenceName")]
        public string ConferenceName { get; set; }
    }
}

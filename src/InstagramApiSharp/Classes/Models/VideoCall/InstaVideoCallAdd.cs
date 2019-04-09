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
    public class InstaVideoCallAdd
    {
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
        [JsonProperty("video_call_id")]
        public long VideoCallId { get; set; }
        [JsonProperty("encoded_server_data_info")]
        public string EncodedServerDataInfo { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}

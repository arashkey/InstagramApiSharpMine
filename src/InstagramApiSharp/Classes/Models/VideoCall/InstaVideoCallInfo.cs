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
    public class InstaVideoCallInfo
    {
        [JsonProperty("video_call_id")]
        public long VideoCallId { get; set; }
        [JsonProperty("state")]
        public InstaVideoCallInfoState State { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class InstaVideoCallInfoState
    {
        [JsonProperty("is_call_expanded")]
        public bool IsCallExpanded { get; set; }
    }
}

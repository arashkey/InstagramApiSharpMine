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

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaDirectRespondResponse : Models.InstaDefaultResponse
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("status_code")]
        public string StatusCode { get; set; }
        [JsonProperty("payload")]
        public InstaDirectRespondPayloadResponse Payload { get; set; }
    }

    public class InstaDirectRespondPayloadResponse
    {
        [JsonProperty("client_context")]
        public string ClientContext { get; set; }
        [JsonProperty("item_id")]
        public string ItemId { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
    }
}

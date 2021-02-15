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
    public class InstaDirectRespondV2Response : Models.InstaDefaultResponse
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("status_code")]
        public string StatusCode { get; set; }
        [JsonProperty("payload")]
        public List<InstaDirectRespondPayloadResponse> Payload { get; set; }
    }
    public class InstaDirectVoiceRespondResponse : Models.InstaDefaultResponse
    {
        [JsonProperty("upload_id")]
        public string UploadId { get; set; }
        [JsonProperty("message_metadata")]
        public List<InstaDirectRespondPayloadResponse> MessageMetadatas { get; set; } = new List<InstaDirectRespondPayloadResponse>();
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
        [JsonProperty("participant_ids")]
        public long[] ParticipantIds { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("client_facing_error_message")]
        public string ClientFacingErrorMessage { get; set; }
    }
}

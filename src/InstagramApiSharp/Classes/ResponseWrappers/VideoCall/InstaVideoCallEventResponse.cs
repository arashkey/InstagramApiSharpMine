/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaVideoCallEventResponse
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("vc_id")]
        public long? VcId { get; set; }
        [JsonProperty("encoded_server_data_info")]
        public string EncodedServerDataInfo { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("text_attributes")]
        public object[] TextAttributes { get; set; }
    }
}

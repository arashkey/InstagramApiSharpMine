using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTwoFactorTrustedNotification : InstaDefaultResponse
    {
        [JsonProperty("review_status")]
        public int ReviewStatus { get; set; }
    }
}

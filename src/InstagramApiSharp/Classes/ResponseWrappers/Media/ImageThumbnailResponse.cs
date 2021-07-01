using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes
{
    public class ImageThumbnailResponse : InstaDefaultResponse
    {
        [JsonProperty("upload_id")]
        public string UploadId { get; set; }
        [JsonProperty("xsharing_nonces")]
        public object XSharingNonces { get; set; }
    }
}

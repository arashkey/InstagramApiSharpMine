using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaMediaListResponse : BaseLoadableResponse
    {
        [JsonProperty("items")]
        public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();

        public List<InstaStoryResponse> Stories { get; set; } = new List<InstaStoryResponse>();
    }
}
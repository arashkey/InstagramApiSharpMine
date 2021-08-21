using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaExploreItemsResponse : BaseLoadableResponse
    {
        [JsonIgnore] public InstaStoryTrayResponse StoryTray { get; set; } = new InstaStoryTrayResponse();

        [JsonIgnore] public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();

        [JsonIgnore] public InstaChannelResponse Channel { get; set; }
    }
}
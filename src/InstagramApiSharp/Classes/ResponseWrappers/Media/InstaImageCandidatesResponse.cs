using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaImageCandidatesResponse
    {
        [JsonProperty("candidates")] public List<ImageResponse> Candidates { get; set; } = new List<ImageResponse>();
    }
}
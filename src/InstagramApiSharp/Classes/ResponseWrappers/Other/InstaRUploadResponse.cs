/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;
using InstagramApiSharp.Classes.Models;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaRUploadResponse : InstaDefaultResponse
    {
        [JsonProperty("stream_id")] public string StreamId { get; set; }
    }

}

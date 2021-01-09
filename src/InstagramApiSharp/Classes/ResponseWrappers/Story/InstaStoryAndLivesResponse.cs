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
    public class InstaStoryAndLivesResponse
    {
        [JsonProperty("reel")] public InstaStoryResponse Reel { get; set; }
        [JsonProperty("broadcast")] public InstaBroadcastResponse Broadcast { get; set; }
        [JsonProperty("post_live_item")] public InstaBroadcastPostLiveResponse PostLiveItems { get; set; }
    }
}
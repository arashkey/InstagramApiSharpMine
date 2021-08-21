/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaBroadcastSuggestedResponse
    {
        [JsonProperty("broadcasts")]
        public List<InstaBroadcastResponse> Broadcasts { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaReelsMediaListResponse : InstaDefaultResponse
    {
        [JsonProperty("items")]
        public List<InstaMediaItemResponse> Medias { get; set; } = new List<InstaMediaItemResponse>();
        [JsonProperty("paging_info")]
        public InstaPagingInfoResponse PagingInfo { get; set; } 
    }
}

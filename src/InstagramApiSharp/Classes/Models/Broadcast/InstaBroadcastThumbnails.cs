/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaBroadcastThumbnails : InstaDefaultResponse
    {
        [JsonProperty("thumbnails")]
        public List<string> Thumbnails { get; set; } = new List<string>();
        [JsonProperty("title_prefill")]
        public string TitlePrefill { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTVCreateSeries : InstaDefaultResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

}

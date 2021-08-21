/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.Models.Business
{
    public class InstaBusinessCityLocation
    {
        [JsonProperty("__typename")]
        internal string TypeName { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class InstaBusinessCityLocationList : List<InstaBusinessCityLocation> { }

    internal class InstaBusinessCityLocationContainer
    {
        [JsonExtensionData]
        internal IDictionary<string, JToken> Extras { get; set; }
    }


}

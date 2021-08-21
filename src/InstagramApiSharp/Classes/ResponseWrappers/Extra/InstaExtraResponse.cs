using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.ResponseWrappers
{
    internal class InstaExtraResponse
    {
        [JsonExtensionData]
        internal IDictionary<string, JToken> Extras { get; set; }
    }
}

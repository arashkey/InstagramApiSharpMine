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
    public class InstaOffensiveText : InstaDefaultResponse
    {
        [JsonProperty("is_offensive")]
        public bool IsOffensive { get; set; }
        [JsonProperty("bully_classifier")]
        public float BullyClassifier { get; set; }
        [JsonProperty("hate_classifier")]
        public float HateClassifier { get; set; }
        [JsonProperty("sexual_classifier")]
        public float SexualClassifier { get; set; }
        [JsonProperty("spam_classifier")]
        public float SpamClassifier { get; set; }
    }
}

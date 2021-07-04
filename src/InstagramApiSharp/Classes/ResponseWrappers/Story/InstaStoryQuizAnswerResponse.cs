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
    public class InstaStoryQuizAnswerResponse
    {
        [JsonProperty("ts")] public string Timestamp { get; set; }
        [JsonProperty("answer")] public int Answer { get; set; }
        [JsonProperty("user")] public InstaUserShortResponse User { get; set; }
    }
}

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
    public class InstaStoryQuizStickerItemResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("quiz_id")]
        public long QuizId { get; set; }
        [JsonProperty("question")]
        public string Question { get; set; }
        [JsonProperty("tallies")]
        public List<InstaStoryTalliesItemResponse> Tallies { get; set; }
        [JsonProperty("correct_answer")]
        public int CorrectAnswer { get; set; }
        [JsonProperty("viewer_can_answer")]
        public bool ViewerCanAnswer { get; set; }
        [JsonProperty("finished")]
        public bool Finished { get; set; }
        [JsonProperty("text_color")]
        public string TextColor { get; set; }
        [JsonProperty("start_background_color")]
        public string StartBackgroundColor { get; set; }
        [JsonProperty("end_background_color")]
        public string EndBackgroundColor { get; set; }
    }
}

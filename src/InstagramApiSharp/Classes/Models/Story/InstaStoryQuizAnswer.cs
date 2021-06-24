/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryQuizAnswer
    {
        public string Timestamp { get; set; }
        public int Answer { get; set; }
        public InstaUserShort User { get; set; }
        public string AnswerText { get; set; }
    }
    public class InstaStoryQuizAnswerUpload
    {
        public string Text { get; set; }
        public int Count { get; set; }
    }
}

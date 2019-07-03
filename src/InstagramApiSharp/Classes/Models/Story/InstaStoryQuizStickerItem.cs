/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryQuizStickerItem
    {
        public string Id { get; set; }

        public long QuizId { get; set; }

        public string Question { get; set; }

        public List<InstaStoryTalliesItem> Tallies { get; set; } = new List<InstaStoryTalliesItem>();

        public int CorrectAnswer { get; set; }

        public bool ViewerCanAnswer { get; set; }

        public bool Finished { get; set; }

        public string TextColor { get; set; }

        public string StartBackgroundColor { get; set; }

        public string EndBackgroundColor { get; set; }

        public long ViewerAnswer { get; set; } = -1;
    }
}

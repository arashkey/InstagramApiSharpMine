/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryQuizParticipant
    {
        public long QuizId { get; set; }

        public List<InstaStoryQuizAnswer> Participants { get; set; } = new List<InstaStoryQuizAnswer>();

        public string MaxId { get; set; }

        public bool? MoreAvailable { get; set; }
    }
}

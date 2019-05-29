/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaStoryQuizAnswerConverter : IObjectConverter<InstaStoryQuizAnswer, InstaStoryQuizAnswerResponse>
    {
        public InstaStoryQuizAnswerResponse SourceObject { get; set; }

        public InstaStoryQuizAnswer Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var quizAnswer = new InstaStoryQuizAnswer
            {
                Count = SourceObject.Count,
                Text = SourceObject.Text
            };
            return quizAnswer;
        }
    }
}

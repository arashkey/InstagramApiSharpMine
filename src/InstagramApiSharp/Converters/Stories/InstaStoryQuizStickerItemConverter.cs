/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
 
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaStoryQuizStickerItemConverter : IObjectConverter<InstaStoryQuizStickerItem, InstaStoryQuizStickerItemResponse>
    {
        public InstaStoryQuizStickerItemResponse SourceObject { get; set; }

        public InstaStoryQuizStickerItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var quizSticker = new InstaStoryQuizStickerItem
            {
                CorrectAnswer = SourceObject.CorrectAnswer,
                EndBackgroundColor = SourceObject.EndBackgroundColor,
                Finished = SourceObject.Finished,
                Id = SourceObject.Id,
                Question = SourceObject.Question,
                QuizId = SourceObject.QuizId,
                StartBackgroundColor = SourceObject.StartBackgroundColor,
                TextColor = SourceObject.TextColor,
                ViewerCanAnswer = SourceObject.ViewerCanAnswer,
                ViewerAnswer = SourceObject.ViewerAnswer ?? -1
            };

            if (SourceObject.Tallies?.Count > 0)
                foreach (var t in SourceObject.Tallies)
                    quizSticker.Tallies.Add(ConvertersFabric.Instance.GetStoryTalliesItemConverter(t).Convert());

            return quizSticker;

        }
    }
}

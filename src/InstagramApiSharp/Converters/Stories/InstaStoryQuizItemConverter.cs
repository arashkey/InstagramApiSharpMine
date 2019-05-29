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
    internal class InstaStoryQuizItemConverter : IObjectConverter<InstaStoryQuizItem, InstaStoryQuizItemResponse>
    {
        public InstaStoryQuizItemResponse SourceObject { get; set; }

        public InstaStoryQuizItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var quiz = new InstaStoryQuizItem
            {
                Height = SourceObject.Height,
                IsHidden = System.Convert.ToBoolean(SourceObject.IsHidden),
                IsPinned = System.Convert.ToBoolean(SourceObject.IsPinned),
                Rotation = SourceObject.Rotation,
                Width = SourceObject.Width,
                X = SourceObject.X,
                Y = SourceObject.Y,
                Z = SourceObject.Z
            };
            if (SourceObject.QuizSticker != null)
            {
                try
                {
                    quiz.QuizSticker = ConvertersFabric.Instance.GetStoryQuizStickerItemConverter(SourceObject.QuizSticker).Convert();
                }
                catch { }
            }
            return quiz;
        }
    }
}

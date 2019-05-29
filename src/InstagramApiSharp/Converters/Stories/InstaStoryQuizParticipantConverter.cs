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
    internal class InstaStoryQuizParticipantConverter : IObjectConverter<InstaStoryQuizParticipant, InstaStoryQuizParticipantResponse>
    {
        public InstaStoryQuizParticipantResponse SourceObject { get; set; }

        public InstaStoryQuizParticipant Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var quizParticipants = new InstaStoryQuizParticipant
            {
                MaxId = SourceObject.MaxId,
                MoreAvailable = SourceObject.MoreAvailable ?? false,
                QuizId = SourceObject.QuizId
            };

            if (SourceObject.Participants?.Count > 0)
                foreach(var answer in SourceObject.Participants)
                    quizParticipants.Participants.Add(ConvertersFabric.Instance.GetStoryQuizAnswerConverter(answer).Convert());

            return quizParticipants;
        }
    }
}

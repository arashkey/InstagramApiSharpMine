using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
/////////////////////////////////////////////////////////////////////
////////////////////// IMPORTANT NOTE ///////////////////////////////
// Please check wiki pages for more information:
// https://github.com/ramtinak/InstagramApiSharp/wiki
////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////
namespace Examples.Samples
{
    internal class Stories : IDemoSample
    {
        private readonly IInstaApi InstaApi;

        public Stories(IInstaApi instaApi)
        {
            InstaApi = instaApi;
        }

        public async Task DoShow()
        {
            var result = await InstaApi.StoryProcessor.GetStoryFeedAsync();
            if (!result.Succeeded)
            {
                Console.WriteLine($"Unable to get story feed: {result.Info}");
                return;
            }
            var storyFeed = result.Value;
            Console.WriteLine($"Got {storyFeed.Items.Count} story reels.");
            foreach (var feedItem in storyFeed.Items)
            {
                Console.WriteLine($"User: {feedItem.User.FullName}");
                foreach (var item in feedItem.Items)
                    Console.WriteLine(
                        $"Story item: {item.Caption?.Text ?? item.Code}, images:{item.Images?.Count ?? 0}, videos: {item.Videos?.Count ?? 0}");
            }
        }

        public async void UploadPhoto()
        {
            var image = new InstaImage { Uri = @"c:\someawesomepicture.jpg" };

            var result = await InstaApi.StoryProcessor.UploadStoryPhotoAsync(image, "someawesomepicture");
            Console.WriteLine(result.Succeeded
                ? $"Story created: {result.Value.Media.Pk}"
                : $"Unable to upload photo story: {result.Info.Message}");
        }

        public async void UploadVideo()
        {
            var video = new InstaVideoUpload
            {
                Video = new InstaVideo(@"c:\video1.mp4", 0, 0),
                VideoThumbnail = new InstaImage(@"c:\video thumbnail 1.jpg", 0, 0)
            };
            var result = await InstaApi.MediaProcessor.UploadVideoAsync(video, "ramtinak");
            Console.WriteLine(result.Succeeded
                ? $"Story created: {result.Value.Pk}"
                : $"Unable to upload video story: {result.Info.Message}");
        }

        public async void UploadWithOptions()
        {
            // You can add hashtags or locations or poll questions to your photo/video stories!
            // Note that you must draw your hashtags/location names/poll questions in your image first and then upload it!

            var storyOptions = new InstaStoryUploadOptions();
            // Add hashtag
            storyOptions.Hashtags.Add(new InstaStoryHashtagUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.3148148,
                Height = 0.110367894,
                Rotation = 0,
                TagName = "IRAN"
            });

            // Add poll question
            storyOptions.Polls.Add(new InstaStoryPollUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.3148148,
                Height = 0.110367894,
                Rotation = 0,
                Question = "Do you love IRAN?",
                Answer1 = "Are", // "YES" answer
                Answer2 = "Na" // "NO" answer
            });

            // Add location
            var locationsResult = await InstaApi.LocationProcessor.SearchLocationAsync(0, 0, "kazeroun");
            var firstLocation = locationsResult.Value.FirstOrDefault();
            var locationId = firstLocation.ExternalId;

            storyOptions.Locations.Add(new InstaStoryLocationUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.3148148,
                Height = 0.110367894,
                Rotation = 0,
                LocationId = locationId
            });


            // Mention people
            storyOptions.Mentions.Add(new InstaStoryMentionUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.7972222,
                Height = 0.21962096,
                Rotation = 0,
                Username = "rmt4006"
            });

            // Add story question
            storyOptions.Questions.Add(new InstaStoryQuestionUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.9507363,
                Height = 0.32469338000000003,
                Rotation = 0,
                Question = "What do you think about InstagramApiSharp?!",
                BackgroundColor = "#ffffff", // #ffffff is white
                TextColor = "#000000" // #000000 is black
            });


            // Add quiz 
            var quiz = new InstaStoryQuizUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.3148148,
                Height = 0.110367894,
                Rotation = 0,
                Question = "Who is better?" // Question
            };
            // At least 2 answer is required, maximum answers is 4.
            // First Answer
            quiz.Options.Add(new InstaStoryQuizAnswer
            {
                Text = "Me"
            });
            // Second Answer
            quiz.Options.Add(new InstaStoryQuizAnswer
            {
                Text = "Myself" // for example, this is my question's answer
            });
            quiz.CorrectAnswer = 1; // Set correct answer.
            storyOptions.StoryQuiz = quiz; // Sets quiz


            // Add story chat
            var chat = new InstaStoryChatUpload
            {
                X = 0.5, // center of image
                Y = 0.5, // center of image
                Z = 0,
                Width = 0.48333332,
                Height = 0.21962096,
                Rotation = 0,
                GroupName = "InstagramApiSharp" // group name [title]
            };
            storyOptions.StoryChats.Add(chat);

            storyOptions.ShareAsReel = true; // Share story as Reel

            var image = new InstaImage { Uri = @"c:\someawesomepicture.jpg" };

            var result = await InstaApi.StoryProcessor.UploadStoryPhotoAsync(image, "someawesomepicture", storyOptions);
            // upload video
            //var result = await InstaApi.MediaProcessor.UploadVideoAsync(video, "ramtinak", storyOptions);
            Console.WriteLine(result.Succeeded
                ? $"Story created: {result.Value.Media.Pk}"
                : $"Unable to upload photo story: {result.Info.Message}");
        }

        public async void ShareMediaAsStory()
        {
            // You can share an media to an story (photo, video and album)
            // Note that you must draw whatever(photo, video and album) you want in your image first! 
            // Also it's on you to calculate clickable media but mostly is 0.5 for width and height


            long mediaPk = 1912406543385492359; // Get it from InstaMedia.Pk, you can use video or album pk too!

            var mediaStory = new InstaMediaStoryUpload
            {
                X = 0.5, // center of photo
                Y = 0.5, // center of photo
                Width = 0.5, // height of clickable media, it's an square in center of photo
                Height = 0.5, // width of clickable media, it's an square in center of photo
                Rotation = 0, // don't change this
                MediaPk = mediaPk
            };

            var image = new InstaImage { Uri = @"c:\someawesomepicture.jpg" };

            var result = await InstaApi.StoryProcessor.ShareMediaAsStoryAsync(image, mediaStory);

            Console.WriteLine(result.Succeeded
                ? $"Story created from an media post: {result.Value.Media.Pk}"
                : $"Unable to share media as story: {result.Info.Message}");
        }

        public async void SetStoryStuffs()
        {
            var userResult = await InstaApi.UserProcessor.GetUserAsync("instagram");

            if (userResult.Succeeded)
            {
                var stories = await InstaApi.StoryProcessor.GetUserStoryAsync(userResult.Value.Pk); // getting user's stories
                if (stories.Succeeded && stories.Value?.Items?.Count > 0)
                {
                    var result = stories.Value;
                    
                    // Seen a story
                    MarkStoryAsSeen(result);


                    // Vote to Story Poll
                    VoteStoryPolls(result);


                    // Answer to Story question
                    AnswerToStoryQuestion(result);


                    // Answer to Story quizz
                    AnswerToStoryQuiz(result);


                    // Vote So story slider
                    VoteToStorySlider(result);


                    // Send story reaction
                    SendStoryReaction(result);
                }
            }
        }


        public async void MarkStoryAsSeen(InstaStory reelStory)
        {
            var storyItem1 = reelStory.Items[0]; // choose a story

            // Mark a story as seen>
            // Since there are 3 different API for seen a story, you have to checks them first

            if (reelStory.IsElection)// checks for election story
            {
                var dic = new List<InstaStoryElectionKeyValue>
                {
                    new InstaStoryElectionKeyValue
                    {
                        StoryId = reelStory.Id,
                        StoryItemId = storyItem1.Id,
                        TakenAtUnix = storyItem1.TakenAt.ToUnixTime().ToString()
                    }
                };
                var seen = await InstaApi.StoryProcessor.MarkMultipleElectionStoriesAsSeenAsync(dic);
                Console.WriteLine($"{storyItem1.Id} seen result: {seen.Succeeded}");
            }
            else // otherwise it's a normal story or hashtag story
            {
                var dic = new Dictionary<string, long>
                {
                    {storyItem1.Id, storyItem1.TakenAt.ToUnixTime()},
                    // you can mark multiple stories in here!
                };
                var seen = await InstaApi.StoryProcessor.MarkMultipleStoriesAsSeenAsync(dic);
                Console.WriteLine($"{storyItem1.Id} seen result: {seen.Succeeded}");
            }
        }


        public async void VoteStoryPolls(InstaStory reelStory)
        {
            var storyItem = reelStory.Items.FirstOrDefault(x=> x.StoryPolls?.Count > 1); // choose a story that has a Story poll
            if (storyItem == null)
            {
                Console.WriteLine("No story poll exist.");
                return;
            }

            var storyPoll = storyItem.StoryPolls[0]; // gets first poll
            // checks for answer, maybe we already voted
            if (storyPoll.PollSticker.ViewerVote != -1 || storyPoll.PollSticker.Finished)
                return;
            var myVote = InstaStoryPollVoteType.Yes;
            // for example we marked the poll as Yes
            var votePoll = await InstaApi.StoryProcessor.VoteStoryPollAsync(storyItem.Id,
                storyPoll.PollSticker.PollId.ToString(), myVote);

            Console.WriteLine($"{storyItem.Id} '{storyPoll.PollSticker.Question}'\t\tvoted to {storyPoll.PollSticker.PollId} as {myVote} result: {votePoll.Succeeded}");
        }


        public async void AnswerToStoryQuestion(InstaStory reelStory)
        {
            var storyItem = reelStory.Items.FirstOrDefault(x => x.StoryQuestions?.Count > 1); // choose a story that has a Story question
            if (storyItem == null)
            {
                Console.WriteLine("No story question exist.");
                return;
            }

            var storyQuestion = storyItem.StoryQuestions[0]; // gets first story question

            var myAnswer = "I'm with InstagramApiSharp's library";
            myAnswer = myAnswer.Trim().Replace("\r", ""); // trim and replace \r

            var answerToQuestion = await InstaApi.StoryProcessor
                .AnswerToStoryQuestionAsync(storyItem.Id, storyQuestion.QuestionSticker.QuestionId, myAnswer);

            Console.WriteLine($"{storyItem.Id} '{storyQuestion.QuestionSticker.Question}'\t\t" +
                $"answered to: {storyQuestion.QuestionSticker.QuestionId} as '{myAnswer}' result: {answerToQuestion.Succeeded}");
        }


        public async void AnswerToStoryQuiz(InstaStory reelStory)
        {
            var storyItem = reelStory.Items.FirstOrDefault(x => x.StoryQuizs?.Count > 1); // choose a story that has a Story quiz
            if (storyItem == null)
            {
                Console.WriteLine("No story quiz exist.");
                return;
            }

            var storyQuiz = storyItem.StoryQuizs[0]; // gets first story quiz

            // checks for answer, maybe we already voted
            if (storyQuiz.QuizSticker.ViewerAnswer != -1 || storyQuiz.QuizSticker.Finished)
                return;
            int myAnswer = 0;
            // story quiz answers can be 2,3 or 4 options ,
            // so we can get the Tallies property in QuizSticker to find out how many options we have
            myAnswer = storyQuiz.QuizSticker.Tallies.Count - 1; // select latest tallies

            var result = await InstaApi.StoryProcessor
                .AnswerToStoryQuizAsync(storyItem.Pk, storyQuiz.QuizSticker.QuizId, myAnswer);

            Console.WriteLine($"{storyItem.Id} '{storyQuiz.QuizSticker.Question}'\t\t" +
                $"answered to: {storyQuiz.QuizSticker.QuizId} '{storyQuiz.QuizSticker.Tallies.Count}' selected '{myAnswer}' result: {result.Succeeded}");
        }


        public async void VoteToStorySlider(InstaStory reelStory)
        {
            var storyItem = reelStory.Items.FirstOrDefault(x => x.StorySliders?.Count > 1); // choose a story that has a Story slider
            if (storyItem == null)
            {
                Console.WriteLine("No story slider exist.");
                return;
            }

            var storySlider = storyItem.StorySliders[0]; // gets first story slider

            // checks for answer, maybe we already voted
            if (storySlider.SliderSticker.ViewerVote != -1 || storySlider.SliderSticker.ViewerCanVote)
                return;
            double myAnswer = 7;
            // story sliders answer is a range between 0 to 1, it's so I but here we use 0 to 10 and then divide it by 10
            // to get's the real answer
            myAnswer /= 10;

            var result = await InstaApi.StoryProcessor
                   .VoteStorySliderAsync(storyItem.Id, storySlider.SliderSticker.SliderId.ToString(), myAnswer);
            Console.WriteLine($"{storyItem.Id} '{storySlider.SliderSticker.Question}'\t\t" +
                $"answered to: {storySlider.SliderSticker.SliderId} '{myAnswer * 10} / 10' result: {result.Succeeded}");
        }


        public async void SendStoryReaction(InstaStory reelStory)
        {
            var storyItem = reelStory.Items[0]; // choose a story

            // note that don't send anything except these stickers!
            var allReactionsText = new[] { "😂", "😮", "😍", "😢", "👏", "🔥", "🎉", "💯" };

            // choose one above reactions!
            var myReaction = allReactionsText.LastOrDefault(); // 💯

            var result = await InstaApi.StoryProcessor
                                 .SendReactionToStoryAsync(storyItem.User.Pk, storyItem.Id, myReaction);

            Console.WriteLine($"{storyItem.Id}\t\treacted {myReaction} result: {result.Succeeded}");
        }
    }
}
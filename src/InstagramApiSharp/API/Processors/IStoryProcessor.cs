﻿using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Story api functions.
    /// </summary>
    public interface IStoryProcessor
    {

        Task<IResult<InstaStorySliderVoterInfoItem>> GetStorySliderVotersAsync(string storyMediaId, string sliderId, PaginationParameters paginationParameters);

        Task<IResult<InstaStoryQuizParticipant>> GetStoryQuizParticipantsAsync(string storyMediaId, string quizId, PaginationParameters paginationParameters);

        Task<IResult<InstaStoryQuestionInfo>> GetStoryQuestionRespondersAsync(string storyMediaId, string questionId, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get user story and lives
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        Task<IResult<InstaStoryAndLives>> GetUserStoryAndLivesAsync(long userId);

        /// <summary>
        ///     Request for joing chats from story
        /// </summary>
        /// <param name="storyChatId">Story chat id (<see cref="InstaStoryChatStickerItem.StoryChatId"/>)</param>
        Task<IResult<bool>> StoryChatRequestAsync(long storyChatId);

        /// <summary>
        ///     Cancel story chat request
        /// </summary>
        /// <param name="storyChatId">Story chat id (<see cref="InstaStoryChatStickerItem.StoryChatId"/>)</param>
        Task<IResult<bool>> CancelStoryChatRequestAsync(long storyChatId);

        /// <summary>
        ///     Reply a photo to story
        /// </summary>
        /// <param name="image">Photo to send</param>
        /// <param name="userId">User id/pk of story creator</param>
        Task<IResult<bool>> ReplyPhotoToStoryAsync(InstaImageUpload image, /*string storyMediaId,*/ long userId);
        /// <summary>
        ///     Reply a photo to story with progress
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <param name="image">Photo to send</param>
        /// <param name="userId">User id/pk of story creator</param>
        Task<IResult<bool>> ReplyPhotoToStoryAsync(Action<InstaUploaderProgress> progress, InstaImageUpload image,
            /*string storyMediaId,*/ long userId);

        /// <summary>
        ///     Answer to an story quiz
        /// </summary>
        /// <param name="storyPk">Story pk (<see cref="InstaStoryItem.Pk"/>)</param>
        /// <param name="quizId">Quiz id (<see cref="InstaStoryQuizParticipant.QuizId"/>)</param>
        /// <param name="answer">Your answer</param>
        Task<IResult<bool>> AnswerToStoryQuizAsync(long storyPk, long quizId, int answer);

        /// <summary>
        ///     Respond to an story question
        /// </summary>
        /// <param name="storyId">Story id (<see cref="InstaStoryItem.Id"/>)</param>
        /// <param name="questionId">Question id (<see cref="InstaStoryQuestionStickerItem.QuestionId"/>)</param>
        /// <param name="responseText">Text to respond</param>
        Task<IResult<bool>> AnswerToStoryQuestionAsync(string storyId, long questionId, string responseText);

        /// <summary>
        ///     Create new highlight
        /// </summary>
        /// <param name="mediaId">Story media id</param>
        /// <param name="title">Highlight title</param>
        /// <param name="cropWidth">Crop width It depends on the aspect ratio/size of device display and the aspect ratio of story uploaded. must be in a range of 0-1, i.e: 0.19545822</param>
        /// <param name="cropHeight">Crop height It depends on the aspect ratio/size of device display and the aspect ratio of story uploaded. must be in a range of 0-1, i.e: 0.8037307</param>
        Task<IResult<InstaHighlightFeed>> CreateHighlightFeedAsync(string mediaId, string title, float cropWidth, float cropHeight);

        /// <summary>
        ///     Delete highlight feed
        /// </summary>
        /// <param name="highlightId">Highlight id</param>
        /// <param name="mediaId">Media id (CoverMedia.MediaId)</param>
        Task<IResult<bool>> DeleteHighlightFeedAsync(string highlightId, string mediaId);

        /// <summary>
        ///     Delete a media story (photo or video)
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="sharingType">The type of the media</param>
        /// <returns>Return true if the story media is deleted</returns>
        Task<IResult<bool>> DeleteStoryAsync(string storyMediaId, InstaSharingType sharingType = InstaSharingType.Video);

        /// <summary>
        ///     Follow countdown story
        /// </summary>
        /// <param name="countdownId">Countdown id (<see cref="InstaStoryCountdownStickerItem.CountdownId"/>)</param>
        Task<IResult<bool>> FollowCountdownStoryAsync(long countdownId);

        /// <summary>
        ///     Get list of users that blocked from seeing your stories
        /// </summary>
        Task<IResult<InstaUserShortList>> GetBlockedUsersFromStoriesAsync();

        /// <summary>
        ///     Get stories countdowns for self accounts
        /// </summary>
        Task<IResult<InstaStoryCountdownList>> GetCountdownsStoriesAsync();

        /// <summary>
        ///     Get user highlight feeds by user id (pk)
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        /// <param name="batteryLevel">Battery level (optional)</param>
        /// <param name="isCharging">Is your phone charging? (optional)</param>
        /// <param name="isDarkMode">Is dark mode? (optional)</param>
        /// <param name="willSoundOn">Will sound on? (optional)</param>
        Task<IResult<InstaHighlightFeeds>> GetHighlightFeedsAsync(long userId,
            ushort batteryLevel = 100,
            bool isCharging = false,
            bool isDarkMode = false,
            bool willSoundOn = false);

        /// <summary>
        ///     Get user highlights archive
        ///     <para>Note: Use <see cref="IStoryProcessor.GetHighlightsArchiveMediasAsync(string)"/> to get hightlight medias of an specific day.</para>
        /// </summary>
        Task<IResult<InstaHighlightShortList>> GetHighlightsArchiveAsync();

        /// <summary>
        ///     Get highlights archive medias
        ///     <para>Note: get highlight id from <see cref="IStoryProcessor.GetHighlightsArchiveAsync"/></para>
        /// </summary>
        /// <param name="highlightId">Highlight id (Get it from <see cref="IStoryProcessor.GetHighlightsArchiveAsync"/>)</param>
        Task<IResult<InstaHighlightSingleFeed>> GetHighlightsArchiveMediasAsync(string highlightId);

        Task<IResult<InstaUserStoriesFeeds>> GetUsersStoriesAsHighlightsAsync(params string[] usersIds);
        /// <summary>
        ///     Get single highlight medias
        ///     <para>Note: get highlight id from <see cref="GetHighlightFeedsAsync(long, ushort, bool, bool, bool)"/></para>
        /// </summary>
        /// <param name="highlightId">Highlight id (Get it from <see cref="GetHighlightFeedsAsync(long, ushort, bool, bool, bool)"/>)</param>
        Task<IResult<InstaHighlightSingleFeed>> GetHighlightMediasAsync(string highlightId);


        /// <summary>
        ///     Get user story feed with POST method requests (new API)
        /// </summary>
        /// <param name="refresh">Refreshing?</param>
        /// <param name="preloadedReelIds">Preloaded reel ids</param>
        /// <returns></returns>
        Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(bool refresh = false, string[] preloadedReelIds = null);

        /// <summary>
        ///     Get user story feed (stories from users followed by current user).
        /// </summary>
        Task<IResult<InstaStoryFeed>> GetStoryFeedAsync();

        /// <summary>
        ///     Get user story feed (stories from users followed by current user) with pagination.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="forceRefresh">Force to use pull refresh</param>
        Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(PaginationParameters paginationParameters, bool forceRefresh = false);

        /// <summary>
        ///     Get user story feed (stories from users followed by current user) with pagination.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="forceRefresh">Force to use pull refresh</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken, bool forceRefresh = false);

        /// <summary>
        ///     Get story media viewers
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="paginationParameters">Pagination parameters</param>
        Task<IResult<InstaReelStoryMediaViewers>> GetStoryMediaViewersAsync(string storyMediaId, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get story poll voters
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="pollId">Story poll id</param>
        /// <param name="paginationParameters">Pagination parameters</param>
        /// <param name="abc"></param>
        Task<IResult<InstaStoryPollVotersList>> GetStoryPollVotersAsync(string storyMediaId, string pollId, PaginationParameters paginationParameters, uint abc);

        /// <summary>
        ///     Get the story by userId
        /// </summary>
        /// <param name="userId">User Id</param>
        Task<IResult<InstaStory>> GetUserStoryAsync(long userId);
        /// <summary>
        ///     Get user story reel feed. Contains user info last story including all story items.
        /// </summary>
        /// <param name="userId">User identifier (PK)</param>
        Task<IResult<InstaReelFeed>> GetUserStoryFeedAsync(long userId);

        /// <summary>
        ///     Seen multiple stories
        /// </summary>
        /// <param name="storiesWithTakenAt">Story media identifier with taken at unix times</param>
        Task<IResult<bool>> MarkMultipleStoriesAsSeenAsync(Dictionary<string, long> storiesWithTakenAt);

        /// <summary>
        ///     Seen multiple election stories
        /// </summary>
        /// <param name="storiesWithTakenAt">Story media identifier with taken at unix times</param>
        Task<IResult<bool>> MarkMultipleElectionStoriesAsSeenAsync(List<InstaStoryElectionKeyValue> storiesWithTakenAt);

        /// <summary>
        ///     Seen story
        /// </summary>
        /// <param name="storyMediaId">Story media identifier</param>
        /// <param name="takenAtUnix">Taken at unix</param>
        Task<IResult<bool>> MarkStoryAsSeenAsync(string storyMediaId, long takenAtUnix);

        /// <summary>
        ///     Seen highlight
        ///     <para>Get media id from <see cref="InstaHighlightFeed.CoverMedia"/>.MediaId</para>
        /// </summary>
        /// <param name="mediaId">Media identifier (get it from <see cref="InstaHighlightFeed.CoverMedia"/>.MediaId)</param>
        /// <param name="highlightId">Highlight id</param>
        /// <param name="takenAtUnix">Taken at unix</param>
        Task<IResult<bool>> MarkHighlightAsSeenAsync(string mediaId, string highlightId, long takenAtUnix);

        /// <summary>
        ///     Send reaction to an story
        /// </summary>
        /// <param name="storyOwnerUserId">Story owner user id/pk</param>
        /// <param name="storyMediaId">Story media identifier</param>
        /// <param name="reactionEmoji">Reaction emoji</param>
        Task<IResult<InstaDirectRespondPayload>> SendReactionToStoryAsync(long storyOwnerUserId, string storyMediaId, string reactionEmoji);

        /// <summary>
        ///     Share an media to story
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="mediaStoryUpload">
        ///     Media options
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </param>
        Task<IResult<InstaStoryMedia>> ShareMediaAsStoryAsync(InstaImage image, InstaMediaStoryUpload mediaStoryUpload);

        /// <summary>
        ///     Share an media to story with progress
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="mediaStoryUpload">
        ///     Media options
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </param>
        Task<IResult<InstaStoryMedia>> ShareMediaAsStoryAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            InstaMediaStoryUpload mediaStoryUpload);

        /// <summary>
        ///     Share story to someone
        /// </summary>
        /// <param name="reelId">Reel id</param>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="text">Text to send (optional)</param>
        /// <param name="recipients">Recipients user ids</param>
        /// <param name="sharingType">Sharing type</param>
        Task<IResult<bool>> ShareStoryAsync(string reelId, string storyMediaId, string[] threadIds, long[] recipients, string text, InstaSharingType sharingType = InstaSharingType.Video);

        /// <summary>
        ///     Reply to story
        ///     <para>Note: Get story media id from <see cref="InstaMedia.InstaIdentifier"/></para>
        /// </summary>
        /// <param name="storyMediaId">Media id (get it from <see cref="InstaMedia.InstaIdentifier"/>)</param>
        /// <param name="userId">Story owner user pk (get it from <see cref="InstaMedia.User"/>.Pk)</param>
        /// <param name="text">Text to send</param>
        /// <param name="sharingType">Sharing type</param>
        Task<IResult<bool>> ReplyToStoryAsync(string storyMediaId, long userId, string text, InstaSharingType sharingType);

        /// <summary>
        ///     UnFollow countdown story
        /// </summary>
        /// <param name="countdownId">Countdown id (<see cref="InstaStoryCountdownStickerItem.CountdownId"/>)</param>
        Task<IResult<bool>> UnFollowCountdownStoryAsync(long countdownId);

        [Obsolete("UploadStoryPhotoAsync is deprecated. Use UploadStoryPhotoAsync(InstaImage, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(InstaImage image, string caption, InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryPhotoAsync is deprecated. Use UploadStoryPhotoAsync(Action<InstaUploaderProgress>, InstaImage, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image, string caption,
            InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryPhotoWithUrlAsync is deprecated. Use UploadStoryPhotoWithUrlAsync(InstaImage, Uri, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(InstaImage image, string caption, Uri uri,
            InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryPhotoWithUrlAsync is deprecated. Use UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress>, InstaImage, Uri, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null);

        /// <summary>
        ///     Upload story photo
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(InstaImage image, InstaStoryUploadOptions uploadOptions = null);
        /// <summary>
        ///     Upload story photo with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            InstaStoryUploadOptions uploadOptions = null);
        /// <summary>
        ///     Upload story photo with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(InstaImage image, Uri uri,
            InstaStoryUploadOptions uploadOptions = null);

        /// <summary>
        ///     Upload story photo with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            Uri uri, InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryVideoAsync is deprecated. Use UploadStoryVideoAsync(InstaVideoUpload, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(InstaVideoUpload video, string caption, InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryVideoAsync is deprecated. Use UploadStoryVideoAsync(Action<InstaUploaderProgress>, InstaVideoUpload, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, string caption, InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryVideoWithUrlAsync is deprecated. Use UploadStoryVideoWithUrlAsync(InstaVideoUpload, Uri, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null);

        [Obsolete("UploadStoryVideoWithUrlAsync is deprecated. Use UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress>, InstaVideoUpload, Uri, InstaStoryUploadOptions) instead." +
            "\r\nThis function will be removed in the future releases.", true)]
        Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null);

        /// <summary>
        ///     Upload story video (to self story)
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(InstaVideoUpload video, InstaStoryUploadOptions uploadOptions = null);
        /// <summary>
        ///     Upload story video (to self story) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, InstaStoryUploadOptions uploadOptions = null);
        /// <summary>
        ///     Upload story video [to self story, to direct threads or both(self and direct)]
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="storyType">Story type</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<bool>> UploadStoryVideoAsync(InstaVideoUpload video,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds);

        /// <summary>
        ///     Upload story video (to self story) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="storyType">Story type</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<bool>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds);

        /// <summary>
        ///     Upload story video (to self story) with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, Uri uri, InstaStoryUploadOptions uploadOptions = null);
        /// <summary>
        ///     Upload story video (to self story) with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, Uri uri, InstaStoryUploadOptions uploadOptions = null);
        /// <summary>
        ///     Upload story video [to self story, to direct threads or both(self and direct)] with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="storyType">Story type</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<bool>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, Uri uri,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds);
        /// <summary>
        ///     Upload story video (to self story) with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="storyType">Story type</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="uploadOptions">Upload options</param>
        Task<IResult<bool>> UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, Uri uri,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds);

        /// <summary>
        ///     Validate uri for adding to story link
        /// </summary>
        /// <param name="uri">Uri address</param>
        Task<IResult<bool>> ValidateUriAsync(Uri uri);

        /// <summary>
        ///     Vote to an story poll
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="pollId">Story poll id</param>
        /// <param name="pollVote">Your poll vote</param>
        Task<IResult<InstaStoryItem>> VoteStoryPollAsync(string storyMediaId, string pollId, InstaStoryPollVoteType pollVote);

        /// <summary>
        ///     Vote to an story slider
        ///     <para>Note: slider vote must be between 0 and 1</para>
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="pollId">Story poll id</param>
        /// <param name="sliderVote">Your slider vote (from 0 to 1)</param>
        Task<IResult<InstaStoryItem>> VoteStorySliderAsync(string storyMediaId, string pollId, double sliderVote = 0.5);

    }
}
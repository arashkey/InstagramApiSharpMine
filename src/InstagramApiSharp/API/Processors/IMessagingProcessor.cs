﻿using System;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Messaging (direct) api functions.
    /// </summary>
    public interface IMessagingProcessor
    {
        /// <summary>
        ///     Enable vanish mode [ ssh mode ] for a specific thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> EnableThreadVanishModeAsync(string threadId);

        /// <summary>
        ///     Forward a direct message
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="text">Text to send</param>
        /// <param name="forwardedThreadId">Forwarded thread id</param>
        /// <param name="forwardedThreadItemId">Forwarded thread item id</param>
        /// <param name="vanishMode">Vanish mode [ it's related to new direct ]</param>
        Task<IResult<InstaDirectRespondPayload>> ForwardDirectMessageAsync(string threadId,
            string text,
            string forwardedThreadId,
            string forwardedThreadItemId,
            bool vanishMode = false);

        /// <summary>
        ///     Reply a message
        /// </summary>
        /// <param name="threadId">Message thread id</param>
        /// <param name="text">Message text</param>
        /// <param name="itemIdToReply">Item id to reply (item id of the message)</param>
        /// <param name="userIdToReply">User id (pk) to reply(the sender of the message)</param>
        /// <param name="clientContextOfMessage">Client-context to reply (Client-context of the message)</param>
        /// <param name="messageType">Message type [ what was the message type ? ]</param>
        /// <param name="vanishMode">Vanish mode [ it's related to new direct ]</param>
        Task<IResult<InstaDirectRespondPayload>> ReplyDirectMessageAsync(string threadId,
            string text,
            string itemIdToReply,
            long userIdToReply,
            string clientContextOfMessage,
            string messageType = "text",
            bool vanishMode = false);
        /// <summary>
        ///     Mark direct visual message as seen
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="itemId">Message id (item id)</param>
        Task<IResult<bool>> MarkDirectVisualThreadAsSeenAsync(string threadId, string itemId);
        /// <summary>
        ///     Send direct ar effect
        /// </summary>
        /// <param name="effectId">Effect id</param>
        /// <param name="queryId">Query id</param>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<bool>> SendDirectArEffectAsync(string effectId, string queryId, params string[] threadIds);
        /// <summary>
        ///     Send/share product to direct thread
        /// </summary>
        /// <param name="merchantId">Merchant id</param>
        /// <param name="productId">Product id</param>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<InstaDirectRespondPayload>> SendDirectProductAsync(string merchantId, string productId, params string[] threadIds);
        Task<IResult<InstaDirectInboxThread>> GetThreadByParticipantsAsync(int seqId, params long[] userIds);
        /// <summary>
        ///     Create group thread
        /// </summary>
        /// <param name="title">Group title</param>
        /// <param name="userIds">User ids (pk)</param>
        Task<IResult<InstaDirectInboxThread>> CreateGroupAsync(string title, params long[] userIds);
        /// <summary>
        ///    Remove a user from group thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="userId">User id (pk)</param>
        Task<IResult<bool>> RemoveUserFromGroupAsync(string threadId, long userId);
        /// <summary>
        ///    Add new admin for group thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="userId">User id (pk)</param>
        Task<IResult<bool>> AddNewGroupAdminAsync(string threadId, long userId);

        /// <summary>
        ///    Remove group thread's admin
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="userId">User id (pk)</param>
        Task<IResult<bool>> RemoveGroupAdminAsync(string threadId, long userId);
        /// <summary>
        ///    Approval is NOT required for new members in group [Admin Only]
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> DisableApprovalForJoiningDirectThreadAsync(string threadId);
        /// <summary>
        ///     Approval required for new members in group [Admin Only]
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> EnableApprovalForJoiningDirectThreadAsync(string threadId);
        /// <summary>
        ///     End chat for a direct group will remove group members from the group
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> EndChatDirectThreadAsync(string threadId);
        /// <summary>
        ///     Add users to group thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="userIds">User ids (pk)</param>
        Task<IResult<InstaDirectInboxThread>> AddUserToGroupThreadAsync(string threadId, params long[] userIds);
        /// <summary>
        ///     Approve direct pending request
        /// </summary>
        /// <param name="threadId">Thread ids</param>
        Task<IResult<bool>> ApproveDirectPendingRequestAsync(params string[] threadIds);

        /// <summary>
        ///     Decline all direct pending requests
        /// </summary>
        Task<IResult<bool>> DeclineAllDirectPendingRequestsAsync();

        /// <summary>
        ///     Decline direct pending requests
        /// </summary>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<bool>> DeclineDirectPendingRequestsAsync(params string[] threadIds);

        /// <summary>
        ///     Delete direct thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> DeleteDirectThreadAsync(string threadId);

        /// <summary>
        ///     Delete self message in direct
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> DeleteSelfMessageAsync(string threadId, string itemId);

        /// <summary>
        ///     Get direct inbox threads for current user asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaDirectInboxContainer" />
        /// </returns>
        Task<IResult<InstaDirectInboxContainer>> GetDirectInboxAsync(PaginationParameters paginationParameters);
        /// <summary>
        ///     Get direct inbox thread by its id asynchronously
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaDirectInboxThread" />
        /// </returns>
        Task<IResult<InstaDirectInboxThread>> GetDirectInboxThreadAsync(string threadId, PaginationParameters paginationParameters);
        /// <summary>
        ///     Get direct pending inbox threads for current user asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaDirectInboxContainer" />
        /// </returns>
        Task<IResult<InstaDirectInboxContainer>> GetPendingDirectAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get ranked recipients (threads and users) asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="InstaRecipients" />
        /// </returns>
        Task<IResult<InstaRecipients>> GetRankedRecipientsAsync();

        /// <summary>
        ///     Get ranked recipients (threads and users) asynchronously
        ///     <para>Note: Some recipient has User, some recipient has Thread</para>
        /// </summary>
        /// <param name="username">Username to search</param>
        /// <returns>
        ///     <see cref="InstaRecipients" />
        /// </returns>
        Task<IResult<InstaRecipients>> GetRankedRecipientsByUsernameAsync(string username);

        /// <summary>
        ///     Get recent recipients (threads and users) asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="InstaRecipients" />
        /// </returns>
        Task<IResult<InstaRecipients>> GetRecentRecipientsAsync();

        /// <summary>
        ///     Get direct users presence
        ///     <para>Note: You can use this function to find out who is online and who isn't.</para>
        /// </summary>
        Task<IResult<InstaUserPresenceList>> GetUsersPresenceAsync();

        /// <summary>
        ///     Leave from group thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> LeaveGroupThreadAsync(string threadId);

        /// <summary>
        ///     Like direct message in a thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="itemId">Item id (message id)</param>
        Task<IResult<bool>> LikeThreadMessageAsync(string threadId, string itemId);

        /// <summary>
        ///     Mark direct message as seen
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="itemId">Message id (item id)</param>
        Task<IResult<bool>> MarkDirectThreadAsSeenAsync(string threadId, string itemId);

        /// <summary>
        ///     Mute direct thread messages
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> MuteDirectThreadMessagesAsync(string threadId);

        /// <summary>
        ///     Mute direct thread video calls
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> MuteDirectThreadVideoCallsAsync(string threadId);


        /// <summary>
        ///     Send gif (animated media) to direct thread
        /// </summary>
        /// <param name="giphyId">Giphy id</param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns>
        ///     <see cref="InstaDirectInboxThread" />
        /// </returns>
        Task<IResult<InstaDirectInboxThread>> SendDirectAnimatedMediaAsync(string giphyId, params string[] threadIds);

        /// <summary>
        ///     Send gif (animated media) to recipients
        /// </summary>
        /// <param name="giphyId">Giphy id</param>
        /// <param name="recipients">Recipients (user ids pk)</param>
        /// <returns>
        ///     <see cref="InstaDirectInboxThread" />
        /// </returns>
        Task<IResult<InstaDirectInboxThread>> SendDirectAnimatedMediaToRecipientsAsync(string giphyId, params string[] recipients);

        /// <summary>
        ///     Send disappearing photo to direct thread (video will remove after user saw it)
        /// </summary>
        /// <param name="image">Image to upload</param>
        /// <param name="viewMode">View mode</param>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<bool>> SendDirectDisappearingPhotoAsync(InstaImage image,
            InstaViewMode viewMode = InstaViewMode.Replayable, params string[] threadIds);

        /// <summary>
        ///     Send disappearing photo to direct thread (video will remove after user saw it) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Image to upload</param>
        /// <param name="viewMode">View mode</param>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<bool>> SendDirectDisappearingPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            InstaViewMode viewMode = InstaViewMode.Replayable, params string[] threadIds);

        /// <summary>
        ///     Send disappearing video to direct thread (video will remove after user saw it)
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="viewMode">View mode</param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns></returns>
        Task<IResult<bool>> SendDirectDisappearingVideoAsync(InstaVideoUpload video,
            InstaViewMode viewMode = InstaViewMode.Replayable, params string[] threadIds);

        /// <summary>
        ///     Send disappearing video to direct thread (video will remove after user saw it) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="viewMode">View mode</param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns></returns>
        Task<IResult<bool>> SendDirectDisappearingVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video,
            InstaViewMode viewMode = InstaViewMode.Replayable, params string[] threadIds);

        /// <summary>
        ///     Send felix share (ig tv) to direct thread
        /// </summary>
        /// <param name="mediaId">Media identifier to send</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="recipients">Recipients ids</param>
        /// <returns>Returns True if felix share sent</returns>
        Task<IResult<bool>> SendDirectFelixShareAsync(string mediaId, string[] threadIds, string[] recipients);

        /// <summary>
        ///     Send hashtag to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="hashtag">Hashtag to send</param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns>Returns True if hashtag sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectHashtagAsync(string text, string hashtag, params string[] threadIds);

        /// <summary>
        ///     Send hashtag to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="hashtag">Hashtag to send</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="recipients">Recipients ids</param>
        /// <returns>Returns True if hashtag sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectHashtagAsync(string text, string hashtag, string[] threadIds, string[] recipients);

        /// <summary>
        ///     Send hashtag to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="hashtag">Hashtag to send</param>
        /// <param name="recipients">Recipients ids</param>
        /// <returns>Returns True if hashtag sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectHashtagToRecipientsAsync(string text, string hashtag, params string[] recipients);

        /// <summary>
        ///     Send link address to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="link">Link to send (only one link will approved)</param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns>Returns True if link sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectLinkAsync(string text, string link, params string[] threadIds);
        
        /// <summary>
        ///     Send link address to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="link">Link to send (only one link will approved)</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="recipients">Recipients ids</param>
        /// <returns>Returns True if link sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectLinkAsync(string text, string link, string[] threadIds, string[] recipients);

        /// <summary>
        ///     Send link address to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="link">Link to send (only one link will approved)</param>
        /// <param name="recipients">Recipients ids</param>
        /// <returns>Returns True if link sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectLinkToRecipientsAsync(string text, string link, params string[] recipients);

        /// <summary>
        ///     Send location to direct thread
        /// </summary>
        /// <param name="externalId">External id (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns>Returns True if location sent</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectLocationAsync(string externalId, params string[] threadIds);

        /// <summary>
        ///     Send photo to direct thread (single) with progress
        /// </summary>
        /// <param name="image">Image to upload</param>
        /// <param name="threadId">Thread id</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectPhotoAsync(InstaImage image, string threadId);

        /// <summary>
        ///     Send photo to direct thread (single)
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Image to upload</param>
        /// <param name="threadId">Thread id</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image, string threadId);

        /// <summary>
        ///     Send photo to multiple recipients (multiple user)
        /// </summary>
        /// <param name="image">Image to upload</param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectPhotoToRecipientsAsync(InstaImage image, params string[] recipients);

        /// <summary>
        ///     Send photo to multiple recipients (multiple user) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Image to upload</param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectPhotoToRecipientsAsync(Action<InstaUploaderProgress> progress, InstaImage image, params string[] recipients);

        /// <summary>
        ///     Send profile to direct thread
        /// </summary>
        /// <param name="userIdToSend">User id to send</param>
        /// <param name="threadIds">Thread ids</param>
        /// <returns>Returns True if profile sent</returns>
        Task<IResult<bool>> SendDirectProfileAsync(long userIdToSend, params string[] threadIds);

        /// <summary>
        ///     Send profile to direct thrad
        /// </summary>
        /// <param name="userIdToSend">User id to send</param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        Task<IResult<bool>> SendDirectProfileToRecipientsAsync(long userIdToSend, string recipients);

        /// <summary>
        ///     Send direct text message to provided users and threads
        /// </summary>
        /// <param name="recipients">Comma-separated users PK</param>
        /// <param name="threadIds">Message thread ids</param>
        /// <param name="text">Message text</param>
        /// <returns>List of threads</returns>
        Task<IResult<InstaDirectRespondPayload>> SendDirectTextAsync(string recipients, string threadIds,
            string text);

        /// <summary>
        ///     Send video to direct thread (single)
        /// </summary>
        /// <param name="video">Video to upload (no need to set thumbnail)</param>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> SendDirectVideoAsync(InstaVideoUpload video, string threadId);

        /// <summary>
        ///     Send video to direct thread (single) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload (no need to set thumbnail)</param>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> SendDirectVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, string threadId);

        /// <summary>
        ///     Send video to multiple recipients (multiple user)
        /// </summary>
        /// <param name="video">Video to upload (no need to set thumbnail)</param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        Task<IResult<bool>> SendDirectVideoToRecipientsAsync(InstaVideoUpload video, params string[] recipients);

        /// <summary>
        ///     Send video to multiple recipients (multiple user) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload (no need to set thumbnail)</param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        Task<IResult<bool>> SendDirectVideoToRecipientsAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, params string[] recipients);

        /// <summary>
        ///     Send voice to direct thread (single)
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </summary>
        /// <param name="audio">
        ///     Voice to upload
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </param>
        /// <param name="threadId">Thread id</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectVoiceAsync(InstaAudioUpload audio, string threadId);

        /// <summary>
        ///     Send voice to direct thread (single) with progress
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="audio">
        ///     Voice to upload
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </param>
        /// <param name="threadId">Thread id</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectVoiceAsync(Action<InstaUploaderProgress> progress, InstaAudioUpload audio, string threadId);

        /// <summary>
        ///     Send video to user id (pk)
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </summary>
        /// <param name="audio">
        ///     Voice to upload
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        Task<IResult<bool>> SendDirectVoiceToRecipientsAsync(InstaAudioUpload audio, params string[] recipients);

        /// <summary>
        ///     Send voice to user id (pk) with progress
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="audio">
        ///     Voice to upload
        ///     <para>Only OGG or M4A files accepted by Instagram</para>
        /// </param>
        /// <param name="recipients">Recipients (user ids/pk)</param>
        /// <returns>Returns True is sent</returns>
        Task<IResult<bool>> SendDirectVoiceToRecipientsAsync(Action<InstaUploaderProgress> progress, InstaAudioUpload audio, params string[] recipients);

        /// <summary>
        ///     Share media to direct thread
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="mediaType">Media type</param>
        /// <param name="text">Text to send</param>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<bool>> ShareMediaToThreadAsync(string mediaId, InstaMediaType mediaType, string text, params string[] threadIds);

        /// <summary>
        ///     Share media to user id
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="mediaType">Media type</param>
        /// <param name="text">Text to send</param>
        /// <param name="userIds">User ids (pk)</param>
        Task<IResult<bool>> ShareMediaToUserAsync(string mediaId, InstaMediaType mediaType, string text, params long[] userIds);

        [Obsolete("ShareUserAsync is deprecated. Use SendDirectProfileAsync instead.")]
        /// <summary>
        ///     Share an user
        /// </summary>
        /// <param name="userIdToSend">User id(PK)</param>
        /// <param name="threadId">Thread id</param>
        Task<IResult<InstaSharing>> ShareUserAsync(string userIdToSend, string threadId);

        /// <summary>
        ///     UnLike direct message in a thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="itemId">Item id (message id)</param>
        Task<IResult<bool>> UnLikeThreadMessageAsync(string threadId, string itemId);
        /// <summary>
        ///     Unmute direct thread messages
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> UnMuteDirectThreadMessagesAsync(string threadId);

        /// <summary>
        ///     Unmute direct thread video calls
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> UnMuteDirectThreadVideoCallsAsync(string threadId);

        /// <summary>
        ///     Update direct thread title (for groups)
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="title">New title</param>
        Task<IResult<bool>> UpdateDirectThreadTitleAsync(string threadId, string title);
        
        /// <summary>
        ///     Send a like to the conversation
        /// </summary>
        /// <param name="threadId">Thread id</param>
        Task<IResult<bool>> SendDirectLikeAsync(string threadId);
    }
}
/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System.Threading.Tasks;
using System;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Live api functions.
    /// </summary>
    public interface ILiveProcessor
    {
        /// <summary>
        ///     Let Instagram know that you invited someone to a live broadcast and joined successfully
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="userIdToInvite">User id (pk) to invite</param>
        /// <param name="offsetToVideoStart">Offset to video start </param>
        Task<IResult<bool>> BroadcastEventAsync(string broadcastId, long userIdToInvite, int offsetToVideoStart = 30);
        /// <summary>
        ///     Leave or cancel a live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="encodedServerDataInfo">Encoded server data information => from <see cref="JoinBroadcastAsync"/> response</param>
        /// <param name="numParticipants">Number of participants</param>
        Task<IResult<bool>> LeaveBroadcastAsync(string broadcastId, string encodedServerDataInfo, int numParticipants = 1);
        /// <summary>
        ///     Invite to a live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="userIdToInvite">User id (pk) to invite</param>
        /// <param name="encodedServerDataInfo">Encoded server data information => from <see cref="JoinBroadcastAsync"/> response</param>
        /// <param name="offsetToVideoStart">Offset to video start </param>
        Task<IResult<bool>> InviteToBroadcastAsync(string broadcastId, long userIdToInvite, string encodedServerDataInfo, int offsetToVideoStart = 30);
        /// <summary>
        ///     Confirm a join broadcast request
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="encodedServerDataInfo">Encoded server data information => from <see cref="JoinBroadcastAsync"/> response</param>
        /// <param name="curVersion">Cur version => 1 or 2 or 3</param>
        Task<IResult<bool>> ConfirmJoinBroadcastAsync(string broadcastId, string encodedServerDataInfo, uint curVersion = 2);
        /// <summary>
        ///     Requests to join a Live broadcast as a co-broadcaster
        ///     <para>If someone sends you a request to join your own live, you should send them a join request as well</para>
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="sdpOffer">Sdp offer => I don't know what is this but it seems it's related to RTSP</param>
        /// <param name="targetVideoWidth">Video width</param>
        /// <param name="targetVideoHeight">Video height</param>
        Task<IResult<InstaBroadcastJoin>> JoinBroadcastAsync(string broadcastId, string sdpOffer, uint targetVideoWidth = 848, uint targetVideoHeight = 512);
        /// <summary>
        ///     Get post live thumbnails
        /// </summary>
        /// <param name="broadcastId">Broadcast identifier</param>
        Task<IResult<InstaBroadcastThumbnails>> GetPostLiveThumbnailsAsync(string broadcastId);
        /// <summary>
        ///     Add an broadcast to post live.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        [Obsolete("AddToPostLiveAsync API is deprecated by Instagram.\nSo this function will be deleted in the future releases.", true)]
        Task<IResult<InstaBroadcastAddToPostLive>> AddToPostLiveAsync(string broadcastId);

        /// <summary>
        ///     Post a new comment to broadcast.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="commentText">Comment text</param>
        Task<IResult<InstaComment>> CommentAsync(string broadcastId, string commentText);

        // broadcast create, start, end
        /// <summary>
        ///     Create live broadcast. After create an live broadcast you must call StartAsync.
        /// </summary>
        /// <param name="previewWidth">Preview width</param>
        /// <param name="previewHeight">Preview height</param>
        Task<IResult<InstaBroadcastCreate>> CreateAsync(int previewWidth = 1080, int previewHeight = 1794);

        /// <summary>
        ///     Delete an broadcast from post live.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        [Obsolete("DeletePostLiveAsync API is deprecated by Instagram.\nSo this function will be deleted in the future releases.", true)]
        Task<IResult<bool>> DeletePostLiveAsync(string broadcastId);

        /// <summary>
        ///     Disable broadcast comments.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        Task<IResult<InstaBroadcastCommentEnableDisable>> DisableCommentsAsync(string broadcastId);

        /// <summary>
        ///     Enable broadcast comments.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        Task<IResult<InstaBroadcastCommentEnableDisable>> EnableCommentsAsync(string broadcastId);

        /// <summary>
        ///     End live broadcast.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="endAfterCopyrightWarning">Copyright warning</param>
        Task<IResult<bool>> EndAsync(string broadcastId, bool endAfterCopyrightWarning = false);

        /// <summary>
        ///     Get broadcast comments.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="lastCommentTs">Last comment time stamp</param>
        /// <param name="commentsRequested">Comments requested count</param>
        Task<IResult<InstaBroadcastCommentList>> GetCommentsAsync(string broadcastId, string lastCommentTs = "", int commentsRequested = 4);

        /// <summary>
        ///     Get discover top live.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaDiscoverTopLive>> GetDiscoverTopLiveAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get final viewer list.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        Task<IResult<InstaUserShortList>> GetFinalViewerListAsync(string broadcastId);

        /// <summary>
        ///     Get heart beat and viewer count.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        Task<IResult<InstaBroadcastLiveHeartBeatViewerCount>> GetHeartBeatAndViewerCountAsync(string broadcastId);
        
        /// <summary>
        ///     Get broadcast information.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        Task<IResult<InstaBroadcastInfo>> GetInfoAsync(string broadcastId);

        /// <summary>
        ///     Get join requests to current live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast</param>
        Task<IResult<InstaUserShortList>> GetJoinRequestsAsync(string broadcastId);

        /// <summary>
        ///     Get broadcast like count.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="likeTs">Like time stamp</param>
        Task<IResult<InstaBroadcastLike>> GetLikeCountAsync(string broadcastId, int likeTs = 0);

        /// <summary>
        ///     Get post live viewer list.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="maxId">Max id</param>
        Task<IResult<InstaUserShortList>> GetPostLiveViewerListAsync(string broadcastId, int? maxId = null);

        /// <summary>
        ///     Get suggested broadcasts
        /// </summary>
        Task<IResult<InstaBroadcastList>> GetSuggestedBroadcastsAsync();
        /// <summary>
        ///     Get top live status.
        /// </summary>
        /// <param name="broadcastIds">Broadcast ids</param>
        Task<IResult<InstaBroadcastTopLiveStatusList>> GetTopLiveStatusAsync(params string[] broadcastIds);
        /// <summary>
        ///     Get broadcast viewer list.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        Task<IResult<InstaUserShortList>> GetViewerListAsync(string broadcastId);
        /// <summary>
        ///     Like broadcast.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="likeCount">Like count (from 1 to 6)</param>
        Task<IResult<InstaBroadcastLike>> LikeAsync(string broadcastId, int likeCount = 1);

        /// <summary>
        ///     Pin comment from broadcast.
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        Task<IResult<InstaBroadcastPinUnpin>> PinCommentAsync(string broadcastId,string commentId);
        /// <summary>
        ///     Start live broadcast. NOTE: YOU MUST CREATE AN BROADCAST FIRST(CreateAsync) AND THEN CALL THIS METHOD. 
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="latitude">longitude of your place</param>
        /// <param name="longitude">longitude of your place</param>
        Task<IResult<InstaBroadcastStart>> StartAsync(string broadcastId, double? latitude = null, double? longitude = null);

        /// <summary>
        ///     Share an live broadcast to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="broadcastId">Broadcast id to send ( <see cref="InstaBroadcast.Id"/> )</param>
        /// <param name="threadIds">Thread ids</param>
        Task<IResult<bool>> ShareLiveToDirectThreadAsync(string text, string broadcastId, params string[] threadIds);

        /// <summary>
        ///     Share an live broadcast to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="broadcastId">Broadcast id to send ( <see cref="InstaBroadcast.Id"/> )</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="recipients">Recipients ids</param>
        Task<IResult<bool>> ShareLiveToDirectThreadAsync(string text, string broadcastId, string[] threadIds, string[] recipients);

        /// <summary>
        ///     Share an live broadcast to direct recipients
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="broadcastId">Broadcast id to send ( <see cref="InstaBroadcast.Id"/> )</param>
        /// <param name="recipients">Recipients ids</param>
        Task<IResult<bool>> ShareLiveToDirectRecipientAsync(string text, string broadcastId, params string[] recipients);

        /// <summary>
        ///     Unpin comment from broadcast.
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <param name="commentId"></param>
        Task<IResult<InstaBroadcastPinUnpin>> UnPinCommentAsync(string broadcastId, string commentId);
        /*
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        Task<IResult<object>> GetPostLiveLikesAsync(string broadcastId, int startingOffset = 0, string encodingTag = "instagram_dash_remuxed");
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        Task<IResult<object>> GetPostLiveCommentsAsync(string broadcastId, int startingOffset = 0, string encodingTag = "instagram_dash_remuxed");
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        Task<IResult<object>> NotifyToFriendsAsync();
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        Task<IResult<object>> SeenBroadcastAsync(string broadcastId, string pk);*/
    }
}

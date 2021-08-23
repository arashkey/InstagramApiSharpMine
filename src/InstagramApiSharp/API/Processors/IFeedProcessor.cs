using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Feed api functions.
    /// </summary>
    public interface IFeedProcessor
    {

        /// <summary>
        ///     Delete activity notification
        /// </summary>
        /// <param name="pk">Activity PK ( from <see cref="InstaRecentActivityFeed.Pk"/> )</param>
        /// <param name="tuuid">Activity Tuuid ( from <see cref="InstaRecentActivityFeed.Tuuid"/> )</param>
        Task<IResult<bool>> DeleteActivityNotificationAsync(string pk, string tuuid);

        /// <summary>
        ///     Get medias for explore channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetExploreChannelVideosAsync(string channelId, string firstMediaId, PaginationParameters paginationParameters);


        /// <summary>
        ///     Get medias for explore channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetExploreChannelVideosAsync(string channelId, string firstMediaId,
            PaginationParameters paginationParameters, CancellationToken cancellationToken);

        /// <summary>
        ///     Get user explore feed (Explore tab info) asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaExploreFeed" />
        /// </returns>
        Task<IResult<InstaExploreFeed>> GetExploreFeedAsync(PaginationParameters paginationParameters,
        CancellationToken cancellationToken);

        /// <summary>
        ///     Get user explore feed (Explore tab info) asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns><see cref="InstaExploreFeed" /></returns>
        Task<IResult<InstaExploreFeed>> GetExploreFeedAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get activity of following asynchronously
        /// </summary>
        /// <param name="paginationParameters"></param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        /// 
        [Obsolete("GetFollowingRecentActivityFeedAsync is deprecated by instagram.", true)]
        Task<IResult<InstaActivityFeed>> GetFollowingRecentActivityFeedAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get feed of media your liked.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetLikedFeedAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get feed of media your liked.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetLikedFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get recent activity info asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        Task<IResult<InstaActivityFeed>> GetRecentActivityFeedAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get recent activity info asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        Task<IResult<InstaActivityFeed>> GetRecentActivityFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get saved media feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetSavedFeedAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Get saved media feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetSavedFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get tag feed by tag value asynchronously
        /// </summary>
        /// <param name="tag">Tag value</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaTagFeed" />
        /// </returns>
        Task<IResult<InstaTagFeed>> GetTagFeedAsync(string tag, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get tag feed by tag value asynchronously
        /// </summary>
        /// <param name="tag">Tag value</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaTagFeed" />
        /// </returns>
        Task<IResult<InstaTagFeed>> GetTagFeedAsync(string tag, PaginationParameters paginationParameters,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get user timeline feed (feed of recent posts from users you follow) asynchronously.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="seenMediaIds">Id of the posts seen till now</param>
        /// <param name="refreshRequest">Request refresh feeds</param>
        /// <param name="paginationSource">Pagination source</param>
        /// <param name="removeAds">Remove ads</param>
        /// <param name="batteryLevel">Battery level</param>
        /// <param name="isCharging">Is phone charging?</param>
        /// <param name="isDarkMode">Is dark mode?</param>
        /// <param name="willSoundOn">Will sound on?</param>
        /// <returns>
        ///     <see cref="InstaFeed" />
        /// </returns>
        Task<IResult<InstaFeed>> GetUserTimelineFeedAsync(PaginationParameters paginationParameters,
            string[] seenMediaIds = null,
            bool refreshRequest = false,
            bool removeAds = false,
            InstaFeedPaginationSource paginationSource = InstaFeedPaginationSource.None,
            ushort batteryLevel = 100,
            bool isCharging = false,
            bool isDarkMode = false,
            bool willSoundOn = false);

        /// <summary>
        ///     Get user timeline feed (feed of recent posts from users you follow) asynchronously.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="seenMediaIds">Id of the posts seen till now</param>
        /// <param name="refreshRequest">Request refresh feeds</param>
        /// <param name="paginationSource">Pagination source</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="removeAds">Remove ads</param>
        /// <param name="batteryLevel">Battery level</param>
        /// <param name="isCharging">Is phone charging?</param>
        /// <param name="isDarkMode">Is dark mode?</param>
        /// <param name="willSoundOn">Will sound on?</param>
        /// <returns>
        ///     <see cref="InstaFeed" />
        /// </returns>
        Task<IResult<InstaFeed>> GetUserTimelineFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken,
            string[] seenMediaIds = null,
            bool refreshRequest = false,
            bool removeAds = false,
            InstaFeedPaginationSource paginationSource = InstaFeedPaginationSource.None,
            ushort batteryLevel = 100,
            bool isCharging = false,
            bool isDarkMode = false,
            bool willSoundOn = false);

        /// <summary>
        ///     Get user topical explore feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="clusterId">Cluster id</param>
        /// <returns><see cref="InstaTopicalExploreFeed" /></returns>
        Task<IResult<InstaTopicalExploreFeed>> GetTopicalExploreFeedAsync(PaginationParameters paginationParameters, string clusterId = null);

        /// <summary>
        ///     Get user topical explore feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="clusterId">Cluster id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="InstaTopicalExploreFeed" /></returns>
        Task<IResult<InstaTopicalExploreFeed>> GetTopicalExploreFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken, string clusterId = null);
    }
}
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
using InstagramApiSharp.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Instagram TV api functions.
    /// </summary>
    public interface ITVProcessor
    {
        /// <summary>
        ///     Add live broadcast to IGTV [ Save live as IGTV ]
        /// </summary>
        /// <param name="broadcastId">Broadcast identifier</param>
        /// <param name="cover">Image cover for IGTV [MOST BE IN VERTICAL]</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="sharePreviewToFeed">Show a preview on the feed</param>
        /// <param name="igtvSeriesId">Igtv series indentifier => Optional => adds this video to a specific TV series.</param>
        Task<IResult<bool>> AddLiveBroadcastToTVAsync(string broadcastId, InstaImage cover, string title,
            string description, bool sharePreviewToFeed = false, string igtvSeriesId = null);
        /// <summary>
        ///     Get creating tools availability for IG TV
        /// </summary>
        Task<IResult<bool>> GetTVCreationToolsAsync();
        /// <summary>
        ///     Get TV series of specific user
        /// </summary>
        /// <param name="userId">User id (pk) => channel owner</param>
        Task<IResult<InstaTV>> GetUserTVSeriesAsync(long userId);
        /// <summary>
        ///     Remove episode from a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        /// <param name="mediaPk">Media pk</param>
        Task<IResult<bool>> RemoveEpisodeFromTVSeriesAsync(string seriesId, string mediaPk);
        /// <summary>
        ///     Add episode to a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        /// <param name="mediaPk">Media pk</param>
        Task<IResult<bool>> AddEpisodeToTVSeriesAsync(string seriesId, string mediaPk);
        /// <summary>
        ///     Update a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        Task<IResult<bool>> UpdateTVSeriesAsync(string seriesId, string title, string description);
        /// <summary>
        ///     Delete a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        Task<IResult<bool>> DeleteTVSeriesAsync(string seriesId);
        /// <summary>
        ///     Create a TV series
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        Task<IResult<InstaTVCreateSeries>> CreateTVSeriesAsync(string title, string description);

        /// <summary>
        ///     Edit TV Media
        /// </summary>
        /// <param name="mediaId">TV Identifier</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        Task<IResult<InstaMedia>> EditMediaAsync(string mediaId, string title, string description);
        /// <summary>
        ///     Browse Feed
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaTVBrowseFeed>> BrowseFeedAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Browse Feed
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<IResult<InstaTVBrowseFeed>> BrowseFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get channel by user id (pk) => channel owner
        /// </summary>
        /// <param name="userId">User id (pk) => channel owner</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaTVChannel>> GetChannelByIdAsync(long userId, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get channel by <seealso cref="InstaTVChannelType"/>
        /// </summary>
        /// <param name="channelType">Channel type</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaTVChannel>> GetChannelByTypeAsync(InstaTVChannelType channelType, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get suggested searches
        /// </summary>
        Task<IResult<InstaTVSearch>> GetSuggestedSearchesAsync();

        /// <summary>
        ///     Get TV Guide (gets popular and suggested channels)
        /// </summary>
        Task<IResult<InstaTV>> GetTVGuideAsync();
        /// <summary>
        ///     Search channels
        /// </summary>
        /// <param name="query">Channel or username</param>
        Task<IResult<InstaTVSearch>> SearchAsync(string query);
        /// <summary>
        ///     Upload video to Instagram TV
        /// </summary>
        /// <param name="video">Video to upload (aspect ratio is very important for thumbnail and video | range 0.5 - 1.0 | Width = 480, Height = 852)</param>
        /// <param name="title">Title</param>
        /// <param name="caption">Caption</param>
        /// <param name="sharePreviewToFeed">Show a preview on the feed</param>
        /// <param name="maxRetriesOnMediaConfiguration">Max retries on media configuration</param>
        Task<IResult<InstaMedia>> UploadVideoAsync(InstaVideoUpload video, string title, string caption, bool sharePreviewToFeed = false,
            int maxRetriesOnMediaConfiguration = 10);
        /// <summary>
        ///     Upload video to Instagram TV with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload (aspect ratio is very important for thumbnail and video | range 0.5 - 1.0 | Width = 480, Height = 852)</param>
        /// <param name="title">Title</param>
        /// <param name="caption">Caption</param>
        /// <param name="sharePreviewToFeed">Show a preview on the feed</param>
        /// <param name="maxRetriesOnMediaConfiguration">Max retries on media configuration</param>
        Task<IResult<InstaMedia>> UploadVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, string title, string caption, bool sharePreviewToFeed = false,
            int maxRetriesOnMediaConfiguration = 10);
        /// <summary>
        ///     Upload segmented video to igtv 
        /// </summary>
        /// <param name="tvVideo">IgTV Video to upload</param>
        /// <param name="title">Title</param>
        /// <param name="caption">Caption</param>
        /// <param name="maxRetriesOnMediaConfiguration">Max retries on media configuration</param>
        Task<IResult<InstaMedia>> UploadSegmentedVideoToTVAsync(InstaTVVideoUpload tvVideo, string title, string caption,
            int maxRetriesOnMediaConfiguration = 10);

        /// <summary>
        ///     Mark a media or medias as seen
        /// </summary>
        /// <param name="mediaPkImpression">Media Pk impression (<see cref="InstaMedia.Pk"/>)</param>
        /// <param name="progress">Progress time</param>
        /// <param name="mediaPKsGridImpressions">Media PKs grid impressions</param>
        Task<IResult<bool>> MarkAsSeenAsync(string mediaPkImpression, int progress = 0, string[] mediaPKsGridImpressions = null);
    }
}

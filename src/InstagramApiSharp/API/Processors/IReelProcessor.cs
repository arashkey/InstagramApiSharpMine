/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using System;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    public interface IReelProcessor
    {
        /// <summary>
        ///     Get user's reels clips (medias)
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaReelsMediaList>> GetUserReelsClipsAsync(long userId, PaginationParameters paginationParameters);

        /// <summary>
        ///     Mark reel feed as seen
        /// </summary>
        /// <param name="mediaPkImpression">Media pk (from <see cref="InstaMedia.Pk"/> )</param>
        Task<IResult<bool>> MarkReelAsSeenAsync(string mediaPkImpression);

        /// <summary>
        ///     Explore reel feeds
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaReelsMediaList>> GetReelsFeedsAsync(PaginationParameters paginationParameters);

        /// <summary>
        ///     Upload reel video
        /// </summary>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        Task<IResult<InstaMedia>> UploadReelVideoAsync(InstaVideoUpload video, string caption);

        /// <summary>
        ///     Upload reel video with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        Task<IResult<InstaMedia>> UploadReelVideoAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string caption);
    }
}

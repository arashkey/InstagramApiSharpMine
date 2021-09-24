/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Represents Instagram Music APIs
    /// </summary>
    public interface IMusicProcessor
    {
        
        /// <summary>
        ///     Get specific playlist
        /// </summary>
        /// <param name="playlistId">Playlist identifier</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaTrendingMusic>> GetPlaylistAsync(string playlistId,
            PaginationParameters paginationParameters);

        /// <summary>
        ///     Get specific playlist
        /// </summary>
        /// <param name="playlistId">Playlist identifier</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<IResult<InstaTrendingMusic>> GetPlaylistAsync(string playlistId,
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Browse musics
        /// </summary>
        /// <param name="cursor">Cursor => 0 means don't add it, if you want to paginate it, you should set to 30 or 60 or 90 or 120 and etc.</param>
        Task<IResult<InstaBrowseMusic>> BrowseMusicAsync(int cursor = 0);

        /// <summary>
        ///     Music keyword search
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <param name="count">Count of results</param>
        Task<IResult<List<string>>> SearchKeywordAsync(string query, uint count = 3);

        /// <summary>
        ///     Get trending musics
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaTrendingMusic>> GetTrendingMusicAsync(PaginationParameters paginationParameters);
        
        /// <summary>
        ///     Get trending musics
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<IResult<InstaTrendingMusic>> GetTrendingMusicAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken);
    }
}

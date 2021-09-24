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

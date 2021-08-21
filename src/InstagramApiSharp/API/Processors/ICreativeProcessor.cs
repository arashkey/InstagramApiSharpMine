/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System.Threading.Tasks;
namespace InstagramApiSharp.API.Processors
{
    public interface ICreativeProcessor
    {
        /// <summary>
        ///     Get clips assets
        /// </summary>
        Task<IResult<InstaStickers>> GetClipsAssetsAsync();

        Task<IResult<bool>> WriteSupportedCapablititiesAsync();

        /// <summary>
        ///     Get creative assets
        /// </summary>
        Task<IResult<InstaStickers>> GetAssetsAsync();
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using System.Threading.Tasks;
using InstagramApiSharp.Classes.Models;

namespace InstagramApiSharp.API.Processors
{
    public interface IVideoCallProcessor 
    {
        /// <summary>
        ///     1
        /// </summary>
        Task<IResult<InstaVideoCallNtpClock>> GetNtpClockAsync();

        /*/// <summary>
        ///     2 NOT COMPELETE
        /// </summary>
        Task<IResult<InstaVideoCallJoin>> JoinAsync();*/

        /*/// <summary>
        ///     3 NOT COMPELETE
        /// </summary>
        Task<IResult<bool>> ConfirmAsync(long videoCallId);*/

        /// <summary>
        ///     4
        /// </summary>
        Task<IResult<InstaVideoCallInfo>> GetInfoAsync(long videoCallId);

        /// <summary>
        ///     5
        /// </summary>
        Task<IResult<InstaVideoCallAdd>> AddVideoCallToDirectAsync(string threadId, long videoCallId);

        /// <summary>
        ///     6
        /// </summary>
        Task<IResult<bool>> LeaveAsync(long videoCallId);
    }
}

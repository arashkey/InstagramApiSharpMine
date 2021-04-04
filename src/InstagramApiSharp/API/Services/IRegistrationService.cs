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

namespace InstagramApiSharp.API.Services
{
    public interface IRegistrationService
    {
        #region Properties

        /// <summary>
        ///     Waterfall id for registration
        /// </summary>
        string RegistrationWaterfallId { get; set; }
        /// <summary>
        ///     Signup code from Registration via Email
        /// </summary>
        string ForceSignupCode { get; set; }
        /// <summary>
        ///     Birthday for age consent
        /// </summary>
        DateTime Birthday { get; set; }
        /// <summary>
        ///     Check Email Registration response
        /// </summary>
        InstaCheckEmailRegistration InstaCheckEmailRegistration { get; set; }
        #endregion Properties

        #region Public functions

        /// <summary>
        ///  Generate random birthday
        /// </summary>
        DateTime GenerateRandomBirthday();

        #endregion


        #region Public Async Functions

        /// <summary>
        ///     Get first contactpoint prefill [ sends before new registration account ]
        /// </summary>
        Task<IResult<bool>> GetFirstContactPointPrefillAsync();

        /// <summary>
        ///     First launcher sync [ sends before new registration account ]
        /// </summary>
        Task<IResult<bool>> FirstLauncherSyncAsync();

        /// <summary>
        ///     First Qe sync [ sends before new registration account ]
        /// </summary>
        Task<IResult<bool>> FirstQeSyncAsync();

        /// <summary>
        ///     Check username availablity
        /// </summary>
        /// <param name="username">Username</param>
        Task<IResult<InstaAccountCheck>> CheckUsernameAsync(string username);

        #endregion Public Async Functions
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Services
{
    internal class RegistrationService : IRegistrationService
    {
        #region Properties

        /// <summary>
        ///     Waterfall id for registration
        /// </summary>
        public string RegistrationWaterfallId { get; set; }
        /// <summary>
        ///     Signup code from Registration via Email
        /// </summary>
        public string ForceSignupCode { get; set; }
        /// <summary>
        ///     Birthday for age consent
        /// </summary>
        public DateTime Birthday { get; set; }

        #endregion Properties

        #region Fields and constructor
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        private readonly InstaApi _instaApi;
        private readonly HttpHelper _httpHelper;

        public RegistrationService(AndroidDevice deviceInfo, UserSessionData user,
            IHttpRequestProcessor httpRequestProcessor, IInstaLogger logger,
            UserAuthValidate userAuthValidate, InstaApi instaApi, HttpHelper httpHelper)
        {
            _deviceInfo = deviceInfo;
            _user = user;
            _httpRequestProcessor = httpRequestProcessor;
            _logger = logger;
            _userAuthValidate = userAuthValidate;
            _instaApi = instaApi;
            _httpHelper = httpHelper;
            RegistrationWaterfallId = Guid.NewGuid().ToString();
        }
        #endregion Properties and constructor
    }
}

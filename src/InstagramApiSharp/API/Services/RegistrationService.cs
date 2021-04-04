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
            Birthday = GenerateRandomBirthday();
        }
        #endregion Properties and constructor

        #region Private functions

        void ValidateUser(InstaUserShortResponse user) =>
            _user.LoggedInUser = ConvertersFabric.Instance.GetUserShortConverter(user)?.Convert();

        #endregion Private functions

        #region Public functions

        /// <summary>
        ///  Generate random birthday
        /// </summary>
        public DateTime GenerateRandomBirthday()
        {
            var rnd = new Random();
            int day = rnd.Next(1, 29);
            int month = rnd.Next(1, 12);
            int year = rnd.Next(1979, 2000);
            return new DateTime(year, month, day);
        }

        #endregion



        #region Public Async Functions

        /// <summary>
        ///     Get first contactpoint prefill [ sends before new registration account ]
        /// </summary>
        public async Task<IResult<bool>> GetFirstContactPointPrefillAsync()
        {
            try
            {
                var data = new JObject
                {
                    {"phone_id",            _deviceInfo.PhoneGuid.ToString()},
                    {"_csrftoken",          _user.CsrfToken},
                    {"usage",               "prefill"},
                };
                var instaUri = UriCreator.GetContactPointPrefillUri(true);
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     First launcher sync [ sends before new registration account ]
        /// </summary>
        public async Task<IResult<bool>> FirstLauncherSyncAsync()
        {
            try
            {
                var data = new JObject
                {
                    {"_csrftoken",                  _user.CsrfToken},
                    {"id",                          _deviceInfo.DeviceGuid.ToString()},
                    {"server_config_retrieval",     "1"}
                };
                var instaUri = UriCreator.GetLauncherSyncUri();
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                _user.SetCsrfTokenIfAvailable(response, _httpRequestProcessor);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }


        #endregion Public Async Functions
    }
}

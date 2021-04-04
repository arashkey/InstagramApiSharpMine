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

        /// <summary>
        ///     Check Email Registration response
        /// </summary>
        public InstaCheckEmailRegistration InstaCheckEmailRegistration { get; set; }

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

        /// <summary>
        ///     First Qe sync [ sends before new registration account ]
        /// </summary>
        public async Task<IResult<bool>> FirstQeSyncAsync()
        {
            try
            {
                var data = new JObject
                {
                    {"_csrftoken",                  _user.CsrfToken},
                    {"id",                          _deviceInfo.DeviceGuid.ToString()},
                    {"server_config_retrieval",     "1"},
                    {"experiments",                 "ig_android_reg_nux_headers_cleanup_universe,ig_android_device_detection_info_upload,ig_android_gmail_oauth_in_reg,ig_android_device_info_foreground_reporting,ig_android_device_verification_fb_signup,ig_android_passwordless_account_password_creation_universe,ig_android_direct_add_direct_to_android_native_photo_share_sheet,ig_growth_android_profile_pic_prefill_with_fb_pic_2,ig_account_identity_logged_out_signals_global_holdout_universe,ig_android_quickcapture_keep_screen_on,ig_android_device_based_country_verification,ig_android_login_identifier_fuzzy_match,ig_android_reg_modularization_universe,ig_android_security_intent_switchoff,ig_android_device_verification_separate_endpoint,ig_android_suma_landing_page,ig_android_sim_info_upload,ig_android_fb_account_linking_sampling_freq_universe,ig_android_retry_create_account_universe,ig_android_caption_typeahead_fix_on_o_universe"},
                };

                var instaUri = UriCreator.GetQeSyncUri();
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

        /// <summary>
        ///     Check username availablity
        /// </summary>
        /// <param name="username">Username</param>
        public async Task<IResult<InstaAccountCheck>> CheckUsernameAsync(string username)
        {
            try
            {
                var instaUri = UriCreator.GetCheckUsernameUri();
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"username", username}
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaAccountCheck>(json);
                _user.SetCsrfTokenIfAvailable(response, _httpRequestProcessor);
                if (!obj.IsSucceed || response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaAccountCheck>(response, json);
                else
                {
                    _instaApi._user.PublicKey = _user.PublicKey = string.Join("", response.Headers.GetValues(InstaApiConstants.RESPONSE_HEADER_IG_PASSWORD_ENC_PUB_KEY));
                    _instaApi._user.PublicKeyId = _user.PublicKeyId = string.Join("", response.Headers.GetValues(InstaApiConstants.RESPONSE_HEADER_IG_PASSWORD_ENC_KEY_ID));
                    return Result.Success(obj);
                }
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaAccountCheck), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaAccountCheck>(exception);
            }
        }

        /// <summary>
        ///     Check email availablity
        /// </summary>
        /// <param name="email">Email</param>
        public async Task<IResult<InstaCheckEmailRegistration>> CheckEmailAsync(string email)
        {
            try
            {
                if (RegistrationWaterfallId == null)
                    RegistrationWaterfallId = Guid.NewGuid().ToString();

                var data = new Dictionary<string, string>
                {
                    {"android_device_id",   _deviceInfo.DeviceId},
                    {"login_nonce_map",     "{}"},
                    {"_csrftoken",          _user.CsrfToken},
                    {"login_nonces",        "[]"},
                    {"email",               email},
                    {"qe_id",               Guid.NewGuid().ToString()},
                    {"waterfall_id",        RegistrationWaterfallId},
                };
                var instaUri = UriCreator.GetCheckEmailUri();
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                _user.SetCsrfTokenIfAvailable(response, _httpRequestProcessor);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<InstaCheckEmailRegistration>(json);
                    if (obj.ErrorType == "fail")
                        return Result.UnExpectedResponse<InstaCheckEmailRegistration>(response, json);
                    if (obj.ErrorType == "email_is_taken")
                        return Result.Fail("Email is taken.", (InstaCheckEmailRegistration)null);
                    if (obj.ErrorType == "invalid_email")
                        return Result.Fail("Please enter a valid email address.", (InstaCheckEmailRegistration)null);

                    return Result.UnExpectedResponse<InstaCheckEmailRegistration>(response, json);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<InstaCheckEmailRegistration>(json);
                    if (obj.ErrorType == "fail")
                        return Result.UnExpectedResponse<InstaCheckEmailRegistration>(response, json);
                    if (obj.ErrorType == "email_is_taken")
                        return Result.Fail("Email is taken.", (InstaCheckEmailRegistration)null);
                    if (obj.ErrorType == "invalid_email")
                        return Result.Fail("Please enter a valid email address.", (InstaCheckEmailRegistration)null);
                    InstaCheckEmailRegistration = obj;
                    return Result.Success(obj);
                }
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaCheckEmailRegistration), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaCheckEmailRegistration>(exception);
            }
        }


        /// <summary>
        ///     Get signup consent config
        /// </summary>
        /// <param name="isMainAccount">Is this main account ? always set to to false</param>
        /// <param name="loggedInUserId">Logged in user id (pk) if available</param>
        public async Task<IResult<InstaSignupConsentConfig>> GetSignupConsentConfigAsync(bool isMainAccount = false, long? loggedInUserId = null)
        {
            try
            {
                var instaUri = UriCreator.GetSignupConsentConfigUri(_deviceInfo.DeviceGuid.ToString(), isMainAccount, loggedInUserId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                _user.SetCsrfTokenIfAvailable(response, _httpRequestProcessor);
                var obj = JsonConvert.DeserializeObject<InstaSignupConsentConfig>(json);

                return obj.IsSucceed ? Result.Success(obj) : Result.UnExpectedResponse<InstaSignupConsentConfig>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaSignupConsentConfig), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaSignupConsentConfig>(exception);
            }
        }

        /// <summary>
        ///     Send registration verify email
        /// </summary>
        /// <param name="email">Email</param>
        public async Task<IResult<bool>> SendRegistrationVerifyEmailAsync(string email)
        {
            try
            {
                var data = new Dictionary<string, string>
                {
                    {"phone_id",            _deviceInfo.PhoneGuid.ToString()},
                    {"_csrftoken",          _user.CsrfToken},
                    {"guid",                _deviceInfo.DeviceGuid.ToString()},
                    {"device_id",           _deviceInfo.DeviceId},
                    {"email",               email},
                    {"waterfall_id",        RegistrationWaterfallId},
                    {"auto_confirm_only",   "false"},
                };
                var instaUri = UriCreator.GetSendRegistrationVerifyEmailUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
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

        /// <summary>
        ///     Check registration confirmation code from email
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="verificationCode">Verification code from email</param>
        public async Task<IResult<InstaRegistrationConfirmationCode>> CheckRegistrationConfirmationCodeAsync(string email, string verificationCode)
        {
            try
            {
                var data = new Dictionary<string, string>
                {
                    {"_csrftoken",          _user.CsrfToken},
                    {"code",                verificationCode},
                    {"device_id",           _deviceInfo.DeviceId},
                    {"email",               email},
                    {"waterfall_id",        RegistrationWaterfallId},
                };
                var instaUri = UriCreator.GetCheckRegistrationConfirmationCodeUri();
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                _user.SetCsrfTokenIfAvailable(response, _httpRequestProcessor);
                var obj = JsonConvert.DeserializeObject<InstaRegistrationConfirmationCode>(json);
                ForceSignupCode = obj.SignupCode;
                return obj.IsSucceed ? Result.Success(obj) : Result.UnExpectedResponse<InstaRegistrationConfirmationCode>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRegistrationConfirmationCode), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaRegistrationConfirmationCode>(exception);
            }
        }

        #endregion Public Async Functions
    }
}

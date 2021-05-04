/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using InstagramApiSharp.Helpers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Classes.Models;
using System.Net;
using InstagramApiSharp.Converters.Json;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Classes.ResponseWrappers.Business;
using System.Linq;

namespace InstagramApiSharp.API.Processors
{
    internal class CreativeProcessor : ICreativeProcessor
    {
        #region Properties and constructor
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        private readonly InstaApi _instaApi;
        private readonly HttpHelper _httpHelper;
        public CreativeProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        }
        #endregion Properties and constructor

        /// <summary>
        ///     Get clips assets
        /// </summary>
        public async Task<IResult<InstaStickers>> GetClipsAssetsAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetCreativeClipsAssetsUri();

                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"type", "static_stickers"},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStickers>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaStickers>(json);

                return obj.IsSucceed ? Result.Success(obj) : Result.UnExpectedResponse<InstaStickers>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStickers), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStickers>(exception);
            }
        }

        public async Task<IResult<bool>> WriteSupportedCapablititiesAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetWriteSupportedCapabilitiesUri();
                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

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
        ///     Get creative assets
        /// </summary>
        public async Task<IResult<InstaStickers>> GetAssetsAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetCreativeAssetsUri();

                var data = new JObject
                {
                    {"horizontalAccuracy", "20.0"},
                    {"alt", "0.0"},
                    {"lat", "0.0"},
                    {"lng", "0.0"},
                    {"type", "static_stickers"},
                    {"speed", "0.0"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStickers>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaStickers>(json);

                return obj.IsSucceed ? Result.Success(obj) : Result.UnExpectedResponse<InstaStickers>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStickers), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStickers>(exception);
            }
        }
    }
}

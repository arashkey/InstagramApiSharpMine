﻿using System;
using System.Net.Http;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Logger;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Classes.SessionHandlers;
using System.Net;
using System.Linq;

namespace InstagramApiSharp.API.Builder
{
    public class InstaApiBuilder : IInstaApiBuilder
    {
        private IConfigureMediaDelay _configureMediaDelay = ConfigureMediaDelay.PreferredDelay();
        private IRequestDelay _delay = RequestDelay.Empty();
        private AndroidDevice _device;
        private HttpClient _httpClient;
        private HttpClientHandler _httpHandler = new HttpClientHandler()
        {
            UseProxy = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        private IHttpRequestProcessor _httpRequestProcessor;
        private IInstaLogger _logger;
        private ApiRequestMessage _requestMessage;
        private UserSessionData _user;
        private InstaApiVersionType? _apiVersionType;
        private ISessionHandler _sessionHandler;

        private InstaApiBuilder()
        {
        }

        /// <summary>
        ///     Create new API instance
        /// </summary>
        /// <returns>
        ///     API instance
        /// </returns>
        /// <exception cref="ArgumentNullException">User auth data must be specified</exception>
        public IInstaApi Build()
        {
            if (_user == null)
                _user = UserSessionData.Empty;

            if (_httpHandler == null) _httpHandler = new HttpClientHandler
            {
                UseProxy = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            if (_httpClient == null)
                _httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri(InstaApiConstants.INSTAGRAM_URL) };

            if (_requestMessage == null)
            {
                if (_device == null)
                    _device = AndroidDeviceGenerator.GetRandomAndroidDevice();
                _requestMessage = new ApiRequestMessage
                {
                    PhoneId = _device.PhoneGuid.ToString(),
                    Guid = _device.DeviceGuid,
                    Password = _user?.Password,
                    Username = _user?.UserName,
                    DeviceId = ApiRequestMessage.GenerateDeviceId(),
                    AdId = _device.AdId.ToString()
                };
            }

            if (string.IsNullOrEmpty(_requestMessage.Password)) _requestMessage.Password = _user?.Password;
            if (string.IsNullOrEmpty(_requestMessage.Username)) _requestMessage.Username = _user?.UserName;

            try
            {
                InstaApiConstants.TIMEZONE_OFFSET = int.Parse(DateTimeOffset.Now.Offset.TotalSeconds.ToString());
            }
            catch { }

            if (_httpRequestProcessor == null)
                _httpRequestProcessor =
                    new HttpRequestProcessor(_delay, _httpClient, _httpHandler, _requestMessage, _logger);

            if (_apiVersionType == null)
                _apiVersionType = InstaApiVersionType.Version169;

            var instaApi = new InstaApi(_user, _logger, _device, _httpRequestProcessor, _apiVersionType.Value, _configureMediaDelay);
            if (_sessionHandler != null)
            {
                _sessionHandler.InstaApi = instaApi;
                instaApi.SessionHandler = _sessionHandler;
            }
            return instaApi;
        }

        /// <summary>
        ///     Use custom logger
        /// </summary>
        /// <param name="logger">IInstaLogger implementation</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder UseLogger(IInstaLogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        ///     Set specific HttpClient
        /// </summary>
        /// <param name="httpClient">HttpClient</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder UseHttpClient(HttpClient httpClient)
        {
            if (httpClient != null)
                httpClient.BaseAddress = new Uri(InstaApiConstants.INSTAGRAM_URL);
                
            _httpClient = httpClient;
            return this;
        }

        /// <summary>
        ///     Set custom HttpClientHandler to be able to use certain features, e.g Proxy and so on
        /// </summary>
        /// <param name="handler">HttpClientHandler</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder UseHttpClientHandler(HttpClientHandler handler)
        {
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            _httpHandler = handler;
            return this;
        }

        /// <summary>
        ///     Specify user login, password from here
        /// </summary>
        /// <param name="user">User auth data</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder SetUser(UserSessionData user)
        {
            _user = user;
            return this;
        }

        /// <summary>
        ///     Set custom request message. Used to be able to customize device info.
        /// </summary>
        /// <param name="requestMessage">Custom request message object</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        /// <remarks>
        ///     Please, do not use if you don't know what you are doing
        /// </remarks>
        public IInstaApiBuilder SetApiRequestMessage(ApiRequestMessage requestMessage)
        {
            _requestMessage = requestMessage;
            return this;
        }

        /// <summary>
        ///     Set delay between requests. Useful when API supposed to be used for mass-bombing.
        /// </summary>
        /// <param name="delay">Timespan delay</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder SetRequestDelay(IRequestDelay delay)
        {
            if (delay == null)
                delay = RequestDelay.Empty();
            _delay = delay;
            return this;
        }
        /// <summary>
        ///     Set delay before configuring medias [only for uploading parts]
        /// </summary>
        /// <param name="configureMediaDelay">Timespan delay for configuring Media</param>
        /// <returns>API Builder</returns>
        public IInstaApiBuilder SetConfigureMediaDelay(IConfigureMediaDelay configureMediaDelay)
        {
            if (configureMediaDelay == null)
                configureMediaDelay = ConfigureMediaDelay.PreferredDelay();
            _configureMediaDelay = configureMediaDelay;
            return this;
        }
        /// <summary>
        ///     Set custom android device.
        ///     <para>Note: this is optional, if you didn't set this, InstagramApiSharp will choose random device.</para>
        /// </summary>
        /// <param name="androidDevice">Android device</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder SetDevice(AndroidDevice androidDevice)
        {
            _device = androidDevice;
            return this;
        }
        /// <summary>
        ///     Set instagram api version (for user agent version)
        /// </summary>
        /// <param name="apiVersion">Api version</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder SetApiVersion(InstaApiVersionType apiVersion)
        {
            _apiVersionType = apiVersion;
            return this;
        }

        /// <summary>
        ///     Set session handler
        /// </summary>
        /// <param name="sessionHandler">Session handler</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder SetSessionHandler(ISessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
            return this;
        }

        /// <summary>
        ///     Set Http request processor
        /// </summary>
        /// <param name="httpRequestProcessor">HttpRequestProcessor</param>
        /// <returns>
        ///     API Builder
        /// </returns>
        public IInstaApiBuilder SetHttpRequestProcessor(IHttpRequestProcessor httpRequestProcessor)
        {
            if (httpRequestProcessor.Client != null)
                httpRequestProcessor.Client.BaseAddress = new Uri(InstaApiConstants.INSTAGRAM_URL);
            if (httpRequestProcessor.HttpHandler != null)
                httpRequestProcessor.HttpHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpRequestProcessor = httpRequestProcessor;
            return this;
        }

        /// <summary>
        ///     Try to parse user agent and set it if possible
        /// </summary>
        /// <param name="userAgent">User agent</param>
        /// <param name="deviceGuid">Device Guid, it's _uuid in Instagram requests</param>
        /// <param name="phoneGuid">Phone Guid, it's phone_id in Instagram requests</param>
        public IInstaApiBuilder TryParseAndSetUserAgent(string userAgent, string deviceGuid = null, string phoneGuid = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userAgent))
                    return this;
                var parts = userAgent.Substring(userAgent.IndexOf(" (") + 2).TrimEnd(')').Split(';')
                                .Select(xx => xx.Trim())?.ToArray();
                if (parts?.Length == 9)
                {
                    var toDeviceGuid = string.IsNullOrEmpty(deviceGuid) ? Guid.NewGuid() : new Guid(deviceGuid);
                    var toPhoneGuid = string.IsNullOrEmpty(phoneGuid) ? Guid.NewGuid() : new Guid(phoneGuid);

                    var f1 = parts[0].Split('/');
                    var lang = parts[7].Replace("_", "-");
                    var part3 = parts[3].Split('/');

                    var androidVer = new AndroidVersion
                    {
                        Codename = f1[0],
                        APILevel = f1[0],
                        VersionNumber = f1[1]
                    };
                    var device = new AndroidDevice
                    {
                        Dpi = parts[1],
                        Resolution = parts[2],
                        HardwareManufacturer = part3.Length > 0 ? part3[0] : parts[3],
                        AndroidBoardName = part3.Length > 1 ? part3[2] : parts[3].ToLower(),
                        DeviceModelIdentifier = parts[4],
                        FirmwareBrand = parts[5],
                        HardwareModel = parts[6],
                        DeviceGuid = toDeviceGuid,
                        PhoneGuid = toPhoneGuid,
                        DeviceId = ApiRequestMessage.GenerateDeviceIdFromGuid(toDeviceGuid),
                        AndroidVer = androidVer
                    };

                    InstaApiConstants.ACCEPT_LANGUAGE = lang;

                    return SetDevice(device);
                }
            }
            catch { }

            return this;
        }
        /// <summary>
        ///     Creates the builder.
        /// </summary>
        /// <returns>
        ///     API Builder
        /// </returns>
        public static IInstaApiBuilder CreateBuilder()
        {
            return new InstaApiBuilder();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InstagramApiSharp.Enums;
using InstagramApiSharp.API.Versions;
using System.Net;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;

namespace InstagramApiSharp.Helpers
{
    public class HttpHelper
    {
        public /*readonly*/ InstaApiVersion _apiVersion;
        readonly Random Rnd = new Random();
        public IHttpRequestProcessor _httpRequestProcessor;
        public IInstaApi _instaApi;
        internal bool IsNewerApis => _instaApi.InstaApiVersionType > InstaApiVersionType.Version146;
        internal static readonly System.Globalization.CultureInfo EnglishCulture = new System.Globalization.CultureInfo("en-us");

        internal HttpHelper(InstaApiVersion apiVersionType, IHttpRequestProcessor httpRequestProcessor, IInstaApi instaApi)
        {
            _apiVersion = apiVersionType;
            _httpRequestProcessor = httpRequestProcessor;
            _instaApi = instaApi;
        }

        public HttpRequestMessage GetDefaultRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo)
        {
            var currentCulture = GetCurrentCulture();
            System.Globalization.CultureInfo.CurrentCulture = EnglishCulture;
            var userAgent = deviceInfo.GenerateUserAgent(_apiVersion);

            var request = new HttpRequestMessage(method, uri);
            var cookies = _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(_httpRequestProcessor.Client
                       .BaseAddress);
            var mid = cookies[InstaApiConstants.COOKIES_MID]?.Value ?? string.Empty;
            var dsUserId = cookies[InstaApiConstants.COOKIES_DS_USER_ID]?.Value ?? string.Empty;
            //var sessionId = cookies[InstaApiConstants.COOKIES_SESSION_ID]?.Value ?? string.Empty;
            var shbid = cookies[InstaApiConstants.COOKIES_SHBID]?.Value ?? string.Empty;
            var shbts = cookies[InstaApiConstants.COOKIES_SHBTS]?.Value ?? string.Empty;
            var rur = cookies[InstaApiConstants.COOKIES_RUR]?.Value ?? string.Empty;
            var igDirectRegionHint = cookies[InstaApiConstants.COOKIES_IG_DIRECT_REGION_HINT]?.Value ?? string.Empty;

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_APP_LOCALE.ToLower() 
                : InstaApiConstants.HEADER_X_IG_APP_LOCALE, InstaApiConstants.ACCEPT_LANGUAGE.Replace("-", "_"));

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_DEVICE_LOCALE.ToLower()
                : InstaApiConstants.HEADER_X_IG_DEVICE_LOCALE, InstaApiConstants.ACCEPT_LANGUAGE.Replace("-", "_"));

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_MAPPED_LOCALE.ToLower() 
                : InstaApiConstants.HEADER_X_IG_MAPPED_LOCALE, InstaApiConstants.ACCEPT_LANGUAGE.Replace("-", "_"));

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_PIGEON_SESSION_ID.ToLower() 
                : InstaApiConstants.HEADER_PIGEON_SESSION_ID, deviceInfo.PigeonSessionId.ToString());

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_PIGEON_RAWCLINETTIME.ToLower()
                : InstaApiConstants.HEADER_PIGEON_RAWCLINETTIME, $"{DateTime.UtcNow.ToUnixTime()}.0{Rnd.Next(10, 99)}");

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_CONNECTION_SPEED.ToLower()
                : InstaApiConstants.HEADER_X_IG_CONNECTION_SPEED, "-1kbps");

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BANDWIDTH_SPEED_KBPS.ToLower() 
                : InstaApiConstants.HEADER_X_IG_BANDWIDTH_SPEED_KBPS, deviceInfo.IGBandwidthSpeedKbps);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALBYTES_B.ToLower() 
                : InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALBYTES_B, deviceInfo.IGBandwidthTotalBytesB);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALTIME_MS.ToLower() 
                : InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALTIME_MS, deviceInfo.IGBandwidthTotalTimeMS);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_APP_STARTUP_COUNTRY.ToLower() 
                : InstaApiConstants.HEADER_IG_APP_STARTUP_COUNTRY, InstaApiConstants.HEADER_IG_APP_STARTUP_COUNTRY_VALUE);

            //request.Headers.Add(InstaApiConstants.HEADER_X_IG_EXTENDED_CDN_THUMBNAIL_CACHE_BUSTING_VALUE, "1000");
            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BLOKS_VERSION_ID.ToLower()
                  : InstaApiConstants.HEADER_X_IG_BLOKS_VERSION_ID, _apiVersion.BloksVersionId);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BLOKS_PANORAMA_ENABLED.ToLower()
                : InstaApiConstants.HEADER_X_IG_BLOKS_PANORAMA_ENABLED, "true");

            var wwwClaim = _instaApi.GetLoggedUser()?.WwwClaim;

            if (!string.IsNullOrEmpty(wwwClaim))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_WWW_CLAIM.ToLower()
                    : InstaApiConstants.HEADER_X_WWW_CLAIM, wwwClaim);
            else
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_WWW_CLAIM.ToLower()
                    : InstaApiConstants.HEADER_X_WWW_CLAIM, InstaApiConstants.HEADER_X_WWW_CLAIM_DEFAULT);

            var authorization = _instaApi.GetLoggedUser()?.Authorization;

            if (!string.IsNullOrEmpty(dsUserId) && !string.IsNullOrEmpty(authorization))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_AUTHORIZATION.ToLower()
                    : InstaApiConstants.HEADER_AUTHORIZATION, authorization);
         
            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BLOKS_IS_LAYOUT_RTL.ToLower() 
                : InstaApiConstants.HEADER_X_IG_BLOKS_IS_LAYOUT_RTL, "false");

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_BLOKS_ENABLE_RENDERCODE.ToLower() 
                : InstaApiConstants.HEADER_X_IG_BLOKS_ENABLE_RENDERCODE, "false");

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_DEVICE_ID.ToLower() 
                : InstaApiConstants.HEADER_X_IG_DEVICE_ID, deviceInfo.DeviceGuid.ToString());

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_ANDROID_ID.ToLower()
                : InstaApiConstants.HEADER_X_IG_ANDROID_ID, deviceInfo.DeviceId);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_CONNECTION_TYPE.ToLower()
                : InstaApiConstants.HEADER_IG_CONNECTION_TYPE, InstaApiConstants.IG_CONNECTION_TYPE);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_CAPABILITIES.ToLower()
                : InstaApiConstants.HEADER_IG_CAPABILITIES, _apiVersion.Capabilities);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_APP_ID.ToLower() 
                : InstaApiConstants.HEADER_IG_APP_ID, InstaApiConstants.IG_APP_ID);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_IG_TIGON_RETRY.ToLower()
                : InstaApiConstants.HEADER_X_IG_TIGON_RETRY, "False");

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_USER_AGENT.ToLower() 
                : InstaApiConstants.HEADER_USER_AGENT, userAgent);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_ACCEPT_LANGUAGE.ToLower() 
                : InstaApiConstants.HEADER_ACCEPT_LANGUAGE, InstaApiConstants.ACCEPT_LANGUAGE);


            if (!string.IsNullOrEmpty(mid))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_MID.ToLower() 
                    : InstaApiConstants.HEADER_X_MID, mid);

            if (!string.IsNullOrEmpty(dsUserId) && !string.IsNullOrEmpty(authorization) && !string.IsNullOrEmpty(igDirectRegionHint))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_U_DIRECT_REGION_HINT.ToLower()
                    : InstaApiConstants.HEADER_IG_U_DIRECT_REGION_HINT, igDirectRegionHint);

            if (!string.IsNullOrEmpty(dsUserId) && !string.IsNullOrEmpty(authorization) && !string.IsNullOrEmpty(shbid))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_U_SHBID.ToLower() 
                    : InstaApiConstants.HEADER_IG_U_SHBID, shbid);

            if (!string.IsNullOrEmpty(dsUserId) && !string.IsNullOrEmpty(authorization) && !string.IsNullOrEmpty(shbts))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_U_SHBTS.ToLower()
                    : InstaApiConstants.HEADER_IG_U_SHBTS, shbts);

            if (!string.IsNullOrEmpty(dsUserId) && !string.IsNullOrEmpty(authorization))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_U_DS_USER_ID.ToLower() 
                    : InstaApiConstants.HEADER_IG_U_DS_USER_ID, dsUserId);

            if (!string.IsNullOrEmpty(dsUserId) && !string.IsNullOrEmpty(authorization) && !string.IsNullOrEmpty(rur))
                request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_IG_U_RUR.ToLower()
                    : InstaApiConstants.HEADER_IG_U_RUR, rur);



            request.Headers.TryAddWithoutValidation(InstaApiConstants.HEADER_ACCEPT_ENCODING, InstaApiConstants.ACCEPT_ENCODING2);

            request.Headers.Add(InstaApiConstants.HOST, InstaApiConstants.HOST_URI);

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_FB_HTTP_ENGINE.ToLower() 
                : InstaApiConstants.HEADER_X_FB_HTTP_ENGINE, "Liger");

            request.Headers.Add(IsNewerApis ? InstaApiConstants.HEADER_X_FB_HTTP_IP.ToLower()
                : InstaApiConstants.HEADER_X_FB_HTTP_IP, "True");
            //request.Properties.Add(new KeyValuePair<string, object>(InstaApiConstants.HEADER_XGOOGLE_AD_IDE,
            //    deviceInfo.GoogleAdId.ToString()));

            System.Globalization.CultureInfo.CurrentCulture = currentCulture;
            return request;
        }
        public HttpRequestMessage GetDefaultRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo, Dictionary<string, string> data)
        {
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(data);
            return request;
        }
        public async Task<HttpRequestMessage> GetDefaultGZipRequestAsync(HttpMethod method, Uri uri, AndroidDevice deviceInfo, Dictionary<string, string> data)
        {
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            var text = string.Empty;
            foreach (var item in data)
                text += $"{item.Key}={item.Value}&";
            text = text.TrimEnd('&');
            byte[] jsonBytes = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
//#if !WINDOWS_UWP
//            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
//                await gzip.WriteAsync(jsonBytes, 0, jsonBytes.Length);
//#else
            using (Ionic.Zlib.GZipStream gzip = new Ionic.Zlib.GZipStream(ms, Ionic.Zlib.CompressionMode.Compress, true))
                await gzip.WriteAsync(jsonBytes, 0, jsonBytes.Length);
//#endif
            ms.Position = 0;
            byte[] compressed = new byte[ms.Length];

            await ms.ReadAsync(compressed, 0, compressed.Length);
            MemoryStream outStream = new MemoryStream(compressed);

            //StreamContent content = new StreamContent(outStream);
            var bytes = outStream.ToArray();
            var content = new ByteArrayContent(bytes);
            content.Headers.Add("Content-Encoding", "gzip");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet= "UTF-8" };
            content.Headers.ContentLength = bytes.Length;
            request.Content = content;
            //request.Content = new FormUrlEncodedContent(data);
            //request.Content.Headers.Add("Content-Encoding", "gzip");
            return request;
        }
        /// <summary>
        ///     This is only for https://instagram.com site
        /// </summary>
        public HttpRequestMessage GetWebRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo)
        {
            var request = GetDefaultRequest(HttpMethod.Get, uri, deviceInfo);
            request.Headers.Remove(InstaApiConstants.HEADER_USER_AGENT);
            request.Headers.Add(InstaApiConstants.HEADER_USER_AGENT, InstaApiConstants.WEB_USER_AGENT);
            return request;
        }
        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            Dictionary<string, string> data, bool appendD = false)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{(IsNewerApis ? _apiVersion.SignatureKey : hash)}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
            };
            if (!IsNewerApis)
                fields.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                fields.Add("d", "0");
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
#if NET
            request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                request.Options.TryAdd("d", "0");
#else
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                request.Properties.Add("d", "0");
#endif
            return request;
        }
        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            Dictionary<string, int> data)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{(IsNewerApis ? _apiVersion.SignatureKey : hash)}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
            };
            if (!IsNewerApis)
                fields.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
#if NET
            request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
#else
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
#endif
            return request;
        }



        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            Dictionary<string, object> data, bool appendD = false)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{(IsNewerApis ? _apiVersion.SignatureKey : hash)}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
            };
            if (!IsNewerApis)
                fields.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                fields.Add("d", "0");
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
#if NET
            request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                request.Options.TryAdd("d", "0");
#else
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                request.Properties.Add("d", "0");
#endif
            return request;
        }

        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            JObject data, bool appendD = false)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                data.ToString(Formatting.None));
            var payload = data.ToString(Formatting.None);
            var signature = $"{(IsNewerApis ? _apiVersion.SignatureKey : hash)}.{payload}";
            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
            };
            if (!IsNewerApis)
                fields.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                fields.Add("d", "0");
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
#if NET
            request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Options.TryAdd(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                request.Options.TryAdd("d", "0");
#else
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!IsNewerApis)
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            if (appendD)
                request.Properties.Add("d", "0");
#endif
            return request;
        }

        public async Task<HttpRequestMessage> GetSignedGZipRequestAsync(HttpMethod method, Uri uri, AndroidDevice deviceInfo, Dictionary<string, string> data)
        {
            return await GetSignedGZipRequest(method, uri, deviceInfo, data);
        }
        public async Task<HttpRequestMessage> GetSignedGZipRequestAsync(HttpMethod method, Uri uri, AndroidDevice deviceInfo, Dictionary<string, int> data)
        {
            return await GetSignedGZipRequest(method, uri, deviceInfo, data);
        }
        public async Task<HttpRequestMessage> GetSignedGZipRequestAsync(HttpMethod method, Uri uri, AndroidDevice deviceInfo, Dictionary<string, object> data)
        {
            return await GetSignedGZipRequest(method, uri, deviceInfo, data);
        }
        public async Task<HttpRequestMessage> GetSignedGZipRequestAsync(HttpMethod method, Uri uri, AndroidDevice deviceInfo, JObject data)
        {
            return await GetSignedGZipRequest(method, uri, deviceInfo, data);
        }
        async Task<HttpRequestMessage> GetSignedGZipRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo, object data)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{InstaApiConstants.HEADER_IG_SIGNATURE}={(IsNewerApis ? _apiVersion.SignatureKey : hash)}.{payload}";
            if (!IsNewerApis)
                signature += $"&{InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION}={InstaApiConstants.IG_SIGNATURE_KEY_VERSION}";
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(signature);
            MemoryStream ms = new MemoryStream();
            //using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
            //    await gzip.WriteAsync(jsonBytes, 0, jsonBytes.Length);
            using (Ionic.Zlib.GZipStream gzip = new Ionic.Zlib.GZipStream(ms, Ionic.Zlib.CompressionMode.Compress, true))
                await gzip.WriteAsync(jsonBytes, 0, jsonBytes.Length);
            ms.Position = 0;
            byte[] compressed = new byte[ms.Length];

            await ms.ReadAsync(compressed, 0, compressed.Length);
            MemoryStream outStream = new MemoryStream(compressed);
            var bytes = outStream.ToArray();
            var content = new ByteArrayContent(bytes);
            content.Headers.Add("Content-Encoding", "gzip");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };
            content.Headers.ContentLength = bytes.Length;
            request.Content = content;
            return request;
        }

        public string GetSignature(JObject data)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey, data.ToString(Formatting.None));
            var payload = data.ToString(Formatting.None);
            var signature = $"{hash}.{payload}";
            return signature;
        }
        internal static System.Globalization.CultureInfo GetCurrentCulture() => System.Globalization.CultureInfo.CurrentCulture;

    }
}
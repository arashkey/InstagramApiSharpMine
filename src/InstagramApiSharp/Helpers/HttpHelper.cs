using InstagramApiSharp.API;
using InstagramApiSharp.API.Versions;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.Helpers
{
    public class HttpHelper
    {
        public InstaApiVersion _apiVersion;
        public IHttpRequestProcessor _httpRequestProcessor;
        public IInstaApi _instaApi;
        internal bool IsNewerApis => _instaApi.InstaApiVersionType > InstaApiVersionType.Version146;
        internal bool NewerThan180 => _instaApi.InstaApiVersionType > InstaApiVersionType.Version180;
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
            var userAgent = deviceInfo.GenerateUserAgent(_apiVersion, _instaApi);
            var currentUser = _instaApi.GetLoggedUser();
            var request = new HttpRequestMessage(method, uri);
            //var cookies = _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(_httpRequestProcessor.Client
            //           .BaseAddress);
            var shbid = currentUser.RespondUShbid;
            var shbts = currentUser.RespondUShbts;
            var igDirectRegionHint = currentUser.RespondUDirectRegionHint;
            var uRur = currentUser.RespondURur;

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_APP_LOCALE, _instaApi.AppLocale);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_DEVICE_LOCALE, _instaApi.DeviceLocale);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_MAPPED_LOCALE, _instaApi.MappedLocale);

            if (NewerThan180)
            {
                var pigeonId = "UFS-" + deviceInfo.PigeonSessionId.ToString() + "-0";
                //(_instaApi.IsUserAuthenticated || request.RequestUri.ToString().IndexOf("/launcher/sync") != -1 ||
                //request.RequestUri.ToString().IndexOf("/accounts/login") != -1 ? "-1" : "-0");
                request.Headers.Add(InstaApiConstants.HEADER_PIGEON_SESSION_ID, pigeonId);
            }
            else
                request.Headers.Add(InstaApiConstants.HEADER_PIGEON_SESSION_ID, deviceInfo.PigeonSessionId.ToString());

            request.Headers.Add(InstaApiConstants.HEADER_PIGEON_RAWCLINETTIME, DateTime.UtcNow.ToUnixTimeAsDouble().ToString());

            //request.Headers.Add(InstaApiConstants.HEADER_X_IG_CONNECTION_SPEED, "-1kbps");

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BANDWIDTH_SPEED_KBPS, deviceInfo.IGBandwidthSpeedKbps);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALBYTES_B, deviceInfo.IGBandwidthTotalBytesB);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALTIME_MS, deviceInfo.IGBandwidthTotalTimeMS);

            request.Headers.Add(InstaApiConstants.HEADER_IG_APP_STARTUP_COUNTRY, InstaApiConstants.HEADER_IG_APP_STARTUP_COUNTRY_VALUE);

            //request.Headers.Add(InstaApiConstants.HEADER_X_IG_EXTENDED_CDN_THUMBNAIL_CACHE_BUSTING_VALUE, "1000");
            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BLOKS_VERSION_ID, _apiVersion.BloksVersionId);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BLOKS_PANORAMA_ENABLED, "true");

            var wwwClaim = currentUser?.WwwClaim;

            if (wwwClaim.IsNotEmpty())
                request.Headers.Add(InstaApiConstants.HEADER_X_WWW_CLAIM, wwwClaim);
            else
                request.Headers.Add(InstaApiConstants.HEADER_X_WWW_CLAIM, InstaApiConstants.HEADER_X_WWW_CLAIM_DEFAULT);

            var authorization = currentUser?.Authorization;

            if (IsLoggedIn())
                request.Headers.Add(InstaApiConstants.HEADER_AUTHORIZATION, authorization);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BLOKS_IS_LAYOUT_RTL, "false");

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BLOKS_ENABLE_RENDERCODE, "false");

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_DEVICE_ID, deviceInfo.DeviceGuid.ToString());

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_FAMILY_DEVICE_ID, deviceInfo.PhoneGuid.ToString());

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_ANDROID_ID, deviceInfo.DeviceId);

            request.Headers.Add(InstaApiConstants.HEADER_IG_TIMEZONE_OFFSET, _instaApi.TimezoneOffset.ToString());

            request.Headers.Add(InstaApiConstants.HEADER_IG_CONNECTION_TYPE, InstaApiConstants.IG_CONNECTION_TYPE);

            request.Headers.Add(InstaApiConstants.HEADER_IG_CAPABILITIES, _apiVersion.Capabilities);

            request.Headers.Add(InstaApiConstants.HEADER_IG_APP_ID, InstaApiConstants.IG_APP_ID);

            request.Headers.Add(InstaApiConstants.HEADER_X_IG_TIGON_RETRY, "False");

            request.Headers.Add(InstaApiConstants.HEADER_USER_AGENT, userAgent);

            request.Headers.Add(InstaApiConstants.HEADER_ACCEPT_LANGUAGE, _instaApi.AcceptLanguage);

            if (currentUser.XMidHeader.IsNotEmpty())
                request.Headers.Add(InstaApiConstants.HEADER_X_MID, currentUser.XMidHeader);

            request.Headers.Add(InstaApiConstants.HEADER_IG_INTENDED_USER_ID, (currentUser.LoggedInUser?.Pk ?? 0).ToString());

            if (igDirectRegionHint.IsNotEmpty() && IsLoggedIn())
                request.Headers.Add(InstaApiConstants.HEADER_IG_U_DIRECT_REGION_HINT, igDirectRegionHint);

            if (shbid.IsNotEmpty() && IsLoggedIn())
                request.Headers.Add(InstaApiConstants.HEADER_IG_U_SHBID, shbid);

            if (shbts.IsNotEmpty() && IsLoggedIn())
                request.Headers.Add(InstaApiConstants.HEADER_IG_U_SHBTS, shbts);

            if (currentUser?.LoggedInUser?.Pk > 0 && IsLoggedIn())
                request.Headers.Add(InstaApiConstants.HEADER_IG_U_DS_USER_ID, currentUser.LoggedInUser.Pk.ToString());
           
            if (currentUser.RespondURur.IsNotEmpty())
                request.Headers.Add(InstaApiConstants.HEADER_IG_U_RUR, uRur);
            else if (currentUser.RurHeader.IsNotEmpty())
                request.Headers.Add(InstaApiConstants.HEADER_IG_U_RUR, currentUser.RurHeader);

            request.Headers.TryAddWithoutValidation(InstaApiConstants.HEADER_ACCEPT_ENCODING, InstaApiConstants.ACCEPT_ENCODING2);

            request.Headers.Add(InstaApiConstants.HOST, InstaApiConstants.HOST_URI);

            request.Headers.Add(InstaApiConstants.HEADER_X_FB_HTTP_ENGINE, "Liger");

            request.Headers.Add(InstaApiConstants.HEADER_X_FB_HTTP_IP, "True");

            request.Headers.Add(InstaApiConstants.HEADER_X_FB_SERVER_CLUSTER, "True");

            request.Headers.Add(InstaApiConstants.HEADER_IG_TIMEZONE_OFFSET, _instaApi.TimezoneOffset.ToString());

            request.Headers.Add(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_3);

            System.Globalization.CultureInfo.CurrentCulture = currentCulture;

            bool IsLoggedIn()
            {
                return authorization.IsNotEmpty() && _instaApi.IsUserAuthenticated;
            }

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
            Dictionary<string, string> data, bool appendD = false, string dValue = null)
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
                fields.Add("d", dValue ?? "0");
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            return request;
        }

        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            JObject data, bool appendD = false, string dValue = null)
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
                fields.Add("d", dValue ?? "0");
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);

            return request;
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
using System;
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
namespace InstagramApiSharp.Helpers
{
    public class HttpHelper
    {
        public /*readonly*/ InstaApiVersion _apiVersion;
        readonly Random Rnd = new Random();
        internal HttpHelper(InstaApiVersion apiVersionType)
        {
            _apiVersion = apiVersionType;
        }

        public HttpRequestMessage GetDefaultRequest(HttpMethod method, Uri uri, AndroidDevice deviceInfo)
        {
            var userAgent = deviceInfo.GenerateUserAgent(_apiVersion);

            var request = new HttpRequestMessage(method, uri);
            request.Headers.Add(InstaApiConstants.HEADER_PIGEON_SESSION_ID, deviceInfo.PigeonSessionId.ToString());
            request.Headers.Add(InstaApiConstants.HEADER_PIGEON_RAWCLINETTIME, $"{DateTime.UtcNow.ToUnixTime()}.0{Rnd.Next(10, 99)}");
            request.Headers.Add(InstaApiConstants.HEADER_X_IG_CONNECTION_SPEED, "-1kbps");
            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BANDWIDTH_SPEED_KBPS, deviceInfo.IGBandwidthSpeedKbps);
            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALBYTES_B, deviceInfo.IGBandwidthTotalBytesB);
            request.Headers.Add(InstaApiConstants.HEADER_X_IG_BANDWIDTH_TOTALTIME_MS, deviceInfo.IGBandwidthTotalTimeMS);
            request.Headers.Add(InstaApiConstants.HEADER_IG_CONNECTION_TYPE, InstaApiConstants.IG_CONNECTION_TYPE);
            request.Headers.Add(InstaApiConstants.HEADER_IG_CAPABILITIES, _apiVersion.Capabilities);
            request.Headers.Add(InstaApiConstants.HEADER_IG_APP_ID, InstaApiConstants.IG_APP_ID);
            request.Headers.Add(InstaApiConstants.HEADER_USER_AGENT, userAgent);
            request.Headers.Add(InstaApiConstants.HEADER_ACCEPT_LANGUAGE, InstaApiConstants.ACCEPT_LANGUAGE);
            //request.Properties.Add(new KeyValuePair<string, object>(InstaApiConstants.HEADER_XGOOGLE_AD_IDE,
            //    deviceInfo.GoogleAdId.ToString()));
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
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
                await gzip.WriteAsync(jsonBytes, 0, jsonBytes.Length);
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
            Dictionary<string, string> data)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{hash}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
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
            var signature = $"{hash}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            return request;
        }



        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            Dictionary<string, object> data)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                JsonConvert.SerializeObject(data));
            var payload = JsonConvert.SerializeObject(data);
            var signature = $"{hash}.{payload}";

            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
            return request;
        }

        public HttpRequestMessage GetSignedRequest(HttpMethod method,
            Uri uri,
            AndroidDevice deviceInfo,
            JObject data)
        {
            var hash = CryptoHelper.CalculateHash(_apiVersion.SignatureKey,
                data.ToString(Formatting.None));
            var payload = data.ToString(Formatting.None);
            var signature = $"{hash}.{payload}";
            var fields = new Dictionary<string, string>
            {
                {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
            };
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            request.Content = new FormUrlEncodedContent(fields);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
            request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
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
            var signature = $"{InstaApiConstants.HEADER_IG_SIGNATURE}={hash}.{payload}&" +
                $"{InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION}={InstaApiConstants.IG_SIGNATURE_KEY_VERSION}";
            var request = GetDefaultRequest(HttpMethod.Post, uri, deviceInfo);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(signature);
            MemoryStream ms = new MemoryStream();
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
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
    }
}
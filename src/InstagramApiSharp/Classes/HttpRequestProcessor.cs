using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;

namespace InstagramApiSharp.Classes
{
    internal class HttpRequestProcessor : IHttpRequestProcessor
    {
        private IRequestDelay _delay;
        private readonly IInstaLogger _logger;
        public IRequestDelay Delay { get { return _delay; } set { _delay = value; } }
        public IConfigureMediaDelay ConfigureMediaDelay { get; set; } = Classes.ConfigureMediaDelay.PreferredDelay();
        public HttpRequestProcessor(IRequestDelay delay, HttpClient httpClient, HttpClientHandler httpHandler,
            ApiRequestMessage requestMessage, IInstaLogger logger)
        {
            _delay = delay;
            Client = httpClient;
            HttpHandler = httpHandler;
            RequestMessage = requestMessage;
            _logger = logger;
        }

        public HttpClientHandler HttpHandler { get; set; }
        public ApiRequestMessage RequestMessage { get; }
        public HttpClient Client { get; set; }
        public void SetHttpClientHandler(HttpClientHandler handler)
        {
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpHandler = handler;
            Client = new HttpClient(handler) { BaseAddress = new Uri(InstaApiConstants.INSTAGRAM_URL) };
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, bool keepAlive = false)
        {
            await AppendOtherHeaders(requestMessage, keepAlive);
            LogHttpRequest(requestMessage);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.SendAsync(requestMessage);
            LogHttpResponse(response);
            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri requestUri, bool keepAlive = false)
        {
            await AppendOtherHeaders(null, keepAlive);
            _logger?.LogRequest(requestUri);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.GetAsync(requestUri);
            LogHttpResponse(response);
            return response;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
            HttpCompletionOption completionOption, bool keepAlive = false)
        {
            await AppendOtherHeaders(requestMessage, keepAlive);
            LogHttpRequest(requestMessage);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.SendAsync(requestMessage, completionOption);
            LogHttpResponse(response);
            return response;
        }

        public async Task<string> SendAndGetJsonAsync(HttpRequestMessage requestMessage,
            HttpCompletionOption completionOption, bool keepAlive = false)
        {
            await AppendOtherHeaders(requestMessage, keepAlive);
            LogHttpRequest(requestMessage);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.SendAsync(requestMessage, completionOption);
            LogHttpResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GeJsonAsync(Uri requestUri, bool keepAlive = false)
        {
            await AppendOtherHeaders(null, keepAlive);
            _logger?.LogRequest(requestUri);
            if (_delay.Exist)
                await Task.Delay(_delay.Value);
            var response = await Client.GetAsync(requestUri);
            LogHttpResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        private void LogHttpRequest(HttpRequestMessage request)
        {
            _logger?.LogRequest(request);
        }

        private void LogHttpResponse(HttpResponseMessage request)
        {
            _logger?.LogResponse(request);
        }

        async Task AppendOtherHeaders(HttpRequestMessage request, bool keepAlive = false)
        {
            var currentCulture = HttpHelper.GetCurrentCulture();
            System.Globalization.CultureInfo.CurrentCulture = HttpHelper.EnglishCulture;
            Client.DefaultRequestHeaders.ConnectionClose = keepAlive;
            if (request != null)
            {
                request.Headers.ConnectionClose = keepAlive;
                if (request.Content != null)
                {
                    if (request.Content.Headers.ContentType != null)
                        request.Content.Headers.ContentType.CharSet = "UTF-8";
                    var requestUri = request.RequestUri.ToString();

                    // since a file can be 600MB, reading it takes time, so we ignore it and don't add Content-Length header
                    if (WasntIndexOf(requestUri, "/upload/") && WasntIndexOf(requestUri, "/rupload_"))
                        request.Content.Headers.ContentLength = request.Content.ReadAsStringAsync().GetAwaiter().GetResult()?.Length;
                }
            }
            System.Globalization.CultureInfo.CurrentCulture = currentCulture;
            await Task.Delay(1).ConfigureAwait(false); // lets force compiler to wait for AppendOtherHeaders
        }

        static bool WasntIndexOf(string str1, string str2) => str1.IndexOf(str2, StringComparison.OrdinalIgnoreCase) == -1;

        //async Task<HttpResponseMessage> CopyResponseAsync(HttpResponseMessage response)
        //{
        //    await Task.Delay(350);
        //    var http = new HttpResponseMessage
        //    {
        //        Content = response.Content,
        //        ReasonPhrase = response.ReasonPhrase,
        //        StatusCode = response.StatusCode,
        //        RequestMessage = response.RequestMessage,
        //        Version = response.Version,
        //    };
        //    foreach (var item in response.Headers)
        //        http.Headers.Add(item.Key, item.Value);
        //    return http;
        //}
    }
}
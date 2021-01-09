using System;
using System.Net.Http;
using System.Threading.Tasks;
using InstagramApiSharp.Classes.Android.DeviceInfo;

namespace InstagramApiSharp.Classes
{
    public interface IHttpRequestProcessor
    {
        HttpClientHandler HttpHandler { get; set; }
        ApiRequestMessage RequestMessage { get; }
        HttpClient Client { get; }
        void SetHttpClientHandler(HttpClientHandler handler);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, bool keepAlive = false);
        Task<HttpResponseMessage> GetAsync(Uri requestUri, bool keepAlive = false);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, bool keepAlive = false);
        Task<string> SendAndGetJsonAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, bool keepAlive = false);
        Task<string> GeJsonAsync(Uri requestUri, bool keepAlive = false);
        IRequestDelay Delay { get; set; }
        IConfigureMediaDelay ConfigureMediaDelay { get; set; }
    }
}
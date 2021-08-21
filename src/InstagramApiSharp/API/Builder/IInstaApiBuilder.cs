using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Logger;
using System.Net.Http;

namespace InstagramApiSharp.API.Builder
{
    public interface IInstaApiBuilder
    {
        /// <summary>
        ///     Create new API instance
        /// </summary>
        /// <returns>API instance</returns>
        IInstaApi Build();

        /// <summary>
        ///     Use custom logger
        /// </summary>
        /// <param name="logger">IInstaLogger implementation</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder UseLogger(IInstaLogger logger);

        /// <summary>
        ///     Set specific HttpClient
        /// </summary>
        /// <param name="httpClient">HttpClient</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder UseHttpClient(HttpClient httpClient);

        /// <summary>
        ///     Set custom HttpClientHandler to be able to use certain features, e.g Proxy and so on
        /// </summary>
        /// <param name="handler">HttpClientHandler</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder UseHttpClientHandler(HttpClientHandler handler);


        /// <summary>
        ///     Specify user login, password from here
        /// </summary>
        /// <param name="user">User auth data</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder SetUser(UserSessionData user);

        /// <summary>
        ///     Set custom request message. Used to be able to customize device info.
        /// </summary>
        /// <param name="requestMessage">Custom request message object</param>
        /// <remarks>Please, do not use if you don't know what you are doing</remarks>
        /// <returns>API Builder</returns>
        IInstaApiBuilder SetApiRequestMessage(ApiRequestMessage requestMessage);

        /// <summary>
        ///     Set delay between requests. Useful when API supposed to be used for mass-bombing.
        /// </summary>
        /// <param name="delay">Timespan delay</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder SetRequestDelay(IRequestDelay delay);

        /// <summary>
        ///     Set delay before configuring medias [only for uploading parts]
        /// </summary>
        /// <param name="configureMediaDelay">Timespan delay for configuring Media</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder SetConfigureMediaDelay(IConfigureMediaDelay configureMediaDelay);
        //[Obsolete("Deprecated. Please use IInstaApi.SetDevice instead.")]
        /// <summary>
        ///     Set custom android device.
        ///     <para>Note: this is optional, if you didn't set this, <see cref="InstagramApiSharp"/> will choose random device.</para>
        /// </summary>
        /// <param name="androidDevice">Android device</param>
        /// <returns>API Builder</returns>
        IInstaApiBuilder SetDevice(AndroidDevice androidDevice);
        //[Obsolete("Deprecated. Please use IInstaApi.SetApiVersion instead.")]
        /// <summary>
        ///     Set instagram api version (for user agent version)
        /// </summary>
        /// <param name="apiVersion">Api version</param>
        IInstaApiBuilder SetApiVersion(InstaApiVersionType apiVersion);

        /// <summary>
        ///     Set Http request processor
        /// </summary>
        /// <param name="httpRequestProcessor">HttpRequestProcessor</param>
        /// <returns></returns>
        IInstaApiBuilder SetHttpRequestProcessor(IHttpRequestProcessor httpRequestProcessor);

        /// <summary>
        ///     Set session handler
        /// </summary>
        /// <param name="sessionHandler">Session handler</param>
        /// <returns></returns>
        IInstaApiBuilder SetSessionHandler(ISessionHandler sessionHandler);

        /// <summary>
        ///     Load proxy from session file
        /// </summary>
        IInstaApiBuilder LoadProxyFromSessionFile();

        /// <summary>
        ///     Try to parse user agent and set it if possible
        /// </summary>
        /// <param name="userAgent">User agent</param>
        /// <param name="deviceGuid">Device Guid, it's _uuid in Instagram requests</param>
        /// <param name="phoneGuid">Phone Guid, it's phone_id in Instagram requests</param>
        IInstaApiBuilder TryParseAndSetUserAgent(string userAgent, string deviceGuid = null, string phoneGuid = null);
    }
}
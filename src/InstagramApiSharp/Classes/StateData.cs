using System;
using System.Net;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using System.Collections.Generic;
using InstagramApiSharp.Enums;
using InstagramApiSharp.API.Push;
namespace InstagramApiSharp.Classes
{
    [Serializable]
    public class StateData
    {
        public AndroidDevice DeviceInfo { get; set; }
        public UserSessionData UserSession { get; set; }
        public bool IsAuthenticated { get; set; }
        public CookieContainer Cookies { get; set; }
        public List<Cookie> RawCookies { get; set; }
        public InstaApiVersionType? InstaApiVersion { get; set; }
        public FbnsConnectionData FbnsConnectionData { get; set; }
        public InstaTwoFactorLoginInfo TwoFactorLoginInfo { get; set; }
        public InstaChallengeLoginInfo ChallengeLoginInfo { get; set; }


        #region Locale
        /// <summary>
        ///     Default is -14400
        /// </summary>
        public int TimezoneOffset { get; set; }
        /// <summary>
        ///     X-IG-App-Startup-Country header
        ///     <para>Default is US</para>
        /// </summary>
        public string StartupCountry { get; set; }
        /// <summary>
        ///     X-IG-Timezone-Offset header
        ///     <para>Default is choosen from your system information</para>
        /// </summary>
        public uint StartupCountryCode { get; set; }
        /// <summary>
        ///     X-IG-Device-Locale header
        ///     <para>Default is en_US</para>
        /// </summary>
        public string DeviceLocale { get; set; }
        /// <summary>
        ///     X-IG-App-Locale header
        ///     <para>Default is en_US</para>
        /// </summary>
        public string AppLocale { get; set; }
        /// <summary>
        ///     X-IG-Mapped-Locale
        ///     <para>Default is en_US</para>
        /// </summary>
        public string MappedLocale { get; set; }
        /// <summary>
        ///     Accept-Language
        ///     <para>Default is en-US</para>
        ///     <para>for UK should be: en-GB, en-US</para>
        /// </summary>
        public string AcceptLanguage { get; set; }


        #endregion
    }
}
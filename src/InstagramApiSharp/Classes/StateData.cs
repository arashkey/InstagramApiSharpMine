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

        public int TimezoneOffset { get; set; }
        public string StartupCountry { get; set; }
        public uint StartupCountryCode { get; set; }
        public string DeviceLocale { get; set; }
        public string AppLocale { get; set; }
        public string MappedLocale { get; set; }
        public string AcceptLanguage { get; set; }

        #endregion

        #region Proxy

        public Uri ProxyAddress { get; set; }
        public bool ProxyUseDefaultCredentials { get; set; }
        public bool ProxyBypassProxyOnLocal { get; set; }
        public string ProxyCredentialUsername { get; set; }
        public string ProxyCredentialPassword { get; set; }
        public string ProxyCredentialDomain { get; set; }
        #endregion
    }
}
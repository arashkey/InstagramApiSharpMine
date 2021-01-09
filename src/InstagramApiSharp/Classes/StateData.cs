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
    }
}
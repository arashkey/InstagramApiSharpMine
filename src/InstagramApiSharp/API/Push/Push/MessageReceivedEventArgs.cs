//#if NETSTANDARD
using System;
using InstagramApiSharp.Helpers;
using Newtonsoft.Json;

namespace InstagramApiSharp.API.Push 
{
    public class PushReceivedEventArgs : MessageReceivedEventArgs
    {
        [JsonIgnore] public IInstaApi InstaApi { get; set; }
    }
    public class MessageReceivedEventArgs : EventArgs
    {
        private string _notificationContentJson;

        [JsonIgnore] public string Json { get; set; }

        [JsonProperty("token")] public string Token { get; set; }
        [JsonProperty("ck")] public string ConnectionKey { get; set; }
        [JsonProperty("pn")] public string PackageName { get; set; }
        [JsonProperty("cp")] public string CollapseKey { get; set; }
        [JsonProperty("fbpushnotif")] public string NotificationContentJson
        {
            get => _notificationContentJson;
            set
            {
                NotificationContent = JsonConvert.DeserializeObject<PushNotification>(value);
                _notificationContentJson = value;
            }
        }
        [JsonIgnore] public PushNotification NotificationContent { get; set; }
        [JsonProperty("nid")] public string NotificationId { get; set; }
        [JsonProperty("bu")] public string IsBuffered { get; set; }
    }

    public struct BadgeCount
    {
        [JsonProperty("di")] public int Direct { get; set; }
        [JsonProperty("ds")] public int Ds { get; set; }
        [JsonProperty("ac")] public int Activities { get; set; }
    }

    public class PushNotification
    {
        private string _badgeCountJson;

        [JsonProperty("t")] public string Title { get; set; }
        [JsonProperty("m")] public string Message { get; set; }
        [JsonProperty("tt")] public string TickerText { get; set; }
        [JsonProperty("ig")] public string IgAction { get; set; }
        [JsonProperty("collapse_key")] public string CollapseKey { get; set; }
        [JsonProperty("i")] public string OptionalImage { get; set; }
        [JsonProperty("a")] public string OptionalAvatarUrl { get; set; }
        [JsonProperty("sound")] public string Sound { get; set; }
        [JsonProperty("pi")] public string PushId { get; set; }
        [JsonProperty("c")] public string PushCategory { get; set; }
        [JsonProperty("u")] public string IntendedRecipientUserId { get; set; }
        [JsonIgnore()] public string IntendedRecipientUserName { get; set; } 
        [JsonProperty("s")] public string SourceUserId { get; set; }
        [JsonProperty("igo")] public string IgActionOverride { get; set; }
        [JsonProperty("bc")] public string BadgeCountJson
        {
            get => _badgeCountJson;
            set
            {
                BadgeCount = JsonConvert.DeserializeObject<BadgeCount>(value);
                _badgeCountJson = value;
            }
        }
        [JsonIgnore] public BadgeCount BadgeCount { get; set; }
        [JsonProperty("ia")] public string InAppActors { get; set; }

        #region New Values
        //{
        //  "bc": "{\\\"dt\\\":0}",
        //  "loc": "Tehran, Iran, IR",
        //  "c": "two_factor_trusted_notification",
        //  "gid": "None",
        //  "device_id": "android-",
        //  "SuppressBadge": "1",
        //  "m": "A device is requesting access to log in (XiaoMi Redmi Note 7 in Tehran, Iran, IR).",
        //  "long": "51.",
        //  "collapse_key": "two_factor_trusted_notification",
        //  "device_name": "Xi",
        //  "s": "None",
        //  "u": 12457575,
        //  "PushNotifID": "",
        //  "time_to_live": "3600",
        //  "pi": "",
        //  "tf_id": "=",
        //  "time": "1624192552",
        //  "ig": "trusted_notification",
        //  "lat": "35."
        //}
        [JsonProperty("loc")] public string Location { get; set; }
        [JsonProperty("gid")] public string Guid { get; set; }
        [JsonProperty("device_id")] public string DeviceId { get; set; }
        [JsonProperty("SuppressBadge")] public string SuppressBadge { get; set; }
        [JsonProperty("long")] public double Longitude { get; set; }
        [JsonProperty("lat")] public double Latitude { get; set; }
        [JsonProperty("device_name")] public string DeviceName { get; set; }
        [JsonProperty("time_to_live")] public string TimezoneOffset { get; set; }
        [JsonProperty("tf_id")] public string TwoFactorIdentifier { get; set; }
        [JsonProperty("time")] public string OriginalTime { get; set; }
        public DateTime Time => !string.IsNullOrEmpty(OriginalTime) ? DateTimeHelper.FromUnixTimeSeconds(long.Parse(OriginalTime)) : DateTime.UtcNow;


        #endregion

    }
}

//#endif
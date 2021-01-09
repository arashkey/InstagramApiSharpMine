/*
 * Created by Ramtin Jokar [ Ramtinak@live.com ] [ https://t.me/ramtinak ]
 * Donation link: [ https://paypal.me/rmt4006 ] 
 * Donation email: RamtinJokar@outlook.com
 * 
 * Copyright (c) 2020 Summer [ Tabestaan 1399 ]
 */

using System;
using Newtonsoft.Json;
using InstagramApiSharp.Helpers;
namespace InstagramApiSharp.API.RealTime.Handlers
{
    internal class PresenceContainer
    {
        [JsonProperty("presence_event")]
        public PresenceEventEventArgs PresenceEvent { get; set; }
        internal bool IsPresence => PresenceEvent != null;
    }

    public class PresenceEventEventArgs : EventArgs
    {
        private string _lastActivityAtMs;
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }
        [JsonProperty("last_activity_at_ms")]
        public string LastActivityAtMs
        {
            get => _lastActivityAtMs;
            set
            {
                LastActivityAt = DateTimeHelper.FromUnixTimeMiliSeconds(long.Parse(value));
                _lastActivityAtMs = value;
            }
        }
        [JsonIgnore()]
        public DateTime LastActivityAt { get; set; }
        [JsonProperty("in_threads")]
        public object InThreads { get; set; }
    }

}

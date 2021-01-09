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
    internal class ThreadTypingContainer
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("data")]
        public ThreadTypingEventsArgs[] Data { get; set; }
        public bool IsThreadTyping => !string.IsNullOrEmpty(Event);
    }

    public class ThreadTypingEventsArgs : EventArgs
    {
        private string _value;
        [JsonProperty("op")]
        public string Option { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("value")]
        public string Value
        {
            get => _value;
            set
            {
                TypingData = JsonConvert.DeserializeObject<ThreadTypingData>(value);
                _value = value;
            }
        }
        [JsonIgnore()]
        public ThreadTypingData TypingData { get; set; }
    }

    public class ThreadTypingData
    {
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("sender_id")]
        public string SenderId { get; set; }
        [JsonProperty("ttl")]
        public long Ttl { get; set; }
        [JsonProperty("activity_status")]
        public int ActivityStatus { get; set; }
    }

}

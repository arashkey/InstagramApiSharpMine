using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoriesNetego : INotifyPropertyChanged
    {
        string _title;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        [JsonProperty("tracking_token")]
        public string TrackingToken { get; set; }
        [JsonProperty("hide_unit_if_seen")]
        public string HideUnitIfSeen { get; set; }
        [JsonProperty("filtering_tag")]
        public string FilteringTag { get; set; }
        [JsonProperty("title")]
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged("Title"); } }
        [JsonProperty("reel_ids")]
        public List<string> ReelIds { get; set; } = new List<string>();
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}

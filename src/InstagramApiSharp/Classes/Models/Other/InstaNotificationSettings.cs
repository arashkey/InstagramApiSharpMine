/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    internal class InstaNotificationSettings : InstaDefaultResponse
    {
        [JsonProperty("sections")]
        public InstaNotificationSettingsSectionList Sections { get; set; }
    }
    public class InstaNotificationSettingsSectionList : List<InstaNotificationSettingsSection> { }
    public class InstaNotificationSettingsSection
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("items")]
        public InstaNotificationSettingsSectionItem[] Items { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("footer")]
        public string Footer { get; set; }
    }

    public class InstaNotificationSettingsSectionItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("ui_type")]
        public string UiType { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("setting_value")]
        public string SettingValue { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("checked")]
        public string Checked { get; set; }
    }
}

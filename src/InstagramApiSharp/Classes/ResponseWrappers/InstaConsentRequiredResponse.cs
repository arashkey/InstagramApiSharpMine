/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaConsentRequiredResponse
    {
        [JsonProperty("screen_key")]
        public string ScreenKey { get; set; }
        [JsonProperty("contents")]
        public InstaConsentContentsResponse Contents { get; set; }
        [JsonProperty("primary_button_text")]
        public string PrimaryButtonText { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class InstaConsentContentsResponse
    {
        [JsonProperty("dob")]
        public InstaConsentDobResponse Dob { get; set; }
    }

    public class InstaConsentDobResponse
    {
        [JsonProperty("consent_key")]
        public string ConsentKey { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("headline")]
        public string Hadline { get; set; }
        [JsonProperty("paragraphs")]
        public InstaConsentParagraphResponse[] Paragraphs { get; set; }
        //[JsonProperty("")]
        //public object[] optional_paragraphs { get; set; }
        [JsonProperty("age")]
        public int Age { get; set; }
        [JsonProperty("today")]
        public string Today { get; set; }
    }

    public class InstaConsentParagraphResponse
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("style")]
        public string Style { get; set; }
    }

}

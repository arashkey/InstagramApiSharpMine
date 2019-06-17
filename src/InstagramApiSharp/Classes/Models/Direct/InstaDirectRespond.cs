/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectRespond
    {
        public string Action { get; set; }
        public string StatusCode { get; set; }
        public InstaDirectRespondPayload Payload { get; set; }
    }

    public class InstaDirectRespondPayload
    {
        public string ClientContext { get; set; }
        public string ItemId { get; set; }
        public string Timestamp { get; set; }
        public string ThreadId { get; set; }
    }
}

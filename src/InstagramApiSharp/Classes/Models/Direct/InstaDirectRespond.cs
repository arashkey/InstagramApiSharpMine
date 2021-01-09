/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDirectRespond
    {
        public string Action { get; set; }
        public string StatusCode { get; set; }
        public InstaDirectRespondPayload Payload { get; set; }
    }

    public class InstaDirectVoiceRespond
    {
        public string UploadId { get; set; }
        public List<InstaDirectRespondPayload> MessageMetadatas { get; set; } = new List<InstaDirectRespondPayload>();
    }
    public class InstaDirectRespondPayload
    {
        public string ClientContext { get; set; }
        public string ItemId { get; set; }
        public string Timestamp { get; set; }
        public string ThreadId { get; set; }
        public string Message { get; set; }

        public List<long> ParticipantIds { get; set; } = new List<long>();

        public bool UnloadableParticipant
        {
            get
            {
                var str = Message;
                if (!string.IsNullOrEmpty(str))
                    return str.ToLower().Contains("unloadable participant");
                return false;
            }
        }
    }
}

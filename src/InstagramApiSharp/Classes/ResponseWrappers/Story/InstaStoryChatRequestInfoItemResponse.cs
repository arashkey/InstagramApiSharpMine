/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaStoryChatRequestInfoItemResponse
    {
        [JsonProperty("users")]
        public List<InstaUserShortResponse> Users { get; set; }
        //public object requester_usernames { get; set; }
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
        [JsonProperty("total_thread_participants")]
        public int? TotalThreadParticipants { get; set; }
        [JsonProperty("total_participant_requests")]
        public int? TotalParticipantRequests { get; set; }
    }
}

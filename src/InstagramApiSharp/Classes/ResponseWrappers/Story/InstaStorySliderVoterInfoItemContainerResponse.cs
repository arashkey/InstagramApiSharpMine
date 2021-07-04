/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaStorySliderVoterInfoItemContainerResponse : InstaDefaultResponse
    {
        [JsonProperty("voter_info")]
        public InstaStorySliderVoterInfoItemResponse VoterInfo { get; set; }
    }
}

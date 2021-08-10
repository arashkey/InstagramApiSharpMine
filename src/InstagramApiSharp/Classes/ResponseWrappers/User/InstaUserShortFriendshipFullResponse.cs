/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaUserShortFriendshipFullContainerResponse : InstaDefaultResponse
    {
        [JsonProperty("users")] public InstaUserShortFriendshipFullResponse[] Users { get; set; }
    }

    public class InstaUserShortFriendshipFullResponse : InstaUserShortResponse
    {
        [JsonProperty("friendship_status")] public InstaFriendshipFullStatusResponse FriendshipStatus { get; set; }
    }
}

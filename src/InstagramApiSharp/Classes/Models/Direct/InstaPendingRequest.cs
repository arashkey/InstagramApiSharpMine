/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.ResponseWrappers;
using System.Collections.Generic;
namespace InstagramApiSharp.Classes.Models
{
    public class InstaPendingRequest
    {
        public string NextMaxId { get; set; }
        public bool BigList { get; set; }
        public int PageSize { get; set; }
        public List<InstaSuggestionItem> SuggestedUsers { get; set; } = new List<InstaSuggestionItem>();
        public int TruncateFollowRequestsAtIndex { get; set; }
        public List<InstaUserShortFriendship> Users { get; set; } = new List<InstaUserShortFriendship>();
    }
}

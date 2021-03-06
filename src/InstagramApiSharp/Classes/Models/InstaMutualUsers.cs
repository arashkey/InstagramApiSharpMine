using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaMutualUsers
    {
        public List<InstaSuggestionItem> SuggestedUsers { get; set; } = new List<InstaSuggestionItem>();
        public bool ShowSeeAllFollowersButton { get; set; }

        public List<InstaUserShortFriendship> MutualFollowers { get; set; } = new List<InstaUserShortFriendship>();
    }
}

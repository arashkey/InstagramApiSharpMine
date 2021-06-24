using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaActivityFeed : IInstaBaseList
    {
        public bool IsOwnActivity { get; set; } = false;
        public List<InstaRecentActivityFeed> Items { get; set; } = new List<InstaRecentActivityFeed>();
        public InstaSuggestionItemList SuggestedItems { get; set; } = new InstaSuggestionItemList();
        public InstaActivityCount Counts { get; set; } = new InstaActivityCount();
        public string NextMaxId { get; set; }
    }
    public class InstaActivityCount
    {
        public int Usertags { get; set; }
        public int Comments { get; set; }
        public int CommentLikes { get; set; }
        public int Relationships { get; set; }
        public int Likes { get; set; }
        public int CampaignNotification { get; set; }
        public int ShoppingNotification { get; set; }
        public int PhotosOfYou { get; set; }
        public int Requests { get; set; }
    }
}
namespace InstagramApiSharp.Classes.Models
{
    public class InstaHashtag
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long MediaCount { get; set; }
        public string ProfilePicture { get; set; }

        public bool FollowStatus { get; set; }
        public bool Following { get; set; }
        public bool NonViolating { get; set; }
        public bool AllowFollowing { get; set; }
        public string FormattedMediaCount { get; set; }
        public string SearchResultSubtitle { get; set; }
    }
}
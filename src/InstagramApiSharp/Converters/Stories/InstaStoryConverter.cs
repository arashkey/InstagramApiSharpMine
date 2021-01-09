using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;

namespace InstagramApiSharp.Converters
{
    internal class InstaStoryConverter : IObjectConverter<InstaStory, InstaStoryResponse>
    {
        public InstaStoryResponse SourceObject { get; set; }

        public InstaStory Convert()
        {
            if (SourceObject == null) return null;
            var story = new InstaStory
            {
                ExpiringAt = DateTimeHelper.UnixTimestampToDateTime(SourceObject?.ExpiringAt ?? 0),
                Id = SourceObject.Id,
                LatestReelMedia = SourceObject.LatestReelMedia ?? 0,
                Muted = SourceObject.Muted ?? false,
                PrefetchCount = SourceObject.PrefetchCount ?? 0,
                RankedPosition = SourceObject.RankedPosition,
                Seen = SourceObject.Seen ?? 0,
                SeenRankedPosition = SourceObject.SeenRankedPosition,
                SocialContext = SourceObject.SocialContext,
                SourceToken = SourceObject.SourceToken,
                TakenAtUnix = SourceObject.TakenAtUnixLike,
                CanViewerSave = SourceObject.CanViewerSave,
                CaptionIsEdited = SourceObject.CaptionIsEdited,
                CaptionPosition = SourceObject.CaptionPosition,
                ClientCacheKey = SourceObject.ClientCacheKey,
                PhotoOfYou = SourceObject.PhotoOfYou,
                IsReelMedia = SourceObject.IsReelMedia,
                VideoDuration = SourceObject.VideoDuration ?? 0,
                SupportsReelReactions = SourceObject.SupportsReelReactions,
                HasSharedToFb = SourceObject.HasSharedToFb,
                ImportedTakenAt = SourceObject.ImportedTakenAt.FromUnixTimeSeconds(),
                CreatedAt = DateTimeHelper.UnixTimestampToDateTime(SourceObject?.CreatedAt ?? System.DateTime.UtcNow.ToUnixTime()),
                ReelType = SourceObject.ReelType
            };
            try
            {
                var canReply = SourceObject.CanReply;
                if (string.IsNullOrEmpty(canReply))
                    canReply = "true";
                else
                {
                    if (!canReply.ToLower().Contains("true") && !canReply.ToLower().Contains("false"))
                        canReply = System.Convert.ToBoolean(int.Parse(SourceObject.CanReply)).ToString();
                }
                story.CanReshare = System.Convert.ToBoolean(canReply);
                if (!string.IsNullOrEmpty(SourceObject.CanReshare))
                    story.CanReshare = bool.Parse(SourceObject.CanReshare);
            }
            catch { }
            if (SourceObject.StoryHashtags != null)
                foreach (var item in SourceObject.StoryHashtags)
                    story.StoryHashtags.Add(ConvertersFabric.Instance.GetMentionConverter(item).Convert());

            if (SourceObject.StoryLocation != null)
                story.StoryLocation = SourceObject.StoryLocation;

            if (SourceObject.Owner != null)
                story.Owner = ConvertersFabric.Instance.GetHashtagOwnerConverter(SourceObject.Owner).Convert();

            if (SourceObject.User != null)
                story.User = ConvertersFabric.Instance.GetUserShortFriendshipFullConverter(SourceObject.User).Convert();

            if (SourceObject.Items != null)
                foreach (var item in SourceObject.Items)
                    story.Items.Add(ConvertersFabric.Instance.GetStoryItemConverter(item).Convert());
            return story;
        }
    }
}
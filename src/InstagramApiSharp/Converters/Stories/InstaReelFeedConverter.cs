using System;
using System.Linq;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;

namespace InstagramApiSharp.Converters
{
    internal class InstaReelFeedConverter : IObjectConverter<InstaReelFeed, InstaReelFeedResponse>
    {
        public InstaReelFeedResponse SourceObject { get; set; }

        public InstaReelFeed Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var reelFeed = new InstaReelFeed
            {
                CanReply = SourceObject.CanReply ?? false,
                ExpiringAt = DateTimeHelper.UnixTimestampToDateTime(SourceObject?.ExpiringAt ?? 0),
                Id = SourceObject.Id,
                LatestReelMedia = SourceObject.LatestReelMedia ?? 0,
                PrefetchCount = SourceObject.PrefetchCount ?? 0,
                Seen = SourceObject.Seen ?? 0,
                MediaCount = SourceObject.MediaCount ?? 0,
                ReelType = SourceObject.ReelType,
                Muted = SourceObject.Muted ?? false,
                CreatedAt = DateTimeHelper.UnixTimestampToDateTime(SourceObject?.CreatedAt ?? DateTime.UtcNow.ToUnixTime())
            };
            try
            {
                if (!string.IsNullOrEmpty(SourceObject.CanReshare))
                    reelFeed.CanReshare = bool.Parse(SourceObject.CanReshare);
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(SourceObject.HasBestiesMedia))
                    reelFeed.HasBestiesMedia = bool.Parse(SourceObject.HasBestiesMedia);
            }
            catch { }
            try
            {
                if (SourceObject.User != null)
                    reelFeed.User = ConvertersFabric.Instance.GetUserShortFriendshipFullConverter(SourceObject.User).Convert();
            }
            catch { }
            try
            {
                if (SourceObject.Owner != null)
                    reelFeed.Owner = ConvertersFabric.Instance.GetHashtagOwnerConverter(SourceObject.Owner).Convert();
            }
            catch { }
            if (SourceObject.Items?.Count > 0)
                foreach (var item in SourceObject.Items)
                    try
                    {
                        reelFeed.Items.Add(ConvertersFabric.Instance.GetStoryItemConverter(item).Convert());
                    }
                    catch { }
            return reelFeed;
        }
    }
}
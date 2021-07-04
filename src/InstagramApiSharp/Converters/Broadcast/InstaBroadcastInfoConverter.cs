/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;
using System;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.Converters
{
    internal class InstaBroadcastInfoConverter : IObjectConverter<InstaBroadcastInfo, InstaBroadcastInfoResponse>
    {
        public InstaBroadcastInfoResponse SourceObject { get; set; }

        public InstaBroadcastInfo Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var unixTime = DateTime.Now.ToUnixTime();
            var broadcastInfo = new InstaBroadcastInfo
            {
                BroadcastMessage = SourceObject.BroadcastMessage,
                CoverFrameUrl = SourceObject.CoverFrameUrl,
                DashPlaybackUrl = SourceObject.DashPlaybackUrl,
                DashAbrPlaybackUrl = SourceObject.DashAbrPlaybackUrl,
                DashManifest = SourceObject.DashManifest,
                EncodingTag = SourceObject.EncodingTag,
                Id = SourceObject.Id,
                InternalOnly = SourceObject.InternalOnly,
                MediaId = SourceObject.MediaId,
                NumberOfQualities = SourceObject.NumberOfQualities,
                OrganicTrackingToken = SourceObject.OrganicTrackingToken,
                TotalUniqueViewerCount = SourceObject.TotalUniqueViewerCount,
                ExpireAt = DateTimeHelper.FromUnixTimeSeconds(SourceObject.ExpireAt ?? unixTime),
                PublishedTime = DateTimeHelper.FromUnixTimeSeconds(SourceObject.PublishedTime ?? unixTime),
            };
            try
            {
                broadcastInfo.BroadcastStatusType = (InstaBroadcastStatusType)Enum.Parse(typeof(InstaBroadcastStatusType), SourceObject.BroadcastStatus?.Replace("_", ""), true);
            }
            catch { }

            if (SourceObject.BroadcastOwner != null)
                broadcastInfo.BroadcastOwner = ConvertersFabric.Instance
                    .GetUserShortFriendshipFullConverter(SourceObject.BroadcastOwner).Convert();
            return broadcastInfo;
        }
    }
}

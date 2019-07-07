using System.Collections.Generic;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;
using Newtonsoft.Json;
using System.Linq;

namespace InstagramApiSharp.Converters
{
    internal class InstaDirectThreadConverter : IObjectConverter<InstaDirectInboxThread, InstaDirectInboxThreadResponse>
    {
        public InstaDirectInboxThreadResponse SourceObject { get; set; }

        public InstaDirectInboxThread Convert()
        {
            var thread = new InstaDirectInboxThread
            {
                Canonical = SourceObject.Canonical ?? false,
                HasNewer = SourceObject.HasNewer ?? false,
                HasOlder = SourceObject.HasOlder ?? false,
                IsSpam = SourceObject.IsSpam ?? false,
                Muted = SourceObject.Muted ?? false,
                Named = SourceObject.Named ?? false,
                Pending = SourceObject.Pending ?? false,
                VieweId = SourceObject.VieweId,
                LastActivity = DateTimeHelper.UnixTimestampMilisecondsToDateTime(SourceObject.LastActivity),
                ThreadId = SourceObject.ThreadId,
                OldestCursor = SourceObject.OldestCursor,
                IsGroup = SourceObject.IsGroup ?? false,
                IsPin = SourceObject.IsPin ?? false,
                ValuedRequest = SourceObject.ValuedRequest ?? false,
                PendingScore = SourceObject.PendingScore ?? 0,
                VCMuted = SourceObject.VCMuted ?? false,
                ReshareReceiveCount = SourceObject.ReshareReceiveCount ?? 0,
                ReshareSendCount = SourceObject.ReshareSendCount ?? 0,
                ExpiringMediaReceiveCount = SourceObject.ExpiringMediaReceiveCount ?? 0,
                ExpiringMediaSendCount = SourceObject.ExpiringMediaSendCount ?? 0,
                NewestCursor = SourceObject.NewestCursor,
                ThreadType = SourceObject.ThreadType,
                Title = SourceObject.Title,
            
                MentionsMuted = SourceObject.MentionsMuted ?? false,
                Archived = SourceObject.Archived ?? false,
                ApprovalRequiredForNewMembers = SourceObject.ApprovalRequiredForNewMembers ?? false,
                Folder = SourceObject.Folder ?? 0,
                InputMode = SourceObject.InputMode ?? 0,
                BusinessThreadFolder = SourceObject.BusinessThreadFolder ?? 0,
                ReadState = SourceObject.ReadState ?? 0
            };

            if (SourceObject.Inviter != null)
            {
                var userConverter = ConvertersFabric.Instance.GetUserShortConverter(SourceObject.Inviter);
                thread.Inviter = userConverter.Convert();
            }

            if (SourceObject.Items?.Count > 0)
            {
                thread.Items = new List<InstaDirectInboxItem>();
                foreach (var item in SourceObject.Items)
                {
                    var converter = ConvertersFabric.Instance.GetDirectThreadItemConverter(item);
                    thread.Items.Add(converter.Convert());
                }
            }

            if (SourceObject.LastPermanentItem != null)
            {
                var converter = ConvertersFabric.Instance.GetDirectThreadItemConverter(SourceObject.LastPermanentItem);
                thread.LastPermanentItem = converter.Convert();
            }
            if (SourceObject.Users?.Count > 0)
            {
                foreach (var user in SourceObject.Users)
                {
                    var converter = ConvertersFabric.Instance.GetUserShortFriendshipConverter(user);
                    thread.Users.Add(converter.Convert());
                }
            }

            if (SourceObject.LeftUsers?.Count > 0)
            {
                foreach (var user in SourceObject.LeftUsers)
                {
                    var converter = ConvertersFabric.Instance.GetUserShortFriendshipConverter(user);
                    thread.LeftUsers.Add(converter.Convert());
                }
            }

            if (SourceObject.LastSeenAt != null)
            {
                try
                {
                    var lastSeenJson = System.Convert.ToString(SourceObject.LastSeenAt);
                    var obj = JsonConvert.DeserializeObject<InstaLastSeenAtResponse>(lastSeenJson);
                    thread.LastSeenAt = new List<InstaLastSeen>();
                    foreach (var extraItem in obj.Extras)
                    {
                        var convertedLastSeen = JsonConvert.DeserializeObject<InstaLastSeenItemResponse>(extraItem.Value.ToString(Formatting.None));
                        var lastSeen = new InstaLastSeen
                        {
                            PK = long.Parse(extraItem.Key),
                            ItemId = convertedLastSeen.ItemId,
                        };
                        if (convertedLastSeen.TimestampPrivate != null)
                            lastSeen.SeenTime = DateTimeHelper.UnixTimestampMilisecondsToDateTime(convertedLastSeen.TimestampPrivate);
                        thread.LastSeenAt.Add(lastSeen);
                    }
                }
                catch { }
            }
            try
            {
                if (thread.LastActivity != thread.LastSeenAt.LastOrDefault().SeenTime)
                    thread.HasUnreadMessage = true;
            }
            catch 
            {
                thread.HasUnreadMessage = false;
            }

            if (SourceObject.DirectStory?.Items?.Count > 0)
            {
                try
                {
                    foreach (var item in SourceObject.DirectStory.Items)
                        thread.DirectStories.Add(ConvertersFabric.Instance.GetDirectThreadItemConverter(item).Convert());
                }
                catch { }
            }

            return thread;
        }
    }
}
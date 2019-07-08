/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaPendingRequestConverter : IObjectConverter<InstaPendingRequest, InstaPendingRequestResponse>
    {
        public InstaPendingRequestResponse SourceObject { get; set; }

        public InstaPendingRequest Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"InstaPendingRequestConverter.Source object");
            var pending = new InstaPendingRequest
            {
                BigList = SourceObject.BigList,
                NextMaxId = SourceObject.NextMaxId,
                PageSize = SourceObject.PageSize,
                TruncateFollowRequestsAtIndex = SourceObject.TruncateFollowRequestsAtIndex,
            };
            try
            {
                if (SourceObject.Users?.Count > 0)
                {
                    foreach (var user in SourceObject.Users)
                        pending.Users.Add(ConvertersFabric.Instance.GetUserShortFriendshipConverter(user).Convert());
                }
            }
            catch { }

            try
            {
                if (SourceObject.SuggestedUsers?.Suggestions?.Count > 0)
                {
                    foreach (var suggestion in SourceObject.SuggestedUsers.Suggestions)
                        pending.SuggestedUsers.Add(ConvertersFabric.Instance.GetSuggestionItemConverter(suggestion).Convert());
                }
            }
            catch { }
            return pending;
        }
    }
}

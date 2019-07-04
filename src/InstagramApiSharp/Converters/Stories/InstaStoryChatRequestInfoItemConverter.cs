/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaStoryChatRequestInfoItemConverter : IObjectConverter<InstaStoryChatRequestInfoItem, InstaStoryChatRequestInfoItemResponse>
    {
        public InstaStoryChatRequestInfoItemResponse SourceObject { get; set; }

        public InstaStoryChatRequestInfoItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"InstaStoryChatRequestInfoItemConverter.Source object");
            var storyChatRequestInfoItem = new InstaStoryChatRequestInfoItem
            {
                Cursor = SourceObject.Cursor,
                TotalParticipantRequests= SourceObject.TotalParticipantRequests ?? 0,
                TotalThreadParticipants = SourceObject.TotalThreadParticipants ?? 0
            };
            if (SourceObject.Users?.Count > 0)
                foreach (var user in SourceObject.Users)
                    storyChatRequestInfoItem.Users.Add(ConvertersFabric.Instance.GetUserShortConverter(user).Convert());

            return storyChatRequestInfoItem;
        }
    }
}

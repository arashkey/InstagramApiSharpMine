/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Helpers;
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaDirectEmojiReactionConverter : IObjectConverter<InstaDirectEmojiReaction, InstaDirectEmojiReactionResponse>
    {
        public InstaDirectEmojiReactionResponse SourceObject { get; set; }

        public InstaDirectEmojiReaction Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var emojiReaction = new InstaDirectEmojiReaction
            {
                SenderId = SourceObject.SenderId,
                ClientContext = SourceObject.ClientContext,
                Timestamp = SourceObject.Timestamp.FromUnixTimeSeconds(),
                Emoji = SourceObject.Emoji,
                SuperReactType = SourceObject.SuperReactType
            };
            return emojiReaction;
        }
    }
}

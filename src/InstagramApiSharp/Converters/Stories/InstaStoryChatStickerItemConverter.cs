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
    internal class InstaStoryChatStickerItemConverter : IObjectConverter<InstaStoryChatStickerItem, InstaStoryChatStickerItemResponse>
    {
        public InstaStoryChatStickerItemResponse SourceObject { get; set; }

        public InstaStoryChatStickerItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"InstaStoryChatStickerItemConverter.Source object");
            return new InstaStoryChatStickerItem
            {
               EndBackgroundColor = SourceObject.EndBackgroundColor,
               HasStartedChat = SourceObject.HasStartedChat,
               StartBackgroundColor = SourceObject.StartBackgroundColor,
               Status = SourceObject.Status,
               StoryChatId = SourceObject.StoryChatId,
               Text = SourceObject.Text,
               ThreadId = SourceObject.ThreadId
            };

        }
    }
}

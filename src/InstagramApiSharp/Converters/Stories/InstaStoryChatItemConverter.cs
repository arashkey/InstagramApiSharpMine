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
    internal class InstaStoryChatItemConverter : IObjectConverter<InstaStoryChatItem, InstaStoryChatItemResponse>
    {
        public InstaStoryChatItemResponse SourceObject { get; set; }

        public InstaStoryChatItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"InstaStoryChatItemConverter.Source object");
            var storyChatItem = new InstaStoryChatItem
            {
                Height = SourceObject.Height,
                IsHidden = System.Convert.ToBoolean(SourceObject.IsHidden),
                IsPinned = System.Convert.ToBoolean(SourceObject.IsPinned),
                Rotation = SourceObject.Rotation,
                Width = SourceObject.Width,
                X = SourceObject.X,
                Y = SourceObject.Y,
                Z = SourceObject.Z
            };
            storyChatItem.ChatSticker = ConvertersFabric.Instance.GetStoryChatStickerItemConverter(SourceObject.ChatSticker).Convert();
            return storyChatItem;
        }
    }
}

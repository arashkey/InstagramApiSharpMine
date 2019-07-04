/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryChatItem
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Rotation { get; set; }

        public bool IsPinned { get; set; }

        public bool IsHidden { get; set; }

        public InstaStoryChatStickerItem ChatSticker { get; set; }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Classes.ResponseWrappers;
using System;

namespace InstagramApiSharp.Converters
{
    class InstaDirectReactionConverter : IObjectConverter<InstaDirectReaction, InstaDirectReactionResponse>
    {
        public InstaDirectReactionResponse SourceObject { get; set; }

        public InstaDirectReaction Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var reaction = new InstaDirectReaction
            {
                LikesCount = SourceObject.LikesCount
            };
            try
            {
                if (SourceObject.Likes?.Count > 0)
                    foreach (var item in SourceObject.Likes)
                        reaction.Likes.Add(ConvertersFabric.Instance.GetDirectLikeReactionConverter(item).Convert());
            }
            catch { }
            return reaction;
        }
    }
}

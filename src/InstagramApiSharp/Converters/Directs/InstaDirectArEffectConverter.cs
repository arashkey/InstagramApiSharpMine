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
using System;
using InstagramApiSharp.Helpers;

namespace InstagramApiSharp.Converters
{
    internal class InstaDirectArEffectConverter : IObjectConverter<InstaDirectArEffect, InstaDirectArEffectResponse>
    {
        public InstaDirectArEffectResponse SourceObject { get; set; }

        public InstaDirectArEffect Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var arEffect = new InstaDirectArEffect
            {
                PreviewVideo = ConvertersFabric.Instance.GetStoryConverter(SourceObject.PreviewVideo).Convert(),
            };
            if (SourceObject.Data != null)
            {
                if (SourceObject.Data.InstagramDirectEffects != null)
                {
                    if (SourceObject.Data.InstagramDirectEffects.TargetEffectPreview != null)
                    {
                        arEffect.Data = new InstaDirectTargetEffectPreview
                        {
                            AttributionUser = new InstaDirectAttributionUser
                            {
                                InstagramUserId = SourceObject.Data.InstagramDirectEffects.TargetEffectPreview.AttributionUser.InstagramUserId,
                                ProfilePicture = SourceObject.Data.InstagramDirectEffects.TargetEffectPreview.AttributionUser.ProfilePicture.Uri,
                                UserName = SourceObject.Data.InstagramDirectEffects.TargetEffectPreview.AttributionUser.UserName
                            },
                            Id = SourceObject.Data.InstagramDirectEffects.TargetEffectPreview.Id,
                            Name = SourceObject.Data.InstagramDirectEffects.TargetEffectPreview.Name,
                            ThumbnailImage = SourceObject.Data.InstagramDirectEffects.TargetEffectPreview.ThumbnailImage.Uri
                        };
                    }
                }
            }
            return arEffect;
        }
    }
}

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaStorySliderStickerItemConverter : IObjectConverter<InstaStorySliderStickerItem, InstaStorySliderStickerItemResponse>
    {
        public InstaStorySliderStickerItemResponse SourceObject { get; set; }

        public InstaStorySliderStickerItem Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var slider = new InstaStorySliderStickerItem
            {
                Emoji = SourceObject.Emoji,
                Question = SourceObject.Question,
                SliderId = SourceObject.SliderId,
                SliderVoteAverage = SourceObject.SliderVoteAverage == null ? 0 : SourceObject.SliderVoteAverage.Value,
                SliderVoteCount = SourceObject.SliderVoteCount == null ? 0 : SourceObject.SliderVoteCount.Value,
                TextColor = SourceObject.TextColor,
                ViewerCanVote = SourceObject.ViewerCanVote,
                ViewerVote = SourceObject.ViewerVote ?? -1,
                BackgroundColor = SourceObject.BackgroundColor
            };
            return slider;
        }
    }
}

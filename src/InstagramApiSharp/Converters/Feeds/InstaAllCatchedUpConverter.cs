/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaAllCatchedUpConverter : IObjectConverter<InstaAllCatchedUp, InstaAllCatchedUpResponse>
    {
        public InstaAllCatchedUpResponse SourceObject { get; set; }

        public InstaAllCatchedUp Convert()
        {
            var catchedUp = new InstaAllCatchedUp
            {
                Style = SourceObject.Style,
                Subtitle = SourceObject.Subtitle,
                Id = SourceObject.Id,
                Pause = SourceObject.Pause ?? false,
                Title = SourceObject.Title
            };
            try
            {
                if (SourceObject.GroupSet != null)
                    catchedUp.GroupSet = ConvertersFabric.Instance.GetFeedGroupSetConverter(SourceObject.GroupSet).Convert();
            }
            catch { }
            return catchedUp;
        }
    }
}

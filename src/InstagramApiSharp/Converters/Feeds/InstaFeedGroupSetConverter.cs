/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaFeedGroupSetConverter : IObjectConverter<InstaFeedGroupSet, InstaFeedGroupSetResponse>
    {
        public InstaFeedGroupSetResponse SourceObject { get; set; }

        public InstaFeedGroupSet Convert()
        {
            var gpSet = new InstaFeedGroupSet
            {
                Id = SourceObject.Id ?? 0,
                HeaderAccessoryStyle = null,
                ActiveGroupId = SourceObject.ActiveGroupId,
                ConnectedGroupId = SourceObject.ConnectedGroupId,
                Format = SourceObject.Format,
                RememberGroupChoice = SourceObject.RememberGroupChoice ?? false,
            };
            gpSet.HeaderAccessoryStyle = SourceObject.Style?.HeaderAccessoryStyle;

            try
            {
                if (SourceObject.Groups?.Length > 0)
                    for (int i = 0; i < SourceObject.Groups.Length; i++)
                    {
                        try
                        {
                            gpSet.Groups.Add(ConvertersFabric.Instance.GetFeedGroupConverter(SourceObject.Groups[i]).Convert());
                        }
                        catch { }
                    }
            }
            catch { }
            return gpSet;
        }
    }
}

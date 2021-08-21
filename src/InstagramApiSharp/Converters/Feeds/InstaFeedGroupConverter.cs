/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaFeedGroupConverter : IObjectConverter<InstaFeedGroup, InstaFeedGroupResponse>
    {
        public InstaFeedGroupResponse SourceObject { get; set; }

        public InstaFeedGroup Convert()
        {
            var gp = new InstaFeedGroup
            {
                ShowGroupText = SourceObject.ShowGroupText,
                PaginationSource = SourceObject.PaginationSource,
                Id = SourceObject.Id,
                NextMaxId = SourceObject.NextMaxId,
                Title = SourceObject.Title
            };
            try
            {
                if (SourceObject.FeedItems?.Count > 0)
                    for (int i = 0; i < SourceObject.FeedItems.Count; i++)
                    {
                        try
                        {
                            gp.FeedItems.Add(ConvertersFabric.Instance.GetSingleMediaConverter(SourceObject.FeedItems[i]).Convert());
                        }
                        catch { }
                    }
            }
            catch { }
            return gp;
        }
    }
}

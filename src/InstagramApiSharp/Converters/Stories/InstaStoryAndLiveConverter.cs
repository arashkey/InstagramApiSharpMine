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

namespace InstagramApiSharp.Converters
{
    public class InstaStoryAndLiveConverter : IObjectConverter<InstaStoryAndLives, InstaStoryAndLivesResponse>
    {
        public InstaStoryAndLivesResponse SourceObject { get; set; }

        public InstaStoryAndLives Convert()
        {
            var Return = new InstaStoryAndLives();

            if (SourceObject.Reel != null)
                Return.Reel = ConvertersFabric.Instance.GetStoryConverter(SourceObject.Reel).Convert();

            if (SourceObject.Broadcast != null)
                Return.Broadcast = ConvertersFabric.Instance.GetBroadcastConverter(SourceObject.Broadcast).Convert();

            if (SourceObject.PostLiveItems != null)
                Return.PostLiveItems = ConvertersFabric.Instance.GetBroadcastPostLiveConverter(SourceObject.PostLiveItems).Convert();

            return Return;
        }
    }
}
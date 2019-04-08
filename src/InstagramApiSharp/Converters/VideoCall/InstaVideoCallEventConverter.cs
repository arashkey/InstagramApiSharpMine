/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;
using System.Linq;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.ResponseWrappers.Business;

namespace InstagramApiSharp.Converters
{
    internal class InstaVideoCallEventConverter : IObjectConverter<InstaVideoCallEvent, InstaVideoCallEventResponse>
    {
        public InstaVideoCallEventResponse SourceObject { get; set; }

        public InstaVideoCallEvent Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var videoCallEvent = new InstaVideoCallEvent
            {
                Description = SourceObject.Description,
                EncodedServerDataInfo = SourceObject.EncodedServerDataInfo,
                TextAttributes = SourceObject.TextAttributes,
                VcId = SourceObject.VcId ?? 0
            };
            try
            {
                var type = SourceObject.Action.Replace("_", "");
                videoCallEvent.Action = (InstaVideoCallActionType)Enum.Parse(typeof(InstaVideoCallActionType), type, true);
            }
            catch { }

            return videoCallEvent;
        }
    }
}

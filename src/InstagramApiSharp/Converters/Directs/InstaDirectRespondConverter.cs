﻿/*
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

namespace InstagramApiSharp.Converters
{
    internal class InstaDirectRespondConverter : IObjectConverter<InstaDirectRespond, InstaDirectRespondResponse>
    {
        public InstaDirectRespondResponse SourceObject { get; set; }

        public InstaDirectRespond Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");

            var respond = new InstaDirectRespond
            {
                Action = SourceObject.Action,
                StatusCode = SourceObject.StatusCode
            };
            if (SourceObject.Payload != null)
            {
                try
                {
                    respond.Payload = new InstaDirectRespondPayload
                    {
                        ClientContext = SourceObject.Payload.ClientContext,
                        ItemId = SourceObject.Payload.ItemId,
                        ThreadId = SourceObject.Payload.ThreadId,
                        Timestamp = SourceObject.Payload.Timestamp,
                        Message = SourceObject.Payload.Message,
                        ClientFacingErrorMessage = SourceObject.Payload.ClientFacingErrorMessage
                    };

                    if (SourceObject.Payload.ParticipantIds?.Length > 0)
                        for (int i = 0; i < SourceObject.Payload.ParticipantIds.Length; i++)
                            respond.Payload.ParticipantIds.Add(SourceObject.Payload.ParticipantIds[i]);
                }
                catch { }
            }

            return respond;
        }
    }
}

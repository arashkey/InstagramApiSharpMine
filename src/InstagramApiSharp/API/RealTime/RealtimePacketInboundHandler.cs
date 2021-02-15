using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Transport.Channels;
using InstagramApiSharp.API.Push.PacketHelpers;
using InstagramApiSharp.API.RealTime.Handlers;
using InstagramApiSharp.API.RealTime.Responses.Models;
using InstagramApiSharp.API.RealTime.Responses.Wrappers;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Helpers;
using Ionic.Zlib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thrift;
using Thrift.Protocol;
using Thrift.Protocol.Entities;
using Thrift.Transport.Client;

namespace InstagramApiSharp.API.RealTime
{
    internal class RealtimePacketInboundHandler : SimpleChannelInboundHandler<Packet>
    {
        public RealtimeResponseList Responses { get; private set; } = new RealtimeResponseList();

        public IChannelHandlerContext ChannelHandlerContext { get; private set; }
        private readonly RealTimeClient _client;
        private const int TIMEOUT = 5;

        public RealtimePacketInboundHandler(RealTimeClient client)
        {
            _client = client;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            // If connection is closed, reconnect
            Task.Delay(TimeSpan.FromSeconds(TIMEOUT)).ContinueWith(async task =>
            {
                //_timerResetToken?.Cancel();
                if (!_client.IsShutdown)
                    await _client.Start();
            });

        }

        async protected override void ChannelRead0(IChannelHandlerContext ctx, Packet msg)
        {
            ChannelHandlerContext = ctx;
            switch (msg.PacketType)
            {
                case PacketType.CONNACK:
                    await _client.SubscribeForDM();
                    await _client.RealtimeSub();
                    await _client.PubSub();
                    if (!string.IsNullOrEmpty(_client.SnapshotAtMs) && !string.IsNullOrEmpty(_client.SeqId))
                        await _client.IrisSub(_client.SeqId, _client.SnapshotAtMs);
                    break;
                case PacketType.PUBLISH:
                    var publishPacket = (PublishPacket)msg;
                    if (publishPacket.QualityOfService == QualityOfService.AtLeastOnce)
                        await ctx.WriteAndFlushAsync(PubAckPacket.InResponseTo(publishPacket));
                    var payload = DecompressPayload(publishPacket.Payload);
                    var json = await GetJsonFromThrift(payload);
                    Debug.WriteLine(json);
                    switch (publishPacket.TopicName)
                    {
                        case "150": break;
                        case "133": //      /ig_send_message_response
                            try
                            {
                                Responses.AddItem(JsonConvert.DeserializeObject<InstaDirectRespondResponse>(json));
                            }
                            catch
                            {
                                try
                                {
                                    var o = JsonConvert.DeserializeObject<InstaDirectRespondV2Response>(json);
                                    Responses.AddItem(new InstaDirectRespondResponse
                                    {
                                        Action = o.Action,
                                        Message = o.Message,
                                        Status = o.Status,
                                        StatusCode = o.StatusCode,
                                        Payload = o.Payload[0]
                                    });
                                }
                                catch { }
                            }
                            break;
                        case "88":
                            {
                                var obj = JsonConvert.DeserializeObject<InstaRealtimeRespondResponse>(json);
                                if (obj?.Data?.Length > 0)
                                {
                                    var typing = new List<InstaRealtimeTypingEventArgs>();
                                    var dm = new List<InstaDirectInboxItem>();
                                    for (int i = 0; i < obj.Data.Length; i++)
                                    {
                                        var item = obj.Data[i];
                                        if (item != null)
                                        {
                                            if (item.IsTyping)
                                            {
                                                var typingResponse = JsonConvert.DeserializeObject<InstaRealtimeTypingResponse>(item.Value);
                                                if (typingResponse != null)
                                                {
                                                    try
                                                    {
                                                        var tr = new InstaRealtimeTypingEventArgs
                                                        {
                                                            SenderId = typingResponse.SenderId,
                                                            ActivityStatus = typingResponse.ActivityStatus,
                                                            RealtimeOp = item.Op,
                                                            RealtimePath = item.Path,
                                                            TimestampUnix = typingResponse.Timestamp,
                                                            Timestamp = DateTimeHelper.FromUnixTimeMiliSeconds(typingResponse.Timestamp),
                                                            Ttl = typingResponse.Ttl
                                                        };
                                                        typing.Add(tr);
                                                    }
                                                    catch { }
                                                }
                                            }
                                            else if (item.IsBroadcast)
                                            {
                                                if (item.HasItemInValue)
                                                {
                                                    var broadcastEventArgs = JsonConvert.DeserializeObject<InstaBroadcastEventArgs>(item.Value);
                                                    if (broadcastEventArgs != null)
                                                        _client.OnBroadcastChanged(broadcastEventArgs);
                                                }
                                            }
                                            else if (item.IsThreadItem || item.IsThreadParticipants)
                                            {
                                                if (item.HasItemInValue)
                                                {
                                                    var directItemResponse = JsonConvert.DeserializeObject<InstaDirectInboxItemResponse>(item.Value);
                                                    if (directItemResponse != null)
                                                    {
                                                        try
                                                        {
                                                            var dI = ConvertersFabric.Instance.GetDirectThreadItemConverter(directItemResponse).Convert();
                                                            dI.RealtimeOp = item.Op;
                                                            dI.RealtimePath = item.Path;
                                                            dm.Add(dI);
                                                        }
                                                        catch { }
                                                    }
                                                }
                                                else
                                                {
                                                    var dI = new InstaDirectInboxItem
                                                    {
                                                        RealtimeOp = item.Op,
                                                        RealtimePath = item.Path,
                                                        ItemId = item.Value
                                                    };
                                                    dm.Add(dI);
                                                }
                                            }
                                        }
                                    }
                                    if (typing.Count > 0)
                                        _client.OnTypingChanged(typing);
                                    if (dm.Count > 0)
                                        _client.OnDirectItemChanged(dm);
                                }

                            }
                            break;

                    }
                    break;
            }
        }
        async Task<string> GetJsonFromThrift(byte[] bytes)
        {
            try
            {
                var _memoryBufferTransport = new TMemoryBufferTransport(bytes, new TConfiguration());
                var _thrift = new TCompactProtocol(_memoryBufferTransport);
                while (true)
                {
                    var field = await _thrift.ReadFieldBeginAsync(CancellationToken.None);


                    if (field.Type == TType.Stop)
                    {
                        break;
                    }

                    if (field.Type == TType.String)
                    {
                        var json = await _thrift.ReadStringAsync(CancellationToken.None);
                        if (!string.IsNullOrEmpty(json))
                        {
                            if (json.StartsWith("{") && json.EndsWith("}"))
                            {
                                return json;
                            }
                        }
                    }
                    await _thrift.ReadFieldEndAsync(CancellationToken.None);
                }
            }
            catch { }
            return Encoding.UTF8.GetString(bytes);

        }
        private byte[] DecompressPayload(IByteBuffer payload)
        {
            var totalLength = payload.ReadableBytes;

            var decompressedStream = new MemoryStream(256);
            using (var compressedStream = new MemoryStream(totalLength))
            {
                payload.GetBytes(0, compressedStream, totalLength);
                compressedStream.Position = 0;
                using (var zlibStream = new ZlibStream(compressedStream, CompressionMode.Decompress, true))
                {
                    zlibStream.CopyTo(decompressedStream);
                }
            }

            var data = new byte[decompressedStream.Length];
            decompressedStream.Position = 0;
            decompressedStream.Read(data, 0, data.Length);
            decompressedStream.Dispose();
            return data;
        }
    }

}

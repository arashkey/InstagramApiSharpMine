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
using InstagramApiSharp.Classes.ResponseWrappers;
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
                    var json = Encoding.UTF8.GetString(payload);
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
                        default:
                            try
                            {
                                json = await HandleThrift(payload);

                                var container = JsonConvert.DeserializeObject<JObject>(json);
                                if (container["presence_event"] != null)
                                {
                                    var presence = JsonConvert.DeserializeObject<PresenceContainer>(json);
                                    _client.OnPresenceChanged(presence.PresenceEvent);
                                }
                                else if (container["event"] != null)
                                {
                                    var typing = JsonConvert.DeserializeObject<ThreadTypingContainer>(json);

                                    if (typing.Data?.Length > 0)
                                        for (int i = 0; i < typing.Data.Length; i++)
                                            _client.OnTypingChanged(typing.Data[i]);
                                }
                            }
                            catch { }
                            Debug.WriteLine($"Unknown topic received:{msg.PacketType} :  {publishPacket.TopicName} : {json}");
                            break;

                    }
                    break;
            }
        }
        async Task<string> HandleThrift(byte[] bytes)
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

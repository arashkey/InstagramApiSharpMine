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
using Ionic.Zlib;
using Newtonsoft.Json;
  
namespace InstagramApiSharp.API.Push
{
    internal class PacketInboundHandler : SimpleChannelInboundHandler<Packet>
    {
        public enum TopicIds
        {
            Message = 76,   // "/fbns_msg"
            RegReq = 79,    // "/fbns_reg_req"
            RegResp = 80    // "/fbns_reg_resp"
        }
        public IChannelHandlerContext ChannelHandlerContext { get; private set; }
        private readonly FbnsClient _client;
        private readonly int _keepAliveDuration;    // seconds
        private int _publishId;
        private bool _waitingForPubAck;
        private const int TIMEOUT = 5;
        private CancellationTokenSource _timerResetToken;

        public PacketInboundHandler(FbnsClient client, int keepAlive = 900)
        {
            _client = client;
            _keepAliveDuration = keepAlive;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            // If connection is closed, reconnect
            Task.Delay(TimeSpan.FromSeconds(TIMEOUT)).ContinueWith(async task =>
            {
                _timerResetToken?.Cancel();
                if (!_client.IsShutdown)
                    await _client.Start();
            });

        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Packet msg)
        {
            ChannelHandlerContext = ctx;
            switch (msg.PacketType)
            {
                case PacketType.CONNACK:
                    _client.ConnectionData.UpdateAuth(((FbnsConnAckPacket) msg).Authentication);
                    System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(_client.ConnectionData, Formatting.Indented));

                    RegisterMqttClient(ctx);
                    break;

                case PacketType.PUBLISH:
                    var publishPacket = (PublishPacket) msg;
                    if (publishPacket.QualityOfService == QualityOfService.AtLeastOnce)
                    {
                        ctx.WriteAndFlushAsync(PubAckPacket.InResponseTo(publishPacket));
                    }
                    var payload = DecompressPayload(publishPacket.Payload);
                    var json = Encoding.UTF8.GetString(payload);
                    switch (Enum.Parse(typeof(TopicIds), publishPacket.TopicName))
                    {
                        case TopicIds.Message:
                            var message = JsonConvert.DeserializeObject<MessageReceivedEventArgs>(json);
                            message.Json = json;
                            _client.OnMessageReceived(message);
                            break;
                        case TopicIds.RegResp:
                            OnRegisterResponse(json);
                            ResetTimer(ctx);
                            break;
                        default:
                            Debug.WriteLine($"Unknown topic received: {publishPacket.TopicName}", "Warning");
                            break;
                    }
                    break;

                case PacketType.PUBACK:
                    _waitingForPubAck = false;
                    break;

                case PacketType.PINGRESP:
                    ResetTimer(ctx);
                    break;
                // Todo: Handle other packet types
            }
        }

        /// <summary>
        ///     After receiving the token, proceed to register over Instagram API
        /// </summary>
        private async void OnRegisterResponse(string json)
        {
            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (!string.IsNullOrEmpty(response["error"]))
            {
                Debug.WriteLine("FBNS error: " + response["error"], "Error");
                return;
            }

            var token = response["token"];

            await _client.RegisterClient(token);
        }


        /// <summary>
        ///     Register this client on the MQTT side stating what application this client is using.
        ///     The server will then return a token for registering over Instagram API side.
        /// </summary>
        private void RegisterMqttClient(IChannelHandlerContext ctx)
        {
            var message = new Dictionary<string, string>
            {
                {"pkg_name", InstaApiConstants.INSTAGRAM_PACKAGE_NAME},
                {"appid", InstaApiConstants.IG_APP_ID}
            };

            var json = JsonConvert.SerializeObject(message);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] compressed;
            using (var compressedStream = new MemoryStream(jsonBytes.Length))
            {
                using (var zlibStream =
                    new ZlibStream(compressedStream, CompressionMode.Compress, CompressionLevel.Level9, true))
                {
                    zlibStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                compressed = new byte[compressedStream.Length];
                compressedStream.Position = 0;
                compressedStream.Read(compressed, 0, compressed.Length);
            }

            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = Unpooled.CopiedBuffer(compressed),
                PacketId = ++_publishId,
                TopicName = ((byte)TopicIds.RegReq).ToString()
            };

            // Send PUBLISH packet then wait for PUBACK
            // Retry after TIMEOUT seconds
            ctx.WriteAndFlushAsync(publishPacket);
            _waitingForPubAck = true;
            Task.Delay(TimeSpan.FromSeconds(TIMEOUT)).ContinueWith(retry =>
            {
                if (_waitingForPubAck && ctx.Channel.Active)
                {
                    RegisterMqttClient(ctx);
                }
            });
        }

        private void ResetTimer(IChannelHandlerContext ctx)
        {
            _timerResetToken?.Cancel();
            _timerResetToken = new CancellationTokenSource();

            var cancellationToken = _timerResetToken.Token;

            Task.Delay(TimeSpan.FromSeconds(_keepAliveDuration - 60),
                    cancellationToken) 
                .ContinueWith(async task =>
                {
                    var packet = PingReqPacket.Instance;
                    await ctx.WriteAndFlushAsync(packet);
                    Debug.WriteLine("PingReq sent");
                }, cancellationToken)
                .ContinueWith(async task =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken); 
                        if(!cancellationToken.IsCancellationRequested)
                            ChannelInactive(ctx);  
                    }
                    catch (TaskCanceledException)
                    {
                        Debug.WriteLine("Keep alive timer reset.");
                    }
                }, cancellationToken);

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


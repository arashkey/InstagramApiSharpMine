using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using InstagramApiSharp.API.Push.PacketHelpers;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Helpers;
using Ionic.Zlib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.API.Push
{
    public sealed class FbnsClient
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private readonly IInstaApi _instaApi;
        private SingleThreadEventLoop _loopGroup;
        private const string DEFAULT_HOST = "mqtt-mini.facebook.com";
        private int _secondsToNextRetry = 5;
        private CancellationTokenSource _connectRetryCancellationToken;

        public bool IsShutdown => _loopGroup?.IsShutdown ?? false;

        internal FbnsConnectionData ConnectionData { get; }

        internal FbnsClient(IInstaApi instaApi, FbnsConnectionData connectionData = null)
        {
            _instaApi = instaApi;
            ConnectionData = connectionData ?? new FbnsConnectionData();

            // If token is older than 24 hours then discard it
            if ((DateTime.Now - ConnectionData.FbnsTokenLastUpdated).TotalHours > 24) ConnectionData.FbnsToken = "";

            // Build user agent for first time setup
            if (string.IsNullOrEmpty(ConnectionData.UserAgent))
                ConnectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(instaApi);
        }
        Bootstrap Bootstrap;
        IChannel FbnsChannel;
        PacketInboundHandler PacketInboundHandler;
        public async Task Start()
        {
            _connectRetryCancellationToken?.Cancel();
            _connectRetryCancellationToken = new CancellationTokenSource();
            if (_loopGroup != null) await _loopGroup.ShutdownGracefullyAsync();
            _loopGroup = new SingleThreadEventLoop();
            var cancellationToken = _connectRetryCancellationToken.Token;

            var connectPacket = new FbnsConnectPacket
            {
                Payload = await PushPayload.BuildPayload(ConnectionData)
            };
            PacketInboundHandler = new PacketInboundHandler(this);
            Bootstrap = new Bootstrap();
            Bootstrap
                .Group(_loopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(5))
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Handler(new ActionChannelInitializer<TcpSocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("tls", new TlsHandler(
                        stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true),
                        new ClientTlsSettings(DEFAULT_HOST)));
                    pipeline.AddLast("encoder", new FbnsPacketEncoder());
                    pipeline.AddLast("decoder", new FbnsPacketDecoder());
                    pipeline.AddLast("handler", PacketInboundHandler);
                }));

            try
            {
                if (cancellationToken.IsCancellationRequested) return;
                //FbnsChannel = await Bootstrap.ConnectAsync(IPAddress.Parse("69.171.250.34"), 443);
                FbnsChannel = await Bootstrap.ConnectAsync(new DnsEndPoint(DEFAULT_HOST, 443));
                await FbnsChannel.WriteAndFlushAsync(connectPacket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine($"Failed to connect to Push/MQTT server. No Internet connection? Retry in {_secondsToNextRetry} seconds.");
                await Task.Delay(TimeSpan.FromSeconds(_secondsToNextRetry), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
                _secondsToNextRetry = _secondsToNextRetry < 300 ? _secondsToNextRetry * 2 : 300;    // Maximum wait time is 5 mins
                await Start();
            }
        }

        internal async Task RegisterClient(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            if (ConnectionData.FbnsToken == token)
            {
                ConnectionData.FbnsToken = token;
                return;
            }
            var deviceInfo = _instaApi.GetCurrentDevice();
            var user = _instaApi.GetLoggedUser();
            var uri = UriCreator.GetPushRegisterUri();
            var fields = new Dictionary<string, string>
            {
                {"device_type", "android_mqtt"},
                {"is_main_push_channel", "true"},
                {"phone_id", deviceInfo.PhoneGuid.ToString()},
                {"device_token", token},
                {"_csrftoken", user.CsrfToken },
                {"guid", deviceInfo.PhoneGuid.ToString() },
                {"_uuid", deviceInfo.DeviceGuid.ToString() },
                {"users", user.LoggedInUser.Pk.ToString() }
            };
            var request = _instaApi.HttpHelper.GetDefaultRequest(HttpMethod.Post, uri, deviceInfo, fields);
            await _instaApi.HttpRequestProcessor.SendAsync(request);

            ConnectionData.FbnsToken = token;
        }

        public async Task Shutdown()
        {
            _connectRetryCancellationToken?.Cancel();
            if (_loopGroup != null) await _loopGroup.ShutdownGracefullyAsync();
        }

        internal void OnMessageReceived(MessageReceivedEventArgs args)
        {
            if (args != null && args.NotificationContent != null)
                args.NotificationContent.IntendedRecipientUserName = _instaApi.GetLoggedUser().UserName;
            MessageReceived?.Invoke(this, args);
        }
        private int _publishId;

        public async Task SendMessageAsync(string threadId, string text)
        {

            var token = Guid.NewGuid().ToString();
            var data = new JObject
            {
                {"action", "send_item"},
                {"thread_id",  threadId  },
                {"client_context", token},
                {"item_type", "text" },
                {"text", text},
                //{"data", new JObject
                //    {
                //        {"item_type", "text" },
                //        {"text", text},
                //        {"client_context", token},
                //    }
                //},
            };

            var json = JsonConvert.SerializeObject(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] compressed;
            using (var compressedStream = new MemoryStream(jsonBytes.Length))
            {
                //using (var zlibStream =
                //    new ZlibStream(compressedStream, CompressionMode.Compress, CompressionLevel.Level9, true))
                //{
                //    zlibStream.Write(jsonBytes, 0, jsonBytes.Length);
                //}
                using (
             var zlibStream =
       new ZlibStream(compressedStream, CompressionMode.Compress, CompressionLevel.Level9, true))
                {
                    zlibStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                compressed = new byte[compressedStream.Length];
                compressedStream.Position = 0;
                compressedStream.Read(compressed, 0, compressed.Length);
            }

            //SEND_MESSAGE:
            //    {
            //    id: '132',
            //path: '/ig_send_message',
            //parser: null,
            var publishPacket = new PublishPacket(QualityOfService.ExactlyOnce, false, false)
            {
                Payload = Unpooled.CopiedBuffer(compressed),
                PacketId = new Random().Next(1, ushort.MaxValue),
                TopicName = /*"/ig_send_message"*/ (132).ToString()
            };

            // Send PUBLISH packet then wait for PUBACK
            // Retry after TIMEOUT seconds
            await FbnsChannel.WriteAndFlushAsync(publishPacket);
            //await PacketInboundHandler.ChannelHandlerContext.WriteAndFlushAsync(publishPacket);

        }

        public void SendMessage(string threadId, string text, string token = null)
        {
            if(token == null)
             token = Guid.NewGuid().ToString();
            var data = new JObject
            {
                {"action", "send_item"},
                {"thread_id",  threadId  },
                {"client_context", token},
                {"item_type", "text" },
                {"text", text},
            };

            var json = JsonConvert.SerializeObject(data);
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
                TopicName = ((byte)132).ToString()
            };

            // Send PUBLISH packet then wait for PUBACK
            // Retry after TIMEOUT seconds
            FbnsChannel.WriteAndFlushAsync(publishPacket);


            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(retry =>
            {
                //if (FbnsChannel.Active)
                {
                    SendMessage(threadId, text, token);
                }
            });
        }
    }
}

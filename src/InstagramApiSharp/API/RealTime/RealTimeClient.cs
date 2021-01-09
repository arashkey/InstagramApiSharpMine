/*
 * Edited by Ramtin Jokar [ Ramtinak@live.com ] [ https://t.me/ramtinak ]
 * Donation link: [ https://paypal.me/rmt4006 ] 
 * Donation email: RamtinJokar@outlook.com
 * 
 * Copyright (c) 2020 Summer [ Tabestaan 1399 ]
 */
using DotNetty.Buffers;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using InstagramApiSharp.API.Push;
using InstagramApiSharp.API.Push.PacketHelpers;
using InstagramApiSharp.API.RealTime.Handlers;
using InstagramApiSharp.API.RealTime.Helpers;
using InstagramApiSharp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace InstagramApiSharp.API.RealTime
{
    public sealed partial class RealTimeClient
    {
        public event EventHandler<PresenceEventEventArgs> PresenceChanged;
        public event EventHandler<ThreadTypingEventsArgs> TypingChanged;
        private readonly IInstaApi _instaApi;
        private SingleThreadEventLoop _loopGroup;
        Bootstrap Bootstrap;
        IChannel RealtimeChannel;
        RealtimePacketInboundHandler PacketInboundHandler;
        private const string DEFAULT_HOST = "edge-mqtt.facebook.com";
        private int _secondsToNextRetry = 5;
        private CancellationTokenSource _connectRetryCancellationToken;

        public bool IsShutdown => _loopGroup?.IsShutdown ?? false;

        private FbnsConnectionData ConnectionData => new FbnsConnectionData();
        public TimeSpan WaitForResponseDelay { get; private set; } = TimeSpan.FromMilliseconds(450);

        internal bool GetInboxAutomatically { get; set; } = true;
        internal string SeqId;
        internal string SnapshotAtMs;
        public RealTimeClient(IInstaApi instaApi)
        {
            _instaApi = instaApi;
            ConnectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(instaApi);
        }
        public async Task Start(bool getInboxAutomatically = true)
        {
            GetInboxAutomatically = getInboxAutomatically;
            if (GetInboxAutomatically)
            {
                var inbox = await _instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
                if (inbox.Succeeded)
                {
                    SeqId = inbox.Value.SeqId.ToString();
                    SnapshotAtMs = inbox.Value.SnapshotAt.ToUnixTimeMiliSeconds().ToString();
                }
            }
            _connectRetryCancellationToken?.Cancel();
            _connectRetryCancellationToken = new CancellationTokenSource();
            if (_loopGroup != null) await _loopGroup.ShutdownGracefullyAsync();
            _loopGroup = new SingleThreadEventLoop();
            var cancellationToken = _connectRetryCancellationToken.Token;

            var connectPacket = new FbnsConnectPacket
            {
                Payload = await RealtimePayload.BuildPayload(_instaApi),
                WillQualityOfService = QualityOfService.AtLeastOnce
            };
            PacketInboundHandler = new RealtimePacketInboundHandler(this);
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
                RealtimeChannel = await Bootstrap.ConnectAsync(new DnsEndPoint(DEFAULT_HOST, 443));
                await RealtimeChannel.WriteAndFlushAsync(connectPacket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine($"Failed to connect to Push/MQTT server. No Internet connection? Retry in {_secondsToNextRetry} seconds.");
                await Task.Delay(TimeSpan.FromSeconds(_secondsToNextRetry), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
                _secondsToNextRetry = _secondsToNextRetry < 300 ? _secondsToNextRetry * 2 : 300;
                await Start();
            }
        }
        internal async Task SubscribeForDM()
        {

            var messageSync = new SubscriptionRequest("/ig_message_sync", QualityOfService.AtLeastOnce);
            var sendMessageResp = new SubscriptionRequest("/ig_send_message_response", QualityOfService.AtLeastOnce);
            var subIrisResp = new SubscriptionRequest("/ig_sub_iris_response", QualityOfService.AtLeastOnce);
            var SubscribePacket = new SubscribePacket(new Random().Next(1, ushort.MaxValue), messageSync, sendMessageResp, subIrisResp);

            await RealtimeChannel.WriteAndFlushAsync(SubscribePacket);
        }

        internal async Task RealtimeSub()
        {
            var user = _instaApi.GetLoggedUser().LoggedInUser;
            var dic = new Dictionary<string, List<string>>
            {
                {  "sub",
                    new List<string>
                    {
                        GraphQLSubscriptions.GetAppPresenceSubscription(),
                        GraphQLSubscriptions.GetZeroProvisionSubscription(_instaApi.GetCurrentDevice().DeviceGuid.ToString()),
                        GraphQLSubscriptions.GetDirectStatusSubscription(),
                        GraphQLSubscriptions.GetDirectTypingSubscription(user?.Pk.ToString()),
                        GraphQLSubscriptions.GetAsyncAdSubscription(user?.Pk.ToString())
                    }
                }
            };
            var json = JsonConvert.SerializeObject(dic);
            var bytes = Encoding.UTF8.GetBytes(json);

            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = Unpooled.CopiedBuffer(ZlibHelper.Compress(bytes)),
                PacketId = new Random().Next(1, ushort.MaxValue),
                TopicName = "/ig_realtime_sub"
            };
            await RealtimeChannel.WriteAndFlushAsync(publishPacket);
        }


        internal async Task PubSub()
        {
            var user = _instaApi.GetLoggedUser().LoggedInUser;
            var dic = new Dictionary<string, List<string>>
            {
                {  "sub",
                    new List<string>
                    {
                        SkyWalker.DirectSubscribe(user?.Pk.ToString()),
                        SkyWalker.LiveSubscribe(user?.Pk.ToString()),
                    }
                }
            };
            var json = JsonConvert.SerializeObject(dic);
            var bytes = Encoding.UTF8.GetBytes(json);

            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = Unpooled.CopiedBuffer(ZlibHelper.Compress(bytes)),
                PacketId = new Random().Next(1, ushort.MaxValue),
                TopicName = "/pubsub"
            };
            await RealtimeChannel.WriteAndFlushAsync(publishPacket);
        }


        public async Task IrisSub(string seqId, string snapshotAtMs)
        {
            var dic = new Dictionary<string, object>
            {
                {"seq_id", seqId },
                {"sub", new List<string>()},
                {"snapshot_at_ms", snapshotAtMs}
            };
            var json = JsonConvert.SerializeObject(dic);
            var bytes = Encoding.UTF8.GetBytes(json);

            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = Unpooled.CopiedBuffer(ZlibHelper.Compress(bytes)),
                PacketId = new Random().Next(1, ushort.MaxValue),
                TopicName = "/ig_sub_iris"
            };
            await RealtimeChannel.WriteAndFlushAsync(publishPacket);
        }
        public async Task Shutdown()
        {
            PacketInboundHandler?.Responses.Clear();
            _connectRetryCancellationToken?.Cancel();
            if (_loopGroup != null) await _loopGroup.ShutdownGracefullyAsync();
        }
        public void SetResponseDelay(TimeSpan? delay)
        {
            if (delay == null)
                WaitForResponseDelay = TimeSpan.FromMilliseconds(450);
            else
            {
                if (delay.Value.TotalMilliseconds < 450)
                    WaitForResponseDelay = TimeSpan.FromMilliseconds(450);
                else
                    WaitForResponseDelay = delay.Value;
            }
        }


        internal void OnPresenceChanged(PresenceEventEventArgs args)
        {
            PresenceChanged?.Invoke(this, args);
        }
        internal void OnTypingChanged(ThreadTypingEventsArgs args)
        {
            TypingChanged?.Invoke(this, args);
        }
    }

}

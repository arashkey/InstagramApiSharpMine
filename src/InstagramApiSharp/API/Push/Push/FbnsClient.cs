using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Linq;
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
using InstagramApiSharp.Logger;
using Ionic.Zlib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace InstagramApiSharp.API.Push
{
    public sealed class FbnsClient
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<PushNotification> KeepingAliveMessageReceived;
        public bool IsShutdown => _loopGroup?.IsShutdown ?? false;
        private readonly IInstaApi _instaApi;
        private MultithreadEventLoopGroup _loopGroup;
        private const string DEFAULT_HOST = "mqtt-mini.facebook.com";
        private int _secondsToNextRetry = 5;
        private bool _retrying = false;
        private CancellationTokenSource _connectRetryCancellationToken;
        private PacketInboundHandler PacketInboundHandler;
        private SslStream SslStream = null;
        private Stream Stream = null;
        private readonly IInstaLogger Logger;
        internal Bootstrap Bootstrap;
        internal IChannel FbnsChannel;
        internal FbnsConnectionData ConnectionData { get; set; }
        internal string[] KeepingAliveUserIds = null;
        internal int? KeepingAliveUserMessageDelay = null;
        internal FbnsClient(IInstaApi instaApi, FbnsConnectionData connectionData = null)
        {
            _instaApi = instaApi;
            Logger = instaApi.GetLogger();
            ConnectionData = connectionData ?? new FbnsConnectionData();

            if (string.IsNullOrEmpty(ConnectionData.UserAgent))
                ConnectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(instaApi);
        }
        /// <summary>
        ///     Keep-alive connection by checking a specific users messages in amount of time
        /// </summary>
        /// <param name="delayInSeconds">Delay between checking time</param>
        /// <param name="userIds">Users id (pk)</param>
        public void KeepAliveByCheckingUserMessages(int delayInSeconds, params string[] userIds)
        {
            if (userIds == null || userIds?.Length == 0) return;

            KeepingAliveUserIds = userIds;
            KeepingAliveUserMessageDelay = delayInSeconds;
        }
        /// <summary>
        ///     Disable keeping alive the connection by checking user message
        /// </summary>
        public void DisableKeepAliveByCheckingUserMessages()
        {
            KeepingAliveUserIds = null;
            KeepingAliveUserMessageDelay = null;
        }
        public async Task Start()
        {
            ConnectionData = new FbnsConnectionData();
            if (string.IsNullOrEmpty(ConnectionData.UserAgent))
                ConnectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(_instaApi);
            _connectRetryCancellationToken?.Cancel();
            _connectRetryCancellationToken = new CancellationTokenSource();
            if (_loopGroup != null) await _loopGroup.ShutdownGracefullyAsync();
            _loopGroup = new MultithreadEventLoopGroup/*SingleThreadEventLoop*/();

            var cancellationToken = _connectRetryCancellationToken.Token;

            var connectPacket = new FbnsConnectPacket
            {
                Payload = await PushPayload.BuildPayload(ConnectionData).ConfigureAwait(false)
            };
            PacketInboundHandler = new PacketInboundHandler(this);
            PacketInboundHandler.RetryConnection += OnRetryConnection;
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
                      stream => 
                      {
                          Stream = stream;
                          SslStream = new SslStream(stream, true, (sender, certificate, chain, errors) => true);
                          return SslStream;
                      },
                      new ClientTlsSettings(DEFAULT_HOST)));
                    pipeline.AddLast("encoder", new FbnsPacketEncoder());
                    pipeline.AddLast("decoder", new FbnsPacketDecoder());
                    pipeline.AddLast("handler", PacketInboundHandler);
                }));

            try
            {
                if (cancellationToken.IsCancellationRequested) return;
                Debug.WriteLine(DateTime.Now + " Push starting");
                //var ipAdd = $"{Dns.GetHostAddresses(DEFAULT_HOST)[0]}:443";
                //FbnsChannel = await Bootstrap.ConnectAsync(IPAddress.Parse("69.171.250.34"), 443);
                FbnsChannel = await Bootstrap.ConnectAsync(new DnsEndPoint(DEFAULT_HOST, 443));
                await FbnsChannel.WriteAndFlushAsync(connectPacket);
                Debug.WriteLine(DateTime.Now + " Push started");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine($"Failed to connect to Push/MQTT server. No Internet connection? Retry in {_secondsToNextRetry} seconds.");
                Logger.LogException(ex);
                await Task.Delay(TimeSpan.FromSeconds(_secondsToNextRetry), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
                _secondsToNextRetry = _secondsToNextRetry < 300 ? _secondsToNextRetry * 2 : 300;    // Maximum wait time is 5 mins
                await Restart();
            }
        }
        private async void OnRetryConnection(object sender, EventArgs e)
        {
            if (!IsShutdown && !_retrying)
            {
                var shutdown = IsShutdown;
                _retrying = true;
                int retryCount = 0;
                while (!NetworkInterface.GetIsNetworkAvailable() || IsShutdown)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_secondsToNextRetry));
                    _secondsToNextRetry = _secondsToNextRetry < 300 ? _secondsToNextRetry * 2 : 300;    // Maximum wait time is 5 mins
                    Debug.WriteLine($"{DateTime.Now:G} _secondsToNextRetry: " + _secondsToNextRetry);
                    if (retryCount > 6)
                        break;
                }
                if (!shutdown)
                { await Restart(); }
            }
        }
        internal async Task Restart()
        {
            try
            {
                await Shutdown();
                await Task.Delay(5000);
            }
            catch { }
            finally
            {
                await Start();
            }

        }
        internal async Task RegisterClient(string token)
        {
            if (string.IsNullOrEmpty(token)) return; /*throw new ArgumentNullException(nameof(token));*/
            //if (ConnectionData.FbnsToken == token)
            //{
            //    ConnectionData.FbnsToken = token;
            //    return;
            //}
            var deviceInfo = _instaApi.GetCurrentDevice();
            var user = _instaApi.GetLoggedUser();
            var uri = UriCreator.GetPushRegisterUri();
            var fields = new Dictionary<string, string>
            {
                {"device_type", "android_mqtt"},
                {"is_main_push_channel", "true"},
                {"phone_id", deviceInfo.PhoneGuid.ToString()},
                {"device_token", token},
                {"guid", deviceInfo.PhoneGuid.ToString()},
                {"_uuid", deviceInfo.DeviceGuid.ToString()},
                {"users", user.LoggedInUser.Pk.ToString()}
            };
            if (!_instaApi.HttpHelper.NewerThan180)
            {
                fields.Add("_csrftoken", user.CsrfToken);
            }
            var request = _instaApi.HttpHelper.GetDefaultRequest(HttpMethod.Post, uri, deviceInfo, fields);
            await _instaApi.HttpRequestProcessor.SendAsync(request);

            ConnectionData.FbnsToken = token;
            PacketInboundHandler?.StartKeepingAlive();
        }

        public async Task Shutdown()
        {
            _connectRetryCancellationToken?.Cancel();
            if (PacketInboundHandler != null)
            {
                PacketInboundHandler.RetryConnection -= OnRetryConnection;
                PacketInboundHandler.TimerResetToken?.Cancel();
            }
            if (_loopGroup != null)
            {
                await _loopGroup.ShutdownGracefullyAsync();
                _loopGroup = null;
            }
            Bootstrap = null;
            try
            {
                SslStream?.Flush();
                SslStream?.Close();
                SslStream?.Dispose();

                Stream?.Flush();
                Stream?.Close();
                Stream?.Dispose();
            } 
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now:G} PushClient.Shutdown [Closing streams] exceptionthrown: " + ex);
            }
        }

        internal void OnMessageReceived(MessageReceivedEventArgs args)
        {
            if (args != null && args.NotificationContent != null)
                args.NotificationContent.IntendedRecipientUserName = _instaApi.GetLoggedUser().UserName;
            MessageReceived?.Invoke(this, args);

            if (KeepingAliveUserMessageDelay.HasValue && KeepingAliveUserIds?.Length > 0)
            {
                var notification = args.NotificationContent;

                var action = notification.IgAction;
                _ = HttpUtility.ParseQueryString(action, out string type);

                var sourceUserId = notification.SourceUserId;           // user id of sender
                var pushCategory = notification.PushCategory;           // category

                if ((type == "direct_v2" && pushCategory.IsEmpty()) || 
                    (type == "direct_v2" && pushCategory == "direct_v2_message"))
                {
                    if (sourceUserId.IsNotEmpty() && KeepingAliveUserIds.Contains(sourceUserId.Trim()))
                    {
                        KeepingAliveMessageReceived?.Invoke(this, notification);

                        PacketInboundHandler.LastCheckedTime = DateTime.Now;
                    }
                }
            }
        }
    }
}

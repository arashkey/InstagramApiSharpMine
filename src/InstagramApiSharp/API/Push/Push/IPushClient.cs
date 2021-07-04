using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Push
{
    public interface IPushClient
    {
        event EventHandler<object> FbnsTokenChanged;
        event EventHandler<PushReceivedEventArgs> MessageReceived;
        event EventHandler<object> LogReceived;
        FbnsConnectionData ConnectionData { get; }
#if WINDOWS_UWP
        void OpenNow();
        bool DontTransferSocket { get; set; }
        Windows.Networking.Sockets.StreamSocket Socket { get; }
        Task StartWithExistingSocket(Windows.Networking.Sockets.StreamSocket socket);
        void UnregisterTasks();
#endif

        Task TransferPushSocket();
        void Start();
        Task StartFresh();
        void Shutdown();
        Task SendPing();
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

 using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using InstagramApiSharp;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Logger;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.API
{
    public class InstaPushApi
    {
        public event InstaPushHandler OnPushed;
        public IInstaApi InstaApi { get; internal set; }
        public InstaPushApi(IInstaApi instaApi)
        {
            InstaApi = instaApi;
        }

        void SendPushEvent()
        {
            OnPushed?.Invoke(InstaApi);
        }
    }
    public delegate void InstaPushHandler(IInstaApi sender /*, PushType? */);
    public class InstaPushEventsArgs : EventArgs
    {

    }
}

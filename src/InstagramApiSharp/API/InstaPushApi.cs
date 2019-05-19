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
        public IInstaApi InstaApi { get; internal set; }
        public InstaPushApi(IInstaApi instaApi)
        {
            InstaApi = instaApi;
        }
    }
    public delegate void InstaPushHandler(IInstaApi sender /*, PushType? */);
    public class InstaPushEventsArgs : EventArgs
    {

    }
}

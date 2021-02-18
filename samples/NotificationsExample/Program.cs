using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.API.RealTime.Responses.Models;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;
using System.Linq;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Helpers;
/////////////////////////////////////////////////////////////////////
////////////////////// IMPORTANT NOTE ///////////////////////////////
//
//
// Please read README.MD file from the original repository
// Note: InstagramApiSharp.csproj doesn't support PUSH Notifications/Realtime client !!!!
// You need to import InstagramApiSharp.WithNotification.csproj and Thrift.csproj to your projects as a reference. 
//
//
//
//
// You must/should reference following packages to your projects, if you got any error:
//| Target | Package name | Version | Level | 
//| ------ | ------ | ------ | ------ |
//| Encrypted password | Portable.BouncyCastle | 1.8.6.7 or newer | Important for InstagramApiSharp |
//| Json wrapper | Newtonsoft.Json | 10.0.3 or newer | Important for InstagramApiSharp |
//| CSharp library | Microsoft.CSharp | 4.3.0 | Important for InstagramApiSharp |
//| GZip | Iconic.Zlib.NetstandardUwp | 1.0.1 or newer |  Important for InstagramApiSharp |
//| Push/Realtime | Thrift | InstagramApiSharp's Port | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Codecs.Mqtt | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Buffers | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
//| - | Microsoft.Extensions.Logging | 3.1.4 | Important for Thrift |
//| - | Microsoft.Extensions.Logging.Abstractions | 3.1.4 | Important for Thrift |
//| - | Microsoft.Extensions.Options | 3.1.4 | Important for Thrift |
//| - | System.Threading.Tasks.Extensions | 4.5.4 | Important for Thrift |


//- Note 1: You MUST reference [Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle/)'s package to your projects.
//- Note 2: You MUST reference Thrift's project (InstagramApiSharp's port) to your project.
//- Note 3: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.


////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////

namespace NotificationsExample
{
    partial class Program
    {
        static void Main()
        {
            Task.Run(ExtendedProgram.MainAsync).GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}

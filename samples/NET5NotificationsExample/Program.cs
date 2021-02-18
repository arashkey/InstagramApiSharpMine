using System;
using System.Threading.Tasks;
/////////////////////////////////////////////////////////////////////
////////////////////// IMPORTANT NOTE ///////////////////////////////
//
//
// Please read README.MD file from the original repository
// Note: InstagramApiSharp.NET5.csproj doesn't support PUSH Notifications/Realtime client !!!!
// You need to import InstagramApiSharp.NET5.WithNotification.csproj and Thrift.NET5.csproj to your projects as a reference. 
//
//
//
//
// You must/should reference following packages to your projects, if you got any error:
//| Target | Package name | Version | Level | 
//| ------ | ------ | ------ | ------ |
//| Json wrapper | Newtonsoft.Json | 10.0.3 or newer | Important for InstagramApiSharp |
//| GZip | Iconic.Zlib.NetstandardUwp | 1.0.1 or newer |  Important for InstagramApiSharp |
//| Push/Realtime | Thrift.NET5 | InstagramApiSharp.NET5's Port | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Codecs.Mqtt | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Buffers | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
//| - | Microsoft.Extensions.Logging | 3.1.4 | Important for Thrift.NET5 |
//| - | Microsoft.Extensions.Logging.Abstractions | 3.1.4 | Important for Thrift.NET5 |
//| - | Microsoft.Extensions.Options | 3.1.4 | Important for Thrift.NET5 |


//- Note 1: You MUST reference Thrift.NET5's project (InstagramApiSharp.NET5's port) to your project.
//- Note 2: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.


////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////

namespace NET5NotificationsExample
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
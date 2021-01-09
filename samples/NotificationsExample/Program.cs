﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
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



//- Note 1: You MUST reference [Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle/)'s package to your projects.
//- Note 2: You MUST reference Thrift's project (InstagramApiSharp's port) to your project.
//- Note 3: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.


////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////
namespace NotificationsExample
{
    class Program
    {
        private static IInstaApi InstaApi;
        static void Main(string[] args)
        {
            Task.Run(MainAsync).GetAwaiter().GetResult();
            Console.ReadKey();
        }
        public static async Task MainAsync()
        {
            Console.WriteLine("Starting demo of InstagramApiSharp's notifications/realtime sample project");
            // create user session data and provide login details
            var userSession = new UserSessionData
            {
                UserName = "username",
                Password = "password"
            };
            // if you want to set custom device (user-agent) please check this:
            // https://github.com/ramtinak/InstagramApiSharp/wiki/Set-custom-device(user-agent)

            //var delay = RequestDelay.FromSeconds(2, 2);

            var delay = RequestDelay.FromSeconds(1, 1);
            // create new InstaApi instance using Builder
            InstaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.All)) // use logger for requests and debug messages
                .SetRequestDelay(delay)
                .Build();

            // Load your data
            if (!InstaApi.IsUserAuthenticated)
            {
                // login
                Console.WriteLine($"Logging in as {userSession.UserName}");
                delay.Disable();
                var logInResult = await InstaApi.LoginAsync();
                delay.Enable();
                if (!logInResult.Succeeded)
                {
                    Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                    return;
                }
            }

            // save your data


            // PUSH NOTIFICATIONS
            // - Configurations>
            InstaApi.PushClient.MessageReceived += PushClientMessageReceived;
            // Starting push notifications listener
            await InstaApi.PushClient.Start();
            Console.WriteLine($"Push notifications listener started");


            //////////////////////////////////////////////////////


            // Realtime client [for direct messaging]
            // - Configurations>
            InstaApi.RealTimeClient.PresenceChanged += RealTimeClientPresenceChanged;
            InstaApi.RealTimeClient.TypingChanged += RealTimeClientTypingChanged;
            // Starting realtime client listener
            await InstaApi.RealTimeClient.Start();
            Console.WriteLine($"Realtime client listener started");
            // You can use realtime to send direct items as well
            // Note that you MUST Start the realtime client first! if you want to use Realtime client functions

            // i.e: Send a message to someone>
            var user = await InstaApi.UserProcessor.GetUserAsync("ministaapp");
            if (user.Succeeded)
                await InstaApi.RealTimeClient.SendDirectTextToRecipientAsync(user.Value.Pk.ToString(), "Hello from IRAN");
        }

        //////////////////////////////////////////////////////////
        /////////////////////// NOTIFICATIONS ////////////////////
        //////////////////////////////////////////////////////////
        private static void PushClientMessageReceived(object sender, InstagramApiSharp.API.Push.MessageReceivedEventArgs e)
        {
            // do whatever you want to do with notifications
            if (e?.NotificationContent != null)
                Console.WriteLine($"Notification received >>> {e.NotificationContent.Message}");
        }



        //////////////////////////////////////////////////////////
        //////////////////// REALTIME CLIENT /////////////////////
        //////////////////////////////////////////////////////////

        private async static void RealTimeClientTypingChanged(object sender, InstagramApiSharp.API.RealTime.Handlers.ThreadTypingEventsArgs e)
        {
            try
            {
                // who is typing?
                if (!string.IsNullOrEmpty(e?.TypingData?.SenderId))
                {
                    var userId = long.Parse(e.TypingData.SenderId);

                    // get username>
                    var user = await InstaApi.UserProcessor.GetUserInfoByIdAsync(userId).ConfigureAwait(false);

                    Console.WriteLine($"@{user.Value.UserName} is typing....");
                }
            }
            catch { }
        }

        private async static void RealTimeClientPresenceChanged(object sender, InstagramApiSharp.API.RealTime.Handlers.PresenceEventEventArgs e)
        {
            // presence changed...Online status of someone changed
            if(!string.IsNullOrEmpty(e?.UserId))
            {
                var userId = long.Parse(e.UserId);
                // get username>
                var user = await InstaApi.UserProcessor.GetUserInfoByIdAsync(userId).ConfigureAwait(false);
                Console.WriteLine($"@{user.Value.UserName} {(e.IsActive ? "is" : "is not")} online");
            }
        }

    }
}
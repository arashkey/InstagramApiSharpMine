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
using NotificationsExample;
/////////////////////////////////////////////////////////////////////
////////////////////// IMPORTANT NOTE ///////////////////////////////
//
//
// Please read README.MD file from the original repository
// Note: InstagramApiSharp.NET5.csproj doesn't support PUSH Notifications/Realtime client !!!!
// You need to import InstagramApiSharp.NET5.WithNotification.csproj and Thrift.csproj to your projects as a reference. 
//
//
//
//
// You must/should reference following packages to your projects, if you got any error:
//| Target | Package name | Version | Level | 
//| ------ | ------ | ------ | ------ |
//| Json wrapper | Newtonsoft.Json | 10.0.3 or newer | Important for InstagramApiSharp |
//| GZip | Iconic.Zlib.NetstandardUwp | 1.0.1 or newer |  Important for InstagramApiSharp |
//| Push/Realtime | Thrift.NET5 | InstagramApiSharp's Port | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Codecs.Mqtt | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Buffers | 0.6.0 | Important for Push notifications/Realtime client |
//| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
//| - | Microsoft.Extensions.Logging | 3.1.4 | Important for Thrift |
//| - | Microsoft.Extensions.Logging.Abstractions | 3.1.4 | Important for Thrift |
//| - | Microsoft.Extensions.Options | 3.1.4 | Important for Thrift |


//- Note 1: You MUST reference Thrift.NET5's project (InstagramApiSharp's port) to your project.
//- Note 2: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.


////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////

#pragma warning disable IDE0059 // Unnecessary assignment of a value
public class ExtendedProgram 
{
    private static IInstaApi InstaApi;
    const string StateFile = "state.bin";
    // You should get threads from the logged in account, to understand what is going on in the receiver's events
    private static List<InstaDirectInboxThread> Threads = new List<InstaDirectInboxThread>();



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
            .SetSessionHandler(new FileSessionHandler { FilePath = StateFile })
            .Build();
        LoadSession();


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
            SaveSession();
        }

        // save your data


        // PUSH NOTIFICATIONS
        // - Configurations>
        InstaApi.PushClient.MessageReceived += PushClientMessageReceived;
        // Starting push notifications listener
        await InstaApi.PushClient.Start();
        Console.WriteLine($"Push notifications listener started");


        //////////////////////////////////////////////////////

        var inboxResult = await InstaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
        if (inboxResult.Succeeded)
            Threads = inboxResult.Value.Inbox.Threads;

        // Realtime client [for direct messaging]
        // - Configurations>
        InstaApi.RealTimeClient.PresenceChanged += RealTimeClientPresenceChanged;
        InstaApi.RealTimeClient.TypingChanged += RealTimeClientTypingChanged;
        InstaApi.RealTimeClient.DirectItemChanged += RealTimeClientDirectItemChanged;
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
    private async static void PushClientMessageReceived(object sender, InstagramApiSharp.API.Push.MessageReceivedEventArgs e)
    {
        // do whatever you want to do with notifications
        if (e?.NotificationContent != null)
        {
            // A notification sample:

            //{
            //  "Title": null,
            //  "Message": "rmt.jokar1373: hello minista",
            //  "TickerText": null,
            //  "IgAction": "direct_v2?id=340282366841710300949128237381813611847&x=29765750603588088089900975323611136",
            //  "CollapseKey": "direct_v2_message",
            //  "OptionalImage": null,
            //  "OptionalAvatarUrl": "https://scontent.cdninstagram.com/v/t51.2885-19/s150x150/148613314_446359293380085_3388830789281106916_n.jpg?_nc_ht=scontent.cdninstagram.com&_nc_ohc=zZhKdVwXXEQAX8X3d1j&tp=1&oh=62258a612e97175aa3c2d1561b6767cf&oe=6055AFED",
            //  "Sound": "default",
            //  "PushId": "5bb904ddc5975H62363020H5bb9097725c47H29",
            //  "PushCategory": null,
            //  "IntendedRecipientUserId": "1647718432",
            //  "SourceUserId": "44579558127",
            //  "IgActionOverride": null,
            //  "BadgeCount": {
            //    "Direct": 5,
            //    "Ds": 0,
            //    "Activities": 0
            //  },
            //  "InAppActors": null
            //}

            var notification = e.NotificationContent;
            Console.WriteLine($"Notification received >>> {notification.Message}");

            var action = notification.IgAction;
            var queries = HttpUtility.ParseQueryString(action, out string type);

            var collapsedKey = notification.CollapseKey;            // type of message
            var sourceUserId = notification.SourceUserId;           // user id of sender
            var pushCategory = notification.PushCategory;           // category
            var id = queries.GetValueIfPossible("id")?.Trim();      // thread id (if available)
            var itemId = queries.GetValueIfPossible("x");           // item id (if available)

            if (type == "direct_v2") // related to direct
            {
                // Example:
                // IgAction:        direct_v2?id=340282366841710300949128237381813611847&x=29765750603588088089900975323611136
                // Message:         rmt.jokar1373: hello minista
                // Message:         rmt.jokar1373 wants to send you a message.

                var threadId = id;
                var messageId = id;
                var userId = await InstaApi.GetUserId(sourceUserId);

                if (string.IsNullOrEmpty(pushCategory) || pushCategory == "direct_v2_message") // messaging
                {
                    // Do whatever you want to do with this message

                    //// Send a message for example:
                    //await InstaApi.MessagingProcessor.SendDirectTextAsync(null, threadId, "Hello from notification");

                }
                else if (pushCategory == "direct_v2_pending") // pending message
                {
                    // Accept   Delete   Block

                    //// Accept
                    //await InstaApi.MessagingProcessor.ApproveDirectPendingRequestAsync(id);

                    //// Decline
                    //await InstaApi.MessagingProcessor.DeclineDirectPendingRequestsAsync(id);

                    //// Delete (Decline and block user)
                    //await InstaApi.MessagingProcessor.DeclineDirectPendingRequestsAsync(id);
                    //await InstaApi.UserProcessor.BlockUserAsync(userId);
                }
            }
            else if (collapsedKey == "private_user_follow_request") // follow request
            {
                // Example:
                // IgAction:        user?username=rmtjj73&sourceUserId=14564882672
                // Message:         Minista App (@ministaapp) has requested to follow you.

                long userPk = await InstaApi.GetUserId(sourceUserId, queries["username"]);
                if (userPk == -1) return;

                // Accept follow request
                //await InstaApi.UserProcessor.AcceptFriendshipRequestAsync(userPk);

                // Ignore/decline follow request
                //await InstaApi.UserProcessor.IgnoreFriendshipRequestAsync(userPk);
            }
            else if (type == "broadcast" && collapsedKey == "live_broadcast")
            {
                // Example:
                // IgAction:        broadcast?id=18035667694304049&reel_id=1647718432&published_time=1607056892
                // Message:         ministaapp started a live video.

                // do whatever you want to do
            }
            else if (collapsedKey == "post")
            {
                // Example:
                // IgAction:        media?id=2455052815714850188_1647718432&media_id=2455052815714850188_1647718432
                // Message:         ministaapp just posted a photo.

                // Like post
                //await InstaApi.MediaProcessor.LikeMediaAsync(id);

                //// Comment
                //await InstaApi.CommentProcessor.CommentMediaAsync(id, "Woooow");
            }
            else if (collapsedKey == "comment")
            {
                // Example:
                // IgAction:        comments_v2?media_id=2450763156807842703_44428109093&target_comment_id=17915232835518492&permalink_enabled=True
                // Message:         ministaapp commented: \"😉😉😉😉😉😉\"

                var mediaId = id ?? queries.GetValueIfPossible("media_id");
                var targetMediaId = queries.GetValueIfPossible("target_comment_id");

                //// Like comment
                ////await InstaApi.CommentProcessor.LikeCommentAsync(targetMediaId);

                //// Reply a comment
                ////await InstaApi.CommentProcessor.ReplyCommentMediaAsync(mediaId, targetMediaId, "I replied to you");
            }
            else if (collapsedKey == "subscribed_igtv_post")
            {
                // Example:
                // IgAction:        tv_viewer?id=2457476214378560971
                // Message:         ministaapp just posted an IGTV video.

                if (string.IsNullOrEmpty(id)) return;
                // tv_viewer?id=2457476214378560971
                var mediaInfo = await InstaApi.MediaProcessor.GetMediaByIdAsync(id);
                if (!mediaInfo.Succeeded) return;
                var mediaId = mediaInfo.Value.InstaIdentifier;
                //// Like IGTV
                ////await InstaApi.CommentProcessor.LikeCommentAsync(mediaId);

                //// Send a comment
                ////await InstaApi.CommentProcessor.CommentMediaAsync(mediaId, "I liked your awesome IGTV");
            }
        }
    }



    //////////////////////////////////////////////////////////
    //////////////////// REALTIME CLIENT /////////////////////
    //////////////////////////////////////////////////////////

    private static void RealTimeClientTypingChanged(object sender, List<InstaRealtimeTypingEventArgs> e)
    {
        try
        {

            if (e?.Count > 0)
            {
                var start = "direct_v2/threads/";
                var typings = e
                    .Where(x => x.RealtimePath?.Contains(start) ?? false)// is it a thread or not?!
                    .Select(x =>
                    {
                            // /direct_v2/threads/340282366841710300949128154931298634193/activity_indicator_id/6685320955800332013
                            try
                        {
                            var threadId = x.RealtimePath.Substring(x.RealtimePath.IndexOf(start) + start.Length);
                            x.RealtimePath = threadId.Substring(0, threadId.IndexOf("/"));
                        }
                        catch { }
                        return x;
                    }).ToList();

                foreach (var item in typings)
                {
                    var userId = long.Parse(item.SenderId); // sender
                    var findThread = Threads.FirstOrDefault(x => x.ThreadId == item.RealtimePath); // find the current thread, if loaded
                    if (findThread != null)
                    {
                        var findUser = findThread.Users.FirstOrDefault(x => x.Pk == userId);

                        Console.WriteLine($"@{findUser.UserName} is typing....");
                    }
                    else // if thread not found in the Threads list, we have to ignore it, but you can do it
                    {
                        // WARNING: don't do this as much as possible!!! because it might block your account 
                        // bad idea for doing this>
                        //var user = await InstaApi.UserProcessor.GetUserInfoByIdAsync(userId).ConfigureAwait(false);
                        //Console.WriteLine($"@{user.Value.UserName} is typing....");
                    }
                }
            }
        }
        catch { }
    }

    private async static void RealTimeClientPresenceChanged(object sender, InstagramApiSharp.API.RealTime.Handlers.PresenceEventEventArgs e)
    {
        // presence changed...Online status of someone changed
        if (!string.IsNullOrEmpty(e?.UserId))
        {
            var userId = long.Parse(e.UserId);
            // get username>
            var user = await InstaApi.UserProcessor.GetUserInfoByIdAsync(userId).ConfigureAwait(false);
            Console.WriteLine($"@{user.Value.UserName} {(e.IsActive ? "is" : "is not")} online");
        }
    }

    private static void RealTimeClientDirectItemChanged(object sender, List<InstaDirectInboxItem> e)
    {
        string GetUserOrUserId(string user, long userId) =>
            !string.IsNullOrEmpty(user) ? user : userId.ToString();

        // Direct item changed means that a specific thread sends a specific update to our side.
        // this means maybe they deleted a message or sends a new one to our side
        if (e?.Count > 0)
        {
            var start = "direct_v2/threads/";
            var threadItems = e
                .Where(x => x.RealtimePath?.Contains(start) ?? false)// is it a thread or not?!
                .Select(x => x).ToList();

            foreach (var item in threadItems)
            {
                var userId = item.UserId; // sender
                var threadId = item.RealtimePath.Substring(item.RealtimePath.IndexOf(start) + start.Length);
                threadId = threadId.Substring(0, threadId.IndexOf("/"));
                var findThread = Threads.FirstOrDefault(x => x.ThreadId == item.RealtimePath);
                var findUser = findThread?.Users?.FirstOrDefault(x => x.Pk == userId);
                var senderUser = GetUserOrUserId(findUser?.UserName, userId);

                if (item.RealtimePath?.Contains("/items/") ?? false && !item.HideInThread)// new message or replace!
                {

                    var itemId = item.RealtimePath.Substring(item.RealtimePath.IndexOf("/items/") + "/items/".Length);
                    if (itemId.Contains("/"))
                        itemId = itemId.Substring(0, itemId.IndexOf("/"));

                    if (item.RealtimeOp == "remove")
                        Console.WriteLine($"{senderUser} has deleted one his message with item id: {itemId}.");
                    else
                        Console.WriteLine($"{senderUser} sent you a message with this type: {item.ItemType} => {item.Text}.");
                }
                else if (item.RealtimePath?.Contains("/participants/") ?? false) // your messages has seen by the user
                {
                    if (item.RealtimePath?.Contains("/has_seen") ?? false)
                    {
                        if (item.RealtimeOp == "replace")
                        {
                            Console.WriteLine($"{senderUser} has seen your messages.");
                        }
                    }
                }
            }
        }
    }
    static void LoadSession() =>
        InstaApi?.SessionHandler?.Load();

    static void SaveSession()
    {
        if (InstaApi == null)
            return;
        if (!InstaApi.IsUserAuthenticated)
            return;
        InstaApi?.SessionHandler?.Save();
    }
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * IRANIAN DEVELOPERS
 */

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using System.Linq;

namespace FollowSample
{
    class Program
    {
        internal static IInstaApi InstaApi;
        private const string StateFile = "state.bin";

#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var result = Task.Run(MainAsync).GetAwaiter().GetResult();
            if (result)
                return;
            Console.ReadKey();
        }
        public static async Task<bool> MainAsync()
        {
            try
            {
                Console.WriteLine("Starting FollowSample of InstagramApiSharp's project");

                // delay between requests
                var delay = RequestDelay.FromSeconds(3, 5);

                var userSession = GetUserSessionData("username", "password");

                InstaApi = BuildApi(userSession, delay);

                try
                {
                    if (File.Exists(StateFile))
                    {
                        Console.WriteLine("Loading state from file");
                        Load();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (!InstaApi.IsUserAuthenticated)
                {
                    Console.WriteLine($"Logging in as {userSession.UserName}");
                    delay.Disable();
                    var logInResult = await InstaApi.LoginAsync();
                    delay.Enable();
                    if (!logInResult.Succeeded)
                    {
                        Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                        return false;
                    }
                }

                Save();


                var followersFile = "Followers.txt";
                var searchedUsername = "instagram";
                int waitTime = 10; // minutes

                Console.WriteLine($"Gathering information of '{searchedUsername}'");

                var getUserResult = await InstaApi.UserProcessor.GetUserAsync(searchedUsername);
                if (!getUserResult.Succeeded)
                    Console.WriteLine("Couldn't get information...");
                else
                {
                    var userId = getUserResult.Value.Pk; // userId (pk)

                    // We should act as real instagram app, so we need to call these APIs too
                    await InstaApi.StoryProcessor.GetHighlightFeedsAsync(userId);
                    await InstaApi.StoryProcessor.GetUserStoryAndLivesAsync(userId);
                    await InstaApi.UserProcessor.GetUserMediaByIdAsync(userId, PaginationParameters.MaxPagesToLoad(1));

                    var followersPagination = PaginationParameters.MaxPagesToLoad(1);// pagination for followers
    
                    InstaUserShortList FollowersList = new InstaUserShortList();
                    Console.WriteLine($"Gathering '{searchedUsername}' followers");

                    Console.WriteLine($"Username\t\t\t\t\t\tUser Id\t\t\t\tIs Private Account?");
                    do
                    {
                        var followersResult = await InstaApi.UserProcessor.GetUserFollowersByIdAsync(userId, followersPagination);

                        if (followersResult.Succeeded && followersResult.Value?.Count > 0)
                        {
                            FollowersList.AddRange(FollowersList);

                            foreach (var follower in followersResult.Value)
                                Console.WriteLine($"{follower.UserName.ToLower()}\t\t\t\t\t\t{follower.Pk}\t\t\t\t{follower.IsPrivate}");

                            var usersForFriendship = followersResult.Value
                                .Select(x => x.Pk).ToArray();

                            await InstaApi.UserProcessor.GetFriendshipStatusesAsync(usersForFriendship);

                            await Task.Delay(7000);
                        }
                        else
                        {
                            if (followersResult.Info.ResponseType == ResponseType.ActionBlocked)
                            {
                                Console.WriteLine("Action blocked.");
                                break;
                            }
                            else if (followersResult.Info.ResponseType == ResponseType.RequestsLimit)
                            {
                                Console.WriteLine("Request limited. waiting ");
                                await Task.Delay(TimeSpan.FromMinutes(waitTime));
                                waitTime *= 2;
                            }
                        }
                    }
                    while (!string.IsNullOrEmpty(followersPagination.NextMaxId));
                    var users = FollowersList
                        .Select(x => $"{x.UserName}\t\t\t{x.Pk}")
                        .ToList();

                    var sb = new StringBuilder();
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine(string.Join(Environment.NewLine, users));
                    File.AppendAllText(followersFile, sb.ToString());
                }

                Console.WriteLine("Press Esc key to exit");
                var key = Console.ReadKey();
                return key.Key == ConsoleKey.Escape;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        private static IInstaApi BuildApi(UserSessionData userSessionData, IRequestDelay delay = null) =>
            InstaApiBuilder.CreateBuilder()
                    .SetUser(userSessionData)
                    .UseLogger(new DebugLogger(LogLevel.None)) // use logger for requests and debug messages
                    .SetRequestDelay(delay)
                    .SetSessionHandler(new FileSessionHandler { FilePath = StateFile })
                    .Build();

        private static UserSessionData GetUserSessionData(string username = "", string pass = "") =>
            UserSessionData.ForUsername(username).WithPassword(pass);

        private static void Load() =>
            InstaApi?.SessionHandler?.Load();

        private static void Save() =>
            InstaApi?.SessionHandler?.Save();
    }
}

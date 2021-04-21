/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * IRANIAN DEVELOPERS
 */

using System;
using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;

namespace MediaSample
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
                Console.WriteLine("Starting demo of InstagramApiSharp project");

                // delay between requests
                var delay = RequestDelay.FromSeconds(1, 2);

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
                    // login
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


                var postUri = new Uri("https://www.instagram.com/p/CN5czMgMm_f/?igshid=ssfmx6dlhxgx"); // for getting it's post's likers

                Console.WriteLine($"Getting {postUri} media id");

                var mediaIdResult = await InstaApi.MediaProcessor.GetMediaIdFromUrlAsync(postUri);

                if (mediaIdResult.Succeeded)
                {
                    var mediaId = mediaIdResult.Value;
                    Console.WriteLine($"Gathering media likers of {postUri}");

                    var mediaLikers = await InstaApi.MediaProcessor.GetMediaLikersAsync(mediaId);

                    if (!mediaLikers.Succeeded)
                        Console.WriteLine("Couldn't get any media liker");
                    else
                    {
                        if (mediaLikers.Value?.Count > 0)
                        {
                            Console.WriteLine($"'{mediaLikers.Value.Count}' accounts gathered from media likers:");

                            Console.WriteLine($"Username\t\t\t\t\t\tUser Id\t\t\t\tIs Private Account?");
                            foreach (var liker in mediaLikers.Value)
                                Console.WriteLine($"{liker.UserName.ToLower()}\t\t\t\t\t\t{liker.Pk}\t\t\t\t{liker.IsPrivate}");
                        }
                        else
                            Console.WriteLine($"No one liked this post {postUri}");
                    }
                }
                else
                    Console.WriteLine("Couldn't load media id");

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;

namespace NET5Example
{
    class Program
    {
        private static IInstaApi InstaApi;
        const string StateFile = "state.bin";

        static void Main()
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
                Console.WriteLine("Starting demo of InstagramApiSharp project in .NET 5 (or NETCore)");
                var userSession = new UserSessionData
                {
                    UserName = "username",
                    Password = "password"
                };
                var delay = RequestDelay.FromSeconds(1, 1);
                // create new InstaApi instance using Builder
                InstaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.All)) // use logger for requests and debug messages
                    .SetSessionHandler(new FileSessionHandler { FilePath = StateFile })
                    .SetRequestDelay(delay)
                    .Build();

                LoadSession();

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
                    SaveSession();
                }
                Console.WriteLine("Done. Press esc key to exit...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        static void LoadSession() =>
            InstaApi?.SessionHandler?.Load();

        static void SaveSession()
        {
            if (InstaApi == null)
                return;
            if (!InstaApi.IsUserAuthenticated)
                return;
            InstaApi.SessionHandler?.Save();
        }
    }
}

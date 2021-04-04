/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Logger;
using System;
using System.Threading.Tasks;

namespace AccountRegistrationWithEmailExample
{
    class Program
    {
        private static IInstaApi InstaApi;

#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Task.Run(MainAsync).GetAwaiter().GetResult();
            Console.ReadKey();
        }

        static async Task MainAsync()
        {
            var username = "username";
            var password = "password";
            var email = "ramtinjokar@outlook.com";
            var firstName = "my name"; // optional, but don't pass null, put string.Empty or ""


            var userSession = new UserSessionData
            {
                UserName = username,
                Password = password
            };

            // we need to put a delay between our requests to manipulate instagram!
            var delay = RequestDelay.FromSeconds(2, 4);
            InstaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.All))
                .SetRequestDelay(delay)
                // session handler, set a file path to save/load your state/session data
                .SetSessionHandler(new FileSessionHandler { FilePath = username + ".state" })
                .Build();

            
            // no need to do this but I just wanted to show you
            InstaApi.RegistrationService.Birthday = InstaApi.RegistrationService.GenerateRandomBirthday();

            // all functions related to account registration is available in the InstaApi.RegistrationService

            // lower all cases for username and email
            email = email.ToLower();
            username = username.ToLower();

            await InstaApi.RegistrationService.FirstLauncherSyncAsync();
            await InstaApi.RegistrationService.FirstLauncherSyncAsync();
            await InstaApi.RegistrationService.FirstQeSyncAsync();

            // check email address
            var checkEmailResult = await InstaApi.RegistrationService.CheckEmailAsync(email);
            // if email address is available to create
            if (checkEmailResult.Succeeded && (checkEmailResult.Value?.Available ?? false))
            {
                await Delay(3.5);
                // no information about this one
                var signupConsent = await InstaApi.RegistrationService.GetSignupConsentConfigAsync();
                await Delay(1.5);
                // send verification code to your email
                await InstaApi.RegistrationService.SendRegistrationVerifyEmailAsync(email);

                // check registration code that instagram sent it to your email:
                var verificationCode = "";
                await Delay(3.5);
                var checkRegistrationConfirmationResult = await InstaApi.RegistrationService
                    .CheckRegistrationConfirmationCodeAsync(email, verificationCode);

                if (checkRegistrationConfirmationResult.Succeeded)
                {

                    await InstaApi.RegistrationService.GetSiFetchHeadersAsync();

                    await Delay(1.5);
                    if (firstName?.Length > 0)
                    {
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName.Substring(0, 1), email);
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName.Substring(0, 3), email);

                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName, email);
                    }
                    else
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync("", email);

                    await Delay(3.5);

                    if (signupConsent.Value?.AgeRequired ?? false)
                        await InstaApi.RegistrationService.CheckAgeEligibilityAsync();

                    await Delay(3.5);
                    // onboarding steps is prefetch
                    await InstaApi.RegistrationService.GetOnboardingStepsAsync(InstaOnboardingProgressState.Prefetch);

                    // call it twice !
                    await InstaApi.RegistrationService.NewUserFlowBeginsConsentAsync();
                    await InstaApi.RegistrationService.NewUserFlowBeginsConsentAsync();

                    await Delay(3.5);
                    // check username
                    var checkUsernameResult = await InstaApi.RegistrationService.CheckUsernameAsync(username);
                    // if username is available to create
                    if (checkUsernameResult.Succeeded && (checkUsernameResult.Value?.Available ?? false))
                    {
                        // we can pass checkRegistrationConfirmationResult.Value.SignupCode to CreateNewAccountAsync function
                        var signupCode = checkRegistrationConfirmationResult.Value.SignupCode;
                        var createAccount =
                            await InstaApi.RegistrationService.CreateNewAccountWithEmailAsync(email, username, password, firstName, signupCode);
                        if (!createAccount.Succeeded)
                        {
                            Console.WriteLine(createAccount.Info.Message);
                            return;
                        }
                        //await InstaApi.RegistrationService.CreateNewAccountWithEmailAsync(email, username, password, firstName, signupCode);

                        // note: you can ignore passing `signupCode` to CreateNewAccountAsync, because library will handle it!
                        // await InstaApi.RegistrationService.CreateNewAccountWithEmailAsync(email, username, password, firstName);


                        // no information about this one
                        await InstaApi.RegistrationService.GetMultipleAccountsFamilyAsync();
                        // no information about this one
                        await InstaApi.RegistrationService.GetZrTokenResultAsync();
                        // no information about this one
                        await InstaApi.RegistrationService.LauncherSyncAsync();
                        // no information about this one
                        await InstaApi.RegistrationService.QeSyncAsync();
                        // no information about this one
                        await InstaApi.RegistrationService.NuxNewAccountSeenAsync();
                        // onboarding steps is started
                        await InstaApi.RegistrationService.GetOnboardingStepsAsync(InstaOnboardingProgressState.Start);
                        // no information about this one
                        await InstaApi.RegistrationService.GetContactPointPrefillAsync();
                        // onboarding steps is finished
                        await InstaApi.RegistrationService.GetOnboardingStepsAsync(InstaOnboardingProgressState.Finish);


                        // now we can save our session to local:
                        InstaApi.SessionHandler?.Save();

                        // extra work
                        await ExtraWorkAfterAccountCreated();
                    }
                }
            }
        }

        static async Task ExtraWorkAfterAccountCreated()
        {
            // every login/creating new account, instagram sends these requests
            // no need to put a delay between these requests, so we remove the delay
            InstaApi.SetRequestDelay(null);

            await InstaApi.StoryProcessor.GetStoryFeedWithPostMethodAsync();
            await InstaApi.FeedProcessor.GetUserTimelineFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await InstaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
            await InstaApi.GetBanyanSuggestionsAsync();
            await InstaApi.UserProcessor.GetCurrentUserAsync();
        }
        static async Task Delay(double seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }
    }
}

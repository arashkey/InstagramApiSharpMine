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

namespace AccountRegistrationWithPhoneExample
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

        /*
         *                                          Read here
         *                                          
         * Instagram uses lots APIs to create 1 account, I've added all of them to a new class called RegistrationService
         *   Which is in IInstaApi.
         * 
         * Warning: This steps is should be just like below!
         * 
         */
        static async Task MainAsync()
        {
            var username = "";
            var password = "";
            var phoneNumber = "+989123456789";
            var phoneNumberWithoutCountryCode = "9123456789";
            var firstName = "";// optional, but don't pass null, put string.Empty or ""


            var userSession = new UserSessionData
            {
                UserName = username,
                Password = password
            };


            async Task Delay(double de) =>
                await Task.Delay(TimeSpan.FromSeconds(de));


            var delay = RequestDelay.FromSeconds(2, 4);

            InstaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.All))
                .SetRequestDelay(delay)
                // session handler, set a file path to save/load your state/session data
                .SetSessionHandler(new FileSessionHandler { FilePath = username + ".state" })
                .Build();

            //// if you want to edit languages and startup country code or timezone, you can use these>
            //InstaApi.StartupCountryCode = 44;
            //InstaApi.StartupCountry = "GB"; // You can try unknown as well, for example if your phone's GPS wasn't turned on, they pass unknown to this header
            //InstaApi.DeviceLocale = "en_GB";
            //InstaApi.MappedLocale = InstaApi.AppLocale = "en_GB";// these two is always is same
            //InstaApi.AcceptLanguage = "en-GB, en-US"; // it seems en-US is set automatically
            //InstaApi.TimezoneOffset = 3600; // set timezone offset

            // no need to do this but I just wanted to show you
            InstaApi.RegistrationService.Birthday = InstaApi.RegistrationService.GenerateRandomBirthday();

            // all functions related to account registration is available in the InstaApi.RegistrationService
            await InstaApi.RegistrationService.FirstLauncherSyncAsync();
            await InstaApi.RegistrationService.FirstLauncherSyncAsync();
            await InstaApi.RegistrationService.FirstQeSyncAsync();

            await InstaApi.RegistrationService.GetFirstContactPointPrefillAsync();

            // check phone number
            var checkPhoneResult = await InstaApi.RegistrationService.CheckPhoneNumberAsync(phoneNumberWithoutCountryCode);
            // if phone number is available to create
            if (checkPhoneResult.Succeeded)
            {
                await Delay(1.5);// delay
                // no information about this one
                var signupConsent = await InstaApi.RegistrationService.GetSignupConsentConfigAsync();
                await Delay(1.5);
                // send verification code to your phone number
                await InstaApi.RegistrationService.SendSignUpSmsCodeAsync(phoneNumber);

                // check registration code that instagram sent it to your phone number:
                var verificationCode = "";
                await Delay(3.5);
                var checkRegistrationConfirmationResult = await InstaApi.RegistrationService
                    .VerifySignUpSmsCodeAsync(phoneNumber, verificationCode);

                if (checkRegistrationConfirmationResult.Succeeded)
                {

                    await InstaApi.RegistrationService.GetSiFetchHeadersAsync();

                    await Delay(1.5);
                    // calling GetUsernameSuggestionsAsync 5 times!!!!
                    if (firstName?.Length > 0)
                    {
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync("");
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName.Substring(0, 1));
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName.Substring(0, 3));
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName.Substring(0, firstName.Length - 2)
                            .Replace(" ", "+"));

                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync(firstName);
                    }
                    else
                        await InstaApi.RegistrationService.GetUsernameSuggestionsAsync("");

                    await Delay(3.5);
                    if (signupConsent.Value?.AgeRequired ?? false)
                        await InstaApi.RegistrationService.CheckAgeEligibilityAsync();

                    await Delay(3.5);
                    // onboarding steps is prefetch
                    await InstaApi.RegistrationService
                        .GetOnboardingStepsAsync(InstaOnboardingProgressState.Prefetch, InstaRegistrationMethod.Phone);

                    // call it twice !
                    await InstaApi.RegistrationService.NewUserFlowBeginsConsentAsync();
                    await InstaApi.RegistrationService.NewUserFlowBeginsConsentAsync();

                    await Delay(3.5);
                    // check username
                    var checkUsernameResult = await InstaApi.RegistrationService.CheckUsernameAsync(username);
                    // if username is available to create
                    if (checkUsernameResult.Succeeded && (checkUsernameResult.Value?.Available ?? false))
                    {
                        var createAccount = await InstaApi.RegistrationService
                            .CreateNewAccountWithPhoneNumberAsync(phoneNumber, username, password, firstName, verificationCode);

                        if (!createAccount.Succeeded)
                        {
                            Console.WriteLine(createAccount.Info.Message);
                            return;
                        }

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
                        await InstaApi.RegistrationService
                            .GetOnboardingStepsAsync(InstaOnboardingProgressState.Start, InstaRegistrationMethod.Phone);
                        // no information about this one
                        await InstaApi.RegistrationService.GetContactPointPrefillAsync();
                        // onboarding steps is finished
                        await InstaApi.RegistrationService
                            .GetOnboardingStepsAsync(InstaOnboardingProgressState.Finish, InstaRegistrationMethod.Phone);

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
    }
}

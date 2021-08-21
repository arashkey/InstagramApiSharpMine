using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
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
                const string username = "arasdfaslaki";
                const string password = "f[sz2-)6=bJ4Z}gV";

                var userSession = new UserSessionData
                {//kivixo6956@1uscare.com
                    UserName = username,
                    Password = password
                };
                var delay = RequestDelay.FromSeconds(1, 1);
                // create new InstaApi instance using Builder
                InstaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.Exceptions)) // use logger for requests and debug messages
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
                        Console.WriteLine($"logInResult as {logInResult.Value}");
                        if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                        {
                            var challenge = await InstaApi.GetChallengeRequireVerifyMethodAsync();
                            if (challenge.Succeeded)
                            {
                                if (challenge.Value.SubmitPhoneRequired)
                                {
                                    //SubmitPhoneChallengeGroup.Visible = true;
                                    //Size = ChallengeSize;
                                }
                                else
                                {
                                    if (challenge.Value.StepData != null)
                                    {
                                        if (!string.IsNullOrEmpty(challenge.Value.StepData.PhoneNumber))
                                        {
                                            //RadioVerifyWithPhoneNumber.Checked = false;
                                            //RadioVerifyWithPhoneNumber.Visible = true;
                                            //RadioVerifyWithPhoneNumber.Text = challenge.Value.StepData.PhoneNumber;
                                            Console.WriteLine(challenge.Value.StepData.PhoneNumber);
                                        }
                                        if (!string.IsNullOrEmpty(challenge.Value.StepData.Email))
                                        {
                                            //RadioVerifyWithEmail.Checked = false;
                                            //RadioVerifyWithEmail.Visible = true;
                                            //RadioVerifyWithEmail.Text = challenge.Value.StepData.Email;
                                            Console.WriteLine(challenge.Value.StepData.Email);

                                        }

                                        //SelectMethodGroupBox.Visible = true;
                                        //Size = ChallengeSize;
                                    }
                                }
                            }
                            else
                                 Console.WriteLine( challenge.Info.Message);
                        }
                        //else if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
                        //{
                        //    // lets check for pending trusted notification first
                        //    if (InstaApi.TwoFactorLoginInfo?.PendingTrustedNotification ?? false)
                        //    {
                        //        var random = new Random();
                        //        ///////////// IF YOU WANT TO SUPPORT NOTIFICATION LOGIN DO THIS> /////////////
                        //        if (!FreshLoginFromTwoFactor) // false
                        //        {
                        //            int tried = 0;
                        //            while (tried <= 3)
                        //            {
                        //                var trustedNotification = await InstaApi.Check2FATrustedNotificationAsync();
                        //                if (trustedNotification.Succeeded)
                        //                {
                        //                    var reviewStatus = trustedNotification.Value.ReviewStatus;

                        //                    switch (reviewStatus)
                        //                    {
                        //                        case Insta2FANotificationReviewStatus.Unchanged:
                        //                            // lets wait 3 times with a different delays
                        //                            if (tried <= 3)
                        //                            {
                        //                                tried++;
                        //                                await Task.Delay(random.Next(2, 6) * 1000).ConfigureAwait(false);

                        //                                continue;
                        //                            }
                        //                            return;

                        //                        case Insta2FANotificationReviewStatus.Approved:
                        //                            {
                        //                                // if user approved login notification, we can simply login, without any further hard work
                        //                                // >>>>>>>>>>>>>> DON'T CHANGE "code" AND "twoFactorOption" VALUES <<<<<<<<<<<<<<

                        //                                var code = ""; // we have to pass "" as a verification code,
                        //                                               // since Instagram doesn't need code in approved case

                        //                                var trustedDevice = false;  // set true if you want to add this to the trusted device list

                        //                                var twoFactorOption = InstaTwoFactorVerifyOptions.Notification;// we must use Notification

                        //                                var twoFactorLogin = await InstaApi
                        //                                    .TwoFactorLoginAsync(code,
                        //                                    trustedDevice,
                        //                                    twoFactorOption);

                        //                                if (twoFactorLogin.Succeeded)
                        //                                {
                        //                                    // connected
                        //                                    // save session
                        //                                    SaveSession();
                        //                                    Size = ChallengeSize;
                        //                                    TwoFactorGroupBox.Visible = false;
                        //                                    GetFeedButton.Visible = true;
                        //                                    Text = $"{AppName} Connected";
                        //                                    Size = NormalSize;
                        //                                }
                        //                                else
                        //                                {
                        //                                    // this shouldn't happen, so I don't know what to do in this situation
                        //                                     Console.WriteLine((twoFactorLogin.Info.Message);
                        //                                }
                        //                            }
                        //                            return;
                        //                        case Insta2FANotificationReviewStatus.Denied:
                        //                            // if user, denied it, we need a fresh login 
                        //                            FreshLoginFromTwoFactor = true;
                        //                            // we ignore notification login, for this situation, although we can use notification again!
                        //                            LoginButton_Click(null, null);
                        //                            return;
                        //                    }
                        //                }
                        //                else
                        //                    break;
                        //            }
                        //            // if none of above codes didn't work, lets try SMS code>
                        //            await InstaApi.SendTwoFactorLoginSMSAsync();
                        //            await InstaApi.Check2FATrustedNotificationAsync();
                        //        }
                        //        else ///////////// IF YOU WANT TO SEND SMS CODE, USE BELOW CODE /////////////
                        //        {
                        //            // Let us check the required APIs by calling these functions

                        //            // >>>>>>>>>>>> NEW <<<<<<<<<<<
                        //            // I checked Instagram again, it seems they wait 9 to 12 seconds before checking /two_factor/check_trusted_notification_status/ API
                        //            // So lets wait between this range
                        //            await DelayAndCheck(random.Next(9, 12));

                        //            await DelayAndCheck(4); // lets wait 4 seconds and try again

                        //            await DelayAndCheck(5); // 5 seconds delay

                        //            await DelayAndCheck(5); // 5 seconds delay

                        //            await Task.Delay(4000); // 4 seconds delay before sending SendTwoFactorLoginSMSAsync

                        //            // now we are allowed to call this function to send it via SMS
                        //            await InstaApi.SendTwoFactorLoginSMSAsync();

                        //            // lets wait 1 second for one last time
                        //            // we have to send trusted device API one more time, after we call SendTwoFactorLoginSMSAsync
                        //            await DelayAndCheck(1);

                        //            async Task DelayAndCheck(int delayInSeconds)
                        //            {
                        //                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
                        //                await InstaApi.Check2FATrustedNotificationAsync().ConfigureAwait(false);
                        //            }
                        //        }
                        //    }
                        //    //TwoFactorGroupBox.Visible = true;
                        //    //Size = ChallengeSize;
                        //}
                        else
                            
                        Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                        return false;
                    }
                    SaveSession();
                }
                //var mediaImage = new InstaImageUpload
                //{
                //    // leave zero, if you don't know how height and width is it.
                //    Height = 320,
                //    Width = 320,
                //    Uri = @"C:\Users\avajang-pc\Pictures\109955012_227254868384561_6263772963076776476_n.jpg"
                //};
                //// Add user tag (tag people)
                //mediaImage.UserTags.Add(new InstaUserTagUpload
                //{
                //    Username = "arashkey2020",
                //    X = 0.5,
                //    Y = 0.5
                //});
                //var result = await InstaApi.MediaProcessor.UploadPhotoAsync(mediaImage, "Picture");
                //Console.WriteLine(result.Succeeded
                //    ? $"Media created: {result.Value.Pk}, {result.Value.Caption.Text}"
                //    : $"Unable to upload photo: {result.Info.Message}");

                var video = new InstaVideoUpload
                {
                    // leave zero, if you don't know how height and width is it.
                    Video = new InstaVideo(@"C:\Users\avajang-pc\Videos\output.mp4", 600, 600),
                    VideoThumbnail = new InstaImage(@"C:\Users\avajang-pc\Pictures\109955012_227254868384561_6263772963076776476_n.jpg", 320, 320)
                };
                // Add user tag (tag people)
                //video.UserTags.Add(new InstaUserTagVideoUpload
                //{
                //    Username = "arashkey2020"
                //});
                var result = await InstaApi.MediaProcessor.UploadVideoAsync(video, "sample yuga");
                Console.WriteLine(result.Succeeded
                    ? $"Media created: {result.Value.Pk}, {result.Value.Caption}"
                    : $"Unable to upload video: {result.Info.Message}");
                /**/
           //     var images = new InstaImageUpload[]
           //{
           //     new InstaImageUpload
           //     {
           //         // leave zero, if you don't know how height and width is it.
           //         Height = 320,
           //         Width = 320,
           //         Uri = @"C:\Users\avajang-pc\Pictures\109955012_227254868384561_6263772963076776476_n.jpg",
           //         // add user tags to your images
           //         UserTags = new List<InstaUserTagUpload>
           //         {
           //             new InstaUserTagUpload
           //             {
           //                 Username = "arashkey2020",
           //                 X = 0.5,
           //                 Y = 0.5
           //             }
           //         }
           //     },
           //     new InstaImageUpload
           //     {
           //         // leave zero, if you don't know how height and width is it.
           //         Height = 320,
           //         Width = 320,
           //         Uri =  @"C:\Users\avajang-pc\Pictures\109955012_227254868384561_6263772963076776476_n.jpg"
           //     }
           //};

           //     var videos = new InstaVideoUpload[]
           //     {
           //     new InstaVideoUpload
           //     {
           //          // leave zero, if you don't know how height and width is it.
           //         Video = new InstaVideo(@"C:\Users\avajang-pc\Videos\output.mp4", 600, 600),
           //         VideoThumbnail = new InstaImage( @"C:\Users\avajang-pc\Pictures\109955012_227254868384561_6263772963076776476_n.jpg", 320, 320),
           //         // Add user tag (tag people)
           //         UserTags = new List<InstaUserTagVideoUpload>
           //         {
           //             new InstaUserTagVideoUpload
           //             {
           //                 Username = "arashkey2020"
           //             }
           //         }
           //     },
           //     new InstaVideoUpload
           //     {
           //          // leave zero, if you don't know how height and width is it.
           //         Video = new InstaVideo(@"C:\Users\avajang-pc\Videos\output.mp4", 600, 600),
           //         VideoThumbnail = new InstaImage( @"C:\Users\avajang-pc\Pictures\109955012_227254868384561_6263772963076776476_n.jpg", 320, 320)
           //     }
           //     };
           //     var result = await InstaApi.MediaProcessor.UploadAlbumAsync(images,
           //         videos,
           //         "Hey, this my first album upload via InstagramApiSharp library.");

           //     // Above result will be something like this: IMAGE1, IMAGE2, VIDEO1, VIDEO2
           //     Console.WriteLine(result.Succeeded
           //         ? $"Media created: {result.Value.Pk}, {result.Value.Caption}"
           //         : $"Unable to upload album: {result.Info.Message}");

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

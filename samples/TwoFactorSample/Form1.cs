/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Logger;
/////////////////////////////////////////////////////////////////////
////////////////////// IMPORTANT NOTE ///////////////////////////////
// Please check wiki pages for more information:
// https://github.com/ramtinak/InstagramApiSharp/wiki
////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////
namespace TwoFactorSample
{
    public partial class Form1 : Form
    {
        const string AppName = "Two Factor";
        const string StateFile = "state.bin";
        private static IInstaApi InstaApi;
        //307, 280
        readonly Size NormalSize = new Size(307, 150);
        readonly Size TwoFactorSize = new Size(307, 280);

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            Size = NormalSize;
            var userSession = new UserSessionData
            {
                UserName = txtUsername.Text,
                Password = txtPassword.Text
            };

            InstaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.All))
                .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                .Build();
            Text = $"{AppName} Connecting";
            LoadSession();
            var freshLoginFromTwoFactor = false;
        RetryFromTwoFactor:

            if (!InstaApi.IsUserAuthenticated)
            {
                var logInResult = await InstaApi.LoginAsync();
                Debug.WriteLine(logInResult.Value);
                if (logInResult.Succeeded)
                {
                    Text = $"{AppName} Connected";
                    // Save session 
                    SaveSession();
                }
                else
                {
                    // two factor is required
                    if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        // lets check for pending trusted notification first
                        if (InstaApi.TwoFactorLoginInfo?.PendingTrustedNotification ?? false)
                        {

                            ///////////// IF YOU WANT TO SUPPORT NOTIFICATION LOGIN DO THIS> /////////////
                            if (!freshLoginFromTwoFactor) // false
                            {
                                var random = new Random();
                                int tried = 0;
                            RetryLabel:
                                var trustedNotification = await InstaApi.Check2FATrustedNotificationAsync();
                                if (trustedNotification.Succeeded)
                                {
                                    var reviewStatus = trustedNotification.Value.ReviewStatus;

                                    switch (reviewStatus)
                                    {
                                        case Insta2FANotificationReviewStatus.Unchanged:
                                            // lets wait 3 times with a different delays
                                            if (tried <= 3)
                                            {
                                                tried++;
                                                await Task.Delay(random.Next(2, 6) * 1000).ConfigureAwait(false);

                                                goto RetryLabel;
                                            }
                                            break;

                                        case Insta2FANotificationReviewStatus.Approved:
                                            {
                                                // if user approved login notification, we can simply login, without any further hard work
                                                // >>>>>>>>>>>>>> DON'T "code" AND "twoFactorOption" VALUES <<<<<<<<<<<<<<

                                                var code = ""; // we have to pass "" as a verification code,
                                                               // since Instagram doesn't need code in approved case

                                                var trustedDevice = false;  // set true if you want to add this to the trusted device list

                                                var twoFactorOption = InstaTwoFactorVerifyOptions.Notification;// we must use Notification

                                                var twoFactorLogin = await InstaApi
                                                    .TwoFactorLoginAsync(code,
                                                    trustedDevice,
                                                    twoFactorOption);

                                                if (twoFactorLogin.Succeeded)
                                                {
                                                    // connected
                                                    // save session
                                                    SaveSession();
                                                    Text = $"{AppName} Connected";
                                                    Size = NormalSize;
                                                }
                                                else
                                                {
                                                    // this shouldn't happen, so I don't know what to do in this situation
                                                    MessageBox.Show(twoFactorLogin.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }
                                            return;
                                        case Insta2FANotificationReviewStatus.Denied:
                                            // if user, denied it, we need a fresh login 
                                            freshLoginFromTwoFactor = true;
                                            // we ignore notification login, for this situation, although we can use notification again!
                                            goto RetryFromTwoFactor;
                                    }
                                }

                                // if none of above codes didn't work, let try SMS code>
                                await InstaApi.SendTwoFactorLoginSMSAsync();
                                await InstaApi.Check2FATrustedNotificationAsync();
                            }
                            else ///////////// IF YOU WANT TO SEND SMS CODE, USE BELOW CODE /////////////
                            {
                                // if we have one, let us check the required API by calling this function
                                await InstaApi.Check2FATrustedNotificationAsync();
                                await Task.Delay(2000);// lets wait 2 seconds and try again [ to act as instagram way! ]
                                await InstaApi.Check2FATrustedNotificationAsync();
                                // why 3 times? Insta checking this value repeatedly,
                                // I tracked it in one login to 19 requests and in another login it was 3 or less and more

                                await Task.Delay(2000);// lets wait 2 seconds more to manipulate instagram

                                // now we are allowed to call this function to send it via SMS
                                await InstaApi.SendTwoFactorLoginSMSAsync();

                                // we have to send trusted device API one more time, after we call SendTwoFactorLoginSMSAsync
                                await InstaApi.Check2FATrustedNotificationAsync();
                            }
                        }

                        // open a box so user can send two factor code
                        Size = TwoFactorSize;
                    }
                }
            }
            else
            {
                Text = $"{AppName} Connected";
            }
        }

        private async void TwoFactorButton_Click(object sender, EventArgs e)
        {
            if (InstaApi == null)
                return;
            if (string.IsNullOrEmpty(txtTwoFactorCode.Text))
            {
                MessageBox.Show("Please type your two factor code and then press Auth button.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // send two factor code
            var twoFactorLogin = await InstaApi.TwoFactorLoginAsync(txtTwoFactorCode.Text);
            if (twoFactorLogin.Succeeded)
            {
                // connected
                // save session
                SaveSession();
                Text = $"{AppName} Connected";
                Size = NormalSize;
            }
            else
            {
                MessageBox.Show(twoFactorLogin.Info.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadSession()
        {
            try
            {
                if (File.Exists(StateFile))
                {
                    Debug.WriteLine("Loading state from file");
                    using (var fs = File.OpenRead(StateFile))
                    {
                        InstaApi.LoadStateDataFromStream(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        void SaveSession()
        {
            if (InstaApi == null)
                return;
            var state = InstaApi.GetStateDataAsStream();
            using (var fileStream = File.Create(StateFile))
            {
                state.Seek(0, SeekOrigin.Begin);
                state.CopyTo(fileStream);
            }
        }
    }
}

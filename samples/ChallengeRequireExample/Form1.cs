/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * Latest update: 20 JULY 2021
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using InstagramApiSharp.Classes;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Logger;
using System.Text.RegularExpressions;
using InstagramApiSharp.Classes.Models;
using System.Net;
using System.Net.Sockets;
using InstagramApiSharp;
using InstagramApiSharp.Classes.SessionHandlers;
using System.Net.Http;
using InstagramApiSharp.Enums;
/////////////////////////////////////////////////////////////////////
////////////////////// IMPORTANT NOTE ///////////////////////////////
// Please check wiki pages for more information:
// https://github.com/ramtinak/InstagramApiSharp/wiki
////////////////////// IMPORTANT NOTE ///////////////////////////////
/////////////////////////////////////////////////////////////////////
namespace ChallengeRequireExample
{
    public partial class Form1 : Form
    {
        // There are two different type of challenge is exists!
        //  - 1. You receive challenge while you already logged in:
        //       "This is me" or "This is not me" option!
        //       If some suspecious login happend, this will promp up, and you should accept it to get rid of it
        //
        //       Use Task<IResult<InstaLoggedInChallengeDataInfo>> GetLoggedInChallengeDataInfoAsync() to get information like coordinate of
        //       login request and more data info
        //
        //       Use Task<IResult<bool>> AcceptChallengeAsync() to accept that you are the ONE that requests for login!




        //  - 2. You receive challenge while you calling LoginAsync

        // Note: new challenge require functions is very easy to use.
        // there are 5 functions I've added to IInstaApi for challenge require (checkpoint_endpoint)

        // here:
        // 1. Task<IResult<ChallengeRequireVerifyMethod>> GetChallengeRequireVerifyMethodAsync();
        // If your login needs challenge, first you should call this function.
        // Note: if you call this and SubmitPhoneRequired was true, you should sumbit phone number
        // with this function:
        // Task<IResult<ChallengeRequireSMSVerify>> SubmitPhoneNumberForChallengeRequireAsync();


        // 2. Task<IResult<ChallengeRequireSMSVerify>> RequestVerifyCodeToSMSForChallengeRequireAsync();
        // This function will send you verification code via SMS.


        // 3. Task<IResult<ChallengeRequireEmailVerify>> RequestVerifyCodeToEmailForChallengeRequireAsync();
        // This function will send you verification code via Email.


        // 4. Task<IResult<ChallengeRequireVerifyMethod>> ResetChallengeRequireVerifyMethodAsync();
        // Reset challenge require.
        // Example: if your account has phone number and email, and you request for email(or phone number)
        // and now you want to change it to another one, you should first call this function,
        // then you have to call GetChallengeRequireVerifyMethodAsync and after that you can change your method!!!


        // 5. Task<IResult<ChallengeRequireVerifyCode>> VerifyCodeForChallengeRequireAsync(string verifyCode);
        // Verify sms or email verification code for login.

        const string AppName = "Challenge Required";
        const string StateFile = "state-rmtjokar1373.bin";
        readonly Size NormalSize = new Size(432, 164);
        readonly Size ChallengeSize = new Size(432, 604);
        private static IInstaApi InstaApi;
        private bool FreshLoginFromTwoFactor = false;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;// lets disable this, note that you should invoke the controls, this isn't right
            // but since it's a example I avoided it 
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Size = NormalSize;
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            Size = NormalSize;
            var userSession = new UserSessionData
            {
                UserName = txtUser.Text,
                Password = txtPass.Text
            };
            // Proxy part
            var proxy = new WebProxy()
            {
                Address = new Uri($"http://1.2.3.4.5:8080"), //i.e: http://1.2.3.4.5:8080
                //BypassProxyOnLocal = false,
                //UseDefaultCredentials = false,

                //// Credentials if needed
                //Credentials = new NetworkCredential(
                //    userName: "rmt",
                //    password: "0jokar")
            };

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
            };
            // NOTE FOR PROXY: WEBPROXY class only supports HTTP proxies (not HTTPS)
            // IF YOU NEED SOCKS OR HTTPS PROXIES, YOU NEED TO REFRENCE THESE PROJECT:
            // https://github.com/MihaZupan/HttpToSocks5Proxy [SUPPORTS SOCKS5 PROXIES]
            // https://github.com/Yozer/BetterHttpClient [SUPPORTS HTTPS/SOCKS PROXIES]
            // a quick example:
            // https://github.com/Yozer/BetterHttpClient/blob/1102325276fb2ee8b44cbc8d4974f85bbd63ba2f/UnitTestBetterHttpClient/UnitTestHttpClient.cs#L106-L152

            if (InstaApi == null)
            {
                InstaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.All))
                    .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                    // Session handler, set a file path to save/load your state/session data
                    .SetSessionHandler(new FileSessionHandler { FilePath = StateFile })

                    //// Setting up proxy if you needed
                    //.UseHttpClientHandler(httpClientHandler)
                    .Build();
            }

            //// if you want to edit languages and startup country code or timezone, you can use these>
            //InstaApi.StartupCountryCode = 44;
            //InstaApi.StartupCountry = "GB"; // You can try unknown as well, for example if your phone's GPS wasn't turned on, they pass unknown to this header
            //InstaApi.DeviceLocale = "en_GB";
            //InstaApi.MappedLocale = InstaApi.AppLocale = "en_GB";// these two is always is same
            //InstaApi.AcceptLanguage = "en-GB, en-US"; // it seems en-US is set automatically
            //InstaApi.TimezoneOffset = 3600; // set timezone offset

            Text = $"{AppName} Connecting";
            //Load session
            LoadSession();

            if (!InstaApi.IsUserAuthenticated)
            {

                // Send requests for login flows (contact prefill, read msisdn header, launcher sync and qe sync)
                // Note 1: You should call this function before you calling IInstaApi.LoginAsync(), if you want your account act like original instagram app.
                // Note 2: One call per one account! No need to call while you are loading a session
                await InstaApi.SendRequestsBeforeLoginAsync();


                var logInResult = await InstaApi.LoginAsync();
                Debug.WriteLine(logInResult.Value);
                if (logInResult.Succeeded)
                {
                    GetFeedButton.Visible = true;
                    Text = $"{AppName} Connected";
                    // Save session 
                    SaveSession();
                }
                else
                {
                    if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                    {
                        var challenge = await InstaApi.GetChallengeRequireVerifyMethodAsync();
                        if (challenge.Succeeded)
                        {
                            if (challenge.Value.SubmitPhoneRequired)
                            {
                                SubmitPhoneChallengeGroup.Visible = true;
                                Size = ChallengeSize;
                            }
                            else
                            {
                                if (challenge.Value.StepData != null)
                                {
                                    if (!string.IsNullOrEmpty(challenge.Value.StepData.PhoneNumber))
                                    {
                                        RadioVerifyWithPhoneNumber.Checked = false;
                                        RadioVerifyWithPhoneNumber.Visible = true;
                                        RadioVerifyWithPhoneNumber.Text = challenge.Value.StepData.PhoneNumber;
                                    }
                                    if (!string.IsNullOrEmpty(challenge.Value.StepData.Email))
                                    {
                                        RadioVerifyWithEmail.Checked = false;
                                        RadioVerifyWithEmail.Visible = true;
                                        RadioVerifyWithEmail.Text = challenge.Value.StepData.Email;
                                    }

                                    SelectMethodGroupBox.Visible = true;
                                    Size = ChallengeSize;
                                }
                            }
                        }
                        else
                            MessageBox.Show(challenge.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        // lets check for pending trusted notification first
                        if (InstaApi.TwoFactorLoginInfo?.PendingTrustedNotification ?? false)
                        {
                            var random = new Random();
                            ///////////// IF YOU WANT TO SUPPORT NOTIFICATION LOGIN DO THIS> /////////////
                            if (!FreshLoginFromTwoFactor) // false
                            {
                                int tried = 0;
                                while (tried <= 3)
                                {
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

                                                    continue;
                                                }
                                                return;

                                            case Insta2FANotificationReviewStatus.Approved:
                                                {
                                                    // if user approved login notification, we can simply login, without any further hard work
                                                    // >>>>>>>>>>>>>> DON'T CHANGE "code" AND "twoFactorOption" VALUES <<<<<<<<<<<<<<

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
                                                        Size = ChallengeSize;
                                                        TwoFactorGroupBox.Visible = false;
                                                        GetFeedButton.Visible = true;
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
                                                FreshLoginFromTwoFactor = true;
                                                // we ignore notification login, for this situation, although we can use notification again!
                                                LoginButton_Click(null, null);
                                                return;
                                        }
                                    }
                                    else
                                        break;
                                }
                                // if none of above codes didn't work, lets try SMS code>
                                await InstaApi.SendTwoFactorLoginSMSAsync();
                                await InstaApi.Check2FATrustedNotificationAsync();
                            }
                            else ///////////// IF YOU WANT TO SEND SMS CODE, USE BELOW CODE /////////////
                            {
                                // Let us check the required APIs by calling these functions

                                // >>>>>>>>>>>> NEW <<<<<<<<<<<
                                // I checked Instagram again, it seems they wait 9 to 12 seconds before checking /two_factor/check_trusted_notification_status/ API
                                // So lets wait between this range
                                await DelayAndCheck(random.Next(9, 12));

                                await DelayAndCheck(4); // lets wait 4 seconds and try again

                                await DelayAndCheck(5); // 5 seconds delay

                                await DelayAndCheck(5); // 5 seconds delay

                                await Task.Delay(4000); // 4 seconds delay before sending SendTwoFactorLoginSMSAsync

                                // now we are allowed to call this function to send it via SMS
                                await InstaApi.SendTwoFactorLoginSMSAsync();

                                // lets wait 1 second for one last time
                                // we have to send trusted device API one more time, after we call SendTwoFactorLoginSMSAsync
                                await DelayAndCheck(1);

                                async Task DelayAndCheck(int delayInSeconds)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
                                    await InstaApi.Check2FATrustedNotificationAsync().ConfigureAwait(false);
                                }
                            }
                        }
                        TwoFactorGroupBox.Visible = true;
                        Size = ChallengeSize;
                    }
                    else
                        MessageBox.Show(logInResult.Info.Message, logInResult.Info.ResponseType.ToString());
                }
            }
            else
            {
                Text = $"{AppName} Connected";
                GetFeedButton.Visible = true;
            }
        }

        private async void SubmitPhoneChallengeButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSubmitPhoneForChallenge.Text) ||
                     string.IsNullOrWhiteSpace(txtSubmitPhoneForChallenge.Text))
                {
                    MessageBox.Show("Please type a valid phone number(with country code).\r\ni.e: +989123456789", "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var phoneNumber = txtSubmitPhoneForChallenge.Text;
                if (!phoneNumber.StartsWith("+"))
                    phoneNumber = $"+{phoneNumber}";

                var submitPhone = await InstaApi.SubmitPhoneNumberForChallengeRequireAsync(phoneNumber);
                if (submitPhone.Succeeded)
                {
                    VerifyCodeGroupBox.Visible = true;
                    SubmitPhoneChallengeGroup.Visible = false;
                }
                else
                    MessageBox.Show(submitPhone.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "EX", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private async void SendCodeButton_Click(object sender, EventArgs e)
        {
            bool isEmail = false;
            if (RadioVerifyWithEmail.Checked)
                isEmail = true;
            //if (RadioVerifyWithPhoneNumber.Checked)
            //    isEmail = false;

            try
            {
                // Note: every request to this endpoint is limited to 60 seconds                 
                if (isEmail)
                {
                    // send verification code to email
                    var email = await InstaApi.RequestVerifyCodeToEmailForChallengeRequireAsync();
                    if (email.Succeeded)
                    {
                        LblForSmsEmail.Text = $"We sent verify code to this email:\n{email.Value.StepData.ContactPoint}";
                        VerifyCodeGroupBox.Visible = true;
                        SelectMethodGroupBox.Visible = false;
                    }
                    else
                        MessageBox.Show(email.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // send verification code to phone number
                    var phoneNumber = await InstaApi.RequestVerifyCodeToSMSForChallengeRequireAsync();
                    if (phoneNumber.Succeeded)
                    {
                        LblForSmsEmail.Text = $"We sent verify code to this phone number(it's end with this):\n{phoneNumber.Value.StepData.ContactPoint}";
                        VerifyCodeGroupBox.Visible = true;
                        SelectMethodGroupBox.Visible = false;
                    }
                    else
                        MessageBox.Show(phoneNumber.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "EX", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private async void ResendButton_Click(object sender, EventArgs e)
        {
            bool isEmail = false;
            if (RadioVerifyWithEmail.Checked)
                isEmail = true;

            try
            {
                // Note: every request to this endpoint is limited to 60 seconds                 
                if (isEmail)
                {
                    // send verification code to email
                    var email = await InstaApi.RequestVerifyCodeToEmailForChallengeRequireAsync(replayChallenge: true);
                    if (email.Succeeded)
                    {
                        LblForSmsEmail.Text = $"We sent verification code one more time\r\nto this email:\n{email.Value.StepData.ContactPoint}";
                        VerifyCodeGroupBox.Visible = true;
                        SelectMethodGroupBox.Visible = false;
                    }
                    else
                        MessageBox.Show(email.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // send verification code to phone number
                    var phoneNumber = await InstaApi.RequestVerifyCodeToSMSForChallengeRequireAsync(replayChallenge: true);
                    if (phoneNumber.Succeeded)
                    {
                        LblForSmsEmail.Text = $"We sent verification code one more time\r\nto this phone number(it's end with this):{phoneNumber.Value.StepData.ContactPoint}";
                        VerifyCodeGroupBox.Visible = true;
                        SelectMethodGroupBox.Visible = false;
                    }
                    else
                        MessageBox.Show(phoneNumber.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "EX", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void VerifyButton_Click(object sender, EventArgs e)
        {
            txtVerifyCode.Text = txtVerifyCode.Text.Trim();
            txtVerifyCode.Text = txtVerifyCode.Text.Replace(" ", "");
            var regex = new Regex(@"^-*[0-9,\.]+$");
            if (!regex.IsMatch(txtVerifyCode.Text))
            {
                MessageBox.Show("Verification code is numeric!!!", "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtVerifyCode.Text.Length != 6)
            {
                MessageBox.Show("Verification code must be 6 digits!!!", "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                // Note: calling VerifyCodeForChallengeRequireAsync function, 
                // if user has two factor enabled, will wait 15 seconds and it will try to
                // call LoginAsync.

                var verifyLogin = await InstaApi.VerifyCodeForChallengeRequireAsync(txtVerifyCode.Text);
                if (verifyLogin.Succeeded)
                {
                    // you are logged in sucessfully.
                    VerifyCodeGroupBox.Visible = SelectMethodGroupBox.Visible = false;
                    Size = ChallengeSize;
                    GetFeedButton.Visible = true;
                    // Save session
                    SaveSession();
                    Text = $"{AppName} Connected";
                }
                else
                {
                    VerifyCodeGroupBox.Visible = SelectMethodGroupBox.Visible = false;
                    // two factor is required
                    if (verifyLogin.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        TwoFactorGroupBox.Visible = true;
                    }
                    else
                        MessageBox.Show(verifyLogin.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "EX", MessageBoxButtons.OK, MessageBoxIcon.Error); }

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
            Debug.WriteLine(twoFactorLogin.Value);
            if (twoFactorLogin.Succeeded)
            {
                // connected
                // save session
                SaveSession();
                Size = ChallengeSize;
                TwoFactorGroupBox.Visible = false;
                GetFeedButton.Visible = true;
                Text = $"{AppName} Connected";
                Size = NormalSize;
            }
            else
            {
                MessageBox.Show(twoFactorLogin.Info.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void GetFeedButton_Click(object sender, EventArgs e)
        {
            if (InstaApi == null)
            {
                MessageBox.Show("Login first.");
                return;
            }
            if (!InstaApi.IsUserAuthenticated)
            {
                MessageBox.Show("Login first.");
                return;
            }
            var x = await InstaApi.FeedProcessor.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(1));

            if (x.Succeeded == false)
            {
                if (x.Info.ResponseType == ResponseType.ChallengeRequired)
                {
                    var challengeData = await InstaApi.GetLoggedInChallengeDataInfoAsync();
                    // Do something to challenge data, if you want!

                    var acceptChallenge = await InstaApi.AcceptChallengeAsync();
                    // If Succeeded was TRUE, you can continue to your work!
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendLine("Like 5 Media>");
                foreach (var item in x.Value.Medias.Take(5))
                {
                    // like media...
                    var liked = await InstaApi.MediaProcessor.LikeMediaAsync(item.InstaIdentifier);
                    sb2.AppendLine($"{item.InstaIdentifier} liked? {liked.Succeeded}");
                }

                sb.AppendLine("Explore Feeds Result: " + x.Succeeded);
                foreach (var media in x.Value.Medias)
                {
                    sb.AppendLine(DebugUtils.PrintMedia("Feed media", media));
                }
                RtBox.Text = sb2.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine;

                RtBox.Text += sb.ToString();
                RtBox.Visible = true;
                Size = ChallengeSize;
            }
        }

        void LoadSession()
        {
            InstaApi?.SessionHandler?.Load();

            //// Old load session
            //try
            //{
            //    if (File.Exists(StateFile))
            //    {
            //        Debug.WriteLine("Loading state from file");
            //        using (var fs = File.OpenRead(StateFile))
            //        {
            //            InstaApi.LoadStateDataFromStream(fs);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex);
            //}
        }
        void SaveSession()
        {
            if (InstaApi == null)
                return;
            if (!InstaApi.IsUserAuthenticated)
                return;
            InstaApi.SessionHandler?.Save();

            //// Old save session 
            //var state = InstaApi.GetStateDataAsStream();
            //using (var fileStream = File.Create(StateFile))
            //{
            //    state.Seek(0, SeekOrigin.Begin);
            //    state.CopyTo(fileStream);
            //}
        }

    }

    public static class DebugUtils
    {
        public static string PrintMedia(string header, InstaMedia media)
        {
            var content = $"{header}: {media.Caption?.Text.Truncate(30)}, {media.Code}";
            Debug.WriteLine(content);
            return content;
        }
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }
}

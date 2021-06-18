﻿/*  
 *  
 *  
 *  Base of everything! Access to any other classes via IInstaApi
 *  
 *  
 *                      IRANIAN DEVELOPERS
 *        
 *        
 *                            2021
 *  
 *  
 */

using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API.Processors;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Classes.SessionHandlers;
using System.Net.Http;
using System.Collections.Generic;
using System;
using InstagramApiSharp.API.Versions;
using InstagramApiSharp.API.Services;
using InstagramApiSharp.Helpers;

using InstagramApiSharp.API.Push;
#if WITH_NOTIFICATION
using InstagramApiSharp.API.RealTime;
#endif
namespace InstagramApiSharp.API
{
    /// <summary>
    ///     Base of everything that you want.
    /// </summary>
    public interface IInstaApi
    {
        #region Properties

        #region Locale
        
        /// <summary>
        ///     Timezone offset
        ///     <para>This property is using in a lot of different places.</para>
        ///     <para>X-IG-Timezone-Offset header</para>
        ///     <para>Default is choosen from your system information</para>
        /// </summary>
        int TimezoneOffset { get; set; }
        /// <summary>
        ///     X-IG-App-Startup-Country header
        ///     <para>Default is US</para>
        /// </summary>
        string StartupCountry { get; set; }
        /// <summary>
        ///     Startup country code
        ///     <para>Default value is 1 which is USA country code</para>
        /// </summary>
        uint StartupCountryCode { get; set; }
        /// <summary>
        ///     X-IG-Device-Locale header
        ///     <para>Default is en_US</para>
        /// </summary>
        string DeviceLocale { get; set; }
        /// <summary>
        ///     X-IG-App-Locale header
        ///     <para>Default is en_US</para>
        /// </summary>
        string AppLocale { get; set; }
        /// <summary>
        ///     X-IG-Mapped-Locale
        ///     <para>Default is en_US</para>
        /// </summary>
        string MappedLocale { get; set; }
        /// <summary>
        ///     Accept-Language
        ///     <para>Default is en-US</para>
        ///     <para>for UK should be: en-GB, en-US</para>
        /// </summary>
        string AcceptLanguage { get; set; }

        #endregion


#if WITH_NOTIFICATION
        /// <summary>
        ///     Realtime client for direct message only [for now]
        /// </summary>
        RealTimeClient RealTimeClient { get; }
        /// <summary>
        ///     Push notifications
        /// </summary>
        FbnsClient PushClient { get; }
#endif
        /// <summary>
        ///     Get current api version of Instagram that <see cref="InstagramApiSharp"/> is using it
        /// </summary>
        InstaApiVersionType InstaApiVersionType { get; }

        /// <summary>
        ///     Gets or sets two factor login info
        /// </summary>
        InstaTwoFactorLoginInfo TwoFactorLoginInfo { get; set; }
        /// <summary>
        ///     Gets or sets challenge login info
        /// </summary>
        InstaChallengeLoginInfo ChallengeLoginInfo { get; set; }
        /// <summary>
        ///     Get HttpHelper class
        /// </summary>
        HttpHelper HttpHelper { get; }
        /// <summary>
        ///     Current <see cref="IHttpRequestProcessor"/>
        /// </summary>
        IHttpRequestProcessor HttpRequestProcessor { get; }
        /// <summary>
        ///     Current HttpClient
        /// </summary>
        HttpClient HttpClient { get; }
        /// <summary>
        ///     Indicates whether user authenticated or not
        /// </summary>
        bool IsUserAuthenticated { get; }

        /// <summary>
        ///     Load instagram's api version from session file
        ///     <para>Default is False</para>
        /// </summary>
        bool LoadApiVersionFromSessionFile { get; set; }
        /// <summary>
        ///     Load proxy from session file.
        ///     <para>
        ///           When first time you set proxy using InstaApiBuilder class, and you try to save the session, proxy will save automatically. 
        ///           Enabling this options means that the proxy set automatically by the library.
        ///     </para>
        ///     <para>Default is false</para>
        /// </summary>
        bool LoadProxyFromSessionFile { get; set; }
        /// <summary>
        ///     Live api functions.
        /// </summary>
        ILiveProcessor LiveProcessor { get; }
        /// <summary>
        ///     Discover api functions.
        /// </summary>
        IDiscoverProcessor DiscoverProcessor { get; }
        /// <summary>
        ///     Account api functions.
        /// </summary>
        IAccountProcessor AccountProcessor { get; }
        /// <summary>
        ///     Story api functions.
        /// </summary>
        IStoryProcessor StoryProcessor { get; }
        /// <summary>
        ///     Media api functions.
        /// </summary>
        IMediaProcessor MediaProcessor { get; }
        /// <summary>
        ///     Comments api functions.
        /// </summary>
        ICommentProcessor CommentProcessor { get; }
        /// <summary>
        ///     Messaging (direct) api functions.
        /// </summary>
        IMessagingProcessor MessagingProcessor { get; }
        /// <summary>
        ///     Feed api functions.
        /// </summary>
        IFeedProcessor FeedProcessor { get; }
        /// <summary>
        ///     Collection api functions.
        /// </summary>
        ICollectionProcessor CollectionProcessor { get; }
        /// <summary>
        ///     Location api functions.
        /// </summary>
        ILocationProcessor LocationProcessor { get; }
        /// <summary>
        ///     Hashtag api functions.
        /// </summary>
        IHashtagProcessor HashtagProcessor { get; }
        /// <summary>
        ///     User api functions.
        /// </summary>
        IUserProcessor UserProcessor { get; }
        /// <summary>
        ///     Instagram TV api functions.
        /// </summary>
        ITVProcessor TVProcessor { get; }
        /// <summary>
        ///     Business api functions
        ///     <para>Note: All functions of this interface only works with business accounts!</para>
        /// </summary>
        IBusinessProcessor BusinessProcessor { get; }
        /// <summary>
        ///     Shopping and commerce api functions
        /// </summary>
        IShoppingProcessor ShoppingProcessor { get; }
        /// <summary>
        ///     Instagram Web api functions.
        ///     <para>It's related to https://instagram.com/accounts/ </para>
        /// </summary>
        IWebProcessor WebProcessor { get; }
        IVideoCallProcessor VideoCallProcessor { get; }
        /// <summary>
        ///     Push notification helper processor
        /// </summary>
        IPushProcessor PushProcessor { get; }

        /// <summary>
        ///     Creative api functions
        /// </summary>
        ICreativeProcessor CreativeProcessor { get; }

        /// <summary>
        ///     Reels api functions
        /// </summary>
        IReelProcessor ReelProcessor { get; }
        /// <summary>
        ///     Session handler
        /// </summary>
        ISessionHandler SessionHandler { get; set; }
        /// <summary>
        ///     Registration Service
        /// </summary>
        IRegistrationService RegistrationService { get; }
#endregion

#region State data

        /// <summary>
        ///     Get current state info as Memory stream
        /// </summary>
        /// <returns>State data</returns>
        Stream GetStateDataAsStream();
        /// <summary>
        ///     Get current state info as Json string
        /// </summary>
        /// <returns>State data</returns>
        string GetStateDataAsString();
        /// <summary>
        ///     Get current state info as Json string asynchronously
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        /// 

        ///<summary>
        ///     Get current state as StateData object
        /// </summary>
        /// <returns>
        ///     State data object
        /// </returns>
        StateData GetStateDataAsObject();
            
        Task<string> GetStateDataAsStringAsync();
        /// <summary>
        ///     Get current state info as Memory stream asynchronously
        /// </summary>
        /// <returns>State data</returns>
        Task<Stream> GetStateDataAsStreamAsync();
        /// <summary>
        ///     Set state data from provided stream
        /// </summary>
        void LoadStateDataFromStream(Stream data);
        /// <summary>
        ///     Set state data from provided json string
        /// </summary>
        void LoadStateDataFromString(string data);
        /// <summary>
        ///     Set state data from provided stream asynchronously
        /// </summary>

        /// <summary>
        ///     Set state data from object
        /// </summary>
        void LoadStateDataFromObject(StateData stateData);

        Task LoadStateDataFromStreamAsync(Stream stream);
        /// <summary>
        ///     Set state data from provided json string asynchronously
        /// </summary>
        Task LoadStateDataFromStringAsync(string json);


#endregion State data

#region Other public functions

        /// <summary>
        ///     Get current API version info (signature key, api version info, app id)
        /// </summary>
        InstaApiVersion GetApiVersionInfo();
        /// <summary>
        ///     Get api version type
        /// </summary>
        InstaApiVersionType GetApiVersionType();
        /// <summary>
        ///     Get user agent of current <see cref="IInstaApi"/>
        /// </summary>
        string GetUserAgent();
        /// <summary>
        ///     Set timeout to <see cref="HttpClient"/>
        /// </summary>
        /// <param name="timeout">Timeout</param>
        void SetTimeout(TimeSpan timeout);
        /// <summary>
        ///     Set custom HttpClientHandler to be able to use certain features, e.g Proxy and so on
        /// </summary>
        /// <param name="handler">HttpClientHandler</param>
        void UseHttpClientHandler(HttpClientHandler handler);
        /// <summary>
        ///     Sets user credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void SetUser(string username, string password);

        /// <summary>
        ///     Sets user credentials
        /// </summary>
        /// <param name="user"></param>
        void SetUser(UserSessionData user);
        /// <summary>
        ///     Update user information (private, profile picture, username and etc.)
        ///     <para>Note 1. Login required!</para>
        ///     <para>Note 2. It's necessary to save session, after you called this function</para>
        /// </summary>
        /// <param name="updatedUser">Updated user</param>
        void UpdateUser(InstaUserShort updatedUser);
        /// <summary>
        ///     Gets current device
        /// </summary>
        AndroidDevice GetCurrentDevice();
        /// <summary>
        ///     Gets logged in user
        /// </summary>
        UserSessionData GetLoggedUser();
        /// <summary>
        ///     Get Accept Language
        /// </summary>
        [Obsolete("GetAcceptLanguage is deprecated. Use AcceptLanguage property instead. This function will no longer effect anything and will be deleted in future releases.")]
        string GetAcceptLanguage();
        /// <summary>
        ///     Get current time zone
        ///     <para>Returns something like: Asia/Tehran</para>
        /// </summary>
        /// <returns>Returns something like: Asia/Tehran</returns>
        string GetTimezone();
        /// <summary>
        ///     Get current time zone offset
        ///     <para>Returns something like this: 16200</para>
        /// </summary>
        /// <returns>Returns something like this: 16200</returns>
        [Obsolete("GetTimezoneOffset is deprecated. Use TimezoneOffset property instead. This function will no longer effect anything and will be deleted in future releases.")]
        int GetTimezoneOffset();
        /// <summary>
        ///     Set delay between requests. Useful when API supposed to be used for mass-bombing.
        /// </summary>
        /// <param name="delay">Timespan delay</param>
        void SetRequestDelay(IRequestDelay delay);
        /// <summary>
        ///     Set delay before configuring medias [only for uploading parts]
        /// </summary>
        /// <param name="configureMediaDelay">Timespan delay for configuring Media</param>
        void SetConfigureMediaDelay(IConfigureMediaDelay configureMediaDelay);
        /// <summary>
        ///     Set instagram api version (for user agent version)
        /// </summary>
        /// <param name="apiVersion">Api version</param>
        void SetApiVersion(InstaApiVersionType apiVersion);
        /// <summary>
        ///     Set custom android device.
        ///     <para>Note 1: If you want to use this method, you should call it before you calling <seealso cref="IInstaApi.LoadStateDataFromStream(Stream)"/> or <seealso cref="IInstaApi.LoadStateDataFromString(string)"/></para>
        ///     <para>Note 2: this is optional, if you didn't set this, <seealso cref="InstagramApiSharp"/> will choose random device.</para>
        /// </summary>
        /// <param name="device">Android device</param>
        void SetDevice(AndroidDevice device);
        /// <summary>
        ///     Set Accept Language
        /// </summary>
        /// <param name="languageCodeAndCountryCode">Language Code and Country Code. For example:
        /// <para>en-US for united states</para>
        /// <para>fa-IR for IRAN</para>
        /// </param>
        [Obsolete("SetAcceptLanguage is deprecated. Use AcceptLanguage property instead. This function will no longer effect anything and will be deleted in future releases.")]
        bool SetAcceptLanguage(string languageCodeAndCountryCode);
        /// <summary>
        ///     Set time zone
        ///     <para>I.e: Asia/Tehran for Iran</para>
        /// </summary>
        /// <param name="timezone">
        ///     time zone
        ///     <para>I.e: Asia/Tehran for Iran</para>
        /// </param>
        void SetTimezone(string timezone);
        /// <summary>
        ///     Set time zone offset
        ///     <para>I.e: 16200 for Iran/Tehran</para>
        /// </summary>
        /// <param name="timezoneOffset">
        ///     timezone offset
        ///     <para>I.e: 16200 for Iran/Tehran</para>
        /// </param>
        [Obsolete("SetTimezoneOffset is deprecated. Use TimezoneOffset property instead. This function will no longer effect anything and will be deleted in future releases.")]
        void SetTimezoneOffset(int timezoneOffset);
        /// <summary>
        ///     Send get request
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        Task<IResult<string>> SendGetRequestAsync(System.Uri uri);
        /// <summary>
        ///     Send signed post request (include signed signature) 
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        Task<IResult<string>> SendSignedPostRequestAsync(System.Uri uri, Dictionary<string, string> data);
        /// <summary>
        ///     Send signed post request (include signed signature) 
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        Task<IResult<string>> SendSignedPostRequestAsync(System.Uri uri, Newtonsoft.Json.Linq.JObject data);
        /// <summary>
        ///     Send signed post request (include signed signature) 
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        Task<IResult<string>> SendSignedPostRequestV2Async(System.Uri uri, Newtonsoft.Json.Linq.JObject data);
        /// <summary>
        ///     Send post request
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        Task<IResult<string>> SendPostRequestAsync(System.Uri uri, Dictionary<string, string> data);
#endregion Other public functions

#region Authentication, challenge functions

#region Challenge part


        //////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////// Challenge for logged in user /////////////////////////////////

        [Obsolete("Deprecated. Please use IInstaApi.ChallengeLoginInfo property instead.")]
        /// <summary>
        ///     Set Challenge Info when server asks for a challenge on calling functions
        /// </summary>
        /// <param name="Challenge">Challenge info</param>
        void SetChallengeInfo(InstaChallengeLoginInfo Challenge);

        /// <summary>
        ///     Get challenge data for logged in user
        ///     <para>This will pop-on, if some suspecious login happend</para>
        /// </summary>
        Task<IResult<InstaLoggedInChallengeDataInfo>> GetLoggedInChallengeDataInfoAsync();

        /// <summary>
        ///     Accept challlenge, it is THIS IS ME feature!!!!
        ///     <para>You must call <see cref="GetLoggedInChallengeDataInfoAsync"/> first,
        ///     if you across to <see cref="ResultInfo.ResponseType"/> equals to <see cref="ResponseType.ChallengeRequired"/> while you logged in!</para>
        /// </summary>
        Task<IResult<bool>> AcceptChallengeAsync();


        /////////////////////////////////// Challenge for logged in user /////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        ///     Get challenge require (checkpoint required) options
        /// </summary>
        Task<IResult<InstaChallengeRequireVerifyMethod>> GetChallengeRequireVerifyMethodAsync();
        /// <summary>
        ///     Reset challenge require (checkpoint required) method
        /// </summary>
        Task<IResult<InstaChallengeRequireVerifyMethod>> ResetChallengeRequireVerifyMethodAsync();
        /// <summary>
        ///     Request verification code sms for challenge require (checkpoint required)
        /// </summary>
        /// <param name="replayChallenge">true if Instagram should resend verification code to you</param>
        Task<IResult<InstaChallengeRequireSMSVerify>> RequestVerifyCodeToSMSForChallengeRequireAsync(bool replayChallenge = false);
        /// <summary>
        ///     Submit phone number for challenge require (checkpoint required)
        ///     <para>Note: This only needs , when you calling <see cref="IInstaApi.GetChallengeRequireVerifyMethodAsync"/> or
        ///     <see cref="IInstaApi.ResetChallengeRequireVerifyMethodAsync"/> and
        ///     <see cref="InstaChallengeRequireVerifyMethod.SubmitPhoneRequired"/> property is true.</para>
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        Task<IResult<InstaChallengeRequireSMSVerify>> SubmitPhoneNumberForChallengeRequireAsync(string phoneNumber, bool replayChallenge = false);
        /// <summary>
        ///     Request verification code email for challenge require (checkpoint required)
        /// </summary>
        /// <param name="replayChallenge">true if Instagram should resend verification code to you</param>
        Task<IResult<InstaChallengeRequireEmailVerify>> RequestVerifyCodeToEmailForChallengeRequireAsync(bool replayChallenge = false);
        /// <summary>
        ///     Verify verification code for challenge require (checkpoint required)
        /// </summary>
        /// <param name="verifyCode">Verification code</param>
        Task<IResult<InstaLoginResult>> VerifyCodeForChallengeRequireAsync(string verifyCode);
#endregion Challenge part
        
        /// <summary>
        ///     Check email availability
        /// </summary>
        /// <param name="email">Email to check</param>
        [Obsolete("CheckEmailAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<InstaCheckEmailRegistration>> CheckEmailAsync(string email);
        /// <summary>
        ///     Check phone number availability
        /// </summary>
        /// <param name="phoneNumber">Phone number to check</param>
        [Obsolete("CheckPhoneNumberAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<bool>> CheckPhoneNumberAsync(string phoneNumber);
        /// <summary>
        ///     Check username availablity. 
        /// </summary>
        /// <param name="username">Username</param>
        [Obsolete("CheckPhoneNumberAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<InstaAccountCheck>> CheckUsernameAsync(string username);
        /// <summary>
        ///     Send sign up sms code
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        [Obsolete("SendSignUpSmsCodeAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<bool>> SendSignUpSmsCodeAsync(string phoneNumber);
        /// <summary>
        ///     Verify sign up sms code
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="verificationCode">Verification code</param>
        [Obsolete("SendSignUpSmsCodeAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<InstaPhoneNumberRegistration>> VerifySignUpSmsCodeAsync(string phoneNumber, string verificationCode);
        /// <summary>
        ///     Get username suggestions
        /// </summary>
        /// <param name="name">Name</param>
        [Obsolete("GetUsernameSuggestionsAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<InstaRegistrationSuggestionResponse>> GetUsernameSuggestionsAsync(string name);
        /// <summary>
        ///     Validate new account creation with phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="verificationCode">Verification code</param>
        /// <param name="username">Username to set</param>
        /// <param name="password">Password to set</param>
        /// <param name="firstName">First name to set</param>
        [Obsolete("ValidateNewAccountWithPhoneNumberAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<InstaAccountCreation>> ValidateNewAccountWithPhoneNumberAsync(string phoneNumber, string verificationCode, string username, string password, string firstName);
        /// <summary>
        ///     Create a new instagram account
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        /// <param name="firstName">First name (optional)</param>
        /// <param name="delay">Delay between requests. null = 2.5 seconds</param>
        [Obsolete("CreateNewAccountAsync is deprecated. Use the one from RegistrationService property instead.\r\nThis function will be deleted.")]
        Task<IResult<InstaAccountCreation>> CreateNewAccountAsync(string username, string password, string email, string firstName = ""/*, TimeSpan? delay = null*/);
        /// <summary>
        ///     Accept consent required (only for GDPR countries)
        /// </summary>
        /// <param name="delay">Delay time between requests (null => 1.5 seconds)</param>
        Task<IResult<bool>> AcceptConsentAsync(TimeSpan? delay = null);
        /// <summary>
        ///     Send requests for login flows (contact prefill, read msisdn header, launcher sync and qe sync)
        ///     <para>Note 1: You should call this function before you calling <see cref="IInstaApi.LoginAsync(bool)"/>, if you want your account act like original instagram app.</para>
        ///     <para>Note 2: One call per one account! No need to call while you are loading a session</para>
        /// </summary>
        Task<IResult<bool>> SendRequestsBeforeLoginAsync();
        /// <summary>
        ///     Send requests after you logged in successfully (Act as an real instagram user)
        /// </summary>
        Task<IResult<bool>> SendRequestsAfterLoginAsync();
        /// <summary>
        ///     Login using given credentials asynchronously
        /// </summary>
        /// <param name="isNewLogin"></param>
        /// <returns>
        ///     Success --> is succeed
        ///     TwoFactorRequired --> requires 2FA login.
        ///     BadPassword --> Password is wrong
        ///     InvalidUser --> User/phone number is wrong
        ///     Exception --> Something wrong happened
        ///     ChallengeRequired --> You need to pass Instagram challenge
        /// </returns>
        Task<IResult<InstaLoginResult>> LoginAsync(bool isNewLogin = true);
        /// <summary>
        ///     Login using cookies
        ///     <para>Note: You won't be able to change password, if you use <see cref="LoginWithCookiesAsync(string)"/> function for logging in!</para>
        /// </summary>
        /// <param name="cookies">Cookies</param>
        Task<IResult<bool>> LoginWithCookiesAsync(string cookies);

        /// <summary>
        ///     Login with Facebook access token
        /// </summary>
        /// <param name="fbAccessToken">Facebook access token</param>
        /// <param name="cookiesContainer">Cookies</param>
        /// <returns>
        ///     Success --> is succeed
        ///     TwoFactorRequired --> requires 2FA login.
        ///     BadPassword --> Password is wrong
        ///     InvalidUser --> User/phone number is wrong
        ///     Exception --> Something wrong happened
        ///     ChallengeRequired --> You need to pass Instagram challenge
        /// </returns>
        Task<IResult<InstaLoginResult>> LoginWithFacebookAsync(string fbAccessToken, string cookiesContainer);

        /// <summary>
        ///     2-Factor Authentication Login using a verification code
        ///     Before call this method, please run LoginAsync first.
        /// </summary>
        /// <param name="verificationCode">Verification Code sent to your phone number</param>
        /// <param name="trustThisDevice">Trust this device or not?!</param>
        /// <param name="twoFactorVerifyOptions">Two factor verification option</param>
        /// <returns>
        ///     Success --> is succeed
        ///     InvalidCode --> The code is invalid
        ///     CodeExpired --> The code is expired, please request a new one.
        ///     Exception --> Something wrong happened
        /// </returns>
        Task<IResult<InstaLoginTwoFactorResult>> TwoFactorLoginAsync(string verificationCode, bool trustThisDevice = false, InstaTwoFactorVerifyOptions twoFactorVerifyOptions = InstaTwoFactorVerifyOptions.SmsCode);

        /// <summary>
        ///     Get Two Factor Authentication details
        /// </summary>
        /// <returns>
        ///     An instance of TwoFactorLoginInfo if success.
        ///     A null reference if not success; in this case, do LoginAsync first and check if Two Factor Authentication is
        ///     required, if not, don't run this method
        /// </returns>
        Task<IResult<InstaTwoFactorLoginInfo>> GetTwoFactorInfoAsync();
        /// <summary>
        ///     Get user lookup for recovery options
        /// </summary>
        /// <param name="usernameOrEmailOrPhoneNumber">Username or email or phone number</param>
        Task<IResult<InstaUserLookup>> GetRecoveryOptionsAsync(string usernameOrEmailOrPhoneNumber);
        /// <summary>
        ///     Send recovery code by Username
        /// </summary>
        /// <param name="username">Username</param>
        Task<IResult<InstaRecovery>> SendRecoveryByUsernameAsync(string username);
        /// <summary>
        ///     Send recovery code by Email
        /// </summary>
        /// <param name="email">Email Address</param>
        Task<IResult<InstaRecovery>> SendRecoveryByEmailAsync(string email);
        /// <summary>
        ///     Send recovery code by Phone number
        /// </summary>
        /// <param name="phone">Phone Number</param>
        Task<IResult<InstaRecovery>> SendRecoveryByPhoneAsync(string phone);
        /// <summary>
        ///    Send Two Factor Login SMS Again
        /// </summary>
        Task<IResult<TwoFactorLoginSMS>> SendTwoFactorLoginSMSAsync();
        /// <summary>
        ///     Check two factor trusted notification status
        /// </summary>
        /// <remarks>
        ///         This will checks for response from another logged in device.
        ///         <para>Review status can be 0, 1 or 2</para>
        ///         <para>At the momemnt I don't know, but I'll check it for understand these values</para>
        /// </remarks>
        Task<IResult<InstaTwoFactorTrustedNotification>> Check2FATrustedNotificationAsync();
        /// <summary>
        ///     Logout from instagram asynchronously
        /// </summary>
        /// <returns>True if logged out without errors</returns>
        Task<IResult<bool>> LogoutAsync();
        /// <summary>
        ///     Get currently logged in user info asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="InstaCurrentUser" />
        /// </returns>
        Task<IResult<InstaCurrentUser>> GetCurrentUserAsync();
        Task<IResult<bool>> LauncherSyncAsync();
        Task<IResult<InstaBanyanSuggestions>> GetBanyanSuggestionsAsync();

        #endregion Authentication, challenge functions

        #region Giphy

        /// <summary>
        ///     Get trending giphy
        /// </summary>
        /// <param name="requestType">Request type for Direct or story</param>
        Task<IResult<GiphyList>> GetGiphyTrendingAsync(InstaGiphyRequestType requestType = InstaGiphyRequestType.Direct);

        /// <summary>
        ///     Search giphy
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <param name="requestType">Request type for Direct or story</param>
        Task<IResult<GiphyList>> SearchGiphyAsync(string query, InstaGiphyRequestType requestType = InstaGiphyRequestType.Direct);

#endregion Giphy
    }
}
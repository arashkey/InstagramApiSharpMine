Private version of [InstagramApiSharp](https://github.com/ramtinak/InstagramApiSharp) ![InstagramApiSharp](http://s8.picofile.com/file/8336601292/insta50x.png) 



| Target | Branch | Version | Download |
| ------ | ------ | ------ | ------ |
| Github | master | v2.8.5 | [v2.8.5](https://github.com/rmt4006/InstagramApiSharpMine/archive/master.zip) |
| Release | master | v2.8.5 | [v2.8.5](https://github.com/rmt4006/InstagramApiSharpMine/releases/) |

#### How to build the project?
Check this youtube video: https://www.youtube.com/watch?v=AFjKryHz9nM

#### Why Different Solutions ?
Since Push notifications and RealtimeClient has a lot of external libraries, I decided to exlcude it from original project.

[InstagramApiSharp.sln] -> Doesn't support Push notifcations/realtime client, so you don't need to add any external libraries except Portable.BouncyCastle.

[InstagramApiSharp.WithNotification.sln] -> Support Push notifcations/realtime client, so you HAVE to add any external libraries except Portable.BouncyCastle.

[InstagramApiSharp.NET5.sln] -> Doesn't support Push notifcations/realtime client, so you don't need to add any external libraries.

[InstagramApiSharp.NET5.WithNotification.sln] -> Support Push notifcations/realtime client, so you HAVE to add any external libraries.

#### Important Note about Packages:
You must/should reference following packages to your projects, if you got any error:

For [InstagramApiSharp.sln]
| Target | Package name | Version | Level | 
| ------ | ------ | ------ | ------ |
| Encrypted password | Portable.BouncyCastle | 1.8.6.7 or newer | Important for InstagramApiSharp |
| Json wrapper | Newtonsoft.Json | 10.0.3 or newer | Important for InstagramApiSharp |
| CSharp library | Microsoft.CSharp | 4.3.0 | Important for InstagramApiSharp |
| GZip | Iconic.Zlib.NetstandardUwp | 1.0.1 or newer |  Important for InstagramApiSharp |

For [InstagramApiSharp.WithNotification.sln] with Push Notifications/realtime client support
| Target | Package name | Version | Level | 
| ------ | ------ | ------ | ------ |
| Encrypted password | Portable.BouncyCastle | 1.8.6.7 or newer | Important for InstagramApiSharp |
| Json wrapper | Newtonsoft.Json | 10.0.3 or newer | Important for InstagramApiSharp |
| CSharp library | Microsoft.CSharp | 4.3.0 | Important for InstagramApiSharp |
| GZip | Iconic.Zlib.NetstandardUwp | 1.0.1 or newer |  Important for InstagramApiSharp |
| Push/Realtime | Thrift | InstagramApiSharp's Port | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Codecs.Mqtt | 0.6.0 | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Buffers | 0.6.0 | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
| - | Microsoft.Extensions.Logging | 3.1.4 | Important for Thrift |
| - | Microsoft.Extensions.Logging.Abstractions | 3.1.4 | Important for Thrift |
| - | Microsoft.Extensions.Options | 3.1.4 | Important for Thrift |
| - | System.Threading.Tasks.Extensions | 4.5.4 | Important for Thrift |

- Note 1: You MUST reference [Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle/)'s package to your projects.
- Note 2: You MUST reference Thrift's project (InstagramApiSharp's port) to your project.
- Note 3: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.

--
For .NET5 and .NETCore 3.1 you DON'T need to add Portable.BouncyCastle because encryption is supported in the .NETCore 3.1 and NET5.


For [InstagramApiSharp.NET5.sln]
| Target | Package name | Version | Level | 
| ------ | ------ | ------ | ------ |
| Json wrapper | Newtonsoft.Json | 12.0.3 or newer | Important for InstagramApiSharp |
| GZip | Iconic.Zlib.NetstandardUwp | 1.0.2 or newer |  Important for InstagramApiSharp |

For [InstagramApiSharp.NET5.WithNotification.sln] with Push Notifications/realtime client support
| Target | Package name | Version | Level | 
| ------ | ------ | ------ | ------ |
| Json wrapper | Newtonsoft.Json | 12.0.3 or newer | Important for InstagramApiSharp.NET5 |
| GZip | Iconic.Zlib.NetstandardUwp | 1.0.1 or newer |  Important for InstagramApiSharp.NET5 |
| Push/Realtime | Thrift.NET5 | InstagramApiSharp.NET5's Port | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Codecs.Mqtt | 0.6.0 | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Buffers | 0.6.0 | Important for Push notifications/Realtime client |
| Push/Realtime | DotNetty.Handlers | 0.6.0 | Important for Push notifications/Realtime client |
| - | Microsoft.Extensions.Logging | 3.1.4 | Important for Thrift.NET5 |
| - | Microsoft.Extensions.Logging.Abstractions | 3.1.4 | Important for Thrift.NET5 |
| - | Microsoft.Extensions.Options | 3.1.4 | Important for Thrift.NET5 |

- Note 1: You MUST reference Thrift.NET5's project (InstagramApiSharp.NET5's port) to your project.
- Note 2: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.


#### Version changes
v2.8.5
- [Tiny fix] for TVProcessor.MarkAsSeenAsync
- [Update] InstaCommentContainerModuleType, InstaMediaContainerModuleType
- [Add] new Reels APIs support!
- [Add] ReelProcessor property to IInstaApi
- [Add] UploadReelVideoAsync to ReelProcessor
- [Add] GetReelsClipsAsync to ReelProcessor
- [Add] GetUserReelsClipsAsync to ReelProcessor
- [Add] MarkReelAsSeenAsync to ReelProcessor
- [Add] SendDirectReelClipsAsync to MessagingProcessor
- [Add] SendDirectReelClipsToRecipientsAsync to MessagingProcessor
- [Add] GetClipsAssetsAsync to CreativeProcessor

v2.8.0
- [Bugfix] for SharePreviewToFeed
- [Tiny fix] for IInstaApiBuilder
- [Add] IRegistrationService and RegistrationService
- [Add] CreateNewAccountWithPhoneNumberAsync to IRegistrationService
- [Add] CreateNewAccountWithEmailAsync to IRegistrationService
- [Add] GenerateRandomBirthday to IRegistrationService
- [Add] GetFirstContactPointPrefillAsync to RegistrationService
- [Add] FirstLauncherSyncAsync to IRegistrationService
- [Add] FirstQeSyncAsync to IRegistrationService
- [Add] CheckUsernameAsync to IRegistrationService
- [Add] InstaCheckEmailRegistration to IRegistrationService
- [Add] CheckEmailAsync to IRegistrationService
- [Add] GetSignupConsentConfigAsync to IRegistrationService
- [Add] SendRegistrationVerifyEmailAsync to IRegistrationService
- [Add] CheckRegistrationConfirmationCodeAsync to IRegistrationService
- [Add] GetSiFetchHeadersAsync to IRegistrationService
- [Add] GetUsernameSuggestionsAsync to IRegistrationService
- [Add] CheckAgeEligibilityAsync to IRegistrationService
- [Add] GetOnboardingStepsAsync to IRegistrationService
- [Add] NewUserFlowBeginsConsentAsync to IRegistrationService
- [Add] GetMultipleAccountsFamilyAsync to IRegistrationService
- [Add] GetZrTokenResultAsync to IRegistrationService
- [Add] LauncherSyncAsync to IRegistrationService
- [Add] QeSyncAsync to IRegistrationService
- [Add] NuxNewAccountSeenAsync to IRegistrationService
- [Add] GetContactPointPrefillAsync to IRegistrationService
- [Add] SmsVerificationCode property to IRegistrationService
- [Add] AccountRegistrationPhoneNumber to IRegistrationService
- [Add] CheckPhoneNumberAsync to IRegistrationService
- [Add] SendSignUpSmsCodeAsync to IRegistrationService
- [Add] VerifySignUpSmsCodeAsync to IRegistrationService

v2.7.0
- [Update] Api version and headers to v180.0.0.31.119 [it's default now]
  also added v169.3.0.30.135
- [Deprecate] UploadStoryPhotoAsync(InstaImage, string, InstaStoryUploadOptions) (caption is deprecated, use new function)
- [Deprecate] UploadStoryPhotoAsync(Action<InstaUploaderProgress>, InstaImage, string, InstaStoryUploadOptions) (caption is deprecated, use new function)
- [Deprecate] UploadStoryPhotoWithUrlAsync(InstaImage, string, Uri, InstaStoryUploadOptions) (caption is deprecated, use new function)
- [Deprecate] UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress> progress, InstaImage image, string, Uri, InstaStoryUploadOptions) (caption is deprecated, use new function)
- [Deprecate] UploadStoryVideoAsync(InstaVideoUpload, string, InstaStoryUploadOptions)
- [Deprecate] UploadStoryVideoAsync(Action<InstaUploaderProgress>, InstaVideoUpload, string, InstaStoryUploadOptions)
- [Deprecate] UploadStoryVideoWithUrlAsync(InstaVideoUpload, string, Uri uri, InstaStoryUploadOptions)
- [Deprecate] UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress>, InstaVideoUpload, string, Uri uri, InstaStoryUploadOptions)
- [Bugfix] for LoginWithCookiesAsync
- [Tiny fix] for AcceptConsentAsync
- [Tiny fix] for DebugLogger for .NET 5 and .NETCore 3.1
- [Tiny fix] for ConfigureStoryVideoAsync
- [Tiny fix] for ConfigureStoryVideoAsync
- [Update] GetUserTimeFeedAsync
- [Update] GetThreadToken
- [Update] GetStoryToken
- [Update] UploadStoryPhotoAsync/UploadStoryPhotoWithUrlAsync to the latest API 
- [Update] UploadStoryVideoAsync/UploadStoryVideoWithUrlAsync to support mentions and other story stuffs 
- [Update] ChangeProfilePictureAsync
- [Update] TypingChanged in RealtimeClient
- [Update] adding header to prevent some bugs related to different culture
- [Add] support for .NET 5 and .NETCore 3.1
- [Add] NextIdsToFetch to PaginationParameters
- [Add] GetStoryFeedWithPostMethod(PaginationParameters,string) to StoryProcessor
- [Add] SessionId to PaginationParameters
- [Add] CreativeProcessor to IInstaApi
- [Add] GetAssetsAsync to CreativeProcessor
- [Add] WriteSupportedCapablititiesAsync to CreativeProcessor
- [Add] support for RepliedToMessage to InstaDirectInboxItem
- [Add] support for Reactions to InstaDirectInboxItem
- [Add] IsReactionLog property to InstaActionLog
- [Add] emoji support to InstaDirectReaction class
- [Add] ClientFacingErrorMessage property to InstaDirectRespondPayload
- [Add] ReplyDirectMessageAsync to MessagingProcessor
- [Add] ForwardDirectMessageAsync to MessagingProcessor
- [Add] EnableThreadVanishModeAsync to MessagingProcessor
- [Add] DisableThreadVanishModeAsync to MessagingProcessor
- [Add] SendReactionMessageAsync to RealtimeClient
- [Add] DirectItemChanged to RealtimeClient 
- [Add] BroadcastChanged to RealtimeClient
- [Add] ParseQueryString(string query, out string type) to HttpUtility
- [Add] SharePreviewToFeed parameter to TVProcessor.UploadVideoAsync
- [Add] support for uploading reels with UploadStoryVideoAsync function

v2.6.0
- [Deprecate] AddToPostLiveAsync, DeletePostLiveAsync [API deprecated by Instagram]
- [Bugfix] for serialization/deserialization in .NET 5.0 
- [Bugfix] for DM last seen
- [Bugfix] for short comments
- [Bugfix] for SearchPeopleAsync
- [Rename] InstaFollowingOrderType to InstaFollowOrderType
- [Update] UserFollowers Uri
- [Update] GetUserFollowers functions to support OrderBy
- [Update] GetGiphyTrendingAsync
- [Update] SearchGiphyAsync
- [Update] supported capabalities
- [Update] ConfirmJoinBroadcastAsync
- [Update] GetMediaCommentsAsync
- [Update] GetUserStoryAndLivesAsync url
- [Update] LikeThreadMessageAsync token
- [Update] UploadSinglePhoto in HelperProcessor
- [Update] InstaBroadcastCommentList class
- [Update] GenerateUserAgent function
- [Update] GetHashtagSection
- [Add] TryParseAndSetUserAgent to IInstaApiBuilder
- [Add] MarkMultipleElectionStoriesAsSeenAsync to StoryProcessor
- [Add] GetUserTVSeriesAsync to TVProcessor
- [Add] GetPostLiveThumbnailsAsync to LiveProcessor
- [Add] GetTVCreationToolsAsync to TVProcessor
- [Add] GetBytesAsync to HelperProcessor
- [Add] AddHeader function to HttpExtensions
- [Add] AddLiveBroadcastToTVAsync to TVProcessor
- [Add] IsMutingReel, IsBlockingReel properties to InstaFriendshipFullStatus
- [Add] JoinBroadcastAsync to LiveProcessor 
- [Add] ConfirmJoinBroadcastAsync to LiveProcessor 
- [Add] GetLiveTransactionToken to ExtensionHelper
- [Add] InviteToBroadcastAsync to LiveProcessor 
- [Add] LeaveBroadcastAsync to LiveProcessor 
- [Add] BroadcastEventAsync to LiveProcessor
- [Add] Creator to InstaAccountType
- [Add] support for BroadcastStatusType in InstaBroadcastLiveHeartBeatViewerCount class
- [Add] support for BroadcastStatusType in InstaBroadcastInfo class
- [Add] SendDirectTextAsync to RealtimeClient
- [Add] SendDirectTextToRecipientAsync to RealtimeClient
- [Add] SendDirectLinkAsync to RealtimeClient
- [Add] SendDirectLinkToRecipientsAsync to RealtimeClient
- [Add] SendDirectLocationAsync to RealtimeClient
- [Add] SendDirectLocationToRecipientsAsync to RealtimeClient
- [Add] SendDirectProfileAsync to RealtimeClient
- [Add] SendDirectProfileToRecipientsAsync to RealtimeClient
- [Add] ShareMediaToThreadAsync to RealtimeClient
- [Add] ShareMediaToUserAsync to RealtimeClient
- [Add] SendDirectFelixShareAsync to RealtimeClient
- [Add] SendDirectFelixShareToRecipientsAsync to RealtimeClient
- [Add] SendDirectHashtagAsync to RealtimeClient
- [Add] SendDirectHashtagToRecipientsAsync to RealtimeClient
- [Add] SendDirectLikeAsync to RealtimeClient
- [Add] SendDirectLikeToRecipientsAsync to RealtimeClient
- [Add] MarkDirectThreadAsSeenAsync to RealtimeClient
- [Add] LikeThreadMessageAsync to RealtimeClient
- [Add] IndicateActivityAsync to RealtimeClient
- [Add] IsElection to InstaReelFeed class
- [Add] notifications sample

v2.5.0
- [Update] Api version and headers to v164.0.0.46.123 [it's default now] 
   also added 157.0.0.37.120
- [Bugfix] for v146 and newer apis Login 
- [Bugfix] for CommentMediaAsync, ReplyCommentMediaAsync
- [Bugfix] for facebook login 
- [Bugfix] for HashtagMedia's channel converter
- [Bugfix] for StateData class
- [Bugfix] for GetUserFollowing's functions
- [Update] TwoFactorLoginAsync
- [Update] GetChallengeRequireVerifyMethodAsync 
    Add ChallengeRequiredV2 to ResponseType
- [Update] GetRequestForDownloadAccountDataAsync
- [Update] ShareMediaToUserAsync, ShareMediaToThreadAsync
- [Add] enc_password support [encrypted password]
- [Add] RemoveEpisodeFromTVSeriesAsync to TVProcessor
- [Add] AddEpisodeToTVSeriesAsync to TVProcessor
- [Add] EditMediaAsync to TVProcessor
- [Add] UpdateTVSeriesAsync to TVProcessor
- [Add] DeleteTVSeriesAsync to TVProcessor
- [Add] CreateTVSeriesAsync to TVProcessor
- [Add] CheckOffensiveTextAsync to CommentProcessor
- [Add] Spam property to ResultInfo class
- [Add] CommentBlock to ResponseType
- [Add] GetContainerType for InstaCommentContainerModuleType
- [Add] InstaCommentContainerModuleType
- [Add] nullable check for HashtagMedia converter


v2.4.0
- [Bugfix] for EditProfileAsync
- [Bugfix] for GetCurrentUserAsync
- [Bugfix] for #19 | revert back to v136 since v146 has login issue [for now]
- [Bugfix] UploadPhotoAsync
- [Bugfix] GetUserFollowersByIdAsync and GetUserFollowersAsync
- [Bugfix] GetUserFollowingByIdAsync and GetUserFollowingAsync
- [Bugfix] LikeMediaAsync and UnLikeMediaAsync
- [Bugfix] AndroidDevice
- [Add] ChallengeLoginInfo,TwoFactorLoginInfo to StateData class [Library can save/load these properties from now.]
- [Add] GetNotificationsSettingsAsync to AccountProcessor 
- [Add] ChangeNotificationsSettingsAsync to AccountProcessor
- [Add] DotNetty and Thrift as dependency
- [Add] push notification support as PushClient in IInstaApi
- [Add] realtime support as RealTimeClient in IInstaApi
- [Add] GenerateMediaUploadId
- [Add] MarkDirectVisualThreadAsSeenAsync to MessagingProcessor
- [Add] MarkActivitesInboxSeenAsync to UserProcessor
- [Add] MarkDiscoverMarkSuSeenAsync to UserProcessor
- [Add] SetBirthdayAsync to AccountProcessor
- [Add] SetGenderAsync to AccountProcessor
- [Add] UserNotFound to ResponseType
- [Add] UserNotFound check to GetFullUserInfoAsync
- [Add] PrivateMedia to ResponseType enum
- [Add] PrivateMedia support to GetMediaIdFromUrl
- [Add] NoMediaMatch to ResponseType enum
- [Add] NoMediaMatch support to GetMediaIdFromUrl
- [Drop] support .net standard 1.3 and uwp and .net framwork v4.5.2! from now on only .NET Standard v2.0 is supported [.net framework 4.6.1 and newer | for uwp 16299 sdk and newer is supported]
- [Support] ffmpeg for netstandard project

v2.3.0
- [Update] signature and api version and headers to v146.0.0.27.125
  Also added:
	v136.0.0.34.124
	v130.0.0.31.121
	v129.0.0.29.119

- [Update] FollowUserAsync & UnFollowUserAsync
- [Update] LikeMediaAsync and UnLikeMediaAsync
- [Update] GetThreadToken
- [Update] some direct functions
- [Update] UploadSegmentedVideoToTVAsync
- [Add] SendDirectArEffectAsync to MessagingProcessor
- [Add] ArEffect support to InstaDirectInboxItem class
- [Add] ProductShare support to InstaDirectInboxItem class
- [Add] SendDirectProductAsync to MessaginProcessor
- [Add] LogoutSessionAsync to AccountProcessor
- [Add] AcceptSessionAsMeAsync to AccountProcessor
- [Add] GetLoginSessionsAsync to AccountProcessor
- [Add] GetPendingUserTagsAsync to AccountProcessor
- [Add] ApproveUsertagsAsync to AccountProcessor
- [Add] DisableManualTagAsync to AccountProcessor
- [Add] EnableManualTagAsync to AccountProcessor
- [Add] HideUsertagFromProfileAsync to AccountProcessor
- [Add] UnlinkContactsAsync to AccountProcessor
- [Add] GetPendingUserTagsCountAsync to AccountProcessor
- [Add] GetUserStoryAndLivesAsync to StoryProcessor
- [Add] UploadSegmentedVideoAsync to MediaProcessor
- [Add] ResizeImage to FFmpegFa class
- [Add] MultipleAccountExample
- [Update] IGTVUploadExample


v2.2.0
- [Update] signature and api version to v126.0.0.25.121
  Also added v124.0.0.17.473

- [Update] Login flow functions
- [Update] headers
------- FRESH LOGIN IS REQUIRED! -------

v2.1.0
- Updated api key and signature and parameters to v123.0.0.21.114 and it's default now 
	Also added:
	v121.0.0.29.119
	v117.0.0.28.123

- [Bugfix] for IGBandwidthSpeedKbps generator
- [Bugfix] for SetHttpRequestProcessor
- [Bugfix] for converting Json 
- [Bugfix] Json response is not including (maybe in some cases) the field "phone_number" {thx to @sopranidaniele }
- [Update] UploadStoryPhotoWithUrlAsync, UploadStoryVideoWithUrlAsync [story link]
- [Update] IInstaLogger to prevent happening this exception: ("An asynchronous module or handler completed while an asynchronous operation was still pending.")
- [Update] Login parameters
- [Update] SendDirectLocationAsync, SendDirectLinkAsync
- [Update] GetUserMedia....
- [Update] MarkDirectThreadAsSeenAsync
- [Update] SendDirectFelixShareAsync
- [Update] SendDirectHashtagAsync
- [Update] SendDirectLinkAsync
- [Update] SendDirectLocationAsync
- [Update] SendDirectProfileAsync
- [Update] SendDirectProfileToRecipientsAsync
- [Update] SendDirectTextAsync
- [Update] ShareMediaToThreadAsync, ShareMediaToUserAsync
- [Update] UnLikeThreadMessageAsync
- [Update] SendDirectLikeAsync
- [Update] SendDirectPhoto
- [Update] SendDirectAnimatedMedia
- [Update] LiveProcessor.CommentAsync
- [Update] LiveProcessor.CreateAsync
- [Update] LiveProcessor.EndAsync
- [Update] LiveProcessor.GetHeartBeatAndViewerCountAsync
- [Update] LiveProcessor.StartAsync
- [Add] UnusablePassword to InstaLoginResult
- [Add] GetRecentFollowersAsync to UserProcessor
- [Add] DismissUserSuggestionAsync to DiscoverProcessor
- [Add] GetThreadByParticipantsAsync to MessagingProcessor
- [Add] CreateGroupAsync to MessagingProcessor
- [Add] RemoveUserFromGroupAsync to MessagingProcessor
- [Add] AddNewGroupAdminAsync to MessagingProcessor
- [Add] RemoveGroupAdminAsync to MessagingProcessor
- [Add] DisableApprovalForJoiningDirectThreadAsync to MessagingProcessor
- [Add] EnableApprovalForJoiningDirectThreadAsync to MessagingProcessor
- [Add] EndChatDirectThreadAsync to MessagingProcessor
- [Add] Pagination to SearchPeopleAsync function
- [Add] MarkHashtagStoryAsSeenAsync to HashtagProcessor 

v2.0.9
- [Bugfix] for facebook login with 2fa enabled
- [Bugfix] for 2factor login
- [Bugfix] for DeleteMediaAsync
- [Update] GetPendingFriendRequestsAsync
- [Update] SendDirectLinkAsync response type
- [Update] GetSuggestedSearchesAsync Uri
- [Update] GetShareLinkFromMediaIdAsync Uri
- [Update] GetChannelByIdAsync, GetChannelByTypeAsync functions
- [Add] GetSecuritySettingsInfoAsync converter [Support trusted devices]
- [Add] RemoveTrustedDeviceAsync to AccountProcessor
- [Add] BrowseFeedAsync to TVProcessor
- [Add] MarkAsSeenAsync to TVProcesser

v2.0.8
- [Update] signature key and API version 100.0.0.17.129 as default 
- [Set] latest ApiVersion automatically [For disable this option just set InstaApi.LoadApiVersionFromSessionFile to TRUE, it's recommended to always be FALSE]
- [Update] GetDirectInboxAsync uri
- [Update] EnableMediaCommentAsync, DisableMediaCommentAsync
- [Update] EditMediaAsync
- [Update] ArchiveMediaAsync, UnArchiveMediaAsync, LikeMediaAsync, UnLikeMediaAsync
- [Update] UploadAlbumAsync
- [Update] UploadPhotoAsync
- [Update] UploadVideoAsync
- [Add] SendReactionToStoryAsync to StoryProcessor
- [Add] StoryChatRequestAsync to StoryProcessor
- [Add] CancelStoryChatRequestAsync to StoryProcessor
- [Add] support for uploading Story Chats to InstaStoryUploadOptions class
- [Add] StoryChats support to InstaStoryItem class
- [Add] StoryChatRequestInfos support to InstaStoryItem class
- [Add] SetChallenge function to IInstaApi [by @NGame1]
- [Add] ViewerAnswer property to InstaStoryQuizStickerItem class
- [Add] ViewerVote property to InstaStorySliderStickerItem class
- [Add] BackgroundColor property to InstaStorySliderStickerItem class
- [Add] some new properties to InstaDirectInboxThread class
- [Remove] UWP support from ISessionHandler

v2.0.7
- [Bugfix] for Caption converter
- [Update] recent activity uri
- [Update] CommentMediaAsync
- [Update] GetUsersStoriesAsHighlightsAsync
- [Update] GetTopicalExploreFeed Uri
- [Update] EditProfileAsync
- [Update] SetBiographyAsync
- [Update] ConfigureMediaDelay time
- [Update] InstaExploreClusterType
- [Change] SendDirectTextAsync response object
- [Add] InstaLinkType for InstaLink class
- [Add] UpdateUser to IInstaApi
- [Add] Order support to GetUserFollowingAsync and GetUserFollowingByIdAsync
- [Add] GetHashtagChannelVideosAsync to HashtagProcessor 
- [Add] GetExploreChannelVideosAsync to FeedProcessor
- [Add] tiny FFmpeg wrapper for .net framework
- [Add] SetConfigureMediaDelay to IInstaApi and IInstaApiBuilder [for setting up delay before configuring media]
- [Add] Birthday consent support for AcceptConsentAsync function
- [Add] UploadSegmentedVideoToTVAsync to TVProcessor [Check IGTVUploader example project]

v2.0.6
- [Update] TwoFactorLoginAsync to latest version
- [Update] FollowUserAsync
- [Update] UnFollowUserAsync
- [Rename] MuteDirectThreadAsync to MuteDirectThreadMessagesAsync
- [Rename] UnMuteDirectThreadAsync to UnMuteDirectThreadMessagesAsync
- [Add] MuteDirectThreadVideoCallsAsync to MessagingProcessor
- [Add] UnMuteDirectThreadVideoCallsAsync to MessagingProcessor
- [Add] GetApiVersionType to IInstaApi
- [Add] AcceptConsentAsync to IInstaApi [for consent_required]
- [Add] SuggestedUserCardsItems to InstaPost class for new suggestions
- [Add] new properties to InstaActivityFeed class
- [Add] clarify types to InstaRecentActivityFeed class
- [Add] support for friend request in InstaRecentActivityFeed class

v2.0.5
- [Bugfix] for ChangeProfilePictureAsync
- [Update] LoginAsync
- [Add] new headers
- [Add] some new properties to AndroidDevice class
- [Dropping] support for Android v4.4 and lower in AndroidVersionList (since instagram is no longer supporting them)

v2.0.4
- [Bugfix] for DeleteSelfMessageAsync
- [Bugfix] for GetUserTimelineFeed
- [Update] LoginAsync
- [Add] SendRequestsBeforeLoginAsync to IInstaApi [login flows: contact prefill, read msisdn header, launcher sync and qe sync requests]

v2.0.3
- [Rename] ProfilePreviewMedias to PreviewMedias in InstaDirectInboxItem class [supports preview medias for hashtags, location, profiles and etc.]
- [Bugfix] for SendDirectTextAsync
- [Bugfix] for SendDirectPhotoAsync and SendDirectPhotoToRecipientsAsync
- [Add] Title property to InstaPlaceholder class
- [Add] some new properties to InstaHashtag class
- [Add] GetHashtagsSectionsAsync to HashtagProcessor
- [Add] public modifier to HttpHelper class
- [Add] HttpHelper property to IInstaApi

v2.0.2
- [Add] support for StoryQuiz in InstaStoryItem class
- [Add] support for StoryQuizsParticipantInfos in InstaStoryItem class
- [Add] support StoryQuiz in InstaStoryUploadOptions class for uploading
- [Add] AnswerToStoryQuizAsync to StoryProcessor

v2.0.1
- API Version updated to 94.0.0.22.116

v2.0.0
- Updated to API Version 91.0.0.18.118 [api version and signature key]
- [Add] SendVoiceAsync to MessagingProcessor
- [Add] SendDirectAnimatedMediaAsync to MessagingProcessor
- [Add] Giphy api (for animated media [gif files])
- [Add] Pigeon to requests (requires to save the session after you logged in)
- [Add] GZip compressor for some requests
- [Add] LauncherSyncAsync (for logins)
- [Add] DismissSuggestionAsync to DiscoverProcessor
- [Add] ReportHashtagMediaAsync to HashtagProcessor
- [Add] ExploreReportAsync to DiscoverProcessor
- [Add] GetHashtagsPostsAsync to HashtagProcessor
- [Add] GetUsersStoriesAsHighlightsAsync to StoryProcessor [for getting multiple users highlights/stories]
- [Add] GetMutualFriendsOrSuggestionAsync to UserProcessor
- [Add] GetBanyanSuggestionsAsync
- [Add] GetStoryFeedWithPostMethodAsync to StoryProcessor
- [Add] MarkMultipleStoriesAsSeenAsync to StoryProcessor
- [Add] ReplyToStoryAsync [text message] to StoryProcessor
- [Add] ReplyPhotoToStoryAsync to StoryProcessor
- [Add] HideSearchEntityAsync to DiscoverProcessor
- [Add] GetDynamicSearchesAsync to DiscoverProcessor
- [Add] GetSuggestionDetailsAsync to UserProcessor
- [Add] SearchPlacesAsync(string, PaginationParameters) to LocationProcessor
- [Add] Some new properties to InstaDirectInboxThread class
- [Add] Some new properties to InstaMedia class
- [Add] Some new properties to InstaFeed class
- [Add] InstaSectionMedia class
- [Add] InstaPost class to UserTimeline response class
- [Add] Url property to InstaMedia class
- [Add] ChainingSuggestions property to InstaUserInfo class
- [Update/Bugfix] SendDirectTextAsync
- [Update] LogoutAsync
- [Update] GetRecoveryOptionsAsync
- [Update] SendRecoveryByEmailAsync
- [Update] SendRecoveryByPhoneAsync
- [Update] GetUserTimelineFeed
- [Update] GetSecuritySettingsInfoAsync
- [Update] TwoFactorEnableAsync
- [Update] ShareStoryAsync
- [Update] GetChainingUsersAsync
- [Update] GetHighlightFeedsAsync
- [Update] InstaHashtag class

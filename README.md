Private version of [InstagramApiSharp](https://github.com/ramtinak/InstagramApiSharp) ![InstagramApiSharp](http://s8.picofile.com/file/8336601292/insta50x.png)



| Target | Branch | Version | Download |
| ------ | ------ | ------ | ------ |
| Github | master | v2.5.0 | [v2.5.0](https://github.com/rmt4006/InstagramApiSharpMine/archive/master.zip) |
| Release | master | v2.5.0 | [v2.5.0](https://github.com/rmt4006/InstagramApiSharpMine/releases/) |

#### How to build the project?
Check this youtube video: https://www.youtube.com/watch?v=AFjKryHz9nM

#### Why Two Solutions ?
Since Push notifications and RealtimeClient has a lot of external libraries, I decided to exlcude it from original project.

[InstagramApiSharp.sln] -> Doesn't support Push notifcations/realtime client, so you don't need to add any external libraries except Portable.BouncyCastle.

[InstagramApiSharp.WithNotification.sln] -> Support Push notifcations/realtime client, so you HAVE to add any external libraries except Portable.BouncyCastle.

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

- Note 1: You MUST reference [Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle/)'s package to your projects.
- Note 2: You MUST reference Thrift's project (InstagramApiSharp's port) to your project.
- Note 3: All other realtime/push libraries is not necessarily IF YOU DON'T WANT TO USE PUSH NOTIFICATIONS/REAL TIME CLIENT.


#### Version changes
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

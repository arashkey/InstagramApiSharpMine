Private version of [InstagramApiSharp](https://github.com/ramtinak/InstagramApiSharp) ![InstagramApiSharp](http://s8.picofile.com/file/8336601292/insta50x.png)



| Target | Branch | Version | Download |
| ------ | ------ | ------ | ------ |
| Github | master | v2.0.9 | [v2.0.9](https://github.com/ramtinak/InstagramApiSharpMine/archive/master.zip) |
| Release | master | v2.0.9 | [v2.0.9](https://github.com/ramtinak/InstagramApiSharpMine/releases/) |

#### How to build the project?
Check this youtube video: https://www.youtube.com/watch?v=AFjKryHz9nM

#### Version changes
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
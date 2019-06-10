Private version of [InstagramApiSharp](https://github.com/ramtinak/PrivateInstagramApiSharp) ![InstagramApiSharp](http://s8.picofile.com/file/8336601292/insta50x.png)



| Target | Branch | Version | Changelog |
| ------ | ------ | ------ | ------ |
| Github | master | v2.0.3 | [v2.0.3](https://github.com/ramtinak/InstagramApiSharp/issues/244#issuecomment-500412428) |
| Release | master | v2.0.3 | [v2.0.3](https://github.com/ramtinak/InstagramApiSharpMine/releases/) |

#### Version changes
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
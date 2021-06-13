using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Converters.Json;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Story api functions.
    /// </summary>
    internal class StoryProcessor : IStoryProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public StoryProcessor(AndroidDevice deviceInfo, UserSessionData user,
            IHttpRequestProcessor httpRequestProcessor, IInstaLogger logger,
            UserAuthValidate userAuthValidate, InstaApi instaApi, HttpHelper httpHelper)
        {
            _deviceInfo = deviceInfo;
            _user = user;
            _httpRequestProcessor = httpRequestProcessor;
            _logger = logger;
            _userAuthValidate = userAuthValidate;
            _instaApi = instaApi;
            _httpHelper = httpHelper;
        }

        /// <summary>
        ///     Get user story and lives
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        public async Task<IResult<InstaStoryAndLives>> GetUserStoryAndLivesAsync(long userId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var userStoryUri = UriCreator.GetUserStoryAndLivesUri(userId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userStoryUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) Result.UnExpectedResponse<InstaStory>(response, json);
                var userStoryResponse = JsonConvert.DeserializeObject<InstaStoryAndLivesResponse>(json);
                var userStory = ConvertersFabric.Instance.GetStoryConverter(userStoryResponse).Convert();
                return Result.Success(userStory);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryAndLives), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryAndLives>(exception);
            }
        }
        /// <summary>
        ///     Request for joing chats from story
        /// </summary>
        /// <param name="storyChatId">Story chat id (<see cref="InstaStoryChatStickerItem.StoryChatId"/>)</param>
        public async Task<IResult<bool>> StoryChatRequestAsync(long storyChatId)
        {
            return await RequestOrCancelStoryChat(UriCreator.GetStoryChatRequestUri(), storyChatId);
        }
        /// <summary>
        ///     Cancel story chat request
        /// </summary>
        /// <param name="storyChatId">Story chat id (<see cref="InstaStoryChatStickerItem.StoryChatId"/>)</param>
        public async Task<IResult<bool>> CancelStoryChatRequestAsync(long storyChatId)
        {
            return await RequestOrCancelStoryChat(UriCreator.GetStoryChatCancelRequestUri(), storyChatId);
        }
        public async Task<IResult<bool>> ReplyPhotoToStoryAsync(InstaImageUpload image, /*string storyMediaId,*/ long userId)
        {
            return await ReplyPhotoToStoryAsync(null, image, /*storyMediaId,*/ userId);
        }
        public async Task<IResult<bool>> ReplyPhotoToStoryAsync(Action<InstaUploaderProgress> progress, InstaImageUpload image,
            /*string storyMediaId,*/ long userId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                var uploadId = ApiRequestMessage.GenerateUnknownUploadId();
                var uploadResult = await _instaApi.HelperProcessor.UploadSinglePhoto(progress, image, upProgress, uploadId, false, userId.ToString());
                if (!uploadResult.Succeeded)
                    Result.Fail(uploadResult.Info, false);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                Random rnd = new Random();
                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"allow_multi_configures", "1"},
                    {"recipient_users", $"[[{userId}]]"},
                    {"view_mode", "replayable"},
                    {"thread_ids", "[]"},
                    {"client_context", Guid.NewGuid().ToString()},
                    {"camera_session_id", Guid.NewGuid().ToString()},
                    {"reply_type", "story"},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"client_shared_at", (long.Parse(ApiRequestMessage.GenerateUploadId())- rnd.Next(25,55)).ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"configure_mode", "2"},
                    {"source_type", "3"},
                    {"creation_surface", "camera"},
                    {"capture_type", "normal"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"client_timestamp", ApiRequestMessage.GenerateUploadId()},
                    {"sampled", "true"},
                    {"upload_id", uploadId},
                    {
                        "extra", JsonConvert.SerializeObject(new JObject
                        {
                            {"source_width", 0},
                            {"source_height", 0}
                        })
                    },
                    {
                        "device", JsonConvert.SerializeObject(new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", _deviceInfo.AndroidVer.APILevel}
                        })
                    }
                };
                var instaUri = UriCreator.GetVideoStoryConfigureUri();
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("retry_context", HelperProcessor.GetRetryContext(), _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, obj.Message, null);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }


        /// <summary>
        ///     Answer to an story quiz
        /// </summary>
        /// <param name="storyPk">Story pk (<see cref="InstaStoryItem.Pk"/>)</param>
        /// <param name="quizId">Quiz id (<see cref="InstaStoryQuizParticipant.QuizId"/>)</param>
        /// <param name="answer">Your answer</param>
        public async Task<IResult<bool>> AnswerToStoryQuizAsync(long storyPk, long quizId, int answer)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetAnswerToStoryQuizUri(storyPk, quizId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"answer", answer.ToString()}
                };
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, obj.Message, null);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, false, ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, false);
            }
        }



        /// <summary>
        ///     Respond to an story question
        /// </summary>
        /// <param name="storyId">Story id (<see cref="InstaStoryItem.Id"/>)</param>
        /// <param name="questionId">Question id (<see cref="InstaStoryQuestionStickerItem.QuestionId"/>)</param>
        /// <param name="responseText">Text to respond</param>
        public async Task<IResult<bool>> AnswerToStoryQuestionAsync(string storyId, long questionId, string responseText)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetStoryQuestionResponseUri(storyId, questionId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"response", responseText ?? string.Empty},
                    {"type", "text"}
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, obj.Message, null);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, false, ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, false);
            }
        }

        /// <summary>
        ///     Append to existing highlight
        /// </summary>
        /// <param name="highlightId">Highlight id</param>
        /// <param name="mediaId">Media id (CoverMedia.MediaId)</param>
        public async Task<IResult<bool>> AppendToHighlightFeedAsync(string highlightId, string mediaId)
        {
            return await AppendOrDeleteHighlight(highlightId, mediaId, false);
        }

        /// <summary>
        ///     Create new highlight
        /// </summary>
        /// <param name="mediaId">Story media id</param>
        /// <param name="title">Highlight title</param>
        /// <param name="cropWidth">Crop width It depends on the aspect ratio/size of device display and the aspect ratio of story uploaded. must be in a range of 0-1, i.e: 0.19545822</param>
        /// <param name="cropHeight">Crop height It depends on the aspect ratio/size of device display and the aspect ratio of story uploaded. must be in a range of 0-1, i.e: 0.8037307</param>
        public async Task<IResult<InstaHighlightFeed>> CreateHighlightFeedAsync(string mediaId, string title, float cropWidth, float cropHeight)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var cover = new JObject
                {
                    {"media_id", mediaId},
                    {"crop_rect", new JArray { 0.0, cropWidth, 1.0, cropHeight }.ToString(Formatting.None) }
                }.ToString(Formatting.None);
                var data = new JObject
                {
                    {"source", "self_profile"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"cover", cover},
                    {"title", title},
                    {"media_ids", $"[{ExtensionHelper.EncodeList(new[] { mediaId })}]"}
                };

                var instaUri = UriCreator.GetHighlightCreateUri();
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaHighlightFeed>(response, json);

                var highlightFeedResponse = JsonConvert.DeserializeObject<InstaHighlightReelResponse>(json,
                    new InstaHighlightReelDataConverter());
                var highlightStoryFeed = ConvertersFabric.Instance.GetHighlightReelConverter(highlightFeedResponse).Convert();
                return Result.Success(highlightStoryFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHighlightFeed), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHighlightFeed>(exception);
            }
        }

        /// <summary>
        ///     Delete highlight feed
        /// </summary>
        /// <param name="highlightId">Highlight id</param>
        /// <param name="mediaId">Media id (CoverMedia.MediaId)</param>
        public async Task<IResult<bool>> DeleteHighlightFeedAsync(string highlightId, string mediaId)
        {
            return await AppendOrDeleteHighlight(highlightId, mediaId, true);
        }

        /// <summary>
        ///     Delete a media story (photo or video)
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="sharingType">The type of the media</param>
        /// <returns>Return true if the story media is deleted</returns>
        public async Task<IResult<bool>> DeleteStoryAsync(string storyMediaId, InstaSharingType sharingType = InstaSharingType.Video)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var deleteMediaUri = UriCreator.GetDeleteStoryMediaUri(storyMediaId, sharingType);

                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_csrftoken", _user.CsrfToken},
                    {"media_id", storyMediaId}
                };

                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, deleteMediaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                var deletedResponse = JsonConvert.DeserializeObject<DeleteResponse>(json);
                return Result.Success(deletedResponse.IsDeleted);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Follow countdown stories
        /// </summary>
        /// <param name="countdownId">Countdown id (<see cref="InstaStoryCountdownStickerItem.CountdownId"/>)</param>
        public async Task<IResult<bool>> FollowCountdownStoryAsync(long countdownId)
        {
            return await FollowUnfollowCountdown(UriCreator.GetStoryFollowCountdownUri(countdownId));
        }

        /// <summary>
        ///     Get list of users that blocked from seeing your stories
        /// </summary>
        public async Task<IResult<InstaUserShortList>> GetBlockedUsersFromStoriesAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var list = new InstaUserShortList();
            try
            {
                var instaUri = UriCreator.GetBlockedStoriesUri();
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                };
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserShortList>(response, json);

                var usersResponse = JsonConvert.DeserializeObject<InstaUserListShortResponse>(json);
                list.AddRange(
                    usersResponse.Items.Select(ConvertersFabric.Instance.GetUserShortConverter)
                        .Select(converter => converter.Convert()));
                return Result.Success(list);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, list, ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, list);
            }
        }

        /// <summary>
        ///     Get stories countdowns for self accounts
        /// </summary>
        public async Task<IResult<InstaStoryCountdownList>> GetCountdownsStoriesAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetStoryCountdownMediaUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStoryCountdownList>(response, json);
                var countdownListResponse = JsonConvert.DeserializeObject<InstaStoryCountdownListResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetStoryCountdownListConverter(countdownListResponse).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryCountdownList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryCountdownList>(exception);
            }
        }

        /// <summary>
        ///     Get user highlight feeds by user id (pk)
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        public async Task<IResult<InstaHighlightFeeds>> GetHighlightFeedsAsync(long userId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetHighlightFeedsUri(userId, _deviceInfo.PhoneGuid.ToString());
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaHighlightFeeds>(response, json);
                var highlightFeedResponse = JsonConvert.DeserializeObject<InstaHighlightFeedsResponse>(json);
                var highlightStoryFeed = ConvertersFabric.Instance.GetHighlightFeedsConverter(highlightFeedResponse).Convert();
                return Result.Success(highlightStoryFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHighlightFeeds), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHighlightFeeds>(exception);
            }
        }

        /// <summary>
        ///     Get user highlights archive
        ///     <para>Note: Use <see cref="IStoryProcessor.GetHighlightsArchiveMediasAsync(string)"/> to get hightlight medias of an specific day.</para>
        /// </summary>
        public async Task<IResult<InstaHighlightShortList>> GetHighlightsArchiveAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetHighlightsArchiveUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaHighlightShortList>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaHighlightShortListResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetHighlightShortListConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHighlightShortList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHighlightShortList>(exception);
            }
        }
        public async Task<IResult<InstaUserStoriesFeeds>> GetUsersStoriesAsHighlightsAsync(params string[] usersIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetReelMediaUri();

                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"source", "reel_feed_timeline"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"user_ids", new JArray(usersIds)}
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaUserStoriesFeeds>(response, json);


                var obj = JsonConvert.DeserializeObject<InstaUserStoriesFeedsResponse>(json,
                    new InstaUserStoriesFeedsDataConverter());

                if(obj != null)
                {
                    var reels = new InstaUserStoriesFeeds();
                    foreach (var item in obj.Items)
                    {
                        try
                        {
                            reels.Items.Add(ConvertersFabric.Instance.GetReelFeedConverter(item).Convert());
                        }
                        catch { }
                    }
                    return Result.Success(reels);
                }

                return Result.Fail<InstaUserStoriesFeeds>("No reels found");
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaUserStoriesFeeds), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaUserStoriesFeeds>(exception);
            }
        }
        /// <summary>
        ///     Get highlights archive medias
        ///     <para>Note: get highlight id from <see cref="IStoryProcessor.GetHighlightsArchiveAsync"/></para>
        /// </summary>
        /// <param name="highlightId">Highlight id (Get it from <see cref="IStoryProcessor.GetHighlightsArchiveAsync"/>)</param>
        public async Task<IResult<InstaHighlightSingleFeed>> GetHighlightsArchiveMediasAsync(string highlightId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (string.IsNullOrEmpty(highlightId))
                    throw new ArgumentNullException("highlightId cannot be null or empty");

                var instaUri = UriCreator.GetReelMediaUri();

                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"source", "reel_highlights_gallery"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"user_ids", new JArray(highlightId)}
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaHighlightSingleFeed>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaHighlightReelResponse>(json,
                    new InstaHighlightReelsListDataConverter());

                return obj?.Reel != null ? Result.Success(ConvertersFabric.Instance.GetHighlightReelConverter(obj).Convert()) : Result.Fail<InstaHighlightSingleFeed>("No reels found");
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHighlightSingleFeed), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHighlightSingleFeed>(exception);
            }
        }

        /// <summary>
        ///     Get single highlight medias
        ///     <para>Note: get highlight id from <see cref="IStoryProcessor.GetHighlightFeedsAsync(long)"/></para>
        /// </summary>
        /// <param name="highlightId">Highlight id (Get it from <see cref="IStoryProcessor.GetHighlightFeedsAsync(long)"/>)</param>
        public async Task<IResult<InstaHighlightSingleFeed>> GetHighlightMediasAsync(string highlightId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (string.IsNullOrEmpty(highlightId))
                    throw new ArgumentNullException("highlightId cannot be null or empty");

                var instaUri = UriCreator.GetReelMediaUri();
                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"source", "profile"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"user_ids", new JArray(highlightId)}
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaHighlightSingleFeed>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaHighlightReelResponse>(json,
                    new InstaHighlightReelsListDataConverter());

                return obj?.Reel != null ? Result.Success(ConvertersFabric.Instance.GetHighlightReelConverter(obj).Convert()) : Result.Fail<InstaHighlightSingleFeed>("No reels found");
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHighlightSingleFeed), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHighlightSingleFeed>(exception);
            }
        }
        /// <summary>
        ///     Get user story feed (stories from users followed by current user).
        /// </summary>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var storyFeedUri = UriCreator.GetStoryFeedUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, storyFeedUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaStoryFeed>(response, json);
                var storyFeedResponse = JsonConvert.DeserializeObject<InstaStoryFeedResponse>(json);
                var instaStoryFeed = ConvertersFabric.Instance.GetStoryFeedConverter(storyFeedResponse).Convert();
                return Result.Success(instaStoryFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryFeed), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryFeed>(exception);
            }
        }

        /// <summary>
        ///     Get user story feed with POST method requests (new API)
        /// </summary>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(bool refresh = false, string[] preloadedReelIds = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var storyFeedUri = UriCreator.GetStoryFeedUri();
                var data = new Dictionary<string, string>
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                if (refresh)
                    data.Add("reason", "pull_to_refresh");
                else
                    data.Add("reason", "cold_start");

                if (preloadedReelIds?.Length > 0)
                    data.Add("preloaded_reel_ids", string.Join(",", preloadedReelIds));

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, storyFeedUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaStoryFeed>(response, json);
                var storyFeedResponse = JsonConvert.DeserializeObject<InstaStoryFeedResponse>(json);
                var instaStoryFeed = ConvertersFabric.Instance.GetStoryFeedConverter(storyFeedResponse).Convert();
                return Result.Success(instaStoryFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryFeed), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryFeed>(exception);
            }
        }


        /// <summary>
        ///     Get user story feed with POST method requests and Pagination support (new API)
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="forceRefresh">Force to use pull refresh</param>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(PaginationParameters paginationParameters,
            bool forceRefresh = false) =>
            await GetStoryFeedWithPostMethodAsync(paginationParameters, CancellationToken.None, forceRefresh).ConfigureAwait(forceRefresh);

        /// <summary>
        ///     Get user story feed with POST method requests and Pagination support (new API)
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="forceRefresh">Force to use pull refresh</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(PaginationParameters paginationParameters, 
            CancellationToken cancellationToken, bool forceRefresh = false)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaStoryFeedResponse storyFeedResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                bool HasMore(PaginationParameters pagination) => !string.IsNullOrEmpty(pagination.SessionId) &&
                    pagination.NextIdsToFetch?.Count > 0 && int.TryParse(pagination.NextMaxId, out int count) && count <= pagination.NextIdsToFetch?.Count;

                var feedResponse = await GetStoryFeedWithPostMethod(paginationParameters, forceRefresh);
                if (!feedResponse.Succeeded)
                {
                    if (feedResponse.Value != null)
                        Result.Fail(feedResponse.Info, Convert(feedResponse.Value));
                    else
                        Result.Fail(feedResponse.Info, default(InstaStoryFeed));
                }
                if (feedResponse.Value == null)
                    Result.Fail(feedResponse.Info, default(InstaStoryFeed));
                
                storyFeedResponse = feedResponse.Value;

                while (HasMore(paginationParameters)
                    && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var moreMedias = await GetStoryFeedWithPostMethod(paginationParameters, false);
                    if (!moreMedias.Succeeded)
                    {
                        if (storyFeedResponse.Tray?.Count > 0)
                            return Result.Success(Convert(storyFeedResponse));
                        else
                            return Result.Fail(moreMedias.Info, Convert(storyFeedResponse));
                    }
                    if (moreMedias.Value.Tray?.Count > 0)
                    {
                        if (storyFeedResponse.Tray == null)
                            storyFeedResponse.Tray = new List<JToken>();

                        storyFeedResponse.Tray.AddRange(moreMedias.Value.Tray);
                    }
                    //feedResponse.Value.MoreAvailable = moreMedias.Value.MoreAvailable;
                    //feedResponse.Value.NextMaxId = paginationParameters.NextMaxId = moreMedias.Value.NextMaxId;
                    //feedResponse.Value.AutoLoadMoreEnabled = moreMedias.Value.AutoLoadMoreEnabled;
                    //feedResponse.Value.NextMediaIds = paginationParameters.NextMediaIds = moreMedias.Value.NextMediaIds;
                    //feedResponse.Value.NextPage = paginationParameters.NextPage = moreMedias.Value.NextPage;
                    //feedResponse.Value.Sections.AddRange(moreMedias.Value.Sections);
                    paginationParameters.PagesLoaded++;
                }

                return Result.Success(GetOrDefault());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, GetOrDefault(), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, GetOrDefault());
            }

            InstaStoryFeed GetOrDefault() => storyFeedResponse != null ? Convert(storyFeedResponse) : default(InstaStoryFeed);

            InstaStoryFeed Convert(InstaStoryFeedResponse instaStoryFeedResponse)
            {
                return ConvertersFabric.Instance.GetStoryFeedConverter(instaStoryFeedResponse).Convert();
            }
        }

        /// <summary>
        ///     Get story media viewers
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="paginationParameters">Pagination parameters</param>
        public async Task<IResult<InstaReelStoryMediaViewers>> GetStoryMediaViewersAsync(string storyMediaId, PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaReelStoryMediaViewers Convert(InstaReelStoryMediaViewersResponse reelResponse)
                {
                    return ConvertersFabric.Instance.GetReelStoryMediaViewersConverter(reelResponse).Convert();
                }

                var storyMediaViewersResult = await GetStoryMediaViewers(storyMediaId, paginationParameters?.NextMaxId);

                if (!storyMediaViewersResult.Succeeded)
                    return Result.Fail(storyMediaViewersResult.Info, default(InstaReelStoryMediaViewers));

                var storyMediaViewersResponse = storyMediaViewersResult.Value;
                paginationParameters.NextMaxId = storyMediaViewersResponse.NextMaxId;

                while (!string.IsNullOrEmpty(paginationParameters.NextMaxId)
                    && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    paginationParameters.PagesLoaded++;
                    var nextStoryViewers = await GetStoryMediaViewers(storyMediaId, paginationParameters.NextMaxId);
                    if (!nextStoryViewers.Succeeded)
                        return Result.Fail(nextStoryViewers.Info, Convert(nextStoryViewers.Value));
                    storyMediaViewersResponse.NextMaxId = paginationParameters.NextMaxId = nextStoryViewers.Value.NextMaxId;
                    storyMediaViewersResponse.Users.AddRange(nextStoryViewers.Value.Users);
                }

                return Result.Success(Convert(storyMediaViewersResponse));
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaReelStoryMediaViewers), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaReelStoryMediaViewers>(exception);
            }
        }

        /// <summary>
        ///     Get story poll voters
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="pollId">Story poll id</param>
        /// <param name="paginationParameters">Pagination parameters</param>
        public async Task<IResult<InstaStoryPollVotersList>> GetStoryPollVotersAsync(string storyMediaId, string pollId, PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaStoryPollVotersList Convert(InstaStoryPollVotersListResponse storyVotersResponse)
                {
                    return ConvertersFabric.Instance.GetStoryPollVotersListConverter(storyVotersResponse).Convert();
                }

                var votersResult = await GetStoryPollVoters(storyMediaId, pollId, paginationParameters?.NextMaxId);

                if (!votersResult.Succeeded)
                    return Result.Fail(votersResult.Info, default(InstaStoryPollVotersList));

                var votersResponse = votersResult.Value;
                paginationParameters.NextMaxId = votersResponse.MaxId;

                while (votersResponse.MoreAvailable &&
                    !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                    && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    paginationParameters.PagesLoaded++;
                    var nextVoters = await GetStoryPollVoters(storyMediaId, pollId, paginationParameters.NextMaxId);
                    if (!nextVoters.Succeeded)
                        return Result.Fail(nextVoters.Info, Convert(nextVoters.Value));
                    votersResponse.MaxId = paginationParameters.NextMaxId = nextVoters.Value.MaxId;
                    votersResponse.Voters.AddRange(nextVoters.Value.Voters);
                    votersResponse.LatestPollVoteTime = nextVoters.Value.LatestPollVoteTime;
                    votersResponse.MoreAvailable = nextVoters.Value.MoreAvailable;
                }

                return Result.Success(Convert(votersResponse));
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryPollVotersList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryPollVotersList>(exception);
            }
        }


        /// <summary>
        ///     Get the story by userId
        /// </summary>
        /// <param name="userId">User Id</param>
        public async Task<IResult<InstaStory>> GetUserStoryAsync(long userId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var userStoryUri = UriCreator.GetUserStoryUri(userId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userStoryUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode != HttpStatusCode.OK) Result.UnExpectedResponse<InstaStory>(response, json);
                var userStoryResponse = JsonConvert.DeserializeObject<InstaStoryResponse>(json);
                var userStory = ConvertersFabric.Instance.GetStoryConverter(userStoryResponse).Convert();
                return Result.Success(userStory);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStory), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStory>(exception);
            }
        }
        /// <summary>
        ///     Get user story reel feed. Contains user info last story including all story items.
        /// </summary>
        /// <param name="userId">User identifier (PK)</param>
        public async Task<IResult<InstaReelFeed>> GetUserStoryFeedAsync(long userId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var feed = new InstaReelFeed();
            try
            {
                var userFeedUri = UriCreator.GetUserReelFeedUri(userId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userFeedUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaReelFeed>(response, json);
                var feedResponse = JsonConvert.DeserializeObject<InstaReelFeedResponse>(json);
                feed = ConvertersFabric.Instance.GetReelFeedConverter(feedResponse).Convert();
                return Result.Success(feed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaReelFeed), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, feed);
            }
        }
        /// <summary>
        ///     Seen multiple stories
        /// </summary>
        /// <param name="storiesWithTakenAt">Story media identifier with taken at unix times</param>
        public async Task<IResult<bool>> MarkMultipleStoriesAsSeenAsync(Dictionary<string, long> storiesWithTakenAt)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSeenMediaStoryUri();
                var dateTimeUnix = DateTime.UtcNow.ToUnixTime();
                var reel = new JObject
                {
                    //{ storyId, new JArray($"{takenAtUnix}_{dateTimeUnix}") }
                };
                foreach(var item in storiesWithTakenAt)
                {
                    var storyId = $"{item.Key}_{item.Key.Split('_')[1]}";
                    reel.Add(storyId, new JArray($"{item.Value}_{dateTimeUnix}"));
                }
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"container_module", "feed_timeline"},
                    {"live_vods_skipped", new JObject()},
                    {"nuxes_skipped", new JObject()},
                    {"nuxes", new JObject()},
                    {"reels", reel},
                    {"live_vods", new JObject()},
                    {"reel_media_skipped", new JObject()}
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Seen multiple stories
        /// </summary>
        /// <param name="storiesWithTakenAt">Story media identifier with taken at unix times</param>
        public async Task<IResult<bool>> MarkMultipleElectionStoriesAsSeenAsync(List<InstaStoryElectionKeyValue> storiesWithTakenAt)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSeenMediaStoryUri();
                var dateTimeUnix = DateTime.UtcNow.ToUnixTime();
                var reel = new JObject
                {
                };
                foreach (var item in storiesWithTakenAt)
                {
                    var storyId = $"{item.StoryItemId}_{item.StoryId}";
                    reel.Add(storyId, new JArray($"{item.TakenAtUnix}_{dateTimeUnix}"));
                }
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"container_module", "feed_timeline"},
                    {"live_vods_skipped", new JObject()},
                    {"nuxes_skipped", new JObject()},
                    {"nuxes", new JObject()},
                    {"reels", reel},
                    {"live_vods", new JObject()},
                    {"reel_media_skipped", new JObject()}
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }
        /// <summary>
        ///     Seen story
        /// </summary>
        /// <param name="storyMediaId">Story media identifier</param>
        /// <param name="takenAtUnix">Taken at unix</param>
        public async Task<IResult<bool>> MarkStoryAsSeenAsync(string storyMediaId, long takenAtUnix)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSeenMediaStoryUri();
                var storyId = $"{storyMediaId}_{storyMediaId.Split('_')[1]}";
                var dateTimeUnix = DateTime.UtcNow.ToUnixTime();
                var reel = new JObject
                {
                    { storyId, new JArray($"{takenAtUnix}_{dateTimeUnix}") }
                };
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"container_module", "feed_timeline"},
                    {"live_vods_skipped", new JObject()},
                    {"nuxes_skipped", new JObject()},
                    {"nuxes", new JObject()},
                    {"reels", reel},
                    {"live_vods", new JObject()},
                    {"reel_media_skipped", new JObject()}
                };
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Seen highlight
        ///     <para>Get media id from <see cref="InstaHighlightFeed.CoverMedia.MediaId"/></para>
        /// </summary>
        /// <param name="mediaId">Media identifier (get it from <see cref="InstaHighlightFeed.CoverMedia.MediaId"/>)</param>
        /// <param name="highlightId">Highlight id</param>
        /// <param name="takenAtUnix">Taken at unix</param>
        public async Task<IResult<bool>> MarkHighlightAsSeenAsync(string mediaId, string highlightId, long takenAtUnix)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSeenMediaStoryUri();
                var reelId = $"{mediaId}_{highlightId}";
                var dateTimeUnix = DateTime.UtcNow.ToUnixTime();

                var reel = new JObject
                {
                    { reelId, new JArray($"{takenAtUnix}_{dateTimeUnix}") }
                };
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"container_module", "profile"},
                    {"live_vods_skipped", new JObject()},
                    {"nuxes_skipped", new JObject()},
                    {"nuxes", new JObject()},
                    {"reels", reel},
                    {"live_vods", new JObject()},
                    {"reel_media_skipped", new JObject()}
                };
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Send reaction to an story
        /// </summary>
        /// <param name="storyOwnerUserId">Story owner user id/pk</param>
        /// <param name="storyMediaId">Story media identifier</param>
        /// <param name="reactionEmoji">Reaction emoji</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendReactionToStoryAsync(long storyOwnerUserId, string storyMediaId, string reactionEmoji)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastReelReactUri();
                var token = Guid.NewGuid().ToString();
                var data = new JObject
                {
                    {"recipient_users", $"[[{storyOwnerUserId}]]"},
                    {"action", "send_item"},
                    {"client_context", token},
                    {"media_id", storyMediaId},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"text", reactionEmoji},
                    {"device_id", _deviceInfo.DeviceId},
                    {"mutation_token", token},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"entry", "reel"},
                    {"reaction_emoji", reactionEmoji},
                    {"reel_id", storyOwnerUserId.ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaDirectRespondPayload>(response, json);
                var result = JsonConvert.DeserializeObject<InstaDirectRespondResponse>(json);

                return result.IsSucceed ? Result.Success(ConvertersFabric.Instance
                    .GetDirectRespondConverter(result).Convert().Payload) : Result.Fail<InstaDirectRespondPayload>(result.StatusCode);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaDirectRespondPayload), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaDirectRespondPayload>(exception);
            }
        }
        /// <summary>
        ///     Share an media to story
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="mediaStoryUpload">
        ///     Media options
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </param>
        public async Task<IResult<InstaStoryMedia>> ShareMediaAsStoryAsync(InstaImage image, InstaMediaStoryUpload mediaStoryUpload)
        {
            return await ShareMediaAsStoryAsync(null, image, mediaStoryUpload);
        }

        /// <summary>
        ///     Share an media to story with progress
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="mediaStoryUpload">
        ///     Media options
        ///     <para>
        ///     Note 1: You must draw whatever you want in your image first! 
        ///     Also it's on you to calculate clickable media but mostly is 0.5 for width and height
        ///     </para>
        ///     <para>
        ///     Note 2: Get media pk from <see cref="InstaMedia.Pk"/>
        ///     </para>
        /// </param>
        public async Task<IResult<InstaStoryMedia>> ShareMediaAsStoryAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            InstaMediaStoryUpload mediaStoryUpload)
        {
            if (image == null)
                return Result.Fail<InstaStoryMedia>("Image cannot be null");

            if (mediaStoryUpload == null)
                return Result.Fail<InstaStoryMedia>("Media story upload option cannot be null");

            return await UploadStoryPhotoWithUrlAsync(progress, image, null, new InstaStoryUploadOptions { MediaStory = mediaStoryUpload });
        }

        /// <summary>
        ///     Share story to someone
        /// </summary>
        /// <param name="reelId">Reel id</param>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="threadId">Thread id</param>
        /// <param name="text">Text to send (optional</param>
        /// <param name="sharingType">Sharing type</param>
        public async Task<IResult<bool>> ShareStoryAsync(string reelId, string storyMediaId, string[] threadIds, long[] recipients, string text, InstaSharingType sharingType = InstaSharingType.Video)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetStoryShareUri(sharingType.ToString().ToLower());
                var guid = Guid.NewGuid().ToString();
                var data = new JObject
                {
                    {"action", "send_item"},
                    //{"unified_broadcast_format", "1"},
                    {"reel_id", reelId},
                    {"text", text ?? ""},
                    {"client_context", guid},
                    {"story_media_id", storyMediaId},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"mutation_token", guid},
                };
                if (threadIds?.Length > 0)
                    data.Add("thread_ids", $"[{threadIds.EncodeList(false)}]");
                if (recipients?.Length > 0)
                    data.Add("recipient_users", "[[" + recipients.EncodeList(false) + "]]");
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                return obj.IsSucceed ? Result.Success(true): Result.Fail("",false);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Reply to story
        ///     <para>Note: Get story media id from <see cref="InstaMedia.InstaIdentifier"/></para>
        /// </summary>
        /// <param name="storyMediaId">Media id (get it from <see cref="InstaMedia.InstaIdentifier"/>)</param>
        /// <param name="userId">Story owner user pk (get it from <see cref="InstaMedia.User.Pk"/>)</param>
        /// <param name="text">Text to send</param>
        /// <param name="sharingType">Sharing type</param>
        public async Task<IResult<bool>> ReplyToStoryAsync(string storyMediaId, long userId, string text, InstaSharingType sharingType)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastReelShareUri(sharingType);
                var clientContext = Guid.NewGuid().ToString();

                //recipient_users=[[6798340126]]&
                //action=send_item&
                //client_context=2a4ff351-a7c7-4159-b385-e2dbbd729b04&
                //media_id=2037333278700594436_6798340126&
                //_csrftoken=4spGTGKweOwOkaiN9UBl4QIJbqQfMx7e&
                //text=Nice&
                //device_id=android-21c311d494a974fe&
                //mutation_token=2a4ff351-a7c7-4159-b385-e2dbbd729b04&
                //_uuid=6324ecb2-e663-4dc8-a3a1-289c699cc876&
                //entry=reel&
                //reel_id=6798340126
                var data = new Dictionary<string, string>
                {
                    {"recipient_users", $"[[{userId}]]"},
                    {"action", "send_item"},
                    {"entry", "reel"},
                    {"reel_id", userId.ToString()},
                    {"client_context", clientContext},
                    {"mutation_token", clientContext},
                    {"media_id", storyMediaId},
                    {"_csrftoken", _user.CsrfToken},
                    {"text", text ?? string.Empty},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceId}
                };

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     UnFollow countdown stories
        /// </summary>
        /// <param name="countdownId">Countdown id (<see cref="InstaStoryCountdownStickerItem.CountdownId"/>)</param>
        public async Task<IResult<bool>> UnFollowCountdownStoryAsync(long countdownId)
        {
            return await FollowUnfollowCountdown(UriCreator.GetStoryUnFollowCountdownUri(countdownId));
        }
        [Obsolete]
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(InstaImage image, string caption,
            InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryPhotoAsync(null, image, caption, uploadOptions);

        [Obsolete]
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image, string caption,
            InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryPhotoWithUrlAsync(progress, image, caption, null, uploadOptions);

        [Obsolete]
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(InstaImage image, string caption, Uri uri,
            InstaStoryUploadOptions uploadOptions = null) =>
             await UploadStoryPhotoWithUrlAsync(null, image, caption, uri, uploadOptions);

        [Obsolete]
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaImage image, string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryPhotoWithUrlAsync(progress, image, uri, uploadOptions);

        /// <summary>
        ///     Upload story photo
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(InstaImage image, InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryPhotoAsync(null, image,  uploadOptions);
        }
        /// <summary>
        ///     Upload story photo with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image,
            InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryPhotoWithUrlAsync(progress, image, null, uploadOptions);
        }
        /// <summary>
        ///     Upload story photo with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(InstaImage image, Uri uri,
            InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryPhotoWithUrlAsync(null, image, uri, uploadOptions);
        }
        /// <summary>
        ///     Upload story photo with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaImage image, Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                if (uploadOptions?.Mentions?.Count > 0)
                {
                    var currentDelay = _instaApi.GetRequestDelay();
                    _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                    foreach (var t in uploadOptions.Mentions)
                    {
                        try
                        {
                            bool tried = false;
                        TryLabel:
                            var u = await _instaApi.UserProcessor.GetUserAsync(t.Username);
                            if (!u.Succeeded)
                            {
                                if (!tried)
                                {
                                    tried = true;
                                    goto TryLabel;
                                }
                            }
                            else
                                t.Pk = u.Value.Pk;
                        }
                        catch { }
                    }
                    _instaApi.SetRequestDelay(currentDelay);
                }
                if(uploadOptions?.Questions?.Count > 0)
                {
                    try
                    {
                        bool tried = false;
                        var profilePicture = string.Empty;
                    TryToGetMyUser:
                        // get latest profile picture
                        var myUser = await _instaApi.UserProcessor.GetUserAsync(_user.UserName.ToLower());
                        if (!myUser.Succeeded)
                        {
                            if (!tried)
                            {
                                tried = true;
                                goto TryToGetMyUser;
                            }
                            else
                                profilePicture = _user.LoggedInUser.ProfilePicture;
                        }
                        else
                            profilePicture = myUser.Value.ProfilePicture;


                        foreach (var question in uploadOptions.Questions)
                            question.ProfilePicture = profilePicture;
                    }
                    catch { }
                }

                var uploadId = ExtensionHelper.GetStoryToken();
                var photoHashCode = Path.GetFileName(image.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetPositiveHashCode();

                var waterfallId = Guid.NewGuid().ToString();

                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);

                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);

                var photoUploadParamsObj = new JObject
                {
                    {"upload_id", uploadId},
                    {"media_type", "1"},
                    {"retry_context", "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}"},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"" + 
                              ExtensionHelper.GetRandomQuality() + "\",\"ssim\":" + ExtensionHelper.GetSSIM() +"}"},
                    {"xsharing_user_ids", $"[\"{ _user.LoggedInUser.Pk}\"]"}
                };
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, photoUri, _deviceInfo);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
                }

                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var imageBytes = image.ImageBytes ?? File.ReadAllBytes(image.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Uploaded;
                    progress?.Invoke(upProgress);
                    await Task.Delay(5000);
                    return await ConfigureStoryPhotoAsync(progress, upProgress, image, uploadId, uri, uploadOptions);
                }
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryMedia>(exception);
            }
        }

        [Obsolete()]
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(InstaVideoUpload video, 
            string caption, InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoAsync(null, video, uploadOptions);

        [Obsolete()]
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, string caption,
            InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoWithUrlAsync(progress, video, null, uploadOptions);

        [Obsolete()]
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, string caption, Uri uri,
            InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoWithUrlAsync(null, video, uri, uploadOptions);

        [Obsolete()]
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoWithUrlAsync(progress, video, uri, uploadOptions);




        /// <summary>
        ///     Upload story video (to self story)
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(InstaVideoUpload video, InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoAsync(null, video, uploadOptions);

        /// <summary>
        ///     Upload story video (to self story) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, 
            InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoWithUrlAsync(progress, video, null, uploadOptions);

        /// <summary>
        ///     Upload story video (to self story)
        /// </summary>
        /// <param name="video">Video to upload</param>
        public async Task<IResult<bool>> UploadStoryVideoAsync(InstaVideoUpload video,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendVideoAsync(null, false, false, "", InstaViewMode.Replayable, storyType, null, threadIds.EncodeList(), video, null, uploadOptions);
        }

        /// <summary>
        ///     Upload story video (to self story) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<bool>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendVideoAsync(progress, false, false, "", InstaViewMode.Replayable, storyType, null, threadIds.EncodeList(), video, null, uploadOptions);
        }

        /// <summary>
        ///     Upload story video (to self story) with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, Uri uri,
            InstaStoryUploadOptions uploadOptions = null) =>
            await UploadStoryVideoWithUrlAsync(null, video, uri, uploadOptions);

        /// <summary>
        ///     Upload story video (to self story) with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                if (uploadOptions?.Mentions?.Count > 0)
                {
                    var currentDelay = _instaApi.GetRequestDelay();
                    _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                    foreach (var t in uploadOptions.Mentions)
                    {
                        try
                        {
                            bool tried = false;
                        TryLabel:
                            var u = await _instaApi.UserProcessor.GetUserAsync(t.Username);
                            if (!u.Succeeded)
                            {
                                if (!tried)
                                {
                                    tried = true;
                                    goto TryLabel;
                                }
                            }
                            else
                                t.Pk = u.Value.Pk;
                        }
                        catch { }
                    }
                    _instaApi.SetRequestDelay(currentDelay);
                }
                if (uploadOptions?.Questions?.Count > 0)
                {
                    try
                    {
                        bool tried = false;
                        var profilePicture = string.Empty;
                    TryToGetMyUser:
                        // get latest profile picture
                        var myUser = await _instaApi.UserProcessor.GetUserAsync(_user.UserName.ToLower());
                        if (!myUser.Succeeded)
                        {
                            if (!tried)
                            {
                                tried = true;
                                goto TryToGetMyUser;
                            }
                            else
                                profilePicture = _user.LoggedInUser.ProfilePicture;
                        }
                        else
                            profilePicture = myUser.Value.ProfilePicture;


                        foreach (var question in uploadOptions.Questions)
                            question.ProfilePicture = profilePicture;
                    }
                    catch { }
                }

                var uploadId = ExtensionHelper.GetStoryToken();
                var uploadUrlId = Guid.NewGuid().ToString().Replace("-", "");
                var videoBytes = video.Video.VideoBytes ?? File.ReadAllBytes(video.Video.Uri);

                var videoBytesLength = videoBytes.Length;

                var waterfallMixedId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12).ToUpper();

                var waterfallId = $"{uploadId}_{waterfallMixedId}_Mixed_0";


                var videoEntityName = $"{uploadUrlId}-0-{videoBytesLength}";
                var unixMilisec = DateTime.UtcNow.ToUnixTimeMiliSeconds();
                var videoUri = UriCreator.GetStoryUploadVideoUri(uploadUrlId, $"{videoBytesLength}-{unixMilisec}-{unixMilisec}");
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);

                var videoUploadParamsObj = new JObject
                {
                    {"upload_media_height", "0"},
                    {"extract_cover_frame", "1"},
                    {"xsharing_user_ids", "[]"},
                    {"upload_media_width", "0"},
                    {"upload_media_duration_ms", "46000"},
                    {"content_tags", "use_default_cover"/*"has-overlay"*/},
                    {"upload_id", uploadId},
                    {"for_album", "1"},
                    {"retry_context", "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}"},
                    {"media_type", "2"},
                };
                var videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
                request.Headers.AddHeader("Segment-Start-Offset", "0", _instaApi);
                request.Headers.AddHeader("Segment-Type", "3", _instaApi);
                request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
                }


                var videoContent = new ByteArrayContent(videoBytes);
                videoContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                videoContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                //var progressContent = new ProgressableStreamContent(videoContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
                request.Content = videoContent;
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var vidExt = Path.GetExtension(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
                //if (vidExt == "mov")
                //    request.Headers.AddHeader("X-Entity-Type", "video/quicktime", _instaApi);
                //else
                    request.Headers.AddHeader("X-Entity-Type", "video/mp4", _instaApi);

                request.Headers.AddHeader("Segment-Start-Offset", "0", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("Segment-Type", "3", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", videoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", videoBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
                }
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);

                upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                progress?.Invoke(upProgress);
                await Task.Delay(15000);
                return await ConfigureStoryVideoAsync(progress, upProgress, video, uploadId, uri, uploadOptions);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryMedia>(exception);
            }
        }

        /// <summary>
        ///     Upload story video [to self story, to direct threads or both(self and direct)] with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="storyType">Story type</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<bool>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, Uri uri,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendVideoAsync(null, false, false, "", InstaViewMode.Replayable, storyType, null, threadIds.EncodeList(), video, uri, uploadOptions);
        }

        /// <summary>
        ///     Upload story video (to self story) with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="storyType">Story type</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<bool>> UploadStoryVideoWithUrlAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, Uri uri,
            InstaStoryType storyType = InstaStoryType.SelfStory, InstaStoryUploadOptions uploadOptions = null, params string[] threadIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendVideoAsync(progress, false, false, "", InstaViewMode.Replayable, storyType, null, threadIds.EncodeList(), video, uri, uploadOptions);
        }

        /// <summary>
        ///     Validate uri for adding to story link
        /// </summary>
        /// <param name="uri">Uri address</param>
        public async Task<IResult<bool>> ValidateUriAsync(Uri uri)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (uri == null)
                    return Result.Fail("Uri cannot be null.", false);

                var instaUri = UriCreator.GetValidateReelLinkAddressUri();
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"url", uri.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Vote to an story poll
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="pollId">Story poll id</param>
        /// <param name="pollVote">Your poll vote</param>
        public async Task<IResult<InstaStoryItem>> VoteStoryPollAsync(string storyMediaId, string pollId, InstaStoryPollVoteType pollVote)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetStoryPollVoteUri(storyMediaId, pollId);
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"radio_type", "wifi-none"},
                    {"vote", ((int)pollVote).ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStoryItem>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaReelStoryMediaViewersResponse>(json);
                var covertedObj = ConvertersFabric.Instance.GetReelStoryMediaViewersConverter(obj).Convert();

                return Result.Success(covertedObj.UpdatedMedia);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryItem), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryItem>(exception);
            }
        }

        /// <summary>
        ///     Vote to an story slider
        ///     <para>Note: slider vote must be between 0 and 1</para>
        /// </summary>
        /// <param name="storyMediaId">Story media id</param>
        /// <param name="pollId">Story poll id</param>
        /// <param name="sliderVote">Your slider vote (from 0 to 1)</param>
        public async Task<IResult<InstaStoryItem>> VoteStorySliderAsync(string storyMediaId, string pollId, double sliderVote = 0.5)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (sliderVote > 1)
                    return Result.Fail<InstaStoryItem>("sliderVote cannot be more than 1.\r\nIt must be between 0 and 1");
                if(sliderVote < 0)
                    return Result.Fail<InstaStoryItem>("sliderVote cannot be less than 0.\r\nIt must be between 0 and 1");

                var instaUri = UriCreator.GetVoteStorySliderUri(storyMediaId, pollId);
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"vote", sliderVote.ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStoryItem>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaReelStoryMediaViewersResponse>(json);
                var covertedObj = ConvertersFabric.Instance.GetReelStoryMediaViewersConverter(obj).Convert();

                return Result.Success(covertedObj.UpdatedMedia);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryItem), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryItem>(exception);
            }
        }

        private async Task<IResult<bool>> AppendOrDeleteHighlight(string highlightId, string mediaId, bool delete)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var data = new JObject
                {
                    {"source", "story_viewer"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()}
                };
                if (delete)
                {
                    data.Add("added_media_ids", "[]");
                    data.Add("removed_media_ids", $"[{ExtensionHelper.EncodeList(new[] { mediaId })}]");
                }
                else
                {
                    data.Add("added_media_ids", $"[{ExtensionHelper.EncodeList(new[] { mediaId })}]");
                    data.Add("removed_media_ids", "[]");
                }
                var instaUri = UriCreator.GetHighlightEditUri(highlightId);
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        /// <summary>
        ///     Configure story photo
        /// </summary>
        /// <param name="image">Photo to configure</param>
        /// <param name="uploadId">Upload id</param>
        /// <param name="uri">Uri to add</param>
        private async Task<IResult<InstaStoryMedia>> ConfigureStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, InstaImage image, string uploadId,
            Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            try
            {
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetVideoStoryConfigureUri();
                var rnd = new Random();
                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"allow_multi_configures", "1"},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"original_media_type", "photo"},
                    {"has_original_sound", "1"},
                    {"date_time_digitalized", DateTime.UtcNow.ToString("yyyy:MM:dd+HH:mm:ss")},
                    {"client_shared_at", (DateTime.UtcNow.ToUnixTime() - rnd.Next(10, 25)).ToString()},
                    {"configure_mode", "1"},
                    {"source_type", "3"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"audience", "default"},
                    {"upload_id", uploadId},
                    {"client_timestamp", DateTime.UtcNow.ToUnixTime().ToString()},
                    {"software", "lavender-user+10+QKQ1.190910.002+V12.0.1.0.QFGMIXM+release-keys"},
                    {"scene_type", "?"},
                    {"camera_position", "unknown"},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier.Replace(" ", "+")},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", _deviceInfo.AndroidVer.APILevel}
                        }
                    },
                    {
                        "extra", new JObject
                        {
                            {"source_width", 0},
                            {"source_height", 0}
                        }
                    }
                };
                if (uri != null)
                {
                    var webUri = new JArray
                    {
                        new JObject
                        {
                            {"linkType", 1},
                            {"webUri", uri.ToString()},
                            {"androidClass", ""},
                            {"package", ""},
                            {"deeplinkUri", ""},
                            {"callToActionTitle", ""},
                            {"redirectUri", null},
                            {"leadGenFormId", ""},
                            {"igUserId", ""},
                            {"appInstallObjectiveInvalidationBehavior", null}
                        }
                    };
                    var storyCta = new JArray
                    {
                        new JObject
                        {
                            {"links",  webUri}
                        }
                    };
                    data.Add("story_cta", storyCta.ToString(Formatting.None));
                }
                if (uploadOptions != null)
                {
                    if (uploadOptions.Hashtags?.Count > 0)
                    {
                        var hashtagArr = new JArray();
                        foreach (var item in uploadOptions.Hashtags)
                            hashtagArr.Add(item.ConvertToJson());

                        data.Add("story_hashtags", hashtagArr.ToString(Formatting.None));
                    }

                    if (uploadOptions.Locations?.Count > 0)
                    {
                        var locationArr = new JArray();
                        foreach (var item in uploadOptions.Locations)
                            locationArr.Add(item.ConvertToJson());

                        data.Add("story_locations", locationArr.ToString(Formatting.None));
                    }
                    if (uploadOptions.Slider != null)
                    {
                        var sliderArr = new JArray
                        {
                            uploadOptions.Slider.ConvertToJson()
                        };

                        data.Add("story_sliders", sliderArr.ToString(Formatting.None));
                        if (uploadOptions.Slider.IsSticker && data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", $"{uploadOptions.Slider.Emoji}");
                    }
                    else
                    {
                        if (uploadOptions.Polls?.Count > 0)
                        {
                            var pollArr = new JArray();
                            foreach (var item in uploadOptions.Polls)
                                pollArr.Add(item.ConvertToJson());

                            data.Add("story_polls", pollArr.ToString(Formatting.None));
                        }
                        if (uploadOptions.Questions?.Count > 0)
                        {
                            var questionArr = new JArray();
                            foreach (var item in uploadOptions.Questions)
                                questionArr.Add(item.ConvertToJson());

                            data.Add("story_questions", questionArr.ToString(Formatting.None));
                        }
                    }
                    if (uploadOptions.MediaStory != null)
                    {
                        var mediaStory = new JArray
                        {
                            uploadOptions.MediaStory.ConvertToJson()
                        };

                        data.Add("attached_media", mediaStory.ToString(Formatting.None));
                    }

                    if (uploadOptions.Mentions?.Count > 0)
                    {
                        var mentionArr = new JArray();
                        foreach (var item in uploadOptions.Mentions)
                            mentionArr.Add(item.ConvertToJson(true));
                        data.Add("tap_models", mentionArr.ToString(Formatting.None));
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "mention_sticker");
                        // old way>
                        //data.Add("reel_mentions", mentionArr.ToString(Formatting.None));
                    }
                    if (uploadOptions.Countdown != null)
                    {
                        var countdownArr = new JArray
                        {
                            uploadOptions.Countdown.ConvertToJson()
                        };

                        data.Add("story_countdowns", countdownArr.ToString(Formatting.None));
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "countdown_sticker_time");
                    }
                    if (uploadOptions.StoryQuiz != null)
                    {
                        var storyQuizArr = new JArray
                        {
                            uploadOptions.StoryQuiz.ConvertToJson()
                        };

                        data.Add("story_quizs", storyQuizArr.ToString(Formatting.None));
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "quiz_story_sticker_default");
                    }
                    if (uploadOptions.StoryChats?.Count > 0)
                    {
                        var chatArr = new JArray();
                        foreach (var item in uploadOptions.StoryChats)
                            chatArr.Add(item.ConvertToJson());

                        data.Add("story_chats", chatArr.ToString(Formatting.None));
                        data.Add("internal_features", "chat_sticker");
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "chat_sticker_id");
                    }

                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var mediaResponse = JsonConvert.DeserializeObject<InstaStoryMediaResponse>(json);
                    var converter = ConvertersFabric.Instance.GetStoryMediaConverter(mediaResponse);
                    var obj = converter.Convert();
                    upProgress.UploadState = InstaUploadState.Configured;
                    progress?.Invoke(upProgress);

                    upProgress.UploadState = InstaUploadState.Completed;
                    progress?.Invoke(upProgress);
                    return Result.Success(obj);
                }
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryMedia>(exception);
            }
        }
        /// <summary>
        ///     Configure story video
        /// </summary>
        /// <param name="video">Video to configure</param>
        /// <param name="uploadId">Upload id</param>
        /// <param name="uri">Uri to add</param>
        private async Task<IResult<InstaStoryMedia>> ConfigureStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, InstaVideoUpload video, string uploadId,
            Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            try
            {
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    if (_httpRequestProcessor.ConfigureMediaDelay != null)
                        await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetVideoStoryConfigureUri(true);
                var rnd = new Random();
                var shareAsReel = uploadOptions?.ShareAsReel ?? false;
                var data = new JObject
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"filter_type", "0"},
                    {"original_media_type", "video"},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"client_shared_at", (long.Parse(ApiRequestMessage.GenerateUploadId())- rnd.Next(25,55)).ToString()},
                    //{"story_media_creation_date", (long.Parse(ApiRequestMessage.GenerateUploadId())- rnd.Next(50,70)).ToString()},
                    {"media_folder", "Camera"},
                    {"configure_mode", "1"},
                    {"source_type", "3"}, //4 bod
                    {"video_result", ""},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"date_time_original", DateTime.Now.ToString("yyyyddMMThmmss0.fffZ")},
                    {"capture_type", shareAsReel ? "clips_v2" :"normal"},
                    //{"mas_opt_in", "NOT_PROMPTED"},
                    {"upload_id", uploadId},
                    {"client_timestamp", ApiRequestMessage.GenerateUploadId()},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier.Replace(" ", "+")},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber.Replace(".0.0", "").Replace(".0", "")},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)}
                        }
                    },
                    {"length", video.Video?.Length ?? 0},
                    {
                        "extra", new JObject
                        {
                            {"source_width", video.Video?.Width ?? 0},
                            {"source_height", video.Video?.Height ??  0}
                        }
                    },
                    {"audio_muted", false},
                    {"poster_frame_index", 0},
                };
                if (uri != null)
                {
                    var webUri = new JArray
                    {
                        new JObject
                        {
                            {"linkType", 1},
                            {"webUri", uri.ToString()},
                            {"androidClass", ""},
                            {"package", ""},
                            {"deeplinkUri", ""},
                            {"callToActionTitle", ""},
                            {"redirectUri", null},
                            {"leadGenFormId", ""},
                            {"igUserId", ""},
                            {"appInstallObjectiveInvalidationBehavior", null}
                        }
                    };
                    var storyCta = new JArray
                    {
                        new JObject
                        {
                            {"links",  webUri}
                        }
                    };
                    data.Add("story_cta", storyCta.ToString(Formatting.None));
                }
                if (uploadOptions != null)
                {
                    if (uploadOptions.ShareAsReel)
                    {
                        data.Add("creation_surface", "clips");

                        var clipsSegments = new JArray
                        {
                            new JObject
                            {
                                {"index", 0},
                                {"face_effect_id", null},
                                {"speed", 100},
                                {"source", "camera"},
                                {"duration_ms", video.Video.Length},
                                {"audio_type", "original"},
                                {"from_draft", "0"},
                                {"camera_position", 2},
                                {"media_folder", null},
                                {"media_type", "video"},
                                {"original_media_type", "video"},
                            }
                        };
                        var clipsSegmentsMetaData = new JObject
                        {
                            {"num_segments", 1},
                            {"clips_segments", clipsSegments}
                        };
                        data.Add("clips_segments_metadata", clipsSegmentsMetaData.ToString());
                    }

                    if (uploadOptions.Hashtags?.Count > 0)
                    {
                        var hashtagArr = new JArray();
                        foreach (var item in uploadOptions.Hashtags)
                            hashtagArr.Add(item.ConvertToJson());

                        data.Add("story_hashtags", hashtagArr.ToString(Formatting.None));
                    }

                    if (uploadOptions.Locations?.Count > 0)
                    {
                        var locationArr = new JArray();
                        foreach (var item in uploadOptions.Locations)
                            locationArr.Add(item.ConvertToJson());

                        data.Add("story_locations", locationArr.ToString(Formatting.None));
                    }
                    if (uploadOptions.Slider != null)
                    {
                        var sliderArr = new JArray
                        {
                            uploadOptions.Slider.ConvertToJson()
                        };

                        data.Add("story_sliders", sliderArr.ToString(Formatting.None));
                        if (uploadOptions.Slider.IsSticker && data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", $"{uploadOptions.Slider.Emoji}");
                    }
                    else
                    {
                        if (uploadOptions.Polls?.Count > 0)
                        {
                            var pollArr = new JArray();
                            foreach (var item in uploadOptions.Polls)
                                pollArr.Add(item.ConvertToJson());

                            data.Add("story_polls", pollArr.ToString(Formatting.None));
                        }
                        if (uploadOptions.Questions?.Count > 0)
                        {
                            var questionArr = new JArray();
                            foreach (var item in uploadOptions.Questions)
                                questionArr.Add(item.ConvertToJson());

                            data.Add("story_questions", questionArr.ToString(Formatting.None));
                        }
                    }
                    if (uploadOptions.MediaStory != null)
                    {
                        var mediaStory = new JArray
                        {
                            uploadOptions.MediaStory.ConvertToJson()
                        };

                        data.Add("attached_media", mediaStory.ToString(Formatting.None));
                    }

                    if (uploadOptions.Mentions?.Count > 0)
                    {
                        var mentionArr = new JArray();
                        foreach (var item in uploadOptions.Mentions)
                            mentionArr.Add(item.ConvertToJson(true));
                        data.Add("tap_models", mentionArr.ToString(Formatting.None));
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "mention_sticker");
                        // old way>
                        //data.Add("reel_mentions", mentionArr.ToString(Formatting.None));
                    }
                    if (uploadOptions.Countdown != null)
                    {
                        var countdownArr = new JArray
                        {
                            uploadOptions.Countdown.ConvertToJson()
                        };

                        data.Add("story_countdowns", countdownArr.ToString(Formatting.None));
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "countdown_sticker_time");
                    }
                    if (uploadOptions.StoryQuiz != null)
                    {
                        var storyQuizArr = new JArray
                        {
                            uploadOptions.StoryQuiz.ConvertToJson()
                        };

                        data.Add("story_quizs", storyQuizArr.ToString(Formatting.None));
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "quiz_story_sticker_default");
                    }
                    if (uploadOptions.StoryChats?.Count > 0)
                    {
                        var chatArr = new JArray();
                        foreach (var item in uploadOptions.StoryChats)
                            chatArr.Add(item.ConvertToJson());

                        data.Add("story_chats", chatArr.ToString(Formatting.None));
                        data.Add("internal_features", "chat_sticker");
                        if (data["story_sticker_ids"] == null)
                            data.Add("story_sticker_ids", "chat_sticker_id");
                    }
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var uploadParamsObj = new JObject
                {
                    {"num_step_auto_retry", 0},
                    {"num_reupload", 0},
                    {"num_step_manual_retry", 0}
                };
                var uploadParams = JsonConvert.SerializeObject(uploadParamsObj);
                request.Headers.AddHeader("retry_context", uploadParams, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var mediaResponse = JsonConvert.DeserializeObject<InstaStoryMediaResponse>(json);
                    var converter = ConvertersFabric.Instance.GetStoryMediaConverter(mediaResponse);
                    var obj = Result.Success(converter.Convert());
                    upProgress.UploadState = InstaUploadState.Configured;
                    progress?.Invoke(upProgress);
                    upProgress.UploadState = InstaUploadState.Completed;
                    progress?.Invoke(upProgress);
                    return obj;
                }
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryMedia>(exception);
            }
        }

        private async Task<IResult<InstaReelStoryMediaViewersResponse>> GetStoryMediaViewers(string storyMediaId, string maxId)
        {
            try
            {
                var directInboxUri = UriCreator.GetStoryMediaViewersUri(storyMediaId, maxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, directInboxUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaReelStoryMediaViewersResponse>(response, json);

                var storyMediaViewersResponse = JsonConvert.DeserializeObject<InstaReelStoryMediaViewersResponse>(json);

                return Result.Success(storyMediaViewersResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaReelStoryMediaViewersResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaReelStoryMediaViewersResponse>(exception);
            }
        }

        private async Task<IResult<InstaStoryPollVotersListResponse>> GetStoryPollVoters(string storyMediaId, string pollId, string maxId)
        {
            try
            {
                var directInboxUri = UriCreator.GetStoryPollVotersUri(storyMediaId, pollId, maxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, directInboxUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStoryPollVotersListResponse>(response, json);

                var storyVotersResponse = JsonConvert.DeserializeObject<InstaStoryPollVotersListContainerResponse>(json);

                return Result.Success(storyVotersResponse.VoterInfo);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryPollVotersListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryPollVotersListResponse>(exception);
            }
        }
        public async Task<IResult<bool>> FollowUnfollowCountdown(Uri instaUri)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_csrftoken", _user.CsrfToken},
                };

                var request =  _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                var resp = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                return resp.IsSucceed ? Result.Success(true) : Result.Fail<bool>("");
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }
        async Task<IResult<bool>> RequestOrCancelStoryChat(Uri instaUri, long storyChatId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var data = new Dictionary<string, string>
                {
                    {"story_chat_id", storyChatId.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()}
                };
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, obj.Message, null);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, false, ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, false);
            }
        }


        private async Task<IResult<InstaStoryFeedResponse>> GetStoryFeedWithPostMethod(PaginationParameters pagination, bool forceRefresh = false, string[] preloadedReelIds = null)
        {
            try
            {
                bool HasMore(bool force) => string.IsNullOrEmpty(pagination.SessionId) || force ||
                    pagination.NextIdsToFetch == null || pagination.NextIdsToFetch?.Count == 0 || string.IsNullOrEmpty(pagination.NextMaxId);

                var isFreshPagination = HasMore(forceRefresh);

                if (isFreshPagination)
                    pagination.SessionId = Guid.NewGuid().ToString();

                var storyFeedUri = UriCreator.GetStoryFeedUri();

                var data = new Dictionary<string, string>
                {
                    {InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"tray_session_id", pagination.SessionId},
                    {"request_id", Guid.NewGuid().ToString()}
                };
                if (forceRefresh)
                    data.Add("reason", "pull_to_refresh");
                else
                    data.Add("reason", "cold_start");
                string nextId = "50";
                if (isFreshPagination || forceRefresh)
                {
                    nextId = "50";
                    data.Add(InstaApiConstants.HEADER_TIMEZONE, _instaApi.TimezoneOffset.ToString());
                    data.Add("page_size", "50");
                }
                else
                {
                    int skipId = int.Parse(string.IsNullOrEmpty(pagination.NextMaxId) ? "50" : pagination.NextMaxId);
                    nextId = (skipId + 50).ToString();
                    data.Add("current_highest_ranked_position", skipId.ToString());
                    data["reason"] = "second_page_of_tray";
                    var l = new List<string>();
                    var idsToFetch = pagination.NextIdsToFetch?.Skip(skipId).Take(50);
                    if (idsToFetch != null)
                        foreach (var u in idsToFetch)
                            l.Add(u.ToString());

                    var jArr = new JArray(idsToFetch);
                    data.Add("reel_ids_to_fetch", jArr.ToString(Formatting.None));
                }
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, storyFeedUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<InstaStoryFeedResponse>(response, json);
                var storyFeedResponse = JsonConvert.DeserializeObject<InstaStoryFeedResponse>(json);
                if (storyFeedResponse.RemainingReelIdsToFetch?.Count > 0)
                    pagination.NextIdsToFetch = storyFeedResponse.RemainingReelIdsToFetch;
                if (int.TryParse(nextId, out int ix) && ix > pagination.NextIdsToFetch.Count && !forceRefresh)
                    nextId = null;
                pagination.NextMaxId = nextId;
                return Result.Success(storyFeedResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaStoryFeedResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaStoryFeedResponse>(exception);
            }
        }
    }
}
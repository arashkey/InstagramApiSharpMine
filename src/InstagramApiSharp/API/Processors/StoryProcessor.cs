using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                    {"timezone_offset", InstaApiConstants.TIMEZONE_OFFSET.ToString()},
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
                request.Headers.Add("retry_context", HelperProcessor.GetRetryContext());
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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, obj.Message, null);
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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);

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
        ///     Get user story feed with POST method requests (new API)
        /// </summary>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedWithPostMethodAsync(bool refresh = false, string[] preloadedReelIds = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                //supported_capabilities_new=[{}]&
                //reason=cold_start&
                //_csrftoken=fghjf&
                //_uuid=6rtrgt&
                //preloaded_reel_ids=8651542203,7470273225,7293779140,4137323183,4728340654,5412390834,3935014064,9129640961,5702637159,8233674376&
                //preloaded_reel_timestamp=1555780890,1555765386,1555654998,1555608277,1555582894,1555572328,1555275303,1554736759,1554732984,1554732411

                //supported_capabilities_new=[{"name":"SUPPORTED_SDK_VERSIONS","value":"13.0,14.0,15.0,16.0,17.0,18.0,19.0,20.0,21.0,22.0,23.0,24.0,25.0,26.0,27.0,28.0,29.0,30.0,31.0,32.0,33.0,34.0,35.0,36.0,37.0,38.0,39.0,40.0,41.0,42.0,43.0,44.0,45.0,46.0,47.0,48.0,49.0,50.0,51.0,52.0,53.0,54.0,55.0,56.0,57.0,58.0,59.0,60.0,61.0"},{"name":"FACE_TRACKER_VERSION","value":"12"},{"name":"segmentation","value":"segmentation_enabled"},{"name":"COMPRESSION","value":"ETC2_COMPRESSION"},{"name":"world_tracker","value":"world_tracker_enabled"},{"name":"gyroscope","value":"gyroscope_enabled"}]&
                //reason=pull_to_refresh&
                //_csrftoken=defrghdf&
                //_uuid=c6a2rtyhrt
                //preloaded_reel_ids=8062492058,7641914022,1593584454,7748367602,7694284101,374897883,1341582726,6179437947,6798340126,3405354889&
                //preloaded_reel_timestamp=1556806882,1556781054,1556760190,1556746331,1556698368,1556666696,1556644081,1556618908,1556614565,1556531878



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
        ///     Get user story feed (stories from users followed by current user).
        /// </summary>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                //supported_capabilities_new=[{}]&
                //reason=cold_start&
                //_csrftoken=ttttttt&
                //_uuid=aaaaaaaaaaaa&
                //preloaded_reel_ids=8651542203,7470273225,7293779140,4137323183,4728340654,5412390834,3935014064,9129640961,5702637159,8233674376&
                //preloaded_reel_timestamp=1555780890,1555765386,1555654998,1555608277,1555582894,1555572328,1555275303,1554736759,1554732984,1554732411





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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
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

            return await UploadStoryPhotoWithUrlAsync(progress, image, string.Empty, null, new InstaStoryUploadOptions { MediaStory = mediaStoryUpload });
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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
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

        /// <summary>
        ///     Upload story photo
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(InstaImage image, string caption,
            InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryPhotoAsync(null, image, caption, uploadOptions);
        }
        /// <summary>
        ///     Upload story photo with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaImage image, string caption,
            InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryPhotoWithUrlAsync(progress, image, caption, null, uploadOptions);
        }
        /// <summary>
        ///     Upload story photo with adding link address
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(InstaImage image, string caption, Uri uri,
            InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryPhotoWithUrlAsync(null, image, caption, uri, uploadOptions);
        }
        /// <summary>
        ///     Upload story photo with adding link address (with progress)
        ///     <para>Note: this function only works with verified account or you have more than 10k followers.</para>
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsync(Action<InstaUploaderProgress> progress,
            InstaImage image, string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
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

                var uploadId = ApiRequestMessage.GenerateRandomUploadId();
                var photoHashCode = Path.GetFileName(image.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();

                var waterfallId = Guid.NewGuid().ToString();

                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);

                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var videoMediaInfoData = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"media_info", new JObject
                        {
                              {"capture_mode", "normal"},
                              {"media_type", 1},
                              {"caption", caption},
                              {"mentions", new JArray()},
                              {"hashtags", new JArray()},
                              {"locations", new JArray()},
                              {"stickers", new JArray()},
                        }
                    }
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, UriCreator.GetStoryMediaInfoUploadUri(), _deviceInfo, videoMediaInfoData);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
               
                var photoUploadParamsObj = new JObject
                {
                    {"upload_id", uploadId},
                    {"media_type", "1"},
                    {"retry_context", "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}"},

                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"}
                };
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Get, photoUri, _deviceInfo);
                request.Headers.Add("X_FB_PHOTO_WATERFALL_ID", waterfallId);
                request.Headers.Add("X-Instagram-Rupload-Params", photoUploadParams);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
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
                imageContent.Headers.Add("Content-Transfer-Encoding", "binary");
                imageContent.Headers.Add("Content-Type", "application/octet-stream");
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.Add("X-Entity-Type", "image/jpeg");
                request.Headers.Add("Offset", "0");
                request.Headers.Add("X-Instagram-Rupload-Params", photoUploadParams);
                request.Headers.Add("X-Entity-Name", photoEntityName);
                request.Headers.Add("X-Entity-Length", imageBytes.Length.ToString());
                request.Headers.Add("X_FB_PHOTO_WATERFALL_ID", waterfallId);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Uploaded;
                    progress?.Invoke(upProgress);
                    await Task.Delay(5000);
                    return await ConfigureStoryPhotoAsync(progress, upProgress, image, uploadId, caption, uri, uploadOptions);
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
        ///     Upload story video (to self story)
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(InstaVideoUpload video, string caption, InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryVideoAsync(null, video, caption, uploadOptions);
        }

        /// <summary>
        ///     Upload story video (to self story) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, 
            string caption, InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryVideoWithUrlAsync(progress, video, caption, null, uploadOptions);
        }

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
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="uri">Uri to add</param>
        /// <param name="uploadOptions">Upload options => Optional</param>
        public async Task<IResult<InstaStoryMedia>> UploadStoryVideoWithUrlAsync(InstaVideoUpload video, string caption, Uri uri,
            InstaStoryUploadOptions uploadOptions = null)
        {
            return await UploadStoryVideoWithUrlAsync(null, video, caption, uri, uploadOptions);
        }

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
            InstaVideoUpload video, string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                var uploadId = ApiRequestMessage.GenerateRandomUploadId();
                var videoHashCode = Path.GetFileName(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").GetHashCode();
                var photoHashCode = Path.GetFileName(video.VideoThumbnail.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();

                var waterfallId = Guid.NewGuid().ToString();

                var videoEntityName = $"{uploadId}_0_{videoHashCode}";
                var videoUri = UriCreator.GetStoryUploadVideoUri(uploadId, videoHashCode);

                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);

                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var videoMediaInfoData = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"media_info", new JObject
                        {
                              {"capture_mode", "normal"},
                              {"media_type", 2},
                              {"caption", caption},
                              {"mentions", new JArray()},
                              {"hashtags", new JArray()},
                              {"locations", new JArray()},
                              {"stickers", new JArray()},
                        }
                    }
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, UriCreator.GetStoryMediaInfoUploadUri(), _deviceInfo, videoMediaInfoData);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                var videoUploadParamsObj = new JObject
                {
                    {"upload_media_height", "0"},
                    {"upload_media_width", "0"},
                    {"upload_media_duration_ms", "46000"},
                    {"upload_id", uploadId},
                    {"for_album", "1"},
                    {"retry_context", "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}"},
                    {"media_type", "2"},
                };
                var videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
                request.Headers.Add("X_FB_VIDEO_WATERFALL_ID", waterfallId);
                request.Headers.Add("X-Instagram-Rupload-Params", videoUploadParams);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
                }


                var videoBytes = video.Video.VideoBytes ?? File.ReadAllBytes(video.Video.Uri);
                var videoContent = new ByteArrayContent(videoBytes);
                videoContent.Headers.Add("Content-Transfer-Encoding", "binary");
                videoContent.Headers.Add("Content-Type", "application/octet-stream");
                //var progressContent = new ProgressableStreamContent(videoContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
                request.Content = videoContent;
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var vidExt = Path.GetExtension(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
                if (vidExt == "mov")
                    request.Headers.Add("X-Entity-Type", "image/quicktime");
                else
                    request.Headers.Add("X-Entity-Type", "image/mp4");
                request.Headers.Add("Offset", "0");
                request.Headers.Add("X-Instagram-Rupload-Params", videoUploadParams);
                request.Headers.Add("X-Entity-Name", videoEntityName);
                request.Headers.Add("X-Entity-Length", videoBytes.Length.ToString());
                request.Headers.Add("X_FB_VIDEO_WATERFALL_ID", waterfallId);
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
                var photoUploadParamsObj = new JObject
                {
                    {"retry_context", "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}"},
                    {"media_type", "2"},
                    {"upload_id", uploadId},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                };
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Get, photoUri, _deviceInfo);
                request.Headers.Add("X_FB_PHOTO_WATERFALL_ID", waterfallId);
                request.Headers.Add("X-Instagram-Rupload-Params", photoUploadParams);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaStoryMedia>(response, json);
                }

                upProgress.UploadState = InstaUploadState.UploadingThumbnail;
                progress?.Invoke(upProgress);
                var imageBytes = video.VideoThumbnail.ImageBytes ?? File.ReadAllBytes(video.VideoThumbnail.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.Add("Content-Transfer-Encoding", "binary");
                imageContent.Headers.Add("Content-Type", "application/octet-stream");
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.Add("X-Entity-Type", "image/jpeg");
                request.Headers.Add("Offset", "0");
                request.Headers.Add("X-Instagram-Rupload-Params", photoUploadParams);
                request.Headers.Add("X-Entity-Name", photoEntityName);
                request.Headers.Add("X-Entity-Length", imageBytes.Length.ToString());
                request.Headers.Add("X_FB_PHOTO_WATERFALL_ID", waterfallId);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //upProgress = progressContent?.UploaderProgress;
                    upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                    progress?.Invoke(upProgress);
                    await Task.Delay(30000);
                    return await ConfigureStoryVideoAsync(progress, upProgress, video, uploadId, caption, uri, uploadOptions);
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
        ///     Validate url for adding to story link
        /// </summary>
        /// <param name="url">Url address</param>
        public async Task<IResult<bool>> ValidateUrlAsync(string url)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (string.IsNullOrEmpty(url))
                    return Result.Fail("Url cannot be null or empty.", false);

                var instaUri = UriCreator.GetValidateReelLinkAddressUri();
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"url", url},
                };
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
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
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
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
        /// <param name="caption">Caption</param>
        /// <param name="uri">Uri to add</param>
        private async Task<IResult<InstaStoryMedia>> ConfigureStoryPhotoAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, InstaImage image, string uploadId,
            string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null)
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
                var instaUri = UriCreator.GetVideoStoryConfigureUri();// UriCreator.GetStoryConfigureUri();
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_csrftoken", _user.CsrfToken},
                    {"source_type", "3"},
                    {"caption", caption},
                    {"upload_id", uploadId},
                    {"edits", new JObject()},
                    {"disable_comments", false},
                    {"configure_mode", 1},
                    {"camera_position", "unknown"},
                    {"allow_multi_configures", "1"},
                };
                if (uri != null)
                {
                    var webUri = new JArray
                    {
                        new JObject
                        {
                            {"webUri", uri.ToString()}
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
                        if (uploadOptions.Slider.IsSticker)
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
                            mentionArr.Add(item.ConvertToJson());

                        data.Add("reel_mentions", mentionArr.ToString(Formatting.None));
                    }
                    if (uploadOptions.Countdown != null)
                    {
                        var countdownArr = new JArray
                        {
                            uploadOptions.Countdown.ConvertToJson()
                        };

                        data.Add("story_countdowns", countdownArr.ToString(Formatting.None));
                        data.Add("story_sticker_ids", "countdown_sticker_time");
                    }
                    if (uploadOptions.StoryQuiz != null)
                    {
                        var storyQuizArr = new JArray
                        {
                            uploadOptions.StoryQuiz.ConvertToJson()
                        };

                        data.Add("story_quizs", storyQuizArr.ToString(Formatting.None));
                        data.Add("story_sticker_ids", "quiz_story_sticker_default");
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
        /// <param name="caption">Caption</param>
        /// <param name="uri">Uri to add</param>
        private async Task<IResult<InstaStoryMedia>> ConfigureStoryVideoAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, InstaVideoUpload video, string uploadId,
            string caption, Uri uri, InstaStoryUploadOptions uploadOptions = null)
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
                var instaUri = UriCreator.GetVideoStoryConfigureUri(false);
                var rnd = new Random();
                var data = new JObject
                {
                    {"filter_type", "0"},
                    {"timezone_offset", InstaApiConstants.TIMEZONE_OFFSET.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"client_shared_at", (long.Parse(ApiRequestMessage.GenerateUploadId())- rnd.Next(25,55)).ToString()},
                    {"story_media_creation_date", (long.Parse(ApiRequestMessage.GenerateUploadId())- rnd.Next(50,70)).ToString()},
                    {"media_folder", "Camera"},
                    {"configure_mode", "1"},
                    {"source_type", "4"},
                    {"video_result", ""},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"caption", caption},
                    {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                    {"capture_type", "normal"},
                    {"mas_opt_in", "NOT_PROMPTED"},
                    {"upload_id", uploadId},
                    {"client_timestamp", ApiRequestMessage.GenerateUploadId()},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", _deviceInfo.AndroidVer.APILevel}
                        }
                    },
                    {"length", 0},
                    {
                        "extra", new JObject
                        {
                            {"source_width", 0},
                            {"source_height", 0}
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
                            {"webUri", uri.ToString()}
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
                        if (uploadOptions.Slider.IsSticker)
                            data.Add("story_sticker_ids", $"emoji_slider_{uploadOptions.Slider.Emoji}");
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

                    if (uploadOptions.Countdown != null)
                    {
                        var countdownArr = new JArray
                        {
                            uploadOptions.Countdown.ConvertToJson()
                        };

                        data.Add("story_countdowns", countdownArr.ToString(Formatting.None));
                        data.Add("story_sticker_ids", "countdown_sticker_time");
                    }
                    if (uploadOptions.StoryQuiz != null)
                    {
                        var storyQuizArr = new JArray
                        {
                            uploadOptions.StoryQuiz.ConvertToJson()
                        };

                        data.Add("story_quizs", storyQuizArr.ToString(Formatting.None));
                        data.Add("story_sticker_ids", "quiz_story_sticker_default");
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
                request.Headers.Add("retry_context", uploadParams);
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

        #region Old functions

        private async Task<IResult<InstaStoryMedia>> UploadStoryPhotoWithUrlAsyncOLD(Action<InstaUploaderProgress> progress, InstaImage image,
            string caption, Uri uri,
            InstaStoryUploadOptions uploadOptions = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
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
                var instaUri = UriCreator.GetUploadPhotoUri();
                var uploadId = ApiRequestMessage.GenerateUploadId();
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var requestContent = new MultipartFormDataContent(uploadId)
                {
                    {new StringContent(uploadId), "\"upload_id\""},
                    //{new StringContent(_deviceInfo.DeviceGuid.ToString()), "\"_uuid\""},
                    //{new StringContent(_user.CsrfToken), "\"_csrftoken\""},
                    {
                        new StringContent("{\"lib_name\":\"jt\",\"lib_version\":\"1.3.0\",\"quality\":\"87\"}"),
                        "\"image_compression\""
                    }
                };
                byte[] imageBytes;
                if (image.ImageBytes == null)
                    imageBytes = File.ReadAllBytes(image.Uri);
                else
                    imageBytes = image.ImageBytes;
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.Add("Content-Transfer-Encoding", "binary");
                imageContent.Headers.Add("Content-Type", "application/octet-stream");

                //var progressContent = new ProgressableStreamContent(imageContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                requestContent.Add(imageContent, "photo", $"pending_media_{ApiRequestMessage.GenerateUploadId()}.jpg");
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                request.Content = requestContent;
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Uploaded;
                    progress?.Invoke(upProgress);
                    //upProgress = progressContent?.UploaderProgress;
                    return await ConfigureStoryPhotoAsync(progress, upProgress, image, uploadId, caption, uri, uploadOptions);
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

        #endregion Old functions
    }
}
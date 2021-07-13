﻿/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Logger;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using InstagramApiSharp.Helpers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Linq;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Converters.Json;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Classes.ResponseWrappers;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Live api functions.
    /// </summary>
    internal class LiveProcessor : ILiveProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public LiveProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Enable join to live broadcast request
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        public async Task<IResult<bool>> EnableJoinToLiveRequestAsync(string broadcastId)
        {
            return await EnableDisableJoinRequest(UriCreator.GetBroadcastEnableJoinRequestUri(broadcastId)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Disable join to live broadcast request
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        public async Task<IResult<bool>> DisableJoinToLiveRequestAsync(string broadcastId)
        {
            return await EnableDisableJoinRequest(UriCreator.GetBroadcastDisbaleJoinRequestUri(broadcastId)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Set live broadcast question status
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="allowQuestionSubmission">Allow people to submit questions</param>
        public async Task<IResult<bool>> SetQuestionStatusAsync(string broadcastId, 
            bool allowQuestionSubmission = false)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastQuestionStatusUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"allow_question_submission", allowQuestionSubmission.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                var status = obj["status"];
                if (status?.Value<string>() == "ok")
                {
                    var enabled = obj["is_question_submission_allowed"].Value<bool>();
                    return Result.Success(allowQuestionSubmission == enabled);
                }
                else
                    return Result.UnExpectedResponse<bool>(response, json);
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
        ///     Let Instagram know that you invited someone to a live broadcast and joined successfully
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="userIdToInvite">User id (pk) to invite</param>
        /// <param name="offsetToVideoStart">Offset to video start </param>
        public async Task<IResult<bool>> BroadcastEventAsync(string broadcastId, long userIdToInvite, int offsetToVideoStart = 30)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastEventLiveUri(broadcastId);
                var data = new JObject
                {
                    {"client_version", "1"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"event_user_id", userIdToInvite.ToString()},
                    {"event_type", "JOINED"},
                    {"offset_to_video_start", offsetToVideoStart.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
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
        ///     Leave or cancel a live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="encodedServerDataInfo">Encoded server data information => from <see cref="JoinBroadcastAsync"/> response</param>
        /// <param name="numParticipants">Number of participants</param>
        public async Task<IResult<bool>> LeaveBroadcastAsync(string broadcastId, string encodedServerDataInfo, int numParticipants = 1)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastLeaveLiveUri(broadcastId);
                var data = new JObject
                {
                    {"reason", "leave_broadcast"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"num_participants", numParticipants.ToString()},
                    {"encoded_server_data_info", encodedServerDataInfo},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
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
        ///     Invite to a live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="userIdToInvite">User id (pk) to invite</param>
        /// <param name="encodedServerDataInfo">Encoded server data information => from <see cref="JoinBroadcastAsync"/> response</param>
        /// <param name="offsetToVideoStart">Offset to video start </param>
        public async Task<IResult<bool>> InviteToBroadcastAsync(string broadcastId, long userIdToInvite, string encodedServerDataInfo, int offsetToVideoStart = 30)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastInviteLiveUri(broadcastId);
                var data = new JObject
                {
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"invitees", userIdToInvite.ToString()},
                    {"offset_to_video_start", offsetToVideoStart.ToString()},
                    {"encoded_server_data_info", encodedServerDataInfo},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
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
        ///     Confirm a join broadcast request
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="encodedServerDataInfo">Encoded server data information => from <see cref="JoinBroadcastAsync"/> response</param>
        /// <param name="curVersion">Cur version => 1 or 2 or 3</param>
        public async Task<IResult<bool>> ConfirmJoinBroadcastAsync(string broadcastId, string encodedServerDataInfo, uint curVersion = 2)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastConfirmLiveUri(broadcastId);
                var data = new JObject
                {
                    {"message_type", "CONFERENCE_STATE"},
                    {"cur_version", curVersion.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"transaction_id", ExtensionHelper.GetLiveTransactionToken()},
                    {"encoded_server_data_info", encodedServerDataInfo},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
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
        ///     Requests to join a Live broadcast as a co-broadcaster
        ///     <para>If someone sends you a request to join your own live, you should send them a join request as well</para>
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="sdpOffer">Sdp offer => I don't know what is this but it seems it's related to RTSP</param>
        /// <param name="targetVideoWidth">Video width</param>
        /// <param name="targetVideoHeight">Video height</param>
        public async Task<IResult<InstaBroadcastJoin>> JoinBroadcastAsync(string broadcastId, string sdpOffer, uint targetVideoWidth = 848, uint targetVideoHeight = 512)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastJoinLiveUri(broadcastId);
                var data = new JObject
                {
                    {"target_video_height", targetVideoHeight.ToString()},
                    {"target_video_width", targetVideoWidth.ToString()},
                    {"sdp_offer", sdpOffer},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastJoin>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastJoin>(json);
                return obj.IsSucceed ? Result.Success(obj) : Result.UnExpectedResponse<InstaBroadcastJoin>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastJoin), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastJoin>(exception);
            }
        }
        /// <summary>
        ///     Get post live thumbnails
        /// </summary>
        /// <param name="broadcastId">Broadcast identifier</param>
        public async Task<IResult<InstaBroadcastThumbnails>> GetPostLiveThumbnailsAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetPostLiveThumbnailsUri(broadcastId, _instaApi.HttpHelper);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastThumbnails>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastThumbnails>(json);
                return obj.IsSucceed ? Result.Success(obj) : Result.UnExpectedResponse<InstaBroadcastThumbnails>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastThumbnails), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastThumbnails>(exception);
            }
        }

        /// <summary>
        ///     Post a new comment to broadcast.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="commentText">Comment text</param>
        public async Task<IResult<InstaComment>> CommentAsync(string broadcastId, string commentText)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastPostCommentUri(broadcastId);
                var breadcrumb = CryptoHelper.GetCommentBreadCrumbEncoded(commentText);
                var data = new JObject
                {
                    {"user_breadcrumb", commentText},
                    {"live_or_vod", "1"},
                    {"idempotence_token",  Guid.NewGuid().ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"comment_text", commentText},
                    {"offset_to_video_start"," 0"},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaComment>(response, json);
                var commentResponse = JsonConvert.DeserializeObject<InstaCommentResponse>(json,
                     new InstaCommentDataConverter());
                var converter = ConvertersFabric.Instance.GetCommentConverter(commentResponse);
                return Result.Success(converter.Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaComment), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaComment>(exception);
            }
        }

        // create, start, end broadcast
        /// <summary>
        ///     Create live broadcast. After create an live broadcast you must call StartAsync.
        /// </summary>
        /// <param name="previewWidth">Preview width</param>
        /// <param name="previewHeight">Preview height</param>
        /// <param name="title">Title => optional</param>
        public async Task<IResult<InstaBroadcastCreate>> CreateAsync(int previewWidth = 720,
            int previewHeight = 1184,
            string title = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastCreateUri();
                var data = new Dictionary<string, string>
                {
                    {"user_pay_enabled", "false"},
                    {"preview_height",  previewHeight.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"should_use_rsys_rtc_infra", "false"},
                    {"broadcast_type",  "RTMP_SWAP_ENABLED"},
                    {"preview_width",  previewWidth.ToString()},
                    {"internal_only",  "0"},
                    {"visibility",  "0"},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (title.IsNotEmpty())
                {
                    data.Add("broadcast_message", title);
                }
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastCreate>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaBroadcastCreateResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastCreateConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastCreate), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastCreate>(exception);
            }
        }

        /// <summary>
        ///     Disable broadcast comments.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        public async Task<IResult<InstaBroadcastCommentEnableDisable>> DisableCommentsAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastDisableCommenstUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastCommentEnableDisable>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastCommentEnableDisableResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastCommentEnableDisableConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastCommentEnableDisable), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastCommentEnableDisable>(exception);
            }
        }

        /// <summary>
        ///     Enable broadcast comments.
        /// </summary>
        /// <param name="broadcastId"></param>
        public async Task<IResult<InstaBroadcastCommentEnableDisable>> EnableCommentsAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastEnableCommenstUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastCommentEnableDisable>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastCommentEnableDisableResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastCommentEnableDisableConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastCommentEnableDisable), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastCommentEnableDisable>(exception);
            }
        }

        /// <summary>
        ///     End live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="endAfterCopyrightWarning">Copyright warning</param>
        public async Task<IResult<bool>> EndAsync(string broadcastId, bool endAfterCopyrightWarning = false)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastEndUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"end_after_copyright_warning", endAfterCopyrightWarning.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
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
        ///     Get broadcast comments.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="lastCommentTs">Last comment time stamp</param>
        /// <param name="commentsRequested">Comments requested count</param>
        public async Task<IResult<InstaBroadcastCommentList>> GetCommentsAsync(string broadcastId, string lastCommentTs = "", int commentsRequested = 4)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastCommentUri(broadcastId, lastCommentTs);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastCommentList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastCommentListResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastCommentListConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastCommentList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastCommentList>(exception);
            }
        }

        /// <summary>
        ///     Get discover top live.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaDiscoverTopLive>> GetDiscoverTopLiveAsync(PaginationParameters paginationParameters) =>
            await GetDiscoverTopLiveAsync(paginationParameters, CancellationToken.None).ConfigureAwait(false);


        /// <summary>
        ///     Get discover top live.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaDiscoverTopLive>> GetDiscoverTopLiveAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var topLive = new InstaDiscoverTopLive();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);
                InstaDiscoverTopLive Convert(InstaDiscoverTopLiveResponse instaDiscoverTop)
                {
                    return ConvertersFabric.Instance.GetDiscoverTopLiveConverter(instaDiscoverTop).Convert();
                }

                var topLiveResult = await GetDiscoverTopLive(paginationParameters.NextMaxId);
                if (!topLiveResult.Succeeded)
                    return Result.Fail(topLiveResult.Info, topLive);
                var topLiveResponse = topLiveResult.Value;
                topLive = Convert(topLiveResponse);
                topLive.NextMaxId = paginationParameters.NextMaxId = topLiveResponse.NextMaxId;
                paginationParameters.PagesLoaded++;

                while (topLiveResponse.MoreAvailable
                      && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                      && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    paginationParameters.PagesLoaded++;
                    var nextTop = await GetDiscoverTopLive(paginationParameters.NextMaxId);
                    if (!nextTop.Succeeded)
                        return Result.Fail(nextTop.Info, topLive);

                    var convertedTopLive = Convert(nextTop.Value);
                    topLive.NextMaxId = paginationParameters.NextMaxId = nextTop.Value.NextMaxId;
                    topLive.MoreAvailable = topLiveResponse.MoreAvailable = nextTop.Value.MoreAvailable;
                    topLive.AutoLoadMoreEnabled = nextTop.Value.AutoLoadMoreEnabled;
                    topLive.Broadcasts.AddRange(convertedTopLive.Broadcasts);
                    topLive.PostLiveBroadcasts.AddRange(convertedTopLive.PostLiveBroadcasts);
                    paginationParameters.PagesLoaded++;
                }
                return Result.Success(topLive);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, topLive, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, topLive);
            }
        }

        /// <summary>
        ///     Get final viewer list.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        public async Task<IResult<InstaUserShortList>> GetFinalViewerListAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var viewers = new InstaUserShortList();
            try
            {
                var instaUri = UriCreator.GetLiveFinalViewerListUri(broadcastId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserShortList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaUserListShortResponse>(json);
                viewers.AddRange(
                   obj.Items?.Select(ConvertersFabric.Instance.GetUserShortConverter)
                       .Select(converter => converter.Convert()));

                return Result.Success(viewers);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, viewers, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, viewers);
            }
        }

        /// <summary>
        ///     Get heart beat and viewer count.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="offsetToVideoStart">
        ///     Offset to video start, multiple by 2. 
        ///     <para>i.e: 0 2 4 6 8 10 12 14 and etc.</para>
        /// </param>
        public async Task<IResult<InstaBroadcastLiveHeartBeatViewerCount>> GetHeartBeatAndViewerCountAsync(string broadcastId,
            uint offsetToVideoStart = 0)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetLiveHeartbeatAndViewerCountUri(broadcastId);
                var uploadId = ApiRequestMessage.GenerateUploadId();
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"offset_to_video_start", offsetToVideoStart.ToString()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastLiveHeartBeatViewerCount>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastLiveHeartBeatViewerCountResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastLiveHeartBeatViewerCountConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastLiveHeartBeatViewerCount), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastLiveHeartBeatViewerCount>(exception);
            }
        }
        /// <summary>
        ///     Get broadcast information.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        public async Task<IResult<InstaBroadcastInfo>> GetInfoAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastInfoUri(broadcastId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastInfo>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastInfoResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastInfoConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastInfo), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastInfo>(exception);
            }
        }

        /// <summary>
        ///     Get join requests to current live broadcast
        /// </summary>
        /// <param name="broadcastId">Broadcast</param>
        public async Task<IResult<InstaUserShortList>> GetJoinRequestsAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var viewers = new InstaUserShortList();
            try
            {
                var instaUri = UriCreator.GetBroadcastJoinRequestsUri(broadcastId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserShortList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaUserListShortResponse>(json);
                viewers.AddRange(
                   obj.Items?.Select(ConvertersFabric.Instance.GetUserShortConverter)
                       .Select(converter => converter.Convert()));
                return Result.Success(viewers);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, viewers, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, viewers);
            }
        }

        /// <summary>
        ///     Get broadcast like count.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="likeTs">Like time stamp</param>
        public async Task<IResult<InstaBroadcastLike>> GetLikeCountAsync(string broadcastId, int likeTs = 0)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetLiveLikeCountUri(broadcastId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastLike>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastLikeResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastLikeConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastLike), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastLike>(exception);
            }
        }

        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> GetPostLiveCommentsAsync(string broadcastId, int startingOffset = 0, string encodingTag = "instagram_dash_remuxed")
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                // kamel nist
                var instaUri = UriCreator.GetBroadcastPostLiveCommentUri(broadcastId, startingOffset, encodingTag);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<object>(response, json);
                var obj = JsonConvert.DeserializeObject<object>(json);
                return Result.Success(json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(string), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<string>(exception);
            }
        }

        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> GetPostLiveLikesAsync(string broadcastId, int startingOffset = 0, string encodingTag = "instagram_dash_remuxed")
        {
            try
            {
                var instaUri = UriCreator.GetBroadcastPostLiveLikesUri(broadcastId, startingOffset, encodingTag);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<object>(response, json);
                var obj = JsonConvert.DeserializeObject<object>(json);
                return Result.Success(json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(string), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<string>(exception);
            }
        }

        /// <summary>
        ///     Get post live viewer list.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="maxId">Max id</param>
        public async Task<IResult<InstaUserShortList>> GetPostLiveViewerListAsync(string broadcastId, int? maxId = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var viewers = new InstaUserShortList();
            try
            {
                var instaUri = UriCreator.GetPostLiveViewersListUri(broadcastId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserShortList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaUserListShortResponse>(json);
                viewers.AddRange(
                    obj.Items?.Select(ConvertersFabric.Instance.GetUserShortConverter)
                    .Select(converter => converter.Convert()));
                return Result.Success(viewers);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, viewers, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, viewers);
            }
        }

        /// <summary>
        ///     Get suggested broadcasts
        /// </summary>
        public async Task<IResult<InstaBroadcastList>> GetSuggestedBroadcastsAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSuggestedBroadcastsUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastSuggestedResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastListConverter(obj?.Broadcasts).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastList>(exception);
            }
        }

        /// <summary>
        ///     Get top live status.
        /// </summary>
        /// <param name="broadcastIds">Broadcast ids</param>
        public async Task<IResult<InstaBroadcastTopLiveStatusList>> GetTopLiveStatusAsync(params string[] broadcastIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            if (broadcastIds == null)
                return Result.Fail<InstaBroadcastTopLiveStatusList>("broadcast ids must be set");
            try
            {
                var instaUri = UriCreator.GetDiscoverTopLiveStatusUri();
                var data = new JObject
                {
                    {"broadcast_ids", new JArray(broadcastIds)},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastTopLiveStatusList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastTopLiveStatusResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastTopLiveStatusListConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastTopLiveStatusList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastTopLiveStatusList>(exception);
            }
        }
        
        /// <summary>
        ///     Get broadcast viewer list.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        public async Task<IResult<InstaUserShortList>> GetViewerListAsync(string broadcastId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var viewers = new InstaUserShortList();
            try
            {
                var instaUri = UriCreator.GetBroadcastViewerListUri(broadcastId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserShortList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaUserListShortResponse>(json);
                viewers.AddRange(
                    obj.Items?.Select(ConvertersFabric.Instance.GetUserShortConverter)
                    .Select(converter => converter.Convert()));
                return Result.Success(viewers);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, viewers, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, viewers);
            }
        }
        
        /// <summary>
        ///     Like broadcast.
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="likeCount">Like count (from 1 to 6)</param>
        public async Task<IResult<InstaBroadcastLike>> LikeAsync(string broadcastId, int likeCount = 1)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetLikeLiveUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"user_like_count", likeCount}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastLike>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastLikeResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastLikeConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastLike), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastLike>(exception);
            }
        }

        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> NotifyToFriendsAsync()
        {
            try
            {
                var instaUri = UriCreator.GetLiveNotifyToFriendsUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<object>(response, json);
                var obj = JsonConvert.DeserializeObject<object>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(object), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<object>(exception);
            }
        }

        /// <summary>
        ///     Pin comment from broadcast.
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <param name="commentId"></param>
        public async Task<IResult<InstaBroadcastPinUnpin>> PinCommentAsync(string broadcastId, string commentId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastPinCommentUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"comment_id", commentId},
                    {"offset_to_video_start", 0}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastPinUnpin>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastPinUnpinResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastPinUnpinConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastPinUnpin), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastPinUnpin>(exception);
            }
        }
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> SeenBroadcastAsync(string broadcastId, string pk)
        {
            try
            {
                var instaUri = new Uri(InstaApiConstants.BASE_INSTAGRAM_API_URL + $"media/seen/?reel=1&live_vod=0");
                Debug.WriteLine(instaUri.ToString());

                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"live_vods_skipped",  new JObject()},
                    {"nuxes_skipped",  new JObject()},
                    {"nuxes",  new JObject()},
                    {"reels",  new JObject{ { broadcastId, new JArray(pk) } } },
                    {"live_vods",  new JObject()},
                    {"reel_media_skipped",  new JObject()},

                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<object>(response, json);
                return Result.Success(json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastLiveHeartBeatViewerCountResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastLiveHeartBeatViewerCountResponse>(exception);
            }
        }

        /// <summary>
        ///     Start live broadcast. NOTE: YOU MUST CREATE AN BROADCAST FIRST(CreateAsync) AND THEN CALL THIS METHOD. 
        /// </summary>
        /// <param name="broadcastId">Broadcast id</param>
        /// <param name="latitude">longitude of your place</param>
        /// <param name="longitude">longitude of your place</param>
        public async Task<IResult<InstaBroadcastStart>> StartAsync(string broadcastId, double? latitude = null, double? longitude = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastStartUri(broadcastId);
                var data = new Dictionary<string,string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                if (latitude != null && longitude != null)
                {
                    data.Add("latitude", latitude.ToString());
                    data.Add("longitude", longitude.ToString());
                }
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaBroadcastStartResponse>(json);
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastStart>(response, json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastStartConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastStart), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastStart>(exception);
            }
        }

        /// <summary>
        ///     Share an live broadcast to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="broadcastId">Broadcast id to send ( <see cref="InstaBroadcast.Id"/> )</param>
        /// <param name="threadIds">Thread ids</param>
        public async Task<IResult<bool>> ShareLiveToDirectThreadAsync(string text, string broadcastId, params string[] threadIds)
        {
            return await ShareLiveToDirectThreadAsync(text, broadcastId, threadIds, null);
        }

        /// <summary>
        ///     Share an live broadcast to direct thread
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="broadcastId">Broadcast id to send ( <see cref="InstaBroadcast.Id"/> )</param>
        /// <param name="threadIds">Thread ids</param>
        /// <param name="recipients">Recipients ids</param>
        public async Task<IResult<bool>> ShareLiveToDirectThreadAsync(string text, string broadcastId, string[] threadIds, string[] recipients)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetShareLiveToDirectUri();
                var clientContext = Guid.NewGuid().ToString();
                var data = new Dictionary<string, string>
                {
                    {"text", text ?? string.Empty},
                    {"broadcast_id", broadcastId},
                    {"action", "send_item"},
                    {"client_context", clientContext},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (threadIds?.Length > 0)
                {
                    data.Add("thread_ids", $"[{threadIds.EncodeList(false)}]");
                }
                if (recipients?.Length > 0)
                {
                    data.Add("recipient_users", "[[" + recipients.EncodeList(false) + "]]");
                }
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
        ///     Share an live broadcast to direct recipients
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="broadcastId">Broadcast id to send ( <see cref="InstaBroadcast.Id"/> )</param>
        /// <param name="recipients">Recipients ids</param>
        public async Task<IResult<bool>> ShareLiveToDirectRecipientAsync(string text, string broadcastId, params string[] recipients)
        {
            return await ShareLiveToDirectThreadAsync(text, broadcastId, null, recipients);
        }

        /// <summary>
        ///     Unpin comment from broadcast.
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <param name="commentId"></param>
        public async Task<IResult<InstaBroadcastPinUnpin>> UnPinCommentAsync(string broadcastId, string commentId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBroadcastUnPinCommentUri(broadcastId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"comment_id", commentId},
                    {"offset_to_video_start", 0}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBroadcastPinUnpin>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaBroadcastPinUnpinResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBroadcastPinUnpinConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBroadcastPinUnpin), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaBroadcastPinUnpin>(exception);
            }
        }

        private async Task<IResult<InstaDiscoverTopLiveResponse>> GetDiscoverTopLive(string maxId)
        {
            try
            {
                var instaUri = UriCreator.GetDiscoverTopLiveUri(maxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaDiscoverTopLiveResponse>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDiscoverTopLiveResponse>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaDiscoverTopLiveResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaDiscoverTopLiveResponse>(exception);
            }
        }

        private async Task<IResult<bool>> EnableDisableJoinRequest(Uri instaUri)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var data = new JObject
                {
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
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
    }
}

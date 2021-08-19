using System;
using System.Collections.Generic;
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
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InstagramApiSharp.Enums;
using System.Threading;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Comments api functions.
    /// </summary>
    internal class CommentProcessor : ICommentProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public CommentProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Check offensive text for caption
        /// </summary>
        /// <param name="captionText">Caption text</param>
        public async Task<IResult<InstaOffensiveText>> CheckOffensiveCaptionAsync(string captionText) =>
            await CheckOffensiveText(captionText, null).ConfigureAwait(false);

        /// <summary>
        ///     Check offensive text for comment
        /// </summary>
        /// <param name="mediaId">Media identifier</param>
        /// <param name="commentText">Comment text</param>
        public async Task<IResult<InstaOffensiveText>> CheckOffensiveTextAsync(string mediaId, string commentText) =>
            await CheckOffensiveText(commentText, mediaId).ConfigureAwait(false);

        /// <summary>
        ///     Block an user from commenting to medias
        /// </summary>
        /// <param name="userIds">User ids (pk)</param>
        public async Task<IResult<bool>> BlockUserCommentingAsync(params long[] userIds)
        {
            return await BlockUnblockCommenting(true, userIds);
        }

        /// <summary>
        ///     Comment media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="text">Comment text</param>
        /// <param name="containerModule">Container module</param>
        /// <param name="feedPosition">Feed position</param>
        /// <param name="isCarouselBumpedPost">Is carousel post?</param>
        /// <param name="carouselIndex">Carousel index</param>
        public async Task<IResult<InstaComment>> CommentMediaAsync(string mediaId, string text,
            InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.FeedTimeline,
            uint feedPosition = 0, bool isCarouselBumpedPost = false, int? carouselIndex = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                text = text.Replace('\r', '\n');
                var instaUri = UriCreator.GetPostCommetUri(mediaId);
                var breadcrumb = CryptoHelper.GetCommentBreadCrumbEncoded(text);
                var data = new Dictionary<string, string>
                {
                    {"user_breadcrumb", breadcrumb},
                    {"delivery_class", "organic"},
                    {"idempotence_token", Guid.NewGuid().ToString()},
                    {"radio_type", "wifi-none"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"comment_text", text},
                    {"is_carousel_bumped_post", isCarouselBumpedPost.ToString().ToLower()},
                    {"container_module", containerModule.GetContainerType()},
                    {"feed_position", feedPosition.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (carouselIndex.HasValue)
                    data.Add("carousel_index", carouselIndex.Value.ToString());
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
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

        /// <summary>
        ///     Delete media comment
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentId">Comment id</param>
        public async Task<IResult<bool>> DeleteCommentAsync(string mediaId, string commentId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetDeleteCommentUri(mediaId, commentId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Delete media comments(multiple)
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentIds">Comment id</param>
        public async Task<IResult<bool>> DeleteMultipleCommentsAsync(string mediaId, params string[] commentIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetDeleteMultipleCommentsUri(mediaId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"comment_ids_to_delete", commentIds.EncodeList(false)}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Disable media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> DisableMediaCommentAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetDisableMediaCommetsUri(mediaId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Allow media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> EnableMediaCommentAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetAllowMediaCommetsUri(mediaId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Get blocked users from commenting
        /// </summary>
        public async Task<IResult<InstaUserShortList>> GetBlockedCommentersAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetBlockedCommentersUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserShortList>(response, json);
                
                var obj = JsonConvert.DeserializeObject<InstaBlockedCommentersResponse>(json);
                
                return Result.Success(ConvertersFabric.Instance.GetBlockedCommentersConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaUserShortList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaUserShortList>(exception);
            }
        }

        /// <summary>
        ///     Get media comments likers
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<InstaLikersList>> GetMediaCommentLikersAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetMediaCommetLikersUri(mediaId);
                var request =
                    _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaLikersList>(response, json);

                var likers = new InstaLikersList();
                var likersResponse = JsonConvert.DeserializeObject<InstaMediaLikersResponse>(json);
                likers.UsersCount = likersResponse.UsersCount;
                likers.AddRange(
                    likersResponse.Users.Select(ConvertersFabric.Instance.GetUserShortConverter)
                        .Select(converter => converter.Convert()));
                return Result.Success(likers);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaLikersList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaLikersList>(exception);
            }
        }

        /// <summary>
        ///     Get media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="targetCommentId">Target comment id</param>
        public async Task<IResult<InstaCommentList>> GetMediaCommentsAsync(string mediaId,
            PaginationParameters paginationParameters, string targetCommentId = "") =>
            await GetMediaCommentsAsync(mediaId, paginationParameters, 
                CancellationToken.None, targetCommentId).ConfigureAwait(false);

        /// <summary>
        ///     Get media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaCommentList>> GetMediaCommentsAsync(string mediaId,
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken, string targetCommentId = "")
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaCommentListResponse commentListResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var commentsUri = UriCreator.GetMediaCommentsUri(mediaId, paginationParameters.NextMaxId, targetCommentId);
                if (!string.IsNullOrEmpty(paginationParameters.NextMinId))
                    commentsUri = UriCreator.GetMediaCommentsMinIdUri(mediaId, paginationParameters.NextMinId, targetCommentId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, commentsUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaCommentList>(response, json);
                commentListResponse = JsonConvert.DeserializeObject<InstaCommentListResponse>(json);
                var pagesLoaded = 1;

                while (commentListResponse.MoreCommentsAvailable
                       && !string.IsNullOrEmpty(commentListResponse.NextMaxId)
                       && pagesLoaded < paginationParameters.MaximumPagesToLoad ||

                       commentListResponse.MoreHeadLoadAvailable
                       && !string.IsNullOrEmpty(commentListResponse.NextMinId)
                       && pagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    IResult<InstaCommentListResponse> nextComments;
                    if(!string.IsNullOrEmpty(commentListResponse.NextMaxId))
                        nextComments = await GetCommentListWithMaxIdAsync(mediaId, commentListResponse.NextMaxId, null, targetCommentId);
                    else 
                        nextComments = await GetCommentListWithMaxIdAsync(mediaId, null, commentListResponse.NextMinId, targetCommentId);

                    if (!nextComments.Succeeded)
                        return Result.Fail(nextComments.Info, GetOrDefault());
                    commentListResponse.NextMaxId = nextComments.Value.NextMaxId;
                    commentListResponse.NextMinId = nextComments.Value.NextMinId;
                    commentListResponse.MoreCommentsAvailable = nextComments.Value.MoreCommentsAvailable;
                    commentListResponse.MoreHeadLoadAvailable = nextComments.Value.MoreHeadLoadAvailable;
                    commentListResponse.Comments.AddRange(nextComments.Value.Comments);
                    paginationParameters.NextMaxId = nextComments.Value.NextMaxId;
                    paginationParameters.NextMinId = nextComments.Value.NextMinId;
                    pagesLoaded++;
                }
                paginationParameters.NextMaxId = commentListResponse.NextMaxId;
                paginationParameters.NextMinId = commentListResponse.NextMinId;

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

            InstaCommentList GetOrDefault() => commentListResponse != null ? Convert(commentListResponse) : default(InstaCommentList);

            InstaCommentList Convert(InstaCommentListResponse commentsResponse)
            {
                return ConvertersFabric.Instance.GetCommentListConverter(commentsResponse).Convert();
            }
        }
        /// <summary>
        ///     Get media inline comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="paginationParameters">Maximum amount of pages to load and start id</param>
        public async Task<IResult<InstaInlineCommentList>> GetMediaRepliesCommentsAsync(string mediaId, string targetCommentId,
            PaginationParameters paginationParameters) =>
            await GetMediaRepliesCommentsAsync(mediaId, targetCommentId, 
                paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get media inline comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="paginationParameters">Maximum amount of pages to load and start id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaInlineCommentList>> GetMediaRepliesCommentsAsync(string mediaId, string targetCommentId,
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaInlineCommentListResponse commentListResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var commentsUri = UriCreator.GetMediaInlineCommentsUri(mediaId, targetCommentId, paginationParameters.NextMaxId);
                if (!string.IsNullOrEmpty(paginationParameters.NextMinId))
                    commentsUri = UriCreator.GetMediaInlineCommentsWithMinIdUri(mediaId, targetCommentId, paginationParameters.NextMinId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, commentsUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaInlineCommentList>(response, json);
                
                commentListResponse = JsonConvert.DeserializeObject<InstaInlineCommentListResponse>(json);

                var pagesLoaded = 1;

                while (commentListResponse.HasMoreTailChildComments
                       && !string.IsNullOrEmpty(commentListResponse.NextMaxId)
                       && pagesLoaded < paginationParameters.MaximumPagesToLoad ||
                       commentListResponse.HasMoreHeadChildComments
                       && !string.IsNullOrEmpty(commentListResponse.NextMinId)
                       && pagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    IResult<InstaInlineCommentListResponse> nextComments;
                    if (!string.IsNullOrEmpty(commentListResponse.NextMaxId))
                        nextComments = await GetInlineCommentListWithMaxIdAsync(mediaId, targetCommentId, commentListResponse.NextMaxId, null);
                    else
                        nextComments = await GetInlineCommentListWithMaxIdAsync(mediaId, targetCommentId, null, commentListResponse.NextMinId);
                    
                    if (!nextComments.Succeeded)
                        return Result.Fail(nextComments.Info, GetOrDefault());

                    commentListResponse.NextMaxId = nextComments.Value.NextMaxId;
                    commentListResponse.NextMinId = nextComments.Value.NextMinId;
                    commentListResponse.HasMoreHeadChildComments = nextComments.Value.HasMoreHeadChildComments;
                    commentListResponse.HasMoreTailChildComments = nextComments.Value.HasMoreTailChildComments;
                    commentListResponse.ChildComments.AddRange(nextComments.Value.ChildComments);
                    paginationParameters.NextMaxId = nextComments.Value.NextMaxId;
                    paginationParameters.NextMinId = nextComments.Value.NextMinId;
                    pagesLoaded++;
                }
                paginationParameters.NextMaxId = commentListResponse.NextMaxId;
                paginationParameters.NextMinId = commentListResponse.NextMinId;

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
            InstaInlineCommentList GetOrDefault() => commentListResponse != null ? Convert(commentListResponse) : default(InstaInlineCommentList);

            InstaInlineCommentList Convert(InstaInlineCommentListResponse commentsResponse)
            {
                return ConvertersFabric.Instance.GetInlineCommentsConverter(commentsResponse).Convert();
            }
        }
        /// <summary>
        ///     Like media comment
        /// </summary>
        /// <param name="commentId">Comment id</param>
        public async Task<IResult<bool>> LikeCommentAsync(string commentId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetLikeCommentUri(commentId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Inline comment media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="text">Comment text</param>
        /// <param name="containerModule">Container module</param>
        /// <param name="feedPosition">Feed position</param>
        /// <param name="isCarouselBumpedPost">Is carousel post?</param>
        /// <param name="carouselIndex">Carousel index</param>
        /// <param name="inventorySource">Inventory source</param>
        public async Task<IResult<InstaComment>> ReplyCommentMediaAsync(string mediaId, string targetCommentId, string text,
            InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.FeedTimeline,
            uint feedPosition = 0, bool isCarouselBumpedPost = false, int? carouselIndex = null,
            InstaMediaInventorySource inventorySource = InstaMediaInventorySource.MediaOrAdd)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                text = text.Replace('\r', '\n');
                var instaUri = UriCreator.GetPostCommetUri(mediaId);
                var breadcrumb = CryptoHelper.GetCommentBreadCrumbEncoded(text);
                var data = new Dictionary<string, string>
                {
                    {"user_breadcrumb", breadcrumb},
                    {"inventory_source", inventorySource.GetInvetorySourceType()},
                    {"delivery_class", "organic"},
                    {"idempotence_token", Guid.NewGuid().ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"replied_to_comment_id", targetCommentId},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"comment_text", text},
                    {"is_carousel_bumped_post", isCarouselBumpedPost.ToString().ToLower()},
                    {"container_module", containerModule.GetContainerType()},
                    {"feed_position", feedPosition.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (carouselIndex.HasValue)
                    data.Add("carousel_index", carouselIndex.Value.ToString());
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
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

        /// <summary>
        ///     Report media comment
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentId">Comment id</param>
        public async Task<IResult<bool>> ReportCommentAsync(string mediaId, string commentId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetReportCommetUri(mediaId, commentId);
                var data = new Dictionary<string, string>
                {
                    {"media_id", mediaId},
                    {"comment_id", commentId},
                    {"reason", "1"},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Unblock an user from commenting to medias
        /// </summary>
        /// <param name="userIds">User ids (pk)</param>
        public async Task<IResult<bool>> UnblockUserCommentingAsync(params long[] userIds)
        {
            return await BlockUnblockCommenting(false, userIds);
        }

        /// <summary>
        ///     Unlike media comment
        /// </summary>
        /// <param name="commentId">Comment id</param>
        public async Task<IResult<bool>> UnlikeCommentAsync(string commentId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetUnLikeCommentUri(commentId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Translate comment or captions
        ///     <para>Note: use this function to translate captions too! (i.e: <see cref="InstaCaption.Pk"/>)</para>
        /// </summary>
        /// <param name="commentIds">Comment id(s) (Array of <see cref="InstaComment.Pk"/>)</param>
        public async Task<IResult<InstaTranslateList>> TranslateCommentAsync(params long[] commentIds)
        {
            try
            {
                if (commentIds == null || commentIds != null && !commentIds.Any())
                    throw new ArgumentException("At least one comment id require");

                var instaUri = UriCreator.GetTranslateCommentsUri(string.Join(",", commentIds));

                var request =
                    _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(json))
                    return Result.UnExpectedResponse<InstaTranslateList>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaTranslateContainerResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetTranslateContainerConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTranslateList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTranslateList>(exception);
            }
        }

        private async Task<IResult<InstaCommentListResponse>> GetCommentListWithMaxIdAsync(string mediaId,
                            string nextMaxId, string nextMinId, string targetCommentId)
        {
            try
            {
                var commentsUri = UriCreator.GetMediaCommentsUri(mediaId, nextMaxId, targetCommentId);
                if (!string.IsNullOrEmpty(nextMinId))
                    commentsUri = UriCreator.GetMediaCommentsMinIdUri(mediaId, nextMinId, targetCommentId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, commentsUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaCommentListResponse>(response, json);
                var comments = JsonConvert.DeserializeObject<InstaCommentListResponse>(json);
                return Result.Success(comments);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaCommentListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaCommentListResponse>(exception);
            }
        }

        private async Task<IResult<InstaInlineCommentListResponse>> GetInlineCommentListWithMaxIdAsync(string mediaId,
    string targetCommandId,
    string nextMaxId, string nextMinId)
        {
            try
            {
                var commentsUri = UriCreator.GetMediaInlineCommentsUri(mediaId, targetCommandId, nextMaxId);
                if (!string.IsNullOrEmpty(nextMinId))
                    commentsUri = UriCreator.GetMediaInlineCommentsWithMinIdUri(mediaId, targetCommandId, nextMinId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, commentsUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaInlineCommentListResponse>(response, json);
                var commentListResponse = JsonConvert.DeserializeObject<InstaInlineCommentListResponse>(json);
                return Result.Success(commentListResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaInlineCommentListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaInlineCommentListResponse>(exception);
            }
        }

        private async Task<IResult<bool>> BlockUnblockCommenting(bool block, long[] userIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (userIds == null || userIds?.Length == 0)
                    Result.Fail<bool>("At least one user id (pk) is require");

                var instaUri = UriCreator.GetSetBlockedCommentersUri();
                //var blockedUsersResponse = await GetBlockedCommentersAsync();
                //var blockedUsers = new List<long>();
                //if (blockedUsersResponse.Succeeded && blockedUsersResponse.Value?.Count > 0)
                //{
                //    foreach (var u in blockedUsersResponse.Value)
                //    {
                //        foreach (var id in userIds)
                //            if (u.Pk == id)
                //                blockedUsers.Add(u.Pk);
                //    }
                //}

                //{
                //	"_csrftoken": "UBPgM6BG1Qr95lO4ofLYpgJXtbVvVnvs",
                //	"_uid": "7405924766",
                //	"_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //	"commenter_block_status": {
                //		"block": [9013775990, 9013775990],
                //		"unblock": [9013775990]
                //	}
                //}
                var commenterBlockStatus = new JObject();
                if (block)
                {
                    commenterBlockStatus.Add("block", new JArray(userIds));
                    commenterBlockStatus.Add("unblock", new JArray());
                }
                else
                {
                    commenterBlockStatus.Add("block", new JArray());
                    commenterBlockStatus.Add("unblock", new JArray(userIds));
                }

                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"commenter_block_status", commenterBlockStatus}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return response.StatusCode == HttpStatusCode.OK
                    ? Result.Success(true)
                    : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        async Task<IResult<InstaOffensiveText>> CheckOffensiveText(string text, string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                text = text?.Replace('\r', '\n');
                var instaUri = UriCreator.GetCheckOffensiveTextUri();
                var data = new Dictionary<string, string>
                {
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (!string.IsNullOrEmpty(mediaId))
                {
                    data.Add("media_id", mediaId);
                    data.Add("comment_text", text ?? string.Empty);
                }
                else
                {
                    data.Add("text", text ?? string.Empty);
                    data.Add("request_type", "caption");
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaOffensiveText>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaOffensiveText>(json);
                if (obj.IsSucceed)
                    return Result.Success(obj);
                else
                    return Result.UnExpectedResponse<InstaOffensiveText>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaOffensiveText), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaOffensiveText));
            }
        }
    }
}
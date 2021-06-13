﻿using System;
using System.Collections.Generic;
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
    ///     Hashtag api functions.
    /// </summary>
    internal class HashtagProcessor : IHashtagProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public HashtagProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Seen hashtag story as seen
        /// </summary>
        /// <param name="hashtagId">Hashtag Id</param>
        /// <param name="storyMediaId">Story media identifier</param>
        /// <param name="takenAtUnix">Taken at unix</param>
        public async Task<IResult<bool>> MarkHashtagStoryAsSeenAsync(string hashtagId, string storyMediaId, long takenAtUnix)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSeenMediaStoryUri();
                var storyId = $"{storyMediaId}_{hashtagId}";
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
                    {"container_module", "hashtag_feed"},
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
        ///     Get medias for hashtag channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetHashtagChannelVideosAsync(string channelId, string firstMediaId,
            PaginationParameters paginationParameters) =>
            await GetHashtagChannelVideosAsync(channelId, firstMediaId, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get medias for hashtag channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetHashtagChannelVideosAsync(string channelId, string firstMediaId,
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            return await _instaApi.HelperProcessor.GetChannelVideosAsync(UriCreator.GetHashtagChannelVideosUri(channelId), firstMediaId, paginationParameters, cancellationToken);
        }
        /// <summary>
        ///     Get hashtag sections
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="hashtagSectionType">Section type</param>
        public async Task<IResult<InstaSectionMedia>> GetHashtagsSectionsAsync(string tagname,
            PaginationParameters paginationParameters, InstaHashtagSectionType hashtagSectionType = InstaHashtagSectionType.All) =>
            await GetHashtagsSectionsAsync(tagname, paginationParameters, CancellationToken.None, hashtagSectionType).ConfigureAwait(false);


        /// <summary>
        ///     Get hashtag sections
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="hashtagSectionType">Section type</param>
        public async Task<IResult<InstaSectionMedia>> GetHashtagsSectionsAsync(string tagname, PaginationParameters paginationParameters,
            CancellationToken cancellationToken, InstaHashtagSectionType hashtagSectionType = InstaHashtagSectionType.All)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaSectionMediaListResponse mediaResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var mediaResult = await GetHashtagSection(tagname,
                   hashtagSectionType,
                    paginationParameters.NextMaxId, paginationParameters.NextPage).ConfigureAwait(false);
                mediaResponse = mediaResult.Value;
                if (!mediaResult.Succeeded)
                    Result.Fail(mediaResult.Info, GetOrDefault());

                paginationParameters.NextMediaIds = mediaResponse.NextMediaIds;
                paginationParameters.NextPage = mediaResponse.NextPage;
                paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                while (mediaResponse.MoreAvailable
                    && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                    && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var moreMedias = await GetHashtagSection(tagname, hashtagSectionType,
                        paginationParameters.NextMaxId, mediaResponse.NextPage).ConfigureAwait(false);
                    if (!moreMedias.Succeeded)
                    {
                        if (mediaResponse.Sections != null && mediaResponse.Sections.Any())
                            return Result.Success(GetOrDefault());
                        else
                            return Result.Fail(moreMedias.Info, GetOrDefault());
                    }

                    mediaResponse.MoreAvailable = moreMedias.Value.MoreAvailable;
                    mediaResponse.NextMaxId = paginationParameters.NextMaxId = moreMedias.Value.NextMaxId;
                    mediaResponse.AutoLoadMoreEnabled = moreMedias.Value.AutoLoadMoreEnabled;
                    mediaResponse.NextMediaIds = paginationParameters.NextMediaIds = moreMedias.Value.NextMediaIds;
                    mediaResponse.NextPage = paginationParameters.NextPage = moreMedias.Value.NextPage;
                    mediaResponse.Sections.AddRange(moreMedias.Value.Sections);
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

            InstaSectionMedia GetOrDefault() => mediaResponse != null ? Convert(mediaResponse) : default(InstaSectionMedia);

            InstaSectionMedia Convert(InstaSectionMediaListResponse hashtagMediaListResponse)
            {
                return ConvertersFabric.Instance.GetHashtagMediaListConverter(hashtagMediaListResponse).Convert();
            }
        }

        /// <summary>
        ///     Pagination nadare, koskhol bazi dar nayaria
        /// </summary>
        public async Task<IResult<InstaHashtagSearch>> GetHashtagsPostsAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var tags = new InstaHashtagSearch();
            try
            {
                var userUri = UriCreator.GetMediaTagsUri(mediaId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaHashtagSearch>(response, json);

                var tagsResponse = JsonConvert.DeserializeObject<InstaHashtagSearchResponse>(json,
                    new InstaHashtagSuggestedDataConverter());

                tags = ConvertersFabric.Instance.GetHashTagsSearchConverter(tagsResponse).Convert();
                return Result.Success(tags);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHashtagSearch), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, tags);
            }
        }

        /// <summary>
        ///     Follow a hashtag
        /// </summary>
        /// <param name="tagname">Tag name</param>
        public async Task<IResult<bool>> FollowHashtagAsync(string tagname)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetFollowHashtagUri(tagname);

                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
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
        ///     Get following hashtags information
        /// </summary>
        /// <param name="userId">User identifier (pk)</param>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        public async Task<IResult<InstaHashtagSearch>> GetFollowingHashtagsInfoAsync(long userId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var tags = new InstaHashtagSearch();
            try
            {
                var userUri = UriCreator.GetFollowingTagsInfoUri(userId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaHashtagSearch>(response, json);

                var tagsResponse = JsonConvert.DeserializeObject<InstaHashtagSearchResponse>(json,
                    new InstaHashtagSuggestedDataConverter());

                tags = ConvertersFabric.Instance.GetHashTagsSearchConverter(tagsResponse).Convert();
                return Result.Success(tags);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHashtagSearch), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, tags);
            }
        }

        /// <summary>
        ///     Gets the hashtag information by user tagname.
        /// </summary>
        /// <param name="tagname">Tagname</param>
        /// <returns>Hashtag information</returns>
        public async Task<IResult<InstaHashtag>> GetHashtagInfoAsync(string tagname)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var userUri = UriCreator.GetTagInfoUri(tagname);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaHashtag>(response, json);

                var tagInfoResponse = JsonConvert.DeserializeObject<InstaHashtagResponse>(json);
                var tagInfo = ConvertersFabric.Instance.GetHashTagConverter(tagInfoResponse).Convert();

                return Result.Success(tagInfo);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHashtag), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHashtag>(exception);
            }
        }

        /// <summary>
        ///     Get stories of an hashtag
        /// </summary>
        /// <param name="tagname">Tag name</param>
        public async Task<IResult<InstaHashtagStory>> GetHashtagStoriesAsync(string tagname)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetHashtagStoryUri(tagname);

                var request =
                    _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaHashtagStory>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaHashtagStoryContainerResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetHashtagStoryConverter(obj.Story).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHashtagStory), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaHashtagStory>(exception);
            }
        }

        /// <summary>
        ///     Get recent hashtag media list
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaSectionMedia>> GetRecentHashtagMediaListAsync(string tagname,
            PaginationParameters paginationParameters) =>
            await GetRecentHashtagMediaListAsync(tagname, paginationParameters, CancellationToken.None).ConfigureAwait(false);


        /// <summary>
        ///     Get recent hashtag media list
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaSectionMedia>> GetRecentHashtagMediaListAsync(string tagname,
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaSectionMediaListResponse mediaResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var mediaResult = await GetHashtagRecentMedia(tagname,
                    _deviceInfo.DeviceGuid.ToString(),
                    paginationParameters.NextMaxId, paginationParameters.NextPage, paginationParameters.NextMediaIds);
                mediaResponse = mediaResult.Value;
                if (!mediaResult.Succeeded)
                    Result.Fail(mediaResult.Info, GetOrDefault());

                paginationParameters.NextMediaIds = mediaResponse.NextMediaIds;
                paginationParameters.NextPage = mediaResponse.NextPage;
                paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                while (mediaResponse.MoreAvailable
                    && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                    && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var moreMedias = await GetHashtagRecentMedia(tagname, _deviceInfo.DeviceGuid.ToString(),
                        paginationParameters.NextMaxId, mediaResponse.NextPage, mediaResponse.NextMediaIds);
                    if (!moreMedias.Succeeded)
                    {
                        if (mediaResponse.Sections != null && mediaResponse.Sections.Any())
                            return Result.Success(GetOrDefault());
                        else
                            return Result.Fail(moreMedias.Info, GetOrDefault());
                    }

                    mediaResponse.MoreAvailable = moreMedias.Value.MoreAvailable;
                    mediaResponse.NextMaxId = paginationParameters.NextMaxId = moreMedias.Value.NextMaxId;
                    mediaResponse.AutoLoadMoreEnabled = moreMedias.Value.AutoLoadMoreEnabled;
                    mediaResponse.NextMediaIds = paginationParameters.NextMediaIds = moreMedias.Value.NextMediaIds;
                    mediaResponse.NextPage = paginationParameters.NextPage = moreMedias.Value.NextPage;
                    mediaResponse.Sections.AddRange(moreMedias.Value.Sections);
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

            InstaSectionMedia GetOrDefault() => mediaResponse != null ? Convert(mediaResponse) : default(InstaSectionMedia);

            InstaSectionMedia Convert(InstaSectionMediaListResponse hashtagMediaListResponse)
            {
                return ConvertersFabric.Instance.GetHashtagMediaListConverter(hashtagMediaListResponse).Convert();
            }
        }

        /// <summary>
        ///     Get suggested hashtags
        /// </summary>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        public async Task<IResult<InstaHashtagSearch>> GetSuggestedHashtagsAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var tags = new InstaHashtagSearch();
            try
            {
                var userUri = UriCreator.GetSuggestedTagsUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaHashtagSearch>(response, json);

                var tagsResponse = JsonConvert.DeserializeObject<InstaHashtagSearchResponse>(json,
                    new InstaHashtagSuggestedDataConverter());

                tags = ConvertersFabric.Instance.GetHashTagsSearchConverter(tagsResponse).Convert();
                return Result.Success(tags);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHashtagSearch), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, tags);
            }
        }

        /// <summary>
        ///     Get top (ranked) hashtag media list
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaSectionMedia>> GetTopHashtagMediaListAsync(string tagname,
            PaginationParameters paginationParameters) =>
            await GetTopHashtagMediaListAsync(tagname, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get top (ranked) hashtag media list
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaSectionMedia>> GetTopHashtagMediaListAsync(string tagname,
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaSectionMediaListResponse mediaResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var mediaResult = await GetHashtagTopMedia(tagname,
                    _deviceInfo.DeviceGuid.ToString(),
                    paginationParameters.NextMaxId, paginationParameters.NextPage, paginationParameters.NextMediaIds).ConfigureAwait(false);
                mediaResponse = mediaResult.Value;
                if (!mediaResult.Succeeded)
                    Result.Fail(mediaResult.Info, GetOrDefault());

                paginationParameters.NextMediaIds = mediaResponse.NextMediaIds;
                paginationParameters.NextPage = mediaResponse.NextPage;
                paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                while (mediaResponse.MoreAvailable
                    && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                    && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var moreMedias = await GetHashtagTopMedia(tagname, _deviceInfo.DeviceGuid.ToString(),
                        paginationParameters.NextMaxId, mediaResponse.NextPage, mediaResponse.NextMediaIds).ConfigureAwait(false);
                    if (!moreMedias.Succeeded)
                    {
                        if (mediaResponse.Sections != null && mediaResponse.Sections.Any())
                            return Result.Success(GetOrDefault());
                        else
                            return Result.Fail(moreMedias.Info, GetOrDefault());
                    }

                    mediaResponse.MoreAvailable = moreMedias.Value.MoreAvailable;
                    mediaResponse.NextMaxId = paginationParameters.NextMaxId = moreMedias.Value.NextMaxId;
                    mediaResponse.AutoLoadMoreEnabled = moreMedias.Value.AutoLoadMoreEnabled;
                    mediaResponse.NextMediaIds = paginationParameters.NextMediaIds = moreMedias.Value.NextMediaIds;
                    mediaResponse.NextPage = paginationParameters.NextPage = moreMedias.Value.NextPage;
                    mediaResponse.Sections.AddRange(moreMedias.Value.Sections);
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

            InstaSectionMedia GetOrDefault() => mediaResponse != null ? Convert(mediaResponse) : default(InstaSectionMedia);

            InstaSectionMedia Convert(InstaSectionMediaListResponse hashtagMediaListResponse)
            {
                return ConvertersFabric.Instance.GetHashtagMediaListConverter(hashtagMediaListResponse).Convert();
            }
        }

        /// <summary>
        ///     Searches for specific hashtag by search query.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="excludeList">Array of numerical hashtag IDs (ie "17841562498105353") to exclude from the response, allowing you to skip tags from a previous call to get more results</param>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        public async Task<IResult<InstaHashtagSearch>> SearchHashtagAsync(string query, IEnumerable<long> excludeList)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var RequestHeaderFieldsTooLarge = (HttpStatusCode)431;
            var count = 50;
            var tags = new InstaHashtagSearch();

            try
            {
                var userUri = UriCreator.GetSearchTagUri(query, count, excludeList, _user.RankToken, _instaApi.TimezoneOffset);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == RequestHeaderFieldsTooLarge)
                    return Result.Success(tags);
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaHashtagSearch>(response, json);

                var tagsResponse = JsonConvert.DeserializeObject<InstaHashtagSearchResponse>(json,
                    new InstaHashtagSearchDataConverter());
                tags = ConvertersFabric.Instance.GetHashTagsSearchConverter(tagsResponse).Convert();

                if (tags.Any() && excludeList != null && excludeList.Contains(tags.First().Id))
                    tags.RemoveAt(0);

                if (!tags.Any())
                    tags = new InstaHashtagSearch();

                return Result.Success(tags);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaHashtagSearch), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, tags);
            }
        }

        public async Task<IResult<bool>> ReportHashtagMediaAsync(string tagname, string hashtagId, string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetHashtagMediaReportUri();
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"tag", tagname},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"h_id", hashtagId},
                    {"m_pk", mediaId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
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
        ///     Unfollow a hashtag
        /// </summary>
        /// <param name="tagname">Tag name</param>
        public async Task<IResult<bool>> UnFollowHashtagAsync(string tagname)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetUnFollowHashtagUri(tagname);

                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
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
        private async Task<IResult<InstaSectionMediaListResponse>> GetHashtagRecentMedia(string tagname,
            string rankToken = null,
            string maxId = null,
            int? page = null,
            List<long> nextMediaIds = null)
        {
            try
            {
                var instaUri = UriCreator.GetHashtagRecentMediaUri(tagname, rankToken,
                    maxId, page, nextMediaIds);

                var request =
                    _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaSectionMediaListResponse>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaSectionMediaListResponse>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaSectionMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaSectionMediaListResponse>(exception);
            }
        }

        private async Task<IResult<InstaSectionMediaListResponse>> GetHashtagTopMedia(string tagname,
            string rankToken = null,
            string maxId = null,
            int? page = null,
            List<long> nextMediaIds = null)
        {
            try
            {
                var instaUri = UriCreator.GetHashtagRankedMediaUri(tagname, rankToken,
                    maxId, page, nextMediaIds);

                var request =
                    _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaSectionMediaListResponse>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaSectionMediaListResponse>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaSectionMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaSectionMediaListResponse>(exception);
            }
        }

        private async Task<IResult<InstaSectionMediaListResponse>> GetHashtagSection(string tagname,
            InstaHashtagSectionType sectionType,
            string nextMaxId = null,
            int? page = null)
        {
            try
            {
                var instaUri = UriCreator.GetHashtagSectionUri(tagname);
                var supportedTabs = new JArray();
                if (sectionType == InstaHashtagSectionType.All && string.IsNullOrEmpty(nextMaxId))
                {
                    supportedTabs.Add("top");
                    supportedTabs.Add("recent");
                    //supportedTabs.Add("discover");
                }
                else
                {
                    if (!string.IsNullOrEmpty(nextMaxId) && sectionType == InstaHashtagSectionType.All)
                        sectionType = InstaHashtagSectionType.Top;

                    supportedTabs.Add(sectionType.ToString().ToLower());
                }
                var data = new Dictionary<string, string>
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"include_persistent", string.IsNullOrEmpty(nextMaxId) ? "true": "false"},
                    {"rank_token", _user.RankToken ?? Guid.NewGuid().ToString()}
                };
                if (string.IsNullOrEmpty(nextMaxId))
                    data.Add("supported_tabs", supportedTabs.ToString(Formatting.None));
                else
                {
                    data.Add("max_id", nextMaxId);
                    data.Add("tab", sectionType.ToString().ToLower());
                    data.Add("page", page.ToString());
                }

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaSectionMediaListResponse>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaSectionMediaListResponse>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaSectionMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaSectionMediaListResponse>(exception);
            }
        }


    }
}
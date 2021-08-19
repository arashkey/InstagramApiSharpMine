﻿using System;
using System.Collections.Generic;
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
    ///     Media api functions.
    /// </summary>
    internal class MediaProcessor : IMediaProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public MediaProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Add an post to archive list (this will show the post only for you!)
        /// </summary>
        /// <param name="mediaId">Media id (<see cref="InstaMedia.InstaIdentifier"/>)</param>
        /// <returns>Return true if the media is archived</returns>
        public async Task<IResult<bool>> ArchiveMediaAsync(string mediaId)
        {
            return await ArchiveUnArchiveMediaInternal(mediaId, UriCreator.GetArchiveMediaUri(mediaId));
        }

        /// <summary>
        ///     Delete a media (photo, video or album)
        /// </summary>
        /// <param name="mediaId">Media id (<see cref="InstaMedia.InstaIdentifier"/>)</param>
        /// <param name="mediaType">The type of the media</param>
        /// <returns>Return true if the media is deleted</returns>
        public async Task<IResult<bool>> DeleteMediaAsync(string mediaId, InstaMediaType mediaType)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var deleteMediaUri = UriCreator.GetDeleteMediaUri(mediaId, mediaType);

                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"media_id", mediaId}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }

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
        ///     Edit the caption/location of the media (photo/video/album)
        /// </summary>
        /// <param name="mediaId">The media ID</param>
        /// <param name="caption">The new caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        /// <param name="userTags">User tags => Optional</param>
        /// <returns>Return true if everything is ok</returns>
        public async Task<IResult<InstaMedia>> EditMediaAsync(string mediaId, string caption, InstaLocationShort location = null, InstaUserTagUpload[] userTags = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                var editMediaUri = UriCreator.GetEditMediaUri(mediaId);

                var currentMedia = await GetMediaByIdAsync(mediaId);

                var data = new JObject
                {
                    {"caption_text", caption ?? string.Empty},
                    {"fb_user_tags", "{\"in\":[]}"},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"container_module", "edit_media_info"}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (location != null)
                    data.Add("location", location.GetJson());
                else
                    data.Add("location", "{}");

                var removeArr = new JArray();
                if (currentMedia.Succeeded)
                {
                    if (currentMedia.Value?.UserTags?.Count > 0)
                    {
                        foreach (var user in currentMedia.Value.UserTags)
                            removeArr.Add(user.User.Pk.ToString());
                    }
                }
                if (userTags?.Length > 0)
                {
                    var currentDelay = _instaApi.GetRequestDelay();
                    _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));

                    var tagArr = new JArray();

                    foreach (var tag in userTags)
                    {
                        try
                        {
                            bool tried = false;
                        TryLabel:
                            var u = await _instaApi.UserProcessor.GetUserAsync(tag.Username);
                            if (!u.Succeeded)
                            {
                                if (!tried)
                                {
                                    tried = true;
                                    goto TryLabel;
                                }
                            }
                            else
                            {
                                var position = new JArray(tag.X, tag.Y);
                                var singleTag = new JObject
                                {
                                    {"user_id", u.Value.Pk},
                                    {"position", position}
                                };
                                tagArr.Add(singleTag);
                            }

                        }
                        catch { }
                    }

                    _instaApi.SetRequestDelay(currentDelay);
                    var root = new JObject
                    {
                        {"in",  tagArr}
                    };
                    if (removeArr.Any())
                        root.Add("removed", removeArr);

                    data.Add("usertags", root.ToString(Formatting.None));
                }
                else
                {
                    if (removeArr.Any())
                    {
                        var root = new JObject
                        {
                            {"removed", removeArr}
                        };
                        data.Add("usertags", root.ToString(Formatting.None));
                    }
                    else
                        data.Add("usertags", "{\"in\":[]}");
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, editMediaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var mediaResponse = JsonConvert.DeserializeObject<InstaMediaItemResponse>(json,
                        new InstaMediaDataConverter());
                    var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);
                    return Result.Success(converter.Convert());
                }
                var error = JsonConvert.DeserializeObject<BadStatusResponse>(json);
                return Result.Fail(error.Message, (InstaMedia)null);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }

        public async Task<IResult<InstaMediaList>> GetArchivedMediaAsync(PaginationParameters paginationParameters) =>
            await GetArchivedMediaAsync(paginationParameters, CancellationToken.None).ConfigureAwait(false);

        public async Task<IResult<InstaMediaList>> GetArchivedMediaAsync(PaginationParameters paginationParameters, 
            CancellationToken cancellationToken)
        {
            var mediaList = new InstaMediaList();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaMediaList Convert(InstaMediaListResponse instaMediaListResponse)
                {
                    return ConvertersFabric.Instance.GetMediaListConverter(instaMediaListResponse).Convert();
                }

                var archivedPostsResult = await GetArchivedMedia(paginationParameters?.NextMaxId);
                if (!archivedPostsResult.Succeeded)
                    return Result.Fail(archivedPostsResult.Info, mediaList);
                var archivedResponse = archivedPostsResult.Value;

                mediaList = Convert(archivedResponse);
                mediaList.NextMaxId = paginationParameters.NextMaxId = archivedResponse.NextMaxId;

                paginationParameters.PagesLoaded++;
                while (archivedResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    paginationParameters.PagesLoaded++;
                    var nextMedia = await GetArchivedMedia(paginationParameters.NextMaxId);
                    if (!nextMedia.Succeeded)
                        return Result.Fail(nextMedia.Info, mediaList);
                    mediaList.NextMaxId = paginationParameters.NextMaxId = nextMedia.Value.NextMaxId;
                    archivedResponse.MoreAvailable = nextMedia.Value.MoreAvailable;
                    archivedResponse.ResultsCount += nextMedia.Value.ResultsCount;
                    mediaList.AddRange(Convert(nextMedia.Value));
                    paginationParameters.PagesLoaded++;
                }

                mediaList.Pages = paginationParameters.PagesLoaded;
                mediaList.PageSize = archivedResponse.ResultsCount;
                return Result.Success(mediaList);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, mediaList, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, mediaList);
            }
        }

        /// <summary>
        ///     Get blocked medias
        ///     <para>Note: returns media ids!</para>
        /// </summary>
        public async Task<IResult<InstaMediaIdList>> GetBlockedMediasAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var mediaIds = new InstaMediaIdList();
            try
            {
                var mediaUri = UriCreator.GetBlockedMediaUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, mediaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMediaIdList>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaMediaIdsResponse>(json);

                return Result.Success(obj.MediaIds);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, mediaIds, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, mediaIds);
            }
        }

        /// <summary>
        ///     Get multiple media by its multiple ids asynchronously
        /// </summary>
        /// <param name="mediaIds">Media ids</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetMediaByIdsAsync(params string[] mediaIds)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var mediaList = new InstaMediaList();
            try
            {
                if (mediaIds?.Length == 0)
                    throw new ArgumentNullException("At least one media id is required");

                var instaUri = UriCreator.GetMediaInfoByMultipleMediaIdsUri(mediaIds, 
                    _deviceInfo.DeviceGuid.ToString(),
                    !_httpHelper.NewerThan180 ? _user.CsrfToken : null);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMediaList>(response, json);

                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaListResponse>(json,
                    new InstaMediaListDataConverter());
                mediaList = ConvertersFabric.Instance.GetMediaListConverter(mediaResponse).Convert();

                return Result.Success(mediaList);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, mediaList, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, mediaList);
            }
        }
        /// <summary>
        ///     Get media by its id asynchronously
        /// </summary>
        /// <param name="mediaId">Media id (<see cref="InstaMedia.InstaIdentifier"/>)</param>
        /// <returns>
        ///     <see cref="InstaMedia" />
        /// </returns>
        public async Task<IResult<InstaMedia>> GetMediaByIdAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var mediaUri = UriCreator.GetMediaUri(mediaId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, mediaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaListResponse>(json,
                    new InstaMediaListDataConverter());
                if (mediaResponse.Medias?.Count > 1)
                {
                    var errorMessage = $"Got wrong media count for request with media id={mediaId}";
                    _logger?.LogInfo(errorMessage);
                    return Result.Fail<InstaMedia>(errorMessage);
                }

                var converter =
                    ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse.Medias.FirstOrDefault());
                return Result.Success(converter.Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }

        /// <summary>
        ///     Get media ID from an url (got from "share link")
        /// </summary>
        /// <param name="uri">Uri to get media ID</param>
        /// <returns>Media ID</returns>
        public async Task<IResult<string>> GetMediaIdFromUrlAsync(Uri uri)
        {
            try
            {
                var collectionUri = UriCreator.GetMediaIdFromUrlUri(uri);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, collectionUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (json?.ToLower() == "private media")
                    return Result.Fail("Private media", ResponseType.PrivateMedia, default(string));

                if (json?.ToLower() == "no media match")
                    return Result.Fail("No Media Match", ResponseType.NoMediaMatch, default(string));

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<string>(response, json);

                var data = JsonConvert.DeserializeObject<InstaOembedUrlResponse>(json);
                return Result.Success(data.MediaId);
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
        ///     Get users (short) who liked certain media. Normaly it return around 1000 last users.
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<InstaLikersList>> GetMediaLikersAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var likers = new InstaLikersList();
                var likersUri = UriCreator.GetMediaLikersUri(mediaId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, likersUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaLikersList>(response, json);
                var mediaLikersResponse = JsonConvert.DeserializeObject<InstaMediaLikersResponse>(json);
                likers.UsersCount = mediaLikersResponse.UsersCount;
                if (mediaLikersResponse.UsersCount < 1) return Result.Success(likers);
                likers.AddRange(
                    mediaLikersResponse.Users.Select(ConvertersFabric.Instance.GetUserShortConverter)
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
        ///     Get share link from media Id
        /// </summary>
        /// <param name="mediaId">media ID</param>
        /// <returns>Share link as Uri</returns>
        public async Task<IResult<Uri>> GetShareLinkFromMediaIdAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var collectionUri = UriCreator.GetShareLinkFromMediaId(mediaId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, collectionUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<Uri>(response, json);

                var data = JsonConvert.DeserializeObject<InstaPermalinkResponse>(json);
                return Result.Success(new Uri(data.Permalink));
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(Uri), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<Uri>(exception);
            }
        }

        /// <summary>
        ///     Like media (photo or video)
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="containerModule">Container model (optional)</param>
        /// <param name="feedPosition">Feed position (optional)</param>
        /// <param name="inventorySource">Inventory source (optional)</param>
        /// <param name="isCarouselBumpedPost">Is carousel post? (optional)</param>
        /// <param name="carouselIndex">Carousel index (optional)</param>
        /// <param name="exploreSourceToken">ExploreSourceToken (optional)</param>
        /// <param name="parentMediaPK">ParentMediaPk (optional)</param>
        /// <param name="chainingSessionId">ChainingSessionId</param>
        /// <param name="navChain">Navigation chain</param>
        /// <returns>Returns True, if succeeded</returns>
        public async Task<IResult<bool>> LikeMediaAsync(string mediaId, InstaMediaContainerModuleType containerModule = InstaMediaContainerModuleType.FeedTimeline,
            uint feedPosition = 0, InstaMediaInventorySource inventorySource = InstaMediaInventorySource.None,
            bool? isCarouselBumpedPost = false, int? carouselIndex = null, string exploreSourceToken = null,
            string parentMediaPK = null, string chainingSessionId = null, string navChain = null)
        {
            return await LikeUnlikeMediaInternal(UriCreator.GetLikeMediaUri(mediaId), mediaId, containerModule,
               feedPosition, inventorySource, isCarouselBumpedPost, carouselIndex, exploreSourceToken, parentMediaPK, chainingSessionId, navChain);
        }

        /// <summary>
        ///     Report media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> ReportMediaAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetReportMediaUri(mediaId);
                var data = new Dictionary<string, string>
                {
                    {"media_id", mediaId},
                    {"reason", "1"},
                    {"source_name", "photo_view_profile"},
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
        ///     Save media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> SaveMediaAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSaveMediaUri(mediaId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
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

        /// <summary>
        ///     Remove an post from archive list (this will show the post for everyone!)
        /// </summary>
        /// <param name="mediaId">Media id (<see cref="InstaMedia.InstaIdentifier"/>)</param>
        /// <returns>Return true if the media is unarchived</returns>
        public async Task<IResult<bool>> UnArchiveMediaAsync(string mediaId)
        {
            return await ArchiveUnArchiveMediaInternal(mediaId, UriCreator.GetUnArchiveMediaUri(mediaId));
        }

        /// <summary>
        ///     Remove like from media (photo or video)
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="containerModule">Container model (optional)</param>
        /// <param name="feedPosition">Feed position (optional)</param>
        /// <param name="inventorySource">Inventory source (optional)</param>
        /// <param name="isCarouselBumpedPost">Is carousel post? (optional)</param>
        /// <param name="carouselIndex">Carousel index (optional)</param>
        /// <param name="exploreSourceToken">ExploreSourceToken (optional)</param>
        /// <param name="parentMediaPK">ParentMediaPk (optional)</param>
        /// <param name="chainingSessionId">ChainingSessionId</param>
        /// <param name="navChain">Navigation chain</param>
        /// <returns>Returns True, if succeeded</returns>
        public async Task<IResult<bool>> UnLikeMediaAsync(string mediaId, InstaMediaContainerModuleType containerModule = InstaMediaContainerModuleType.FeedTimeline,
           uint feedPosition = 0, InstaMediaInventorySource inventorySource = InstaMediaInventorySource.None,
            bool? isCarouselBumpedPost = false, int? carouselIndex = null, string exploreSourceToken = null,
            string parentMediaPK = null, string chainingSessionId = null, string navChain = null)
        {
            return await LikeUnlikeMediaInternal(UriCreator.GetUnLikeMediaUri(mediaId), mediaId, containerModule,
               feedPosition, inventorySource, isCarouselBumpedPost, carouselIndex, exploreSourceToken, parentMediaPK, chainingSessionId, navChain);
        }

        /// <summary>
        ///     Unsave media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> UnSaveMediaAsync(string mediaId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetUnSaveMediaUri(mediaId);
                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
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

        /// <summary>
        ///     Upload album (videos and photos)
        /// </summary>
        /// <param name="images">Array of photos to upload</param>
        /// <param name="videos">Array of videos to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadAlbumAsync(InstaImageUpload[] images, InstaVideoUpload[] videos, string caption, InstaLocationShort location = null)
        {
            return await UploadAlbumAsync(null, images, videos, caption, location);
        }

        /// <summary>
        ///     Upload album (videos and photos)
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="images">Array of photos to upload</param>
        /// <param name="videos">Array of videos to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadAlbumAsync(Action<InstaUploaderProgress> progress, InstaImageUpload[] images, InstaVideoUpload[] videos, string caption, InstaLocationShort location = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                upProgress.Name = "Album upload";
                progress?.Invoke(upProgress);
                var imagesUploadIds = new Dictionary<string, InstaImageUpload>();
                var index = 1;
                if (images?.Length > 0)
                {
                    foreach (var image in images)
                    {
                        if (image.UserTags?.Count > 0)
                        {
                            var currentDelay = _instaApi.GetRequestDelay();
                            _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                            foreach (var t in image.UserTags)
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
                    }
                    foreach (var image in images)
                    {
                        upProgress.Name = $"[Album] Photo uploading {index}/{images.Length}";
                        upProgress.UploadState = InstaUploadState.Uploading;
                        progress?.Invoke(upProgress);
                        upProgress.UploadState = InstaUploadState.Uploading;
                        progress?.Invoke(upProgress);
                        var uploadId = await _instaApi.HelperProcessor.UploadSinglePhoto(progress, image, upProgress);
                        if (uploadId.Succeeded)
                        {
                            upProgress.UploadState = InstaUploadState.Uploaded;
                            progress?.Invoke(upProgress);
                            imagesUploadIds.Add(uploadId.Value, image);
                        }
                        else
                        {
                            upProgress.UploadState = InstaUploadState.Error;
                            progress?.Invoke(upProgress);
                            return Result.Fail<InstaMedia>(uploadId.Info.Message);
                        }
                    }
                }

                var videosDic = new Dictionary<string, InstaVideoUpload>();
                var vidIndex = 1;
                if (videos?.Length > 0)
                {
                    foreach (var video in videos)
                    {
                        foreach (var t in video.UserTags)
                        {
                            var currentDelay = _instaApi.GetRequestDelay();
                            _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                            if (t.Pk <= 0)
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
                    }

                    foreach (var video in videos)
                    {
                        upProgress.Name = $"[Album] Video uploading {vidIndex}/{videos.Length}";
                        upProgress.UploadState = InstaUploadState.Uploading;
                        progress?.Invoke(upProgress);
                        var uploadId = await _instaApi.HelperProcessor.UploadSingleVideo(progress, video, upProgress);
                        var thumb = await _instaApi.HelperProcessor.UploadSinglePhoto(progress, video.VideoThumbnail.ConvertToImageUpload(), upProgress, uploadId.Value.Item1);
                        videosDic.Add(uploadId.Value.Item1, video);

                        upProgress.UploadState = InstaUploadState.Uploaded;
                        progress?.Invoke(upProgress);
                        vidIndex++;
                    }
                }
                var config = await ConfigureAlbumAsync(progress, upProgress, imagesUploadIds, videosDic, caption, location);
                return config;
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }

        /// <summary>
        ///     Upload album (videos and photos)
        /// </summary>
        /// <param name="album">Array of photos or videos to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadAlbumAsync(InstaAlbumUpload[] album, string caption, InstaLocationShort location = null)
        {
            return await UploadAlbumAsync(null, album, caption, location);
        }

        /// <summary>
        ///     Upload album (videos and photos) with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="album">Array of photos or videos to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadAlbumAsync(Action<InstaUploaderProgress> progress, InstaAlbumUpload[] album, string caption, InstaLocationShort location = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                upProgress.Name = "Album upload";
                progress?.Invoke(upProgress);
                var uploadIds = new Dictionary<string, InstaAlbumUpload>();
                var index = 1;

                foreach (var al in album)
                {
                    if (al.IsImage)
                    {
                        var image = al.ImageToUpload;
                        if (image.UserTags?.Count > 0)
                        {
                            var currentDelay = _instaApi.GetRequestDelay();
                            _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                            foreach (var t in image.UserTags)
                            {
                                if (t.Pk <= 0)
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
                            }
                            _instaApi.SetRequestDelay(currentDelay);
                        }
                    }
                    else if (al.IsVideo)
                    {
                        var video = al.VideoToUpload;
                        if (video.UserTags?.Count > 0)
                        {
                            var currentDelay = _instaApi.GetRequestDelay();
                            _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                            foreach (var t in video.UserTags)
                            {
                                if (t.Pk <= 0)
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
                            }
                            _instaApi.SetRequestDelay(currentDelay);
                        }
                    }
                }
                foreach (var al in album)
                {
                    if (al.IsImage)
                    {
                        upProgress.Name = $"[Album] uploading {index}/{album.Length}";
                        upProgress.UploadState = InstaUploadState.Uploading;
                        progress?.Invoke(upProgress);
                        var image = await _instaApi.HelperProcessor.UploadSinglePhoto(progress, al.ImageToUpload, upProgress);
                        if (image.Succeeded)
                            uploadIds.Add(image.Value, al);
                    }
                    else if (al.IsVideo)
                    {
                        upProgress.Name = $"[Album] uploading {index}/{album.Length}";
                        upProgress.UploadState = InstaUploadState.Uploading;
                        progress?.Invoke(upProgress);
                        var video = await _instaApi.HelperProcessor.UploadSingleVideo(progress, al.VideoToUpload, upProgress);
                        if (video.Succeeded)
                        {
                            var image = await _instaApi.HelperProcessor.UploadSinglePhoto(progress, al.VideoToUpload.VideoThumbnail.ConvertToImageUpload(), upProgress, video.Value.Item1);
                            uploadIds.Add(video.Value.Item1, al);
                        }
                    }
                    index++;
                }
                var config = await ConfigureAlbumAsync(progress, upProgress, uploadIds, caption, location);
                return config;
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }

        private async Task<IResult<InstaMedia>> ConfigureAlbumAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, Dictionary<string, InstaAlbumUpload> album, string caption, InstaLocationShort location)
        {
            try
            {
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                upProgress.Name = "Album upload";
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaAlbumConfigureUri();
                var clientSidecarId = ApiRequestMessage.GenerateUnknownUploadId();
                var childrenArray = new JArray();

                foreach (var al in album)
                {
                    if (al.Value.IsImage)
                        childrenArray.Add(_instaApi.HelperProcessor.GetImageConfigure(al.Key, al.Value.ImageToUpload));
                    else if (al.Value.IsVideo)
                        childrenArray.Add(_instaApi.HelperProcessor.GetVideoConfigure(al.Key, al.Value.VideoToUpload));
                }

                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"caption", caption},
                    {"client_sidecar_id", clientSidecarId},
                    {"upload_id", clientSidecarId},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"source_type", "4"},
                    {"device_id", _deviceInfo.DeviceId},
                    {"creation_logger_session_id", Guid.NewGuid().ToString()},
                    {
                        "device", new JObject
                        {
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)}
                        }
                    },
                    {"children_metadata", childrenArray},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (location != null)
                {
                    data.Add("location", location.GetJson());
                    data.Add("date_time_digitalized", DateTime.Now.ToString("yyyy:dd:MM+h:mm:ss"));
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }
                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaAlbumResponse>(json);
                var converter = ConvertersFabric.Instance.GetSingleMediaFromAlbumConverter(mediaResponse);
                var obj = converter.Convert();
                if (obj.Caption == null && !string.IsNullOrEmpty(caption))
                {
                    var editedMedia = await _instaApi.MediaProcessor.EditMediaAsync(obj.InstaIdentifier, caption, location);
                    if (editedMedia.Succeeded)
                    {
                        upProgress.UploadState = InstaUploadState.Configured;
                        progress?.Invoke(upProgress);
                        upProgress.UploadState = InstaUploadState.Completed;
                        progress?.Invoke(upProgress);
                        return Result.Success(editedMedia.Value);
                    }
                }
                upProgress.UploadState = InstaUploadState.Configured;
                progress?.Invoke(upProgress);
                upProgress.UploadState = InstaUploadState.Completed;
                progress?.Invoke(upProgress);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }




        /// <summary>
        ///     Upload photo [Supports user tags]
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadPhotoAsync(InstaImageUpload image, string caption, InstaLocationShort location = null)
        {
            return await UploadPhotoAsync(null, image, caption, location);
        }

        /// <summary>
        ///     Upload photo with progress [Supports user tags]
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadPhotoAsync(Action<InstaUploaderProgress> progress, InstaImageUpload image, string caption,
            InstaLocationShort location = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendMediaPhotoAsync(progress, image, caption, location);
        }

        /// <summary>
        ///     Upload video [Supports user tags]
        /// </summary>
        /// <param name="video">Video and thumbnail to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadVideoAsync(InstaVideoUpload video, string caption, InstaLocationShort location = null)
        {
            return await UploadVideoAsync(null, video, caption, location);
        }
        /// <summary>
        ///     Upload video with progress [Supports user tags]
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video and thumbnail to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, string caption, InstaLocationShort location = null)
        {
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                if (video?.UserTags?.Count > 0)
                {
                    var currentDelay = _instaApi.GetRequestDelay();
                    _instaApi.SetRequestDelay(RequestDelay.FromSeconds(1, 2));
                    foreach (var t in video.UserTags)
                    {
                        if (t.Pk <= 0)
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
                    }
                    _instaApi.SetRequestDelay(currentDelay);
                }
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var uploadVideo = await _instaApi.HelperProcessor.UploadSingleVideo(progress, 
                    video, upProgress, false, true);

                if (!uploadVideo.Succeeded)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.Fail<InstaMedia>(uploadVideo.Info.Message);
                }
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);

                upProgress.UploadState = InstaUploadState.UploadingThumbnail;
                progress?.Invoke(upProgress);

                var uploadPhoto = await _instaApi.HelperProcessor.UploadSinglePhoto(progress,
                    video.VideoThumbnail.ConvertToImageUpload(),
                    upProgress, 
                    uploadVideo.Value.Item1, 
                    false, null, null,
                    uploadVideo.Value.Item2);

                if (uploadPhoto.Succeeded)
                {
                    //upProgress = progressContent?.UploaderProgress;
                    upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                    progress?.Invoke(upProgress);
                    return await ConfigureVideoAsync(progress, upProgress, video, uploadVideo.Value.Item1, caption, location);
                }
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.Fail<InstaMedia>(uploadPhoto.Value);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }
        /// <summary>
        ///     Upload segmented video to timeline 
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="caption">Caption</param>
        /// <param name="location">Location => Optional (get it from <seealso cref="LocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaMedia>> UploadSegmentedVideoAsync(InstaSegmentedVideoUpload video, string caption, InstaLocationShort location = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendSegmentedVideoAsync(video, caption, location);
        }
        private async Task<IResult<InstaMedia>> ConfigureAlbumAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, Dictionary<string, InstaImageUpload> imagesUploadIds, Dictionary<string, InstaVideoUpload> videos, string caption, InstaLocationShort location)
        {
            try
            {
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                upProgress.Name = "Album upload";
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaAlbumConfigureUri();
                var clientSidecarId = ApiRequestMessage.GenerateUnknownUploadId();
                var childrenArray = new JArray();
                if (imagesUploadIds != null && imagesUploadIds.Any())
                {
                    foreach (var img in imagesUploadIds)
                    {
                        childrenArray.Add(_instaApi.HelperProcessor.GetImageConfigure(img.Key, img.Value));
                    }
                }
                if (videos != null && videos.Any())
                {
                    foreach (var id in videos)
                    {
                        childrenArray.Add(_instaApi.HelperProcessor.GetVideoConfigure(id.Key, id.Value));
                    }
                }
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"caption", caption},
                    {"client_sidecar_id", clientSidecarId},
                    {"device_id", _deviceInfo.DeviceId},
                    {"upload_id", clientSidecarId},
                    {
                        "device", new JObject
                        {
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)}
                        }
                    },
                    {"children_metadata", childrenArray},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (location != null)
                {
                    data.Add("location", location.GetJson());
                    data.Add("date_time_digitalized", DateTime.Now.ToString("yyyy:dd:MM+h:mm:ss"));
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }
                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaAlbumResponse>(json);
                var converter = ConvertersFabric.Instance.GetSingleMediaFromAlbumConverter(mediaResponse);
                var obj = converter.Convert();
                if (obj.Caption == null && !string.IsNullOrEmpty(caption))
                {
                    var editedMedia = await _instaApi.MediaProcessor.EditMediaAsync(obj.InstaIdentifier, caption, location);
                    if (editedMedia.Succeeded)
                    {
                        upProgress.UploadState = InstaUploadState.Configured;
                        progress?.Invoke(upProgress);
                        upProgress.UploadState = InstaUploadState.Completed;
                        progress?.Invoke(upProgress);
                        return Result.Success(editedMedia.Value);
                    }
                }
                upProgress.UploadState = InstaUploadState.Configured;
                progress?.Invoke(upProgress);
                upProgress.UploadState = InstaUploadState.Completed;
                progress?.Invoke(upProgress);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }

        private async Task<IResult<InstaMedia>> ConfigureVideoAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, InstaVideoUpload video, string uploadId, string caption, InstaLocationShort location)
        {
            try
            {
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaConfigureUri(true);
                var data = new JObject
                {
                    {"caption", caption ?? string.Empty},
                    {"upload_id", uploadId},
                    {"source_type", "4"},
                    {"device_id", _deviceInfo.DeviceId},
                    {"camera_position", "unknown"},
                    {"creation_logger_session_id", Guid.NewGuid().ToString()},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                    {
                        "extra", new JObject
                        {
                            {"source_width", 0},
                            {"source_height", 0}
                        }
                    },
                    {
                        "clips", new JArray{
                            new JObject
                            {
                                {"length", 0},
                                {"creation_date", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fff")},
                                {"source_type", "3"},
                                {"camera_position", "back"}
                            }
                        }
                    },
                    {"poster_frame_index", 0},
                    {"audio_muted", false},
                    {"filter_type", "0"},
                    {"video_result", ""},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.UserName}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (location != null)
                {
                    data.Add("location", location.GetJson());
                    data.Add("date_time_digitalized", DateTime.Now.ToString("yyyy:dd:MM+h:mm:ss"));
                }
                if (video.UserTags?.Count > 0)
                {
                    var tagArr = new JArray();
                    foreach (var tag in video.UserTags)
                    {
                        if (tag.Pk != -1)
                        {
                            var position = new JArray(0.0, 0.0);
                            var singleTag = new JObject
                            {
                                {"user_id", tag.Pk},
                                {"position", position}
                            };
                            tagArr.Add(singleTag);
                        }
                    }

                    var root = new JObject
                    {
                        {"in",  tagArr}
                    };
                    data.Add("usertags", root.ToString(Formatting.None));
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, UriCreator.GetMediaUploadFinishUri(), _deviceInfo, data);
                
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }
                upProgress.UploadState = InstaUploadState.Configured;
                progress?.Invoke(upProgress);

                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaItemResponse>(json,
                                    new InstaMediaDataConverter());
                var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);
                var obj = converter.Convert();
                if (obj.Caption == null && !string.IsNullOrEmpty(caption))
                {
                    var editedMedia = await _instaApi.MediaProcessor.EditMediaAsync(obj.InstaIdentifier, caption, location);
                    if (editedMedia.Succeeded)
                        return Result.Success(editedMedia.Value);
                }
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMedia), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<InstaMedia>(exception);
            }
        }
        private async Task<IResult<bool>> LikeUnlikeMediaInternal(Uri instaUri, string mediaId,
            InstaMediaContainerModuleType containerModule = InstaMediaContainerModuleType.FeedTimeline,
            uint feedPosition = 0, 
            InstaMediaInventorySource inventorySource = InstaMediaInventorySource.None, 
            bool? isCarouselBumpedPost = false,
            int? carouselIndex = null, 
            string exploreSourceToken = null,
            string parentMediaPK = null,
            string chainingSessionId = null,
            string navChain = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var data = new Dictionary<string, string>
                {
                    {"media_id", mediaId},
                    {"delivery_class", "organic"},
                    {"radio_type", "wifi-none"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"feed_position", feedPosition.ToString()},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var containerType = containerModule.GetContainerType();
                var inventorySourceString = inventorySource.GetInvetorySourceType();
                if (!string.IsNullOrEmpty(inventorySourceString))
                    data.Add("inventory_source", inventorySourceString);
                if (!string.IsNullOrEmpty(containerType))
                    data.Add("container_module", containerType);
                if (isCarouselBumpedPost != null)
                    data.Add("is_carousel_bumped_post", isCarouselBumpedPost.Value.ToString().ToLower());
                if (carouselIndex != null)
                    data.Add("carousel_index", carouselIndex.Value.ToString());
                if (!string.IsNullOrEmpty(exploreSourceToken))
                    data.Add("explore_source_token", exploreSourceToken);
                if (!string.IsNullOrEmpty(parentMediaPK))
                    data.Add("parent_m_pk", parentMediaPK);
                if (!string.IsNullOrEmpty(chainingSessionId))
                    data.Add("chaining_session_id", chainingSessionId);
                if (navChain.IsNotEmpty())
                    data.Add("nav_chain", navChain);
                var likeD = instaUri.ToString().IndexOf("/like/") != -1 ? "1" : null;

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post,
                    instaUri, 
                    _deviceInfo,
                    data,
                    true, 
                    likeD);

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
                return Result.Fail<bool>(exception);
            }
        }

        private async Task<IResult<bool>> ArchiveUnArchiveMediaInternal(string mediaId, Uri instaUri)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var data = new Dictionary<string, string>
                {
                    {"media_id", mediaId},
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
                return Result.Fail<bool>(exception);
            }
        }
        private async Task<IResult<bool>> LikeUnlikeArchiveUnArchiveMediaInternal(string mediaId, Uri instaUri)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var fields = new Dictionary<string, string>
                {
                    {"media_id", mediaId},
                    {"_csrftoken", _user.CsrfToken},
                    {"radio_type", "wifi-none"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceId}
                };
                var request =
                    _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
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
                return Result.Fail<bool>(exception);
            }
        }

        private async Task<IResult<bool>> UploadVideoThumbnailAsync(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, InstaImage image, string uploadId)
        {
            try
            {
                var instaUri = UriCreator.GetUploadPhotoUri();
                upProgress.UploadState = InstaUploadState.UploadingThumbnail;
                progress?.Invoke(upProgress);
                var requestContent = new MultipartFormDataContent(uploadId)
                {
                    {new StringContent(uploadId), "\"upload_id\""},
                    {new StringContent(_deviceInfo.DeviceGuid.ToString()), "\"_uuid\""},
                    {new StringContent(_user.CsrfToken), "\"_csrftoken\""},
                    {
                        new StringContent("{\"lib_name\":\"jt\",\"lib_version\":\"1.3.0\",\"quality\":\"87\"}"),
                        "\"image_compression\""
                    }
                };
                byte[] fileBytes;
                if (image.ImageBytes == null)
                    fileBytes = File.ReadAllBytes(image.Uri);
                else
                    fileBytes = image.ImageBytes;

                var imageContent = new ByteArrayContent(fileBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                requestContent.Add(imageContent, "photo", $"pending_media_{uploadId}.jpg");
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                request.Content = requestContent;
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var imgResp = JsonConvert.DeserializeObject<ImageThumbnailResponse>(json);
                if (imgResp.IsSucceed)
                {
                    upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                    progress?.Invoke(upProgress);
                    return Result.Success(true);
                }

                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.Fail<bool>("Could not upload thumbnail");
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

        private async Task<IResult<InstaMediaListResponse>> GetArchivedMedia(string nextMaxId)
        {
            try
            {
                var instaUri = UriCreator.GetArchivedMediaFeedsListUri(nextMaxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMediaListResponse>(response, json);
                var archivedResponse = JsonConvert.DeserializeObject<InstaMediaListResponse>(json);
                return Result.Success(archivedResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail<InstaMediaListResponse>(ex);
            }
        }

    }
}
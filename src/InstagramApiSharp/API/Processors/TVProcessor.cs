/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
 
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using InstagramApiSharp.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Instagram TV api functions.
    /// </summary>
    internal class TVProcessor : ITVProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public TVProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Add live broadcast to IGTV [ Save live as IGTV ]
        /// </summary>
        /// <param name="broadcastId">Broadcast identifier</param>
        /// <param name="cover">Image cover for IGTV [MOST BE IN VERTICAL]</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="sharePreviewToFeed">Show a preview on the feed</param>
        /// <param name="igtvSeriesId">Igtv series indentifier => Optional => adds this video to a specific TV series.</param>
        public async Task<IResult<bool>> AddLiveBroadcastToTVAsync(string broadcastId, InstaImage cover, string title,
            string description, bool sharePreviewToFeed = false, string igtvSeriesId = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetMediaConfigureToIGTVUri(false);
                var retryContext = HelperProcessor.GetRetryContext();
                await _instaApi.LiveProcessor.GetPostLiveThumbnailsAsync(broadcastId);

                var uploadId = await _instaApi.HelperProcessor.UploadSinglePhoto(null,
                    new InstaImageUpload { ImageBytes = cover.ImageBytes, Uri = cover.Uri }, new InstaUploaderProgress(),
                    ExtensionHelper.GetThreadToken(), false, null, broadcastId);
                if (!uploadId.Succeeded)
                    return Result.Fail<bool>(uploadId.Info.Message);

                var data = new JObject
                {
                    {"igtv_ads_toggled_on", "0"},
                    {"timezone_offset", InstaApiConstants.TIMEZONE_OFFSET.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"source_type", "4"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"keep_shoppable_products", "0"},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"title", title ?? string.Empty },
                    {"caption", description ?? string.Empty},
                    {"igtv_share_preview_to_feed", Convert.ToInt16(sharePreviewToFeed).ToString()},
                    {"upload_id", uploadId.Value},
                    {"igtv_composer_session_id", Guid.NewGuid().ToString()},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber}
                        }
                    },
                    {
                        "extra", new JObject
                        {
                            {"source_width",  0},
                            {"source_height", 0}
                        }
                    },
                };
                if (!string.IsNullOrEmpty(igtvSeriesId))
                    data.Add("igtv_series_id", igtvSeriesId);
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.Success(true);
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
                return Result.Fail(exception, default(bool));
            }
        }
        /// <summary>
        ///     Get creating tools availability for IG TV
        /// </summary>
        public async Task<IResult<bool>> GetTVCreationToolsAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetIgTvCreationToolsUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.Success(true);
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
                return Result.Fail(exception, default(bool));
            }
        }
        /// <summary>
        ///     Get TV series of specific user
        /// </summary>
        /// <param name="userId">User id (pk) => channel owner</param>
        public async Task<IResult<InstaTV>> GetUserTVSeriesAsync(long userId) => await GetTV(UriCreator.GetUserTVSeriesUri(userId, _instaApi.HttpHelper));

        /// <summary>
        ///     Remove episode from a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        /// <param name="mediaPk">Media pk</param>
        public async Task<IResult<bool>> RemoveEpisodeFromTVSeriesAsync(string seriesId, string mediaPk) => await EpisodeTVSeriesAsync(UriCreator.GetRemoveEpisodeFromTvSeriesUri(seriesId), mediaPk);

        /// <summary>
        ///     Add episode to a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        /// <param name="mediaPk">Media pk</param>
        public async Task<IResult<bool>> AddEpisodeToTVSeriesAsync(string seriesId, string mediaPk) => await EpisodeTVSeriesAsync(UriCreator.GetAddEpisodeToTvSeriesUri(seriesId), mediaPk);

        /// <summary>
        ///     Update a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        public async Task<IResult<bool>> UpdateTVSeriesAsync(string seriesId, string title, string description)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetUpdateTvSeriesUri(seriesId);
                var fields = new Dictionary<string, string>
                {
                    {"description", description ?? string.Empty},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"title", title ?? string.Empty},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.Success(true);
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
                return Result.Fail(exception, default(bool));
            }
        }

        /// <summary>
        ///     Delete a TV series
        /// </summary>
        /// <param name="seriesId">TV series identifier</param>
        public async Task<IResult<bool>> DeleteTVSeriesAsync(string seriesId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetDeleteTvSeriesUri(seriesId);
                var fields = new Dictionary<string, string>
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.Success(true);
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
                return Result.Fail(exception, default(bool));
            }
        }

        /// <summary>
        ///     Create a TV series
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        public async Task<IResult<InstaTVCreateSeries>> CreateTVSeriesAsync(string title, string description)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetCreateTvSeriesUri();
                var fields = new Dictionary<string, string>
                {
                    {"description", description ?? string.Empty},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"title", title ?? string.Empty},
                    {"igtv_composer_session_id", Guid.NewGuid().ToString()},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTVCreateSeries>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaTVCreateSeries>(json);
                if (obj.IsSucceed)
                    return Result.Success(obj);
                else
                    return Result.UnExpectedResponse<InstaTVCreateSeries>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTVCreateSeries), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaTVCreateSeries));
            }
        }

        /// <summary>
        ///     Edit TV Media
        /// </summary>
        /// <param name="mediaId">TV Identifier</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        public async Task<IResult<InstaMedia>> EditMediaAsync(string mediaId, string title, string description)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetEditMediaUri(mediaId);

                var data = new JObject
                {
                    {"igtv_ads_toggled_on", "0"},
                    {"caption_text", description ?? string.Empty},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"title", title ?? string.Empty},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var mediaResponse = JsonConvert.DeserializeObject<InstaMediaItemResponse>(json,
                        new Converters.Json.InstaMediaDataConverter());
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


        /// <summary>
        ///     Browse Feed
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaTVBrowseFeed>> BrowseFeedAsync(PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var tvFeed = new InstaTVBrowseFeed();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaTVBrowseFeed Convert(InstaTVBrowseFeedResponse instaTVBrowseFeedResponse)
                {
                    return ConvertersFabric.Instance.GetTVBrowseFeedConverter(instaTVBrowseFeedResponse).Convert();
                }
                var browseFeed = await BrowseFeed(paginationParameters.NextMaxId);
                if (!browseFeed.Succeeded)
                    return Result.Fail(browseFeed.Info, tvFeed);

                var feedResponse = browseFeed.Value;

                tvFeed = Convert(feedResponse);
                paginationParameters.NextMaxId = tvFeed.MaxId;
                paginationParameters.PagesLoaded++;

                while (tvFeed.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    var nextFeed = await BrowseFeed(paginationParameters.NextMaxId);
                    if (!nextFeed.Succeeded)
                        return Result.Fail(nextFeed.Info, tvFeed);

                    var convertedFeed = Convert(nextFeed.Value);
                    tvFeed.BrowseItems.AddRange(convertedFeed.BrowseItems);
                    tvFeed.MoreAvailable = nextFeed.Value.MoreAvailable;
                    paginationParameters.NextMaxId = nextFeed.Value.MaxId;
                    if (convertedFeed.MyChannel != null)
                        tvFeed.MyChannel = convertedFeed.MyChannel;
                    tvFeed.BannerToken = convertedFeed.BannerToken;
                    paginationParameters.PagesLoaded++;
                }

                return Result.Success(tvFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, tvFeed, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, tvFeed);
            }
        }

        /// <summary>
        ///     Get channel by user id (pk) => channel owner
        /// </summary>
        /// <param name="userId">User id (pk) => channel owner</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaTVChannel>> GetChannelByIdAsync(long userId, PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await GetChannel(null, userId, paginationParameters);
        }

        /// <summary>
        ///     Get channel by <seealso cref="InstaTVChannelType"/>
        /// </summary>
        /// <param name="channelType">Channel type</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaTVChannel>> GetChannelByTypeAsync(InstaTVChannelType channelType, PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await GetChannel(channelType, null, paginationParameters);
        }

        /// <summary>
        ///     Get suggested searches
        /// </summary>
        public async Task<IResult<InstaTVSearch>> GetSuggestedSearchesAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetIGTVSuggestedSearchesUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTVSearch>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaTVSearchResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetTVSearchConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTVSearch), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTVSearch>(exception);
            }
        }

        /// <summary>
        ///     Get TV Guide (gets popular and suggested channels)
        /// </summary>
        public async Task<IResult<InstaTV>> GetTVGuideAsync() => await GetTV(UriCreator.GetIGTVGuideUri());
        /// <summary>
        ///     Search channels
        /// </summary>
        /// <param name="query">Channel or username</param>
        public async Task<IResult<InstaTVSearch>> SearchAsync(string query)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetIGTVSearchUri(query);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTVSearch>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaTVSearchResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetTVSearchConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTVSearch), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTVSearch>(exception);
            }
        }

        /// <summary>
        ///     Upload video to Instagram TV
        /// </summary>
        /// <param name="video">Video to upload (aspect ratio is very important for thumbnail and video | range 0.5 - 1.0 | Width = 480, Height = 852)</param>
        /// <param name="title">Title</param>
        /// <param name="caption">Caption</param>
        /// <param name="videoUploadOption">Video options</param>
        public async Task<IResult<InstaMedia>> UploadVideoAsync(InstaVideoUpload video, string title, string caption/*, InstaVideoUploadOption videoUploadOption = null*/)
        {
            return await UploadVideoAsync(null, video, title, caption/*, videoUploadOption*/);
        }

        /// <summary>
        ///     Upload video to Instagram TV with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload (aspect ratio is very important for thumbnail and video | range 0.5 - 1.0 | Width = 480, Height = 852)</param>
        /// <param name="title">Title</param>
        /// <param name="caption">Caption</param>
        /// <param name="videoUploadOption">Video options</param>
        public async Task<IResult<InstaMedia>> UploadVideoAsync(Action<InstaUploaderProgress> progress, InstaVideoUpload video, string title, string caption/*, InstaVideoUploadOption videoUploadOption = null*/)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendIGTVVideoAsync(progress, video, title, caption/*, videoUploadOption*/);
        }

        /// <summary>
        ///     Upload segmented video to igtv 
        /// </summary>
        /// <param name="tvVideo">IgTV Video to upload</param>
        /// <param name="title">Title</param>
        /// <param name="caption">Caption</param>
        public async Task<IResult<InstaMedia>> UploadSegmentedVideoToTVAsync(InstaTVVideoUpload tvVideo, string title, string caption)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            return await _instaApi.HelperProcessor.SendTVVideoAsync(tvVideo, title, caption);
        }

        /// <summary>
        ///     Mark a media or medias as seen
        /// </summary>
        /// <param name="mediaPkImpression">Media Pk impression (<see cref="InstaMedia.Pk"/>)</param>
        /// <param name="progress">Progress time</param>
        /// <param name="mediaPKsGridImpressions">Media PKs grid impressions</param>
        public async Task<IResult<bool>> MarkAsSeenAsync(string mediaPkImpression, int progress = 0, string[] mediaPKsGridImpressions = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetSeenTVUri();
                var root = new JObject();
                var impression = new JObject();
                var gridImpressions = new JArray();

                if (!string.IsNullOrEmpty(mediaPkImpression))
                {
                    impression.Add(mediaPkImpression, new JObject
                    {
                        {"view_progress_s", progress }
                    });
                }
                if (mediaPKsGridImpressions?.Length > 0)
                    gridImpressions = new JArray(mediaPKsGridImpressions);

                root.Add("impressions", impression);
                root.Add("grid_impressions", gridImpressions);

                var data = new JObject
                {
                    {"seen_state", root.ToString(Formatting.None)},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                return obj.IsSucceed ? Result.Success(true) : Result.Fail<bool>(obj.Message);
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
        private async Task<IResult<InstaTVChannel>> GetChannel(InstaTVChannelType? channelType, long? userId, PaginationParameters paginationParameters)
        {
            try
            {
                var instaUri = UriCreator.GetIGTVChannelUri();
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken}
                };
                if (channelType != null)
                    data.Add("id", channelType.Value.GetRealChannelType());
                else
                    data.Add("id", $"user_{userId}");
                if (paginationParameters != null && !string.IsNullOrEmpty(paginationParameters.NextMaxId))
                    data.Add("max_id", paginationParameters.NextMaxId);
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                var rnd = new Random();
                request.Headers.AddHeader("X-Ads-Opt-Out", "0", _instaApi);
                request.Headers.AddHeader("X-Google-AD-ID", _deviceInfo.GoogleAdId.ToString(), _instaApi);
                request.Headers.AddHeader("X-DEVICE-ID", _deviceInfo.DeviceGuid.ToString(), _instaApi);
                request.Headers.AddHeader("X-CM-Bandwidth-KBPS", $"{rnd.Next(678, 987)}.{rnd.Next(321, 876)}", _instaApi);
                request.Headers.AddHeader("X-CM-Latency", $"{rnd.Next(100, 250)}.{rnd.Next(321, 876)}", _instaApi);

                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTVChannel>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaTVChannelResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetTVChannelConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTVChannel), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTVChannel>(exception);
            }
        }

        private async Task<IResult<InstaTVBrowseFeedResponse>> BrowseFeed(string maxId)
        {
            try
            {
                var instaUri = UriCreator.GetTVBrowseFeedUri(maxId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTVBrowseFeedResponse>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaTVBrowseFeedResponse>(json);

                return obj.IsSucceed ? Result.Success(obj) : Result.Fail<InstaTVBrowseFeedResponse>(obj.Message);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTVBrowseFeedResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTVBrowseFeedResponse>(exception);
            }
        }
        private async Task<IResult<bool>> EpisodeTVSeriesAsync(Uri instaUri, string mediaPk) 
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var fields = new Dictionary<string, string>
                {
                    {"media_id", mediaPk},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.Success(true);
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
                return Result.Fail(exception, default(bool));
            }
        }

        private async Task<IResult<InstaTV>> GetTV(Uri instaUri)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTV>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaTVResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetTVConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTV), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTV>(exception);
            }
        }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using InstagramApiSharp.Helpers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Classes.Models;
using System.Net;
using InstagramApiSharp.Converters.Json;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Classes.ResponseWrappers.Business;
using System.Linq;
using System.Threading;

#pragma warning disable IDE0052 // Remove unread private members
namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Represents Instagram Music APIs
    /// </summary>
    internal class MusicProcessor : IMusicProcessor
    {
        #region Properties and constructor
        
        private string BrowseSessionId, SearchSessionId;
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        private readonly InstaApi _instaApi;
        private readonly HttpHelper _httpHelper;
        public MusicProcessor(AndroidDevice deviceInfo, UserSessionData user,
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

        #endregion Properties and constructor

        /// <summary>
        ///     Get specific playlist
        /// </summary>
        /// <param name="playlistId">Playlist identifier</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaTrendingMusic>> GetPlaylistAsync(string playlistId,
            PaginationParameters paginationParameters) 
            => await GetPlaylistAsync(playlistId,
                paginationParameters,
                CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get specific playlist
        /// </summary>
        /// <param name="playlistId">Playlist identifier</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaTrendingMusic>> GetPlaylistAsync(string playlistId,
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
            => await GetMusicAsync(UriCreator.GetMusicPlaylistUri(playlistId),
                paginationParameters,
                cancellationToken).ConfigureAwait(false);

        /// <summary>
        ///     Browse musics
        /// </summary>
        /// <param name="cursor">Cursor => 0 means don't add it, if you want to paginate it, you should set to 30 or 60 or 90 or 120 and etc.</param>
        public async Task<IResult<InstaBrowseMusic>> BrowseMusicAsync(int cursor = 0)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (string.IsNullOrEmpty(BrowseSessionId))
                    BrowseSessionId = Guid.NewGuid().ToString();
                var instaUri = UriCreator.GetBrowseMusicUri();
                var data = new Dictionary<string, string>()
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"product", "story_camera_music_overlay_post_capture"},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"browse_session_id", BrowseSessionId},
                };
                if (cursor != 0)
                    data.Add("cursor", cursor.ToString());
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaBrowseMusic>(response, json);
                var respObj = JsonConvert.DeserializeObject<InstaBrowseMusicResponse>(json);
                return Result.Success(ConvertersFabric.Instance.GetBrowseMusicConverter(respObj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaBrowseMusic), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaBrowseMusic));
            }
        }

        /// <summary>
        ///     Music keyword search
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <param name="count">Count of results</param>
        public async Task<IResult<List<string>>> SearchKeywordAsync(string query, uint count = 3)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                if (string.IsNullOrEmpty(SearchSessionId))
                    SearchSessionId = Guid.NewGuid().ToString();

                if (string.IsNullOrEmpty(BrowseSessionId))
                    BrowseSessionId = Guid.NewGuid().ToString();

                var instaUri = UriCreator.GetMusicKeywordSearchUri(query, count, SearchSessionId, "story_camera_music_overlay_post_capture", BrowseSessionId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<List<string>>(response, json);
                var respObj = JsonConvert.DeserializeObject<InstaMusicKeywordSearchResponse>(json);
                return respObj.IsSucceed ? Result.Success(respObj.Keywords) : Result.UnExpectedResponse<List<string>>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(List<string>), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(List<string>));
            }
        }

        /// <summary>
        ///     Get trending musics
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaTrendingMusic>> GetTrendingMusicAsync(PaginationParameters paginationParameters)
            => await GetTrendingMusicAsync(paginationParameters,
                CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get trending musics
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaTrendingMusic>> GetTrendingMusicAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
            => await GetMusicAsync(UriCreator.GetTrendingMusicUri(),
                paginationParameters,
                cancellationToken).ConfigureAwait(false);

        private async Task<IResult<InstaTrendingMusic>> GetMusicAsync(Uri instaUri, 
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken,
            string query = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaTrendingMusicResponse musicResponse = null;
            try
            {
                if (paginationParameters == null)
                {
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);
                    BrowseSessionId = Guid.NewGuid().ToString();
                    SearchSessionId = Guid.NewGuid().ToString();
                }
                if (string.IsNullOrEmpty(BrowseSessionId))
                    BrowseSessionId = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(SearchSessionId))
                    SearchSessionId = Guid.NewGuid().ToString();


                var musicResult = await GetMusic(instaUri, paginationParameters, BrowseSessionId, query, SearchSessionId);

                if (!musicResult.Succeeded)
                {
                    if (musicResult.Value != null)
                        return Result.Fail(musicResult.Info, Convert(musicResult.Value));
                    else
                        return Result.Fail(musicResult.Info, default(InstaTrendingMusic));
                }
                musicResponse = musicResult.Value;

                paginationParameters.NextMaxId = musicResponse.PageInfo?.NextMaxId;
                paginationParameters.PagesLoaded++;

                var moreAvailable = musicResponse.PageInfo != null && (musicResponse.PageInfo.MoreAvailable ?? false);
                while (moreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextMusic = await GetMusic(instaUri, paginationParameters, BrowseSessionId, query, SearchSessionId);
                    if (!nextMusic.Succeeded)
                        return Result.Fail(nextMusic.Info, GetOrDefault());
                    musicResponse.AlacornSessionId = nextMusic.Value.AlacornSessionId;
                    musicResponse.MusicReels = nextMusic.Value.MusicReels;
                    musicResponse.PageInfo = nextMusic.Value.PageInfo;
                    musicResponse.DarkBannerMessage = nextMusic.Value.Items;
                    if (nextMusic.Value.Items?.Count > 0)
                        musicResponse.Items.AddRange(nextMusic.Value.Items);
                    moreAvailable = musicResponse.PageInfo != null && (musicResponse.PageInfo.MoreAvailable ?? false);
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

            InstaTrendingMusic GetOrDefault() => musicResponse != null ? Convert(musicResponse) : default(InstaTrendingMusic);

            InstaTrendingMusic Convert(InstaTrendingMusicResponse trendingMusicResponse)
            {
                return ConvertersFabric.Instance.GetMusicTrendingConverter(trendingMusicResponse).Convert();
            }
        }

        private async Task<IResult<InstaTrendingMusicResponse>> GetMusic(Uri instaUri,
            PaginationParameters paginationParameters,
            string browseSessionId,
            string query = null,
            string searchSessionId = null,
            string product = "story_camera_music_overlay_post_capture")
        {
            try
            {
                var data = new Dictionary<string, string>()
                {
                    {"product", product},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"browse_session_id", browseSessionId},
                };
                if (!_httpHelper.NewerThan180 && _user.CsrfToken.IsNotEmpty())
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    data.Add("from_typeahead", "false");
                    data.Add("q", query);
                    data.Add("search_session_id", searchSessionId);
                }
                if (!string.IsNullOrEmpty(paginationParameters?.NextMaxId))
                    data.Add("cursor", paginationParameters.NextMaxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTrendingMusicResponse>(response, json);
                var respObj = JsonConvert.DeserializeObject<InstaTrendingMusicResponse>(json);

                return Result.Success(respObj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTrendingMusicResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaTrendingMusicResponse));
            }
        }

    }
}
#pragma warning restore IDE0052 // Remove unread private members
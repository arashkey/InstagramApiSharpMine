﻿using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Location api functions.
    /// </summary>
    internal class LocationProcessor : ILocationProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public LocationProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Gets the stories of particular location.
        /// </summary>
        /// <param name="locationId">Location identifier (location pk, external id, facebook id)</param>
        /// <returns>
        ///     Location stories
        /// </returns>
        public async Task<IResult<InstaStory>> GetLocationStoriesAsync(long locationId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var uri = UriCreator.GetLocationFeedUri(locationId.ToString());
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, uri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaStory>(response, json);

                var feedResponse = JsonConvert.DeserializeObject<InstaLocationFeedResponse>(json);
                var feed = ConvertersFabric.Instance.GetLocationFeedConverter(feedResponse).Convert();

                return Result.Success(feed.Story);
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
        ///     Get location(place) information by external id or facebook places id
        ///     <para>Get external id from this function: <see cref="ILocationProcessor.SearchLocationAsync(double, double, string)"/></para>
        ///     <para>Get facebook places id from this function: <see cref="ILocationProcessor.SearchPlacesAsync(double, double, PaginationParameters)"/></para>
        /// </summary>
        /// <param name="externalIdOrFacebookPlacesId">
        ///     External id or facebook places id of an location/place
        ///     <para>Get external id from this function: <see cref="ILocationProcessor.SearchLocationAsync(double, double, string)"/></para>
        ///     <para>Get facebook places id from this function: <see cref="ILocationProcessor.SearchPlacesAsync(double, double, PaginationParameters)"/></para>
        /// </param>
        public async Task<IResult<InstaPlaceShort>> GetLocationInfoAsync(string externalIdOrFacebookPlacesId)
        {
            try
            {
                var instaUri = UriCreator.GetLocationInfoUri(externalIdOrFacebookPlacesId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaPlaceShort>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaPlaceResponse>(json);

                return Result.Success(ConvertersFabric.Instance.GetPlaceShortConverter(obj.Location).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaPlaceShort), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaPlaceShort>(exception);
            }
        }

        /// <summary>
        ///     Get recent location media feeds.
        ///     <para>Important note: Be careful of using this function, because it's an POST request</para>
        /// </summary>
        /// <param name="locationId">Location identifier (location pk, external id, facebook id)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaSectionMedia>> GetRecentLocationFeedsAsync(long locationId, PaginationParameters paginationParameters) =>
            await GetRecentLocationFeedsAsync(locationId, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get recent location media feeds.
        ///     <para>Important note: Be careful of using this function, because it's an POST request</para>
        /// </summary>
        /// <param name="locationId">Location identifier (location pk, external id, facebook id)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaSectionMedia>> GetRecentLocationFeedsAsync(long locationId, PaginationParameters paginationParameters,
            CancellationToken cancellationToken) =>
            await GetSectionAsync(locationId, paginationParameters, InstaSectionType.Recent, cancellationToken).ConfigureAwait(false);

        /// <summary>
        ///     Get top (ranked) location media feeds.
        ///     <para>Important note: Be careful of using this function, because it's an POST request</para>
        /// </summary>
        /// <param name="locationId">Location identifier (location pk, external id, facebook id)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaSectionMedia>> GetTopLocationFeedsAsync(long locationId, PaginationParameters paginationParameters) =>
            await GetTopLocationFeedsAsync(locationId, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get top (ranked) location media feeds.
        ///     <para>Important note: Be careful of using this function, because it's an POST request</para>
        /// </summary>
        /// <param name="locationId">Location identifier (location pk, external id, facebook id)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<IResult<InstaSectionMedia>> GetTopLocationFeedsAsync(long locationId, PaginationParameters paginationParameters,
            CancellationToken cancellationToken) =>
            await GetSectionAsync(locationId, paginationParameters, InstaSectionType.Ranked, cancellationToken).ConfigureAwait(false);

        /// <summary>
        ///     Searches for specific location by provided geo-data or search query.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="query">Search query</param>
        /// <returns>
        ///     List of locations (short format)
        /// </returns>
        public async Task<IResult<InstaLocationShortList>> SearchLocationAsync(double latitude, double longitude, string query)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var uri = UriCreator.GetLocationSearchUri();

                var data = new Dictionary<string, string>
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"latitude", latitude.ToString(CultureInfo.InvariantCulture)},
                    {"longitude", longitude.ToString(CultureInfo.InvariantCulture)},
                    {"rank_token", _user.RankToken}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }

                if (!string.IsNullOrEmpty(query))
                    data.Add("search_query", query);
                else
                    data.Add("timestamp", DateTimeHelper.GetUnixTimestampSeconds().ToString());
                if (!Uri.TryCreate(uri, data.AsQueryString(), out var newuri))
                    return Result.Fail<InstaLocationShortList>("Unable to create uri for location search");

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, newuri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaLocationShortList>(response, json);
                var locations = JsonConvert.DeserializeObject<InstaLocationSearchResponse>(json);
                var converter = ConvertersFabric.Instance.GetLocationsSearchConverter(locations);
                return Result.Success(converter.Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaLocationShortList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaLocationShortList>(exception);
            }
        }
        /// <summary>
        ///     Search user by location
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="desireUsername">Desire username</param>
        /// <param name="count">Maximum user count</param>
        public async Task<IResult<InstaUserSearchLocation>> SearchUserByLocationAsync(double latitude, double longitude, string desireUsername, int count = 50)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var uri = UriCreator.GetUserSearchByLocationUri();
                if (count <= 0)
                    count = 30;
                var fields = new Dictionary<string, string>
                {
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"lat", latitude.ToString(CultureInfo.InvariantCulture)},
                    {"lng", longitude.ToString(CultureInfo.InvariantCulture)},
                    {"count", count.ToString()},
                    {"query", desireUsername},
                    {"context", "blended"},
                    {"rank_token", _user.RankToken}
                };
                if (!Uri.TryCreate(uri, fields.AsQueryString(), out var newuri))
                    return Result.Fail<InstaUserSearchLocation>("Unable to create uri for user search by location");

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, newuri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaUserSearchLocation>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaUserSearchLocation>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(obj) : Result.UnExpectedResponse<InstaUserSearchLocation>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaUserSearchLocation), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaUserSearchLocation>(exception);
            }
        }

        /// <summary>
        ///     Search places in facebook
        ///     <para>Note: This works for non-facebook accounts too!</para>
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaPlaceList" />
        /// </returns>
        public async Task<IResult<InstaPlaceList>> SearchPlacesAsync(double latitude, double longitude, PaginationParameters paginationParameters) =>
            await SearchPlacesAsync(latitude, longitude, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Search places in facebook
        ///     <para>Note: This works for non-facebook accounts too!</para>
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaPlaceList" />
        /// </returns>
        public async Task<IResult<InstaPlaceList>> SearchPlacesAsync(double latitude, double longitude,
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken) =>
            await SearchPlacesAsync(latitude, longitude, null, paginationParameters, cancellationToken).ConfigureAwait(false);

        /// <summary>
        ///     Search places in facebook
        ///     <para>Note: This works for non-facebook accounts too!</para>
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="query">Query to search (city, country or ...)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaPlaceList" />
        /// </returns>
        public async Task<IResult<InstaPlaceList>> SearchPlacesAsync(double latitude, double longitude, string query, PaginationParameters paginationParameters) =>
            await SearchPlacesAsync(latitude, longitude, query, paginationParameters).ConfigureAwait(false);

        /// <summary>
        ///     Search places in facebook
        ///     <para>Note: This works for non-facebook accounts too!</para>
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="query">Query to search (city, country or ...)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaPlaceList" />
        /// </returns>
        public async Task<IResult<InstaPlaceList>> SearchPlacesAsync(double latitude, double longitude, string query,
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaPlaceListResponse placesResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var places = await SearchPlaces(latitude, longitude, query, paginationParameters).ConfigureAwait(false);
                if (!places.Succeeded)
                    return Result.Fail(places.Info, GetOrDefault());

                placesResponse = places.Value;
                paginationParameters.NextMaxId = placesResponse.RankToken;
                paginationParameters.ExcludeList = placesResponse.ExcludeList;
                var pagesLoaded = 1;
                while (placesResponse.HasMore != null
                      && placesResponse.HasMore.Value
                      && !string.IsNullOrEmpty(placesResponse.RankToken)
                      && pagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextPlaces = await SearchPlaces(latitude, longitude, query, paginationParameters).ConfigureAwait(false);

                    if (!nextPlaces.Succeeded)
                        return Result.Fail(nextPlaces.Info, GetOrDefault());

                    placesResponse.RankToken = paginationParameters.NextMaxId = nextPlaces.Value.RankToken;
                    placesResponse.HasMore = nextPlaces.Value.HasMore;
                    placesResponse.Items.AddRange(nextPlaces.Value.Items);
                    placesResponse.Status = nextPlaces.Value.Status;
                    paginationParameters.ExcludeList = nextPlaces.Value.ExcludeList;
                    pagesLoaded++;
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

            InstaPlaceList GetOrDefault() => placesResponse != null ? Convert(placesResponse) : default(InstaPlaceList);

            InstaPlaceList Convert(InstaPlaceListResponse placelistResponse)
            {
                return ConvertersFabric.Instance.GetPlaceListConverter(placelistResponse).Convert();
            }
        }
        /// <summary>
        ///     Search places in facebook
        ///     <para>Note: This works for non-facebook accounts too!</para>
        /// </summary>
        /// <param name="query">Query to search (city, country or ...)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaPlaceList" />
        /// </returns>
        public async Task<IResult<InstaPlaceList>> SearchPlacesAsync(string query, PaginationParameters paginationParameters) =>
            await SearchPlacesAsync(query, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Search places in facebook
        ///     <para>Note: This works for non-facebook accounts too!</para>
        /// </summary>
        /// <param name="query">Query to search (city, country or ...)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaPlaceList" />
        /// </returns>
        public async Task<IResult<InstaPlaceList>> SearchPlacesAsync(string query, PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaPlaceListResponse placesResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var places = await SearchPlaces(query, paginationParameters).ConfigureAwait(false);
                if (!places.Succeeded)
                    return Result.Fail(places.Info, GetOrDefault());

                placesResponse = places.Value;
                paginationParameters.NextMaxId = placesResponse.RankToken;
                paginationParameters.ExcludeList = placesResponse.ExcludeList;
                var pagesLoaded = 1;
                while (placesResponse.HasMore != null
                      && placesResponse.HasMore.Value
                      && !string.IsNullOrEmpty(placesResponse.RankToken)
                      && pagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextPlaces = await SearchPlaces(query, paginationParameters).ConfigureAwait(false);

                    if (!nextPlaces.Succeeded)
                        return Result.Fail(nextPlaces.Info, GetOrDefault());

                    placesResponse.RankToken = paginationParameters.NextMaxId = nextPlaces.Value.RankToken;
                    placesResponse.HasMore = nextPlaces.Value.HasMore;
                    placesResponse.Items.AddRange(nextPlaces.Value.Items);
                    placesResponse.Status = nextPlaces.Value.Status;
                    paginationParameters.ExcludeList = nextPlaces.Value.ExcludeList;
                    pagesLoaded++;
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

            InstaPlaceList GetOrDefault() => placesResponse != null ? Convert(placesResponse) : default(InstaPlaceList);

            InstaPlaceList Convert(InstaPlaceListResponse placelistResponse)
            {
                return ConvertersFabric.Instance.GetPlaceListConverter(placelistResponse).Convert();
            }
        }

        private async Task<IResult<InstaPlaceListResponse>> SearchPlaces(string query,
            PaginationParameters paginationParameters)
        {
            return await SearchPlaces(null, null, query, paginationParameters);
        }
        private async Task<IResult<InstaPlaceListResponse>> SearchPlaces(double? latitude,
            double? longitude,
            string query,
            PaginationParameters paginationParameters)
        {
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var instaUri = UriCreator.GetSearchPlacesUri(query, paginationParameters.NextMaxId, paginationParameters.ExcludeList, latitude, longitude, _instaApi.TimezoneOffset);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaPlaceListResponse>(json);

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaPlaceListResponse>(response, json);
                if (obj.Items?.Count > 0)
                {
                    foreach (var item in obj.Items)
                        obj.ExcludeList.Add(item.Location.Pk);
                }

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaPlaceListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaPlaceListResponse>(exception);
            }
        }

        private async Task<IResult<InstaSectionMedia>> GetSectionAsync(long locationId,
            PaginationParameters paginationParameters, InstaSectionType sectionType, CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaSectionMediaListResponse mediaResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var mediaResult = await GetSectionMedia(sectionType,
                    locationId,
                    paginationParameters.NextMaxId,
                    paginationParameters.NextPage,
                    paginationParameters.NextMediaIds).ConfigureAwait(false);

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

                    var moreMedias = await GetSectionMedia(sectionType,
                        locationId,
                        paginationParameters.NextMaxId,
                        mediaResponse.NextPage,
                        mediaResponse.NextMediaIds).ConfigureAwait(false);
                    if (!moreMedias.Succeeded)
                    {
                        if (mediaResponse.Sections?.Count > 0)
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

        private async Task<IResult<InstaSectionMediaListResponse>> GetSectionMedia(InstaSectionType sectionType,
            long locationId,
            string maxId = null,
            int? page = null,
            List<long> nextMediaIds = null)
        {
            try
            {
                var instaUri = UriCreator.GetLocationSectionUri(locationId.ToString());
                var data = new Dictionary<string, string>
                {
                    {"rank_token", _deviceInfo.DeviceGuid.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"session_id", Guid.NewGuid().ToString()},
                    {"tab", sectionType.ToString().ToLower()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }

                if (!string.IsNullOrEmpty(maxId))
                    data.Add("max_id", maxId);

                if (page != null && page > 0)
                    data.Add("page", page.ToString());

                if (nextMediaIds?.Count > 0)
                {
                    var mediaIds = $"[{string.Join(",", nextMediaIds)}]";
                    if (sectionType == InstaSectionType.Ranked)
                        data.Add("next_media_ids", mediaIds.EncodeUri());
                    else
                        data.Add("next_media_ids", mediaIds);
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
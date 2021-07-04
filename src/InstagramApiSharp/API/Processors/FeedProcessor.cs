using System;
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
using InstaRecentActivityConverter = InstagramApiSharp.Converters.Json.InstaRecentActivityConverter;
using System.Diagnostics;
using System.Collections.Generic;
using InstagramApiSharp.Enums;
using System.Threading;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Feed api functions.
    /// </summary>
    internal class FeedProcessor : IFeedProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public FeedProcessor(AndroidDevice deviceInfo, UserSessionData user, IHttpRequestProcessor httpRequestProcessor,
            IInstaLogger logger, UserAuthValidate userAuthValidate, InstaApi instaApi, HttpHelper httpHelper)
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
        ///     Get medias for explore channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetExploreChannelVideosAsync(string channelId, string firstMediaId,
            PaginationParameters paginationParameters) =>
            await GetExploreChannelVideosAsync(channelId, firstMediaId, paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get medias for explore channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetExploreChannelVideosAsync(string channelId, string firstMediaId,
            PaginationParameters paginationParameters, CancellationToken cancellationToken) =>
            await _instaApi.HelperProcessor.GetChannelVideosAsync(UriCreator.GetExploreChannelVideosUri(channelId), 
                firstMediaId, paginationParameters, cancellationToken).ConfigureAwait(false);

        /// <summary>
        ///     Get user explore feed (Explore tab info) asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaExploreFeed" />
        /// </returns>
        public async Task<IResult<InstaExploreFeed>> GetExploreFeedAsync(PaginationParameters paginationParameters) =>
            await GetExploreFeedAsync(paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get user explore feed (Explore tab info) asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaExploreFeed" />
        /// </returns>
        public async Task<IResult<InstaExploreFeed>> GetExploreFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var exploreFeed = new InstaExploreFeed();
            try
            {
                InstaExploreFeed Convert(InstaExploreFeedResponse exploreFeedResponse)
                {
                    return ConvertersFabric.Instance.GetExploreFeedConverter(exploreFeedResponse).Convert();
                }
                var feeds = await GetExploreFeed(paginationParameters);
                if (!feeds.Succeeded)
                {
                    if (feeds.Value != null)
                        return Result.Fail(feeds.Info, Convert(feeds.Value));
                    else
                        return Result.Fail(feeds.Info, (InstaExploreFeed)null);
                }
                var feedResponse = feeds.Value;
                exploreFeed = Convert(feedResponse);
                paginationParameters.NextMaxId = feedResponse.MaxId;
                paginationParameters.RankToken = feedResponse.RankToken;

                while (feedResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextFeed = await GetExploreFeed(paginationParameters);
                    if (!nextFeed.Succeeded)
                        return Result.Fail(nextFeed.Info, Convert(feeds.Value));

                    feedResponse.NextMaxId = paginationParameters.NextMaxId = nextFeed.Value.MaxId;
                    feedResponse.RankToken = paginationParameters.RankToken = nextFeed.Value.RankToken;
                    feedResponse.MoreAvailable = nextFeed.Value.MoreAvailable;
                    feedResponse.AutoLoadMoreEnabled = nextFeed.Value.AutoLoadMoreEnabled;
                    feedResponse.NextMaxId = nextFeed.Value.NextMaxId;
                    feedResponse.ResultsCount = nextFeed.Value.ResultsCount;
                    feedResponse.Items.Channel = nextFeed.Value.Items.Channel;
                    feedResponse.Items.Medias.AddRange(nextFeed.Value.Items.Medias);
                    if (nextFeed.Value.Items.StoryTray == null)
                        feedResponse.Items.StoryTray = nextFeed.Value.Items.StoryTray;
                    else
                    {
                        feedResponse.Items.StoryTray.Id = nextFeed.Value.Items.StoryTray.Id;
                        feedResponse.Items.StoryTray.IsPortrait = nextFeed.Value.Items.StoryTray.IsPortrait;

                        feedResponse.Items.StoryTray.Tray.AddRange(nextFeed.Value.Items.StoryTray.Tray);
                        if (nextFeed.Value.Items.StoryTray.TopLive == null)
                            feedResponse.Items.StoryTray.TopLive = nextFeed.Value.Items.StoryTray.TopLive;
                        else
                        {
                            feedResponse.Items.StoryTray.TopLive.RankedPosition = nextFeed.Value.Items.StoryTray.TopLive.RankedPosition;
                            feedResponse.Items.StoryTray.TopLive.BroadcastOwners.AddRange(nextFeed.Value.Items.StoryTray.TopLive.BroadcastOwners);
                        }
                    }

                    paginationParameters.PagesLoaded++;
                }
                exploreFeed = Convert(feedResponse);
                exploreFeed.Medias.Pages = paginationParameters.PagesLoaded;
                exploreFeed.Medias.PageSize = feedResponse.ResultsCount;
                return Result.Success(exploreFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, exploreFeed, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, exploreFeed);
            }
        }

        /// <summary>
        ///     Get activity of following asynchronously
        /// </summary>
        /// <param name="paginationParameters"></param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        [Obsolete()]
        public async Task<IResult<InstaActivityFeed>> GetFollowingRecentActivityFeedAsync(
            PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var uri = UriCreator.GetFollowingRecentActivityUri();
            return await GetRecentActivityInternalAsync(uri, paginationParameters, CancellationToken.None);
        }

        /// <summary>
        ///     Get feed of media your liked.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetLikedFeedAsync(PaginationParameters paginationParameters) =>
            await GetLikedFeedAsync(paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get feed of media your liked.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetLikedFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaMediaListResponse mediaResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var mediaResult = await GetAnyFeeds(UriCreator.GetUserLikeFeedUri(paginationParameters.NextMaxId)).ConfigureAwait(false);
                mediaResponse = mediaResult.Value;

                if (!mediaResult.Succeeded)
                    return Result.Fail(mediaResult.Info, GetOrDefault());

                paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                paginationParameters.PagesLoaded++;
                while (mediaResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = await GetAnyFeeds(UriCreator.GetUserLikeFeedUri(paginationParameters.NextMaxId)).ConfigureAwait(false);
                    if (!result.Succeeded)
                        return Result.Fail(result.Info, GetOrDefault());

                    var convertedResult = result.Value;
                    paginationParameters.PagesLoaded++;
                    mediaResponse.NextMaxId = paginationParameters.NextMaxId = result.Value.NextMaxId;
                    mediaResponse.MoreAvailable = result.Value.MoreAvailable;
                    mediaResponse.ResultsCount += result.Value.ResultsCount;
                    mediaResponse.TotalCount += result.Value.TotalCount;
                    mediaResponse.Medias.AddRange(convertedResult.Medias);
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

            InstaMediaList GetOrDefault()
            {
                var ret = mediaResponse != null ? Convert(mediaResponse) : default(InstaMediaList);
                if (ret != default(InstaMediaList))
                {
                    ret.PageSize = mediaResponse.ResultsCount;
                    ret.Pages = paginationParameters.PagesLoaded;
                }
                return ret;
            }

            InstaMediaList Convert(InstaMediaListResponse mediaListResponse)
            {
                return ConvertersFabric.Instance.GetMediaListConverter(mediaListResponse).Convert();
            }
        }

        /// <summary>
        ///     Get recent activity info asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        public async Task<IResult<InstaActivityFeed>> GetRecentActivityFeedAsync(
            PaginationParameters paginationParameters) =>
            await GetRecentActivityFeedAsync(paginationParameters, CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        ///     Get recent activity info asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        public async Task<IResult<InstaActivityFeed>> GetRecentActivityFeedAsync(
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var uri = UriCreator.GetRecentActivityUri();
            return await GetRecentActivityInternalAsync(uri, paginationParameters, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        ///     Get saved media feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetSavedFeedAsync(PaginationParameters paginationParameters) =>
            await GetSavedFeedAsync(paginationParameters, CancellationToken.None).ConfigureAwait(false);


        /// <summary>
        ///     Get saved media feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetSavedFeedAsync(PaginationParameters paginationParameters, 
            CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            InstaMediaListResponse mediaResponse = null;
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                var mediaFeedsResult = await GetAnyFeeds(UriCreator.GetSavedFeedUri(paginationParameters?.NextMaxId)).ConfigureAwait(false);
                if (!mediaFeedsResult.Succeeded)
                    return Result.Fail(mediaFeedsResult.Info, GetOrDefault());

                mediaResponse = mediaFeedsResult.Value;
                paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                paginationParameters.PagesLoaded++;
                while (mediaResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = await GetAnyFeeds(UriCreator.GetSavedFeedUri(paginationParameters?.NextMaxId)).ConfigureAwait(false);
                    if (!result.Succeeded)
                        return Result.Fail(result.Info, GetOrDefault());

                    mediaResponse.NextMaxId = paginationParameters.NextMaxId = result.Value.NextMaxId;
                    mediaResponse.MoreAvailable = result.Value.MoreAvailable;
                    mediaResponse.AutoLoadMoreEnabled = result.Value.AutoLoadMoreEnabled;
                    mediaResponse.ResultsCount += result.Value.ResultsCount;
                    mediaResponse.TotalCount += result.Value.TotalCount;
                    mediaResponse.Medias.AddRange(result.Value.Medias);
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

            InstaMediaList GetOrDefault()
            {
                var ret = mediaResponse != null ? Convert(mediaResponse) : default(InstaMediaList);
                if (ret != default(InstaMediaList))
                {
                    ret.PageSize = mediaResponse.ResultsCount;
                    ret.Pages = paginationParameters.PagesLoaded;
                }
                return ret;
            }
            InstaMediaList Convert(InstaMediaListResponse mediaListResponse)
            {
                return ConvertersFabric.Instance.GetMediaListConverter(mediaListResponse).Convert();
            }
        }

        /// <summary>
        ///     Get tag feed by tag value asynchronously
        /// </summary>
        /// <param name="tag">Tag value</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaTagFeed" />
        /// </returns>
        public async Task<IResult<InstaTagFeed>> GetTagFeedAsync(string tag, PaginationParameters paginationParameters) =>
            await GetTagFeedAsync(tag, paginationParameters, CancellationToken.None).ConfigureAwait(false);


        /// <summary>
        ///     Get tag feed by tag value asynchronously
        /// </summary>
        /// <param name="tag">Tag value</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaTagFeed" />
        /// </returns>
        public async Task<IResult<InstaTagFeed>> GetTagFeedAsync(string tag, PaginationParameters paginationParameters,
            CancellationToken cancellationToken)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var tagFeed = new InstaTagFeed();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);
                
                InstaTagFeed Convert(InstaTagFeedResponse instaTagFeedResponse)
                {
                    return ConvertersFabric.Instance.GetTagFeedConverter(instaTagFeedResponse).Convert();
                }

                var tags = await GetTagFeed(tag, paginationParameters).ConfigureAwait(false);
                if (!tags.Succeeded)
                {
                    if (tags.Value != null)
                        return Result.Fail(tags.Info, Convert(tags.Value));
                    else
                        return Result.Fail(tags.Info, default(InstaTagFeed));
                }
                var feedResponse = tags.Value;

                tagFeed = Convert(feedResponse);

                paginationParameters.NextMaxId = feedResponse.NextMaxId;
                paginationParameters.PagesLoaded++;

                while (feedResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextFeed = await GetTagFeed(tag, paginationParameters).ConfigureAwait(false);
                    if (!nextFeed.Succeeded)
                        return Result.Fail(nextFeed.Info, tagFeed);

                    var convertedFeeds = Convert(nextFeed.Value);
                    tagFeed.NextMaxId = paginationParameters.NextMaxId = nextFeed.Value.NextMaxId;
                    tagFeed.Medias.AddRange(convertedFeeds.Medias);
                    tagFeed.Stories.AddRange(convertedFeeds.Stories);
                    feedResponse.MoreAvailable = nextFeed.Value.MoreAvailable;
                    paginationParameters.PagesLoaded++;
                }
                return Result.Success(tagFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, tagFeed, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, tagFeed);
            }
        }
        /// <summary>
        ///     Get user timeline feed (feed of recent posts from users you follow) asynchronously.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="seenMediaIds">Id of the posts seen till now</param>
        /// <param name="refreshRequest">Request refresh feeds</param>
        /// <param name="paginationSource">Pagination source</param>
        /// <returns>
        ///     <see cref="InstaFeed" />
        /// </returns>
        public async Task<IResult<InstaFeed>> GetUserTimelineFeedAsync(PaginationParameters paginationParameters,
            string[] seenMediaIds = null, bool refreshRequest = false,
            InstaFeedPaginationSource paginationSource = InstaFeedPaginationSource.None,
            ushort batteryLevel = 100,
            bool isCharging = false,
            bool isDarkMode = false,
            bool willSoundOn = false) =>
            await GetUserTimelineFeedAsync(paginationParameters, CancellationToken.None, seenMediaIds,
               refreshRequest, paginationSource, batteryLevel, isCharging, isDarkMode, willSoundOn).ConfigureAwait(false);


        /// <summary>
        ///     Get user timeline feed (feed of recent posts from users you follow) asynchronously.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="seenMediaIds">Id of the posts seen till now</param>
        /// <param name="refreshRequest">Request refresh feeds</param>
        /// <param name="paginationSource">Pagination source</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     <see cref="InstaFeed" />
        /// </returns>
        public async Task<IResult<InstaFeed>> GetUserTimelineFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken, string[] seenMediaIds = null, bool refreshRequest = false,
            InstaFeedPaginationSource paginationSource = InstaFeedPaginationSource.None,
            ushort batteryLevel = 100,
            bool isCharging = false,
            bool isDarkMode = false,
            bool willSoundOn = false)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var feed = new InstaFeed();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);
                
                if (refreshRequest || string.IsNullOrEmpty(paginationParameters.SessionId))
                    paginationParameters.SessionId = Guid.NewGuid().ToString();

                InstaFeed Convert(InstaFeedResponse instaFeedResponse)
                {
                    return ConvertersFabric.Instance.GetFeedConverter(instaFeedResponse).Convert();
                }
                var timelineFeeds = await GetUserTimelineFeed(paginationParameters,
                    seenMediaIds, refreshRequest, paginationSource,
                    batteryLevel, isCharging, isDarkMode, willSoundOn);

                if (!timelineFeeds.Succeeded)
                    return Result.Fail(timelineFeeds.Info, feed);

                var feedResponse = timelineFeeds.Value;

                feed = Convert(feedResponse);
                paginationParameters.NextMaxId = feed.NextMaxId;
                paginationParameters.PagesLoaded++;

                while (feed.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextFeed = await GetUserTimelineFeed(paginationParameters, null, false, paginationSource,
                    batteryLevel, isCharging, isDarkMode, willSoundOn);
                    
                    if (!nextFeed.Succeeded)
                        return Result.Fail(nextFeed.Info, feed);

                    var convertedFeed = Convert(nextFeed.Value);
                    feed.Medias.AddRange(convertedFeed.Medias);
                    feed.Stories.AddRange(convertedFeed.Stories);
                    feed.MoreAvailable = nextFeed.Value.MoreAvailable;
                    paginationParameters.NextMaxId = nextFeed.Value.NextMaxId;
                    paginationParameters.PagesLoaded++;
                }
                
                return Result.Success(feed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, feed, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, feed);
            }
        }

        /// <summary>
        ///     Get user topical explore feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="clusterId">Cluster id</param>
        /// <returns><see cref="InstaTopicalExploreFeed" /></returns>
        public async Task<IResult<InstaTopicalExploreFeed>> GetTopicalExploreFeedAsync(PaginationParameters paginationParameters,
            string clusterId = null) =>
            await GetTopicalExploreFeedAsync(paginationParameters, CancellationToken.None, clusterId).ConfigureAwait(false);


        /// <summary>
        ///     Get user topical explore feeds asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="clusterId">Cluster id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="InstaTopicalExploreFeed" /></returns>
        public async Task<IResult<InstaTopicalExploreFeed>> GetTopicalExploreFeedAsync(PaginationParameters paginationParameters,
            CancellationToken cancellationToken,string clusterId = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var topicalExploreFeed = new InstaTopicalExploreFeed();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaTopicalExploreFeed Convert(InstaTopicalExploreFeedResponse topicalExploreFeedResponse)
                {
                    return ConvertersFabric.Instance.GetTopicalExploreFeedConverter(topicalExploreFeedResponse).Convert();
                }

                var feeds = await GetTopicalExploreFeed(paginationParameters, clusterId);
                if (!feeds.Succeeded)
                {
                    if (feeds.Value != null)
                        return Result.Fail(feeds.Info, Convert(feeds.Value));
                    else
                        return Result.Fail(feeds.Info, (InstaTopicalExploreFeed)null);
                }
                var feedResponse = feeds.Value;
                topicalExploreFeed = Convert(feedResponse);
                paginationParameters.NextMaxId = feedResponse.MaxId;
                paginationParameters.RankToken = feedResponse.RankToken;
                while (feedResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextFeed = await GetTopicalExploreFeed(paginationParameters, clusterId);
                    if (!nextFeed.Succeeded)
                        return Result.Fail(nextFeed.Info, Convert(feeds.Value));

                    feedResponse.NextMaxId = paginationParameters.NextMaxId = nextFeed.Value.MaxId;
                    feedResponse.RankToken = paginationParameters.RankToken = nextFeed.Value.RankToken;
                    feedResponse.MoreAvailable = nextFeed.Value.MoreAvailable;
                    feedResponse.AutoLoadMoreEnabled = nextFeed.Value.AutoLoadMoreEnabled;
                    feedResponse.NextMaxId = nextFeed.Value.NextMaxId;
                    feedResponse.ResultsCount = nextFeed.Value.ResultsCount;
                    feedResponse.Channel = nextFeed.Value.Channel;
                    feedResponse.Medias.AddRange(nextFeed.Value.Medias);
                    feedResponse.TVChannels.AddRange(nextFeed.Value.TVChannels);
                    feedResponse.Clusters.AddRange(nextFeed.Value.Clusters);
                    feedResponse.MaxId = nextFeed.Value.MaxId;
                    feedResponse.HasShoppingChannelContent = nextFeed.Value.HasShoppingChannelContent;
                    paginationParameters.PagesLoaded++;
                }
                topicalExploreFeed = Convert(feedResponse);
                topicalExploreFeed.Medias.Pages = paginationParameters.PagesLoaded;
                topicalExploreFeed.Medias.PageSize = feedResponse.ResultsCount;
                return Result.Success(topicalExploreFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, topicalExploreFeed, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, topicalExploreFeed);
            }
        }




        private async Task<IResult<InstaRecentActivityResponse>> GetFollowingActivityWithMaxIdAsync(string maxId)
        {
            try
            {
                var uri = UriCreator.GetFollowingRecentActivityUri(maxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, uri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaRecentActivityResponse>(response, json);
                var followingActivity = JsonConvert.DeserializeObject<InstaRecentActivityResponse>(json,
                    new InstaRecentActivityConverter());
                return Result.Success(followingActivity);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRecentActivityResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaRecentActivityResponse>(exception);
            }
        }

        private async Task<IResult<InstaActivityFeed>> GetRecentActivityInternalAsync(Uri uri,
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var activityFeed = new InstaActivityFeed();

            try
            {
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, uri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaActivityFeed>(response, json);
                var feedPage = JsonConvert.DeserializeObject<InstaRecentActivityResponse>(json,
                    new InstaRecentActivityConverter());
                activityFeed.IsOwnActivity = feedPage.IsOwnActivity;
                var nextId = feedPage.NextMaxId;
                activityFeed.Items.AddRange(
                    feedPage.Stories.Select(ConvertersFabric.Instance.GetSingleRecentActivityConverter)
                        .Select(converter => converter.Convert()));
                paginationParameters.PagesLoaded++;
                activityFeed.NextMaxId = paginationParameters.NextMaxId = feedPage.NextMaxId;
                while (!string.IsNullOrEmpty(nextId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextFollowingFeed = await GetFollowingActivityWithMaxIdAsync(nextId);
                    if (!nextFollowingFeed.Succeeded)
                        return Result.Fail(nextFollowingFeed.Info, activityFeed);
                    nextId = nextFollowingFeed.Value.NextMaxId;
                    activityFeed.Items.AddRange(
                        feedPage.Stories.Select(ConvertersFabric.Instance.GetSingleRecentActivityConverter)
                            .Select(converter => converter.Convert()));
                    paginationParameters.PagesLoaded++;
                    activityFeed.NextMaxId = paginationParameters.NextMaxId = nextId;
                }

                return Result.Success(activityFeed);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, activityFeed, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, activityFeed);
            }
        }

        private async Task<IResult<InstaExploreFeedResponse>> GetExploreFeed(PaginationParameters paginationParameters)
        {
            try
            {
                var exploreUri = UriCreator.GetExploreUri(paginationParameters.NextMaxId, paginationParameters.RankToken, _instaApi.TimezoneOffset);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, exploreUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaExploreFeedResponse>(response, json);
                var feedResponse = JsonConvert.DeserializeObject<InstaExploreFeedResponse>(json,
                    new InstaExploreFeedDataConverter());
                return Result.Success(feedResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaExploreFeedResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaExploreFeedResponse>(exception);
            }
        }

        private async Task<IResult<InstaFeedResponse>> GetUserTimelineFeed(PaginationParameters paginationParameters, 
            string[] seenMediaIds = null, bool refreshRequest = false,
            InstaFeedPaginationSource paginationSource = InstaFeedPaginationSource.None,
            ushort batteryLevel = 100,
            bool isCharging = false,
            bool isDarkMode = false,
            bool willSoundOn = false)
        {
            
            try
            {
                var userFeedUri = UriCreator.GetUserFeedUri();
                var data = new Dictionary<string, string>
                {
                    {"phone_id", _deviceInfo.PhoneGuid.ToString()},
                    {"battery_level", batteryLevel.ToString()},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"device_id", _deviceInfo.DeviceGuid.ToString()},
                    {"request_id", Guid.NewGuid().ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"is_charging", Convert.ToUInt16(isCharging).ToString()},
                    {"is_dark_mode", Convert.ToUInt16(isDarkMode).ToString()},
                    {"will_sound_on", Convert.ToUInt16(willSoundOn).ToString()},
                    {"session_id", paginationParameters.SessionId},
                    {"bloks_versioning_id", _instaApi.GetApiVersionInfo().BloksVersionId},
                    //{"is_prefetch", "0"},
                    //{"_csrftoken", _user.CsrfToken},
                };

                if (seenMediaIds != null)
                    data.Add("seen_posts", seenMediaIds.EncodeList(false));

                if (paginationSource != InstaFeedPaginationSource.None)
                    data.Add("pagination_source", paginationSource.GetPaginationSource());

                if (refreshRequest)
                {
                    data.Add("feed_view_info", "[]");
                    data.Add("reason", "pull_to_refresh");
                    data.Add("is_pull_to_refresh", "1");
                    data.Add("is_split_head_load", "0");
                }
                else 
                {
                    if (string.IsNullOrEmpty(paginationParameters.NextMaxId))
                    {
                        data.Add("feed_view_info", "[]");
                        data.Add("reason", "cold_start_fetch");
                        //data.Add("reason", "warm_start_fetch");
                        data.Add("is_split_head_load", "0");
                    }
                    else
                    {
                        data.Add("reason", "pagination");
                        data.Add("max_id", paginationParameters.NextMaxId);
                    }
                    data.Add("is_pull_to_refresh", "0");
                }

                var request = /*await*/ _httpHelper.GetDefaultRequest(HttpMethod.Post, userFeedUri, _deviceInfo, data);
                request.Headers.AddHeader("X-Ads-Opt-Out", "0", _instaApi);
                request.Headers.AddHeader("X-Google-AD-ID", _deviceInfo.GoogleAdId.ToString(), _instaApi);
                request.Headers.AddHeader("X-DEVICE-ID", _deviceInfo.DeviceGuid.ToString(), _instaApi);
                request.Headers.AddHeader("X-CM-Bandwidth-KBPS", "-1.000", _instaApi);
                request.Headers.AddHeader("X-CM-Latency", "-1.000", _instaApi);
                request.Headers.AppendPriorityHeader(InstaApiConstants.HEADER_PRIORITY_VALUE_0, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaFeedResponse>(response, json);

                var feedResponse = JsonConvert.DeserializeObject<InstaFeedResponse>(json,
                    new InstaFeedResponseDataConverter(_user.LoggedInUser.Pk, removeAds));
                return Result.Success(feedResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaFeedResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaFeedResponse));
            }
        }
        
        private async Task<IResult<InstaTagFeedResponse>> GetTagFeed(string tag, PaginationParameters paginationParameters)
        {
            try
            {
                var userFeedUri = UriCreator.GetTagFeedUri(tag, paginationParameters?.NextMaxId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, userFeedUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTagFeedResponse>(response, json);
                var feedResponse = JsonConvert.DeserializeObject<InstaTagFeedResponse>(json,
                    new InstaTagFeedDataConverter());
                return Result.Success(feedResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTagFeedResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaTagFeedResponse));
            }
        }

        private async Task<IResult<InstaMediaListResponse>> GetAnyFeeds(Uri instaUri)
        {
            try
            {
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMediaListResponse>(response, json);

                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaListResponse>(json,
                    new InstaMediaListDataConverter());
                
                return Result.Success(mediaResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaMediaListResponse>(exception);
            }
        }

        private string TopicalSessionId = null;
        private async Task<IResult<InstaTopicalExploreFeedResponse>> GetTopicalExploreFeed(PaginationParameters paginationParameters, string clusterId)
        {
            try
            {
                if (string.IsNullOrEmpty(clusterId))
                    clusterId = "explore_all:0";

                if (string.IsNullOrEmpty(TopicalSessionId))
                    TopicalSessionId = Guid.NewGuid().ToString();
                var exploreUri = UriCreator.GetTopicalExploreUri(TopicalSessionId, paginationParameters?.NextMaxId, clusterId, _instaApi.TimezoneOffset);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, exploreUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request, true);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaTopicalExploreFeedResponse>(response, json);

                var feedResponse = JsonConvert.DeserializeObject<InstaTopicalExploreFeedResponse>(json,
                    new InstaTopicalExploreFeedDataConverter());

                return Result.Success(feedResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaTopicalExploreFeedResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaTopicalExploreFeedResponse>(exception);
            }
        }
    }
}
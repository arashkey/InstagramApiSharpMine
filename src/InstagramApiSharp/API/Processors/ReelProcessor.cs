﻿/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
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
    internal class ReelProcessor : IReelProcessor
    {

        #region Fields and constructor
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        private readonly InstaApi _instaApi;
        private readonly HttpHelper _httpHelper;

        public ReelProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     Get user's reels clips (medias)
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaReelsMediaList>> GetUserReelsClipsAsync(long userId, 
            PaginationParameters paginationParameters) =>
            await GetReelsClips(paginationParameters, userId).ConfigureAwait(false);

        /// <summary>
        ///     Mark reel feed as seen
        /// </summary>
        /// <param name="mediaPkImpression">Media pk (from <see cref="InstaMedia.Pk"/> )</param>
        public async Task<IResult<bool>> MarkReelAsSeenAsync(string mediaPkImpression)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetMarkReelAsSeenUri();
                var impression = new JObject();

                if (!string.IsNullOrEmpty(mediaPkImpression))
                    impression.Add(mediaPkImpression);

                var data = new JObject
                {
                    {"impressions", impression.ToString(Formatting.None)},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
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

        /// <summary>
        ///     Explore reel feeds
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        public async Task<IResult<InstaReelsMediaList>> GetReelsClipsAsync(PaginationParameters paginationParameters) =>
            await GetReelsClips(paginationParameters).ConfigureAwait(false);

        /// <summary>
        ///     Upload reel video
        /// </summary>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        /// <param name="sharePreviewToFeed">Share preview to feed</param>
        public async Task<IResult<InstaMedia>> UploadReelVideoAsync(InstaVideoUpload video, string caption, bool sharePreviewToFeed = false) =>
            await UploadReelVideoAsync(null, video, caption, sharePreviewToFeed).ConfigureAwait(false);

        /// <summary>
        ///     Upload reel video with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        /// <param name="sharePreviewToFeed">Share preview to feed</param>
        public async Task<IResult<InstaMedia>> UploadReelVideoAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string caption, bool sharePreviewToFeed = false)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var upProgress = new InstaUploaderProgress
            {
                Caption = string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
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
                    {"xsharing_user_ids", "[]"},
                    {"upload_media_width", "0"},
                    {"is_clips_video", "1"},
                    {"upload_media_duration_ms", "46000"},
                    {"content_tags", "use_default_cover"},
                    {"upload_id", uploadId},
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
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }


                var videoContent = new ByteArrayContent(videoBytes);
                videoContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                videoContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
                request.Content = videoContent;
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var vidExt = Path.GetExtension(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
                if (vidExt == "mov")
                    request.Headers.AddHeader("X-Entity-Type", "video/quicktime", _instaApi);
                else
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
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);
                var thumbnailUploader = await _instaApi.HelperProcessor
                    .UploadSinglePhoto(progress, video.VideoThumbnail.ConvertToImageUpload(),
                    upProgress, uploadId, false);

                upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                progress?.Invoke(upProgress);
                await Task.Delay(15000);
                return await ConfigureVideoAsync(progress, upProgress,
                    video, uploadId, caption, sharePreviewToFeed);
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

        private async Task<IResult<InstaMedia>> ConfigureVideoAsync(Action<InstaUploaderProgress> progress,
            InstaUploaderProgress upProgress, InstaVideoUpload video, 
            string uploadId, string caption, bool sharePreviewToFeed = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                //try
                //{
                //    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                //}
                //catch { }
                var instaUri = UriCreator.GetReelsMediaConfigureUri();

           
                var clipsSegments = new JObject
                {
                    {"index", 0},
                    {"face_effect_id", null},
                    {"speed", 100},
                    {"source", "camera"},
                    {"duration_ms", video.Video.Length * 1000},
                    {"audio_type", "original"},
                    {"from_draft", "0"},
                    {"camera_position", "2"},
                    {"media_folder", null},
                    {"media_type", "video"},
                    {"original_media_type", "video"},
                };
                var clipsSegmentsMetadata = new JObject
                {
                    {"num_segments", 1},
                    {"clips_segments", new JArray(clipsSegments)},
                };
                var data = new JObject
                {
                    {"is_clips_edited", "0"},
                    {"caption", caption ?? string.Empty},
                    {"upload_id", uploadId},
                    {"source_type", "3"},
                    {"device_id", _deviceInfo.DeviceId},
                    {"camera_position", "back"},
                    {"camera_session_id", Guid.NewGuid().ToString()},
                    {"timezone_offset", InstaApiConstants.TIMEZONE_OFFSET.ToString()},
                    {"capture_type", "clips_v2"},
                    {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                    {
                        "additional_audio_info", new JObject
                        {
                            {"has_voiceover_attribution", 0},
                        }
                    },
                    {
                        "extra", new JObject
                        {
                            {"source_width", 0},
                            {"source_height", 0}
                        }
                    },
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier.Replace(" ", "+")},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber.Replace(".0.0", "").Replace(".0", "")},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)}
                        }
                    },
                    {"length", video.Video.Length},
                    {
                        "clips", new JArray{
                            new JObject
                            {
                                {"length", video.Video.Length},
                                {"source_type", "3"},
                                {"camera_position", "back"}
                            }
                        }
                    },
                    {"poster_frame_index", 0},
                    {"audio_muted", false},
                    {"filter_type", "0"},
                    {"video_result", ""},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {
                        new JProperty("clips_audio_metadata", new JObject
                        {
                            new JProperty("original", new JObject
                            {
                                {"volume_level", 1.0},
                            })
                        })
                    },
                    {"clips_segments_metadata", clipsSegmentsMetadata}
                };
                if (sharePreviewToFeed)
                    data.Add("clips_share_preview_to_feed", "1");
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
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
                //if (obj.Caption == null && !string.IsNullOrEmpty(caption))
                //{
                //    var editedMedia = await _instaApi.MediaProcessor.EditMediaAsync(obj.InstaIdentifier, caption);
                //    if (editedMedia.Succeeded)
                //        return Result.Success(editedMedia.Value);
                //}
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

        private async Task<IResult<InstaReelsMediaList>> GetReelsClips(PaginationParameters paginationParameters, long? userId = null)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var reelFeeds = new InstaReelsMediaList();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaReelsMediaList Convert(InstaReelsMediaListResponse instaReelFeedResponse)
                {
                    return ConvertersFabric.Instance.GetReelsMediaListConverter(instaReelFeedResponse).Convert();
                }
                var timelineFeeds = await GetReels(paginationParameters, userId);
                if (!timelineFeeds.Succeeded)
                    return Result.Fail(timelineFeeds.Info, reelFeeds);

                var reelFeedResponse = timelineFeeds.Value;

                reelFeeds = Convert(reelFeedResponse);
                paginationParameters.NextMaxId = reelFeeds.NextMaxId;
                paginationParameters.PagesLoaded++;

                while (reelFeeds.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded <= paginationParameters.MaximumPagesToLoad)
                {
                    var nextFeed = await GetReels(paginationParameters, userId);
                    if (!nextFeed.Succeeded)
                        return Result.Fail(nextFeed.Info, reelFeeds);

                    var convertedFeed = Convert(nextFeed.Value);
                    reelFeeds.Medias.AddRange(convertedFeed.Medias);
                    reelFeeds.MoreAvailable = nextFeed.Value.PagingInfo?.MoreAvailable ?? false;
                    reelFeeds.NextMaxId = paginationParameters.NextMaxId = nextFeed.Value.PagingInfo?.MaxId;
                    paginationParameters.PagesLoaded++;
                }

                return Result.Success(reelFeeds);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, reelFeeds, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, reelFeeds);
            }
        }

        private async Task<IResult<InstaReelsMediaListResponse>> GetReels(PaginationParameters paginationParameters,
            long? userId = null)
        {
            try
            {
                Uri instaUri;
                Dictionary<string, string> data;
                if (userId.HasValue)
                {
                    instaUri = UriCreator.GetUserReelsClipsUri();
                    data = new Dictionary<string, string>
                    {
                        {"_csrftoken", _user.CsrfToken},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        {"target_user_id", userId.ToString()},
                    };
                }
                else
                {
                    instaUri = UriCreator.GetReelsClipsUri();

                    var sessionInfo = new JObject
                    {
                        {"session_id", Guid.NewGuid().ToString()},
                        {"media_info", new JObject()},
                    };
                    data = new Dictionary<string, string>
                    {
                        {"seen_reels", "0"},
                        {"pct_reels", "0"},
                        {"tab_type", "clips_tab"},
                        {"_csrftoken", _user.CsrfToken},
                        {"session_info" ,sessionInfo.ToString(Formatting.None)},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    };
                }

                if (!string.IsNullOrEmpty(paginationParameters.NextMaxId))
                    data.Add("max_id", paginationParameters.NextMaxId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaReelsMediaListResponse>(response, json);

                return Result.Success(JsonConvert.DeserializeObject<InstaReelsMediaListResponse>(json));
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaReelsMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaReelsMediaListResponse));
            }
        }
    }
}
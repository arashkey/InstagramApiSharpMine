/*
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

#pragma warning disable IDE0052
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
        ///     Upload reel video
        /// </summary>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        public async Task<IResult<InstaMedia>> UploadReelVideoAsync(InstaVideoUpload video, string caption) =>
            await UploadReelVideoAsync(null, video, caption);

        /// <summary>
        ///     Upload reel video with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        public async Task<IResult<InstaMedia>> UploadReelVideoAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string caption)
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
                return await ConfigureVideoAsync(progress, upProgress, video, uploadId, caption);
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
            InstaUploaderProgress upProgress, InstaVideoUpload video, string uploadId, string caption)
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
                    {"clips_share_preview_to_feed", "1"},
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
                        "clips_audio_metadata", new JObject
                        {
                            "original", new JObject
                            {
                                new JObject {"volume_level", 1.0},
                            }
                        }
                    },
                    {"clips_segments_metadata", clipsSegmentsMetadata}
                };
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

    }
}

#pragma warning restore IDE0052
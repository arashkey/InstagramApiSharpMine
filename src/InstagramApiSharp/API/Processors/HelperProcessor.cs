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
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Converters.Json;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Helper processor for other processors
    /// </summary>
    internal class HelperProcessor
    {
        #region Properties and constructor
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        private readonly InstaApi _instaApi;
        private readonly HttpHelper _httpHelper;
        public HelperProcessor(AndroidDevice deviceInfo, UserSessionData user,
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

        public async Task<IResult<byte[]>> GetBytesAsync(string url) => await GetBytesAsync(new Uri(url));
        public async Task<IResult<byte[]>> GetBytesAsync(Uri uri)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return Result.Success(bytes);
                }
                else
                    return Result.Fail<byte[]>("Failed to download bytes");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////// SINGLE UPLOADER ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<IResult<string>> UploadSinglePhoto(Action<InstaUploaderProgress> progress,
            InstaImageUpload image, InstaUploaderProgress upProgress, string uploadId = null,
            bool album = true, string recipient = null, string broadcastId = null,
            string preferredWaterfallId = null)
        {
            if (string.IsNullOrEmpty(uploadId))
                uploadId = ExtensionHelper.GenerateUploadingUploadId()/*ApiRequestMessage.GenerateUnknownUploadId()*/;
            var photoHashCode = Path.GetFileName(image.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetPositiveHashCode();
            photoHashCode *= 2;
            if (photoHashCode > 0)
                photoHashCode *= -1;

            var photoEntityName = $"{uploadId}_0_{photoHashCode}";
            var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);
            var photoUploadParamsObj = new JObject
            {
                {"upload_id", uploadId},
                {"media_type", "1"},
                {"retry_context", GetRetryContext()},
                {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                {"xsharing_user_ids", $"[{recipient ?? string.Empty}]"},
            };
            if (album)
                photoUploadParamsObj.Add("is_sidecar", "1");
            if (!string.IsNullOrEmpty(broadcastId))
            {
                photoUploadParamsObj.Add("broadcast_id", broadcastId);
                photoUploadParamsObj.Add("is_post_live_igtv", "1");
            }
            upProgress.UploadState = InstaUploadState.UploadingThumbnail;
            progress?.Invoke(upProgress);
            var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
            var imageBytes = image.ImageBytes ?? File.ReadAllBytes(image.Uri);
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
            imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
            var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
            request.Content = imageContent;
            request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
            request.Headers.AddHeader("Offset", "0", _instaApi);
            request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
            request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
            request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
            request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", preferredWaterfallId ?? Guid.NewGuid().ToString(), _instaApi);
            request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
            var response = await _httpRequestProcessor.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);
                return Result.Success(uploadId);
            }
            else
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.Fail<string>("NO UPLOAD ID");
            }
        }

        public async Task<IResult<(string, string)>> UploadSingleVideo(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video,
            InstaUploaderProgress upProgress,
            bool album = true,
            bool useDefaultCover = true)
        {
            var uploadId = ExtensionHelper.GenerateUploadingUploadId();
            var videoHashCode = Path.GetFileName(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").GetHashCode();
            videoHashCode *= 2;
            if (videoHashCode < 0)
                videoHashCode *= -1;

            var waterfallId = Guid.NewGuid().ToString();
            var videoEntityName = $"{uploadId}_0_{videoHashCode}";
            var videoUri = UriCreator.GetStoryUploadVideoUri(uploadId, videoHashCode.ToString());
            var retryContext = GetRetryContext();
            //{
            //  "upload_media_height": "480",
            //  "xsharing_user_ids": "[\"11292195227\",\"48321748823\",\"8651542203\"]",
            //  "upload_media_width": "480",
            //  "upload_media_duration_ms": "6827",
            //  "content_tags": "use_default_cover",
            //  "upload_id": "403988992440848",
            //  "retry_context": "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
            //  "media_type": "2",
            //  "is_fmp4": "1"
            //}
            var videoUploadParamsObj = new JObject
            {
                {"upload_media_height", video.Video.Height.ToString()},
                {"xsharing_user_ids", "[]"},
                {"upload_media_width", video.Video.Width.ToString()},
                {"upload_media_duration_ms", video.Video.Length.ToString()},
                {"upload_id", uploadId},
                {"retry_context", retryContext},
                {"media_type", "2"},
            };
            if (useDefaultCover)
            {
                videoUploadParamsObj.Add("content_tags", "use_default_cover");
            }

            if (album)
                videoUploadParamsObj.Add("is_sidecar", "1");

            var videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
            var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
            request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
            request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
            request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);

            var response = await _httpRequestProcessor.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();


            if (response.StatusCode != HttpStatusCode.OK)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<(string, string)>(response, json);
            }

            var videoBytes = video.Video.VideoBytes ?? File.ReadAllBytes(video.Video.Uri);

            var videoContent = new ByteArrayContent(videoBytes);
            //var progressContent = new ProgressableStreamContent(videoContent, 4096, progress)
            //{
            //    UploaderProgress = upProgress
            //};
            request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
            request.Content = videoContent;
            upProgress.UploadState = InstaUploadState.Uploading;
            progress?.Invoke(upProgress);
            var vidExt = Path.GetExtension(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
            //if (vidExt == "mov")
            //    request.Headers.AddHeader("X-Entity-Type", "video/quicktime", _instaApi);
            //else
            request.Headers.AddHeader("X-Entity-Type", "video/mp4", _instaApi);

            request.Headers.AddHeader("Offset", "0", _instaApi);
            request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
            request.Headers.AddHeader("X-Entity-Name", videoEntityName, _instaApi);
            request.Headers.AddHeader("X-Entity-Length", videoBytes.Length.ToString(), _instaApi);
            request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
            request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);

            response = await _httpRequestProcessor.SendAsync(request);
            json = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<(string, string)>(response, json);
            }
            return Result.Success((uploadId, waterfallId));
        }

        public JObject GetImageConfigure(string uploadId, InstaImageUpload image)
        {
            var imgData = new JObject
            {
                {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                {"source_type", "4"},
                {"upload_id", uploadId},
                {"caption", ""},
                {
                    "extra", JsonConvert.SerializeObject(new JObject
                    {
                        {"source_width", 0},
                        {"source_height", 0}
                    })
                },
                {
                    "device", JsonConvert.SerializeObject(new JObject{
                        {"manufacturer", _deviceInfo.HardwareManufacturer},
                        {"model", _deviceInfo.DeviceModelIdentifier},
                        {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                        {"android_version", _deviceInfo.AndroidVer.APILevel}
                    })
                }
            };
            if (image.UserTags?.Count > 0)
            {
                var tagArr = new JArray();
                foreach (var tag in image.UserTags)
                {
                    if (tag.Pk != -1)
                    {
                        var position = new JArray(tag.X, tag.Y);
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
                imgData.Add("usertags", root.ToString(Formatting.None));
            }
            return imgData;
        }

        public JObject GetVideoConfigure(string uploadId, InstaVideoUpload video)
        {
            var vidData = new JObject
            {
                {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                {"caption", ""},
                {"upload_id", uploadId},
                {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                {"source_type", "4"},
                {
                    "extra", JsonConvert.SerializeObject(new JObject
                    {
                        {"source_width", 0},
                        {"source_height", 0}
                    })
                },
                {
                    "clips", JsonConvert.SerializeObject(new JArray{
                        new JObject
                        {
                            {"length", video.Video.Length},
                            {"source_type", "4"},
                        }
                    })
                },
                {
                    "device", JsonConvert.SerializeObject(new JObject{
                        {"manufacturer", _deviceInfo.HardwareManufacturer},
                        {"model", _deviceInfo.DeviceModelIdentifier},
                        {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                        {"android_version", _deviceInfo.AndroidVer.APILevel}
                    })
                },
                {"length", video.Video.Length.ToString()},
                {"poster_frame_index", "0"},
                {"audio_muted", "false"},
                {"filter_type", "0"},
                {"video_result", ""},
            };
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
                vidData.Add("usertags", root.ToString(Formatting.None));
            }
            return vidData;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////// SINGLE UPLOADER ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

















        /// <summary>
        ///     Send video story, direct video, disappearing video
        /// </summary>
        /// <param name="isDirectVideo">Direct video</param>
        /// <param name="isDisappearingVideo">Disappearing video</param>
        public async Task<IResult<bool>> SendVideoAsync(Action<InstaUploaderProgress> progress, bool isDirectVideo, bool isDisappearingVideo, string caption,
            InstaViewMode viewMode, InstaStoryType storyType, string recipients, string threadId, InstaVideoUpload video, Uri uri = null, InstaStoryUploadOptions uploadOptions = null)
        {
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                var uploadId = ApiRequestMessage.GenerateUnknownUploadId();
                var videoHashCode = Path.GetFileName(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").GetHashCode();
                var waterfallId = Guid.NewGuid().ToString();
                var videoEntityName = $"{uploadId}_0_{videoHashCode}";
                var videoUri = UriCreator.GetStoryUploadVideoUri(uploadId, videoHashCode.ToString());
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string videoUploadParams = null;
                string json = null;
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var videoUploadParamsObj = new JObject();
                if (isDirectVideo)
                {
                    videoUploadParamsObj = new JObject
                    {
                        {"upload_media_height", "0"},
                        {"direct_v2", "1"},
                        {"upload_media_width", "0"},
                        {"upload_media_duration_ms", "0"},
                        {"upload_id", uploadId},
                        {"retry_context", retryContext},
                        {"media_type", "2"}
                    };

                    videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                    request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
                    request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                    request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                    response = await _httpRequestProcessor.SendAsync(request);
                    json = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        upProgress.UploadState = InstaUploadState.Error;
                        progress?.Invoke(upProgress);
                        return Result.UnExpectedResponse<bool>(response, json);
                    }
                }
                else
                {
                    videoUploadParamsObj = new JObject
                    {
                        {"_uid", _user.LoggedInUser.Pk},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        {"media_info", new JObject
                            {
                                    {"capture_mode", "normal"},
                                    {"media_type", 2},
                                    {"caption", caption ?? string.Empty},
                                    {"mentions", new JArray()},
                                    {"hashtags", new JArray()},
                                    {"locations", new JArray()},
                                    {"stickers", new JArray()},
                            }
                        }
                    };
                    if (!_httpHelper.NewerThan180)
                    {
                        videoUploadParamsObj.Add("_csrftoken", _user.CsrfToken);
                    }
                    request = _httpHelper.GetSignedRequest(HttpMethod.Post, UriCreator.GetStoryMediaInfoUploadUri(), _deviceInfo, videoUploadParamsObj);
                    response = await _httpRequestProcessor.SendAsync(request);
                    json = await response.Content.ReadAsStringAsync();


                    videoUploadParamsObj = new JObject
                    {
                        {"upload_media_height", "0"},
                        {"upload_media_width", "0"},
                        {"upload_media_duration_ms", "0"},
                        {"upload_id", uploadId},
                        {"retry_context", "{\"num_step_auto_retry\":0,\"num_reupload\":0,\"num_step_manual_retry\":0}"},
                        {"media_type", "2"}
                    };
                    if (isDisappearingVideo)
                    {
                        videoUploadParamsObj.Add("for_direct_story", "1");
                    }
                    else
                    {
                        switch (storyType)
                        {
                            case InstaStoryType.SelfStory:
                            default:
                                videoUploadParamsObj.Add("for_album", "1");
                                break;
                            case InstaStoryType.Direct:
                                videoUploadParamsObj.Add("for_direct_story", "1");
                                break;
                            case InstaStoryType.Both:
                                videoUploadParamsObj.Add("for_album", "1");
                                videoUploadParamsObj.Add("for_direct_story", "1");
                                break;
                        }
                    }
                    videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                    request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
                    request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                    request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                    response = await _httpRequestProcessor.SendAsync(request);
                    json = await response.Content.ReadAsStringAsync();


                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        upProgress.UploadState = InstaUploadState.Error;
                        progress?.Invoke(upProgress);
                        return Result.UnExpectedResponse<bool>(response, json);
                    }
                }

                // video part
                byte[] videoBytes;
                if (video.Video.VideoBytes == null)
                    videoBytes = File.ReadAllBytes(video.Video.Uri);
                else
                    videoBytes = video.Video.VideoBytes;

                var videoContent = new ByteArrayContent(videoBytes);
                //var progressContent = new ProgressableStreamContent(videoContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
                request.Content = videoContent;
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var vidExt = Path.GetExtension(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
                //if (vidExt == "mov")
                //    request.Headers.AddHeader("X-Entity-Type", "video/quicktime", _instaApi);
                //else
                request.Headers.AddHeader("X-Entity-Type", "video/mp4", _instaApi);

                request.Headers.AddHeader("Offset", "0", _instaApi);
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
                    return Result.UnExpectedResponse<bool>(response, json);
                }
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);
                //upProgress = progressContent?.UploaderProgress;
                if (!isDirectVideo)
                {
                    upProgress.UploadState = InstaUploadState.UploadingThumbnail;
                    progress?.Invoke(upProgress);
                    var photoHashCode = Path.GetFileName(video.VideoThumbnail.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();
                    var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                    var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);
                    var photoUploadParamsObj = new JObject
                    {
                        {"retry_context", retryContext},
                        {"media_type", "2"},
                        {"upload_id", uploadId},
                        {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                    };

                    var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                    byte[] imageBytes;
                    if (video.VideoThumbnail.ImageBytes == null)
                        imageBytes = File.ReadAllBytes(video.VideoThumbnail.Uri);
                    else
                        imageBytes = video.VideoThumbnail.ImageBytes;
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                    imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                    request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                    request.Content = imageContent;
                    request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                    request.Headers.AddHeader("Offset", "0", _instaApi);
                    request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                    request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                    request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                    request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                    response = await _httpRequestProcessor.SendAsync(request);
                    json = await response.Content.ReadAsStringAsync();
                    upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                    progress?.Invoke(upProgress);
                }
                return await ConfigureVideo(progress, upProgress, uploadId, isDirectVideo, isDisappearingVideo, caption, viewMode, storyType, recipients, threadId, uri, uploadOptions);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }

        private async Task<IResult<bool>> ConfigureVideo(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, string uploadId, bool isDirectVideo, bool isDisappearingVideo, string caption,
            InstaViewMode viewMode, InstaStoryType storyType, string recipients, string threadId, Uri uri, InstaStoryUploadOptions uploadOptions = null)
        {
            try
            {
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetDirectConfigureVideoUri();
                var retryContext = GetRetryContext();
                var clientContext = ExtensionHelper.GetThreadToken();
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                if (isDirectVideo)
                {
                    var data = new Dictionary<string, string>
                    {
                         {"action", "send_item"},
                         {"client_context", clientContext},
                         {"video_result", ""},
                         {"device_id", _deviceInfo.DeviceId},
                         {"mutation_token", clientContext},
                         {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                         {"upload_id", uploadId}
                    };
                    if (!_httpHelper.NewerThan180)
                    {
                        data.Add("_csrftoken", _user.CsrfToken);
                    }
                    if (!string.IsNullOrEmpty(recipients))
                        data.Add("recipient_users", $"[[{recipients}]]");
                    else
                        data.Add("thread_ids", $"[{threadId}]");

                    instaUri = UriCreator.GetDirectConfigureVideoUri();
                    var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                    request.Content = new FormUrlEncodedContent(data);
                    request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                    var response = await _httpRequestProcessor.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        upProgress.UploadState = InstaUploadState.Error;
                        progress?.Invoke(upProgress);
                        return Result.UnExpectedResponse<bool>(response, json);
                    }
                    var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                    upProgress.UploadState = obj.IsSucceed ? InstaUploadState.Configured : InstaUploadState.Completed;
                    progress?.Invoke(upProgress);
                    return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
                }
                else
                {
                    var rnd = new Random();
                    var data = new JObject
                    {
                        {"filter_type", "0"},
                        {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                        {"client_shared_at", (DateTime.UtcNow.ToUnixTime() - rnd.Next(25,55)).ToString()},
                        {"story_media_creation_date", (DateTime.UtcNow.ToUnixTime() - rnd.Next(50,70)).ToString()},
                        {"media_folder", "Camera"},
                        {"source_type", "4"},
                        {"video_result", ""},
                        {"_uid", _user.LoggedInUser.Pk.ToString()},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        //{"caption", caption ?? string.Empty},
                        {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                        {"capture_type", "normal"},
                        {"mas_opt_in", "NOT_PROMPTED"},
                        {"upload_id", uploadId},
                        {"client_timestamp", DateTime.UtcNow.ToUnixTime()},
                        {
                            "device", new JObject{
                                {"manufacturer", _deviceInfo.HardwareManufacturer},
                                {"model", _deviceInfo.DeviceModelIdentifier},
                                {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                                {"android_version", _deviceInfo.AndroidVer.APILevel}
                            }
                        },
                        {"length", 0},
                        {
                            "extra", new JObject
                            {
                                {"source_width", 0},
                                {"source_height", 0}
                            }
                        },
                        {"audio_muted", false},
                        {"poster_frame_index", 0},
                    };
                    if (!_httpHelper.NewerThan180)
                    {
                        data.Add("_csrftoken", _user.CsrfToken);
                    }
                    if (isDisappearingVideo)
                    {
                        data.Add("view_mode", viewMode.ToString().ToLower());
                        data.Add("configure_mode", "2");
                        data.Add("recipient_users", "[]");
                        data.Add("thread_ids", $"[{threadId}]");
                    }
                    else
                    {
                        switch (storyType)
                        {
                            case InstaStoryType.SelfStory:
                            default:
                                data.Add("configure_mode", "1");
                                break;
                            case InstaStoryType.Direct:
                                data.Add("configure_mode", "2");
                                data.Add("view_mode", "replayable");
                                data.Add("recipient_users", "[]");
                                data.Add("thread_ids", $"[{threadId}]");
                                break;
                            case InstaStoryType.Both:
                                data.Add("configure_mode", "3");
                                data.Add("view_mode", "replayable");
                                data.Add("recipient_users", "[]");
                                data.Add("thread_ids", $"[{threadId}]");
                                break;
                        }

                        if (uri != null)
                        {
                            var webUri = new JArray
                            {
                                new JObject
                                {
                                    {"linkType", 1},
                                    {"webUri", uri.ToString()},
                                    {"androidClass", ""},
                                    {"package", ""},
                                    {"deeplinkUri", ""},
                                    {"callToActionTitle", ""},
                                    {"redirectUri", null},
                                    {"leadGenFormId", ""},
                                    {"igUserId", ""},
                                    {"appInstallObjectiveInvalidationBehavior", null}
                                }
                            };
                            var storyCta = new JArray
                            {
                                new JObject
                                {
                                    {"links",  webUri}
                                }
                            };
                            data.Add("story_cta", storyCta.ToString(Formatting.None));
                        }

                        if (uploadOptions != null)
                        {
                            if (uploadOptions.Hashtags?.Count > 0)
                            {
                                var hashtagArr = new JArray();
                                foreach (var item in uploadOptions.Hashtags)
                                    hashtagArr.Add(item.ConvertToJson());

                                data.Add("story_hashtags", hashtagArr.ToString(Formatting.None));
                            }

                            if (uploadOptions.Locations?.Count > 0)
                            {
                                var locationArr = new JArray();
                                foreach (var item in uploadOptions.Locations)
                                    locationArr.Add(item.ConvertToJson());

                                data.Add("story_locations", locationArr.ToString(Formatting.None));
                            }
                            if (uploadOptions.Slider != null)
                            {
                                var sliderArr = new JArray
                                {
                                    uploadOptions.Slider.ConvertToJson()
                                };

                                data.Add("story_sliders", sliderArr.ToString(Formatting.None));
                                if (uploadOptions.Slider.IsSticker)
                                    data.Add("story_sticker_ids", $"emoji_slider_{uploadOptions.Slider.Emoji}");
                            }
                            else
                            {
                                if (uploadOptions.Polls?.Count > 0)
                                {
                                    var pollArr = new JArray();
                                    foreach (var item in uploadOptions.Polls)
                                        pollArr.Add(item.ConvertToJson());

                                    data.Add("story_polls", pollArr.ToString(Formatting.None));
                                }
                                if (uploadOptions.Questions?.Count > 0)
                                {
                                    var questionArr = new JArray();
                                    foreach (var item in uploadOptions.Questions)
                                        questionArr.Add(item.ConvertToJson());

                                    data.Add("story_questions", questionArr.ToString(Formatting.None));
                                }
                            }
                            if (uploadOptions.Countdown != null)
                            {
                                var countdownArr = new JArray
                                {
                                    uploadOptions.Countdown.ConvertToJson()
                                };

                                data.Add("story_countdowns", countdownArr.ToString(Formatting.None));
                                data.Add("story_sticker_ids", "countdown_sticker_time");
                            }
                            if (uploadOptions.StoryQuiz != null)
                            {
                                var storyQuizArr = new JArray
                                {
                                    uploadOptions.StoryQuiz.ConvertToJson()
                                };

                                data.Add("story_quizs", storyQuizArr.ToString(Formatting.None));
                                data.Add("story_sticker_ids", "quiz_story_sticker_default");
                            }
                            if (uploadOptions.StoryChats?.Count > 0)
                            {
                                var chatArr = new JArray();
                                foreach (var item in uploadOptions.StoryChats)
                                    chatArr.Add(item.ConvertToJson());

                                data.Add("story_chats", chatArr.ToString(Formatting.None));
                                data.Add("internal_features", "chat_sticker");
                                data.Add("story_sticker_ids", "chat_sticker_id");
                            }
                        }
                    }
                    instaUri = UriCreator.GetVideoStoryConfigureUri(true);
                    var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                    request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                    var response = await _httpRequestProcessor.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var mediaResponse = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                        upProgress.UploadState = mediaResponse.IsSucceed ? InstaUploadState.Configured : InstaUploadState.Completed;
                        progress?.Invoke(upProgress);

                        return mediaResponse.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
                    }
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }
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



        public async Task<IResult<bool>> SendPhotoAsync(Action<InstaUploaderProgress> progress, bool isDirectPhoto, bool isDisappearingPhoto, string caption, InstaViewMode viewMode, InstaStoryType storyType, string recipients, string threadId, InstaImage image)
        {
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                var uploadId = ApiRequestMessage.GenerateUnknownUploadId();
                var photoHashCode = Path.GetFileName(image.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();
                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);
                var waterfallId = Guid.NewGuid().ToString();
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string json = null;
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var photoUploadParamsObj = new JObject
                {
                    {"retry_context", retryContext},
                    {"media_type", "1"},
                    {"upload_id", uploadId},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                };
                var uploadParamsObj = new JObject
                {
                    {"_uid", _user.LoggedInUser.Pk},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"media_info", new JObject
                        {
                                {"capture_mode", "normal"},
                                {"media_type", 1},
                                {"caption", caption ?? string.Empty},
                                {"mentions", new JArray()},
                                {"hashtags", new JArray()},
                                {"locations", new JArray()},
                                {"stickers", new JArray()},
                        }
                    }
                };
                if (!_httpHelper.NewerThan180)
                {
                    uploadParamsObj.Add("_csrftoken", _user.CsrfToken);
                }
                request = _httpHelper.GetSignedRequest(HttpMethod.Post, UriCreator.GetStoryMediaInfoUploadUri(), _deviceInfo, uploadParamsObj);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();



                var uploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Get, photoUri, _deviceInfo);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", uploadParams, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();



                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }

                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                byte[] imageBytes;
                imageBytes = image.ImageBytes ?? File.ReadAllBytes(image.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                //var progressContent = new ProgressableStreamContent(imageContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                //upProgress = progressContent?.UploaderProgress;
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);

                return await ConfigurePhoto(progress, upProgress, uploadId, isDirectPhoto, isDisappearingPhoto, caption, viewMode, storyType, recipients, threadId);
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

        private async Task<IResult<bool>> ConfigurePhoto(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress, string uploadId, bool isDirectPhoto, bool isDisappearingPhoto, string caption, InstaViewMode viewMode, InstaStoryType storyType, string recipients, string threadId)
        {
            try
            {

                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                if (!string.IsNullOrEmpty(caption))
                    caption = caption.Replace("\r", "");
                var instaUri = UriCreator.GetDirectConfigureVideoUri();
                var retryContext = GetRetryContext();
                var clientContext = ExtensionHelper.GetThreadToken();

                //if (isDirectVideo)
                //{

                //}
                //else
                {

                    //{
                    //	"recipient_users": "[]",
                    //	"view_mode": "permanent",
                    //	"thread_ids": "[\"340282366841710300949128132202173515958\"]",
                    //	"timezone_offset": "16200",
                    //	"_csrftoken": "gRMgctLzzC9MfJBQTz3MzxeYMtBxCY4s",
                    //	"client_shared_at": "1536323374",
                    //	"configure_mode": "2",
                    //	"source_type": "3",
                    //	"_uid": "7405924766",
                    //	"_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                    //	"capture_type": "normal",
                    //	"mas_opt_in": "NOT_PROMPTED",
                    //	"upload_id": "469885239145487",
                    //	"client_timestamp": "1536323328",
                    //	"device": {
                    //		"manufacturer": "HUAWEI",
                    //		"model": "PRA-LA1",
                    //		"android_version": 24,
                    //		"android_release": "7.0"
                    //	},
                    //	"edits": {
                    //		"crop_original_size": [2240.0, 3968.0],
                    //		"crop_center": [0.0, -2.5201612E-4],
                    //		"crop_zoom": 1.0595461
                    //	},
                    //	"extra": {
                    //		"source_width": 2232,
                    //		"source_height": 3745
                    //	}
                    //}
                    var rnd = new Random();
                    var data = new JObject
                    {
                        {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                        {"client_shared_at", (DateTime.UtcNow.ToUnixTime() - rnd.Next(25,55)).ToString()},
                        {"story_media_creation_date", (DateTime.UtcNow.ToUnixTime() - rnd.Next(50,70)).ToString()},
                        {"media_folder", "Camera"},
                        {"source_type", "3"},
                        {"video_result", ""},
                        {"_uid", _user.LoggedInUser.Pk.ToString()},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        {"caption", caption ?? string.Empty},
                        {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                        {"capture_type", "normal"},
                        {"mas_opt_in", "NOT_PROMPTED"},
                        {"upload_id", uploadId},
                        {"client_timestamp", DateTime.UtcNow.ToUnixTime()},
                        {
                            "device", new JObject{
                                {"manufacturer", _deviceInfo.HardwareManufacturer},
                                {"model", _deviceInfo.DeviceModelIdentifier},
                                {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                                {"android_version", _deviceInfo.AndroidVer.APILevel}
                            }
                        },
                        {
                            "extra", new JObject
                            {
                                {"source_width", 0},
                                {"source_height", 0}
                            }
                        }
                    };
                    if (!_httpHelper.NewerThan180)
                    {
                        data.Add("_csrftoken", _user.CsrfToken);
                    }
                    if (isDisappearingPhoto)
                    {
                        data.Add("view_mode", viewMode.ToString().ToLower());
                        data.Add("configure_mode", "2");
                        data.Add("recipient_users", "[]");
                        data.Add("thread_ids", $"[{threadId}]");
                    }
                    else
                    {
                        switch (storyType)
                        {
                            case InstaStoryType.SelfStory:
                            default:
                                data.Add("configure_mode", "1");
                                break;
                            case InstaStoryType.Direct:
                                data.Add("configure_mode", "2");
                                data.Add("view_mode", "replayable");
                                data.Add("recipient_users", "[]");
                                data.Add("thread_ids", $"[{threadId}]");
                                break;
                            case InstaStoryType.Both:
                                data.Add("configure_mode", "3");
                                data.Add("view_mode", "replayable");
                                data.Add("recipient_users", "[]");
                                data.Add("thread_ids", $"[{threadId}]");
                                break;
                        }
                    }
                    instaUri = UriCreator.GetVideoStoryConfigureUri(false);
                    var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                    request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                    var response = await _httpRequestProcessor.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var mediaResponse = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                        upProgress.UploadState = InstaUploadState.Configured;
                        progress?.Invoke(upProgress);
                        return mediaResponse.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
                    }
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }
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


        public async Task<IResult<InstaMedia>> SendMediaPhotoAsync(Action<InstaUploaderProgress> progress,
            InstaImageUpload image, string caption, InstaLocationShort location, bool configureAsNameTag = false)
        {
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {

                if (image.UserTags != null && image.UserTags.Any())
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

                var uploadId = ExtensionHelper.GenerateMediaUploadId();
                var photoHashCode = Path.GetFileName(image.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();
                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);
                var waterfallId = Guid.NewGuid().ToString();
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string json = null;
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var photoUploadParamsObj = new JObject
                {
                    {"retry_context", retryContext},
                    {"media_type", "1"},
                    {"upload_id", uploadId},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                    {"xsharing_user_ids", $"[\"{_instaApi.GetLoggedUser().LoggedInUser.Pk}\"]"}
                };

                //var uploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                //request = _httpHelper.GetDefaultRequest(HttpMethod.Get, photoUri, _deviceInfo);
                //request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId);
                //request.Headers.AddHeader("X-Instagram-Rupload-Params", uploadParams);
                //response = await _httpRequestProcessor.SendAsync(request);
                //json = await response.Content.ReadAsStringAsync();


                //if (response.StatusCode != HttpStatusCode.OK)
                //{
                //    upProgress.UploadState = InstaUploadState.Error;
                //    progress?.Invoke(upProgress);
                //    return Result.UnExpectedResponse<InstaMedia>(response, json);
                //}
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                var imageBytes = image.ImageBytes ?? File.ReadAllBytes(image.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);

                //var progressContent = new ProgressableStreamContent(imageContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request.Content = imageContent;
                request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    //upProgress = progressContent.UploaderProgress;
                    upProgress.UploadState = InstaUploadState.Uploaded;
                    progress?.Invoke(upProgress);
                    if (configureAsNameTag)
                        return await ConfigureMediaPhotoAsNametagAsync(progress, upProgress, uploadId);
                    return await ConfigureMediaPhotoAsync(progress, upProgress, uploadId, caption, location, image.UserTags);
                }

                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<InstaMedia>(response, json);
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
        private async Task<IResult<InstaMedia>> ConfigureMediaPhotoAsync(Action<InstaUploaderProgress> progress,
            InstaUploaderProgress upProgress, string uploadId, string caption, InstaLocationShort location, List<InstaUserTagUpload> userTags = null)
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
                var instaUri = UriCreator.GetMediaConfigureUri();
                var retryContext = GetRetryContext();
                var rnd = new Random();
                var spl = new string[] { "Camera", "Telegram", "Instagram", "GZ", "App", "Twitter", "YouTube" };
                var data = new JObject
                {
                    //{"date_time_digitalized", DateTime.UtcNow.ToString("yyyy:MM:dd+hh:mm:ss")},
                    //{"date_time_original", DateTime.UtcNow.ToString("yyyy:MM:dd+hh:mm:ss")},
                    //{"is_suggested_venue", "false"},
                    {"scene_capture_type", ""},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"media_folder", spl[rnd.Next(spl.Length)]},
                    {"source_type", "4"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"caption", caption ?? string.Empty},
                    {"upload_id", uploadId},
                    {"multi_sharing", "1"},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)}
                        }
                    },
                    {
                        "edits", new JObject
                        {
                            {"crop_original_size", new JArray(1033.0, 1280.0)},
                            {"crop_center", new JArray(0.0, -7.8125E-4)},
                            {"crop_zoom", 1.2391094}
                        }
                    },
                    {
                        "extra", new JObject
                        {
                            {"source_width", 0},
                            {"source_height", 0}
                        }
                    }
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (location != null)
                {
                    data.Add("location", location.GetJson());
                }
                if (userTags != null && userTags.Any())
                {
                    var tagArr = new JArray();
                    foreach (var tag in userTags)
                    {
                        if (tag.Pk != -1)
                        {
                            var position = new JArray(tag.X, tag.Y);
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
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }
                var mediaResponse =
                     JsonConvert.DeserializeObject<InstaMediaItemResponse>(json, new InstaMediaDataConverter());
                var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);
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

        private async Task<IResult<InstaMedia>> ConfigureMediaPhotoAsNametagAsync(Action<InstaUploaderProgress> progress,
            InstaUploaderProgress upProgress, string uploadId)
        {
            try
            {
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaNametagConfigureUri();
                var retryContext = GetRetryContext();
                var data = new JObject
                {
                    {"upload_id", uploadId},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                var mediaResponse =
                     JsonConvert.DeserializeObject<InstaMediaItemResponse>(json, new InstaMediaDataConverter());
                var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);
                var obj = converter.Convert();
                //{
                //	"_csrftoken": "5zpWUcNSwJQuYlua9fKDWWXzUhUofqul",
                //	"selfie_sticker": "1",
                //	"_uid": "7405924766",
                //	"mode": "2",
                //	"gradient": "0",
                //	"_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //	"emoji": "😀"
                //} 
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






        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////// DIRECT VOICE ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<IResult<bool>> SendVoiceAsync(Action<InstaUploaderProgress> progress,
            InstaAudioUpload audio,
            string recipients, string threadId)
        {
            var upProgress = new InstaUploaderProgress
            {
                Caption = "Uploading audio",
                UploadState = InstaUploadState.Preparing
            };
            try
            {
                var uploadId = ApiRequestMessage.GenerateUnknownUploadId();
                var videoHashCode = Path.GetFileName(audio.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").GetHashCode();
                var waterfallId = Guid.NewGuid().ToString();
                var videoEntityName = $"{uploadId}_0_{videoHashCode}";
                var videoUri = UriCreator.GetStoryUploadVideoUri(uploadId, videoHashCode.ToString());
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string voiceUploadParams = null;
                string json = null;
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);
                var voiceUploadParamsObj = new JObject();
                //{
                //  "xsharing_user_ids": "[]",
                //  "is_direct_voice": "1",
                //  "upload_media_duration_ms": "2978",
                //  "upload_id": "39826791291317",
                //  "retry_context": "{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}",
                //  "media_type": "11"
                //}


                voiceUploadParamsObj = new JObject
                {
                    {"xsharing_user_ids", "[]"},
                    {"is_direct_voice", "1"},
                    {"upload_media_duration_ms", "0"},
                    {"upload_id", uploadId},
                    {"retry_context", retryContext},
                    {"media_type", "11"}
                };

                voiceUploadParams = JsonConvert.SerializeObject(voiceUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
                request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", voiceUploadParams, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }


                // video part
                byte[] videoBytes;
                if (audio.VoiceBytes == null)
                    videoBytes = File.ReadAllBytes(audio.Uri);
                else
                    videoBytes = audio.VoiceBytes;

                var videoContent = new ByteArrayContent(videoBytes);
                //var progressContent = new ProgressableStreamContent(videoContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
                request.Content = videoContent;
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var vidExt = Path.GetExtension(audio.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
                request.Headers.AddHeader("X-Entity-Type", "audio/mp4", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", voiceUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", videoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", videoBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);
                //upProgress = progressContent?.UploaderProgress;
                return await FinishVoice(progress, upProgress, audio, uploadId, recipients, threadId);
            }
            catch (Exception exception)
            {
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }


        private async Task<IResult<bool>> FinishVoice(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress,
            InstaAudioUpload audio, string uploadId, string recipients, string threadId)
        {
            try
            {
                var instaUri = UriCreator.GetMediaUploadFinishUri();
                var retryContext = GetRetryContext();
                //var pigeonSessionId = Guid.NewGuid().ToString();

                //{
                //  "timezone_offset": "16200",
                //  "_csrftoken": "SAR8V58g7jORGU1bVykRYoxTkKbHNCoN",
                //  "source_type": "4",
                //  "_uid": "8651542203",
                //  "device_id": "android-21c311d494a974fe",
                //  "_uuid": "b2caeea0-e663-42a5-8e64-b0578a6f47b2",
                //  "upload_id": "39826791291317",
                //  "device": {
                //    "manufacturer": "HUAWEI",
                //    "model": "PRA-LA1",
                //    "android_version": 26,
                //    "android_release": "8.0.0"
                //  }
                //}
                var data = new JObject
                {
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"source_type", "4"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"upload_id", uploadId},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber},
                            {"android_version", _deviceInfo.AndroidVer.APILevel}
                        }
                    }
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }

                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                return obj.IsSucceed ? await ConfigureVoice(progress, upProgress, audio, uploadId, recipients, threadId/*, pigeonSessionId*/) : Result.UnExpectedResponse<bool>(response, json);

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





        private async Task<IResult<bool>> ConfigureVoice(Action<InstaUploaderProgress> progress, InstaUploaderProgress upProgress,
            InstaAudioUpload audio, string uploadId, string recipients, string threadId)
        {
            try
            {
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetBroadcastVoiceUri();
                var retryContext = GetRetryContext();
                var clientContext = ExtensionHelper.GetThreadToken();
                var waveformData = audio.WaveformData?.Count > 0 ? string.Join(",", audio.WaveformData) : string.Empty;

                var data = new Dictionary<string, string>
                {
                    {"action", "send_item"},
                    {"client_context", clientContext},
                    {"device_id", _deviceInfo.DeviceId},
                    {"mutation_token", clientContext},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"waveform", $"[{waveformData}]"},
                    {"waveform_sampling_frequency_hz", audio.WaveformSamplingFrequencyHz.ToString()},
                    {"upload_id", uploadId}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }
                if (!string.IsNullOrEmpty(recipients))
                    data.Add("recipient_users", $"[[{recipients}]]");
                else
                    data.Add("thread_ids", $"[{threadId}]");

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<bool>(response, json);
                }
                var obj = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);

                if (obj.IsSucceed)
                {
                    upProgress.UploadState = InstaUploadState.Configured;
                    progress?.Invoke(upProgress);
                }
                else
                {
                    upProgress.UploadState = InstaUploadState.Completed;
                    progress?.Invoke(upProgress);
                }
                return obj.IsSucceed ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);

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





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////// INSTAGRAM TV ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<IResult<InstaMedia>> SendIGTVVideoAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string title, string caption, bool sharePreviewToFeed = false)
        {
            var upProgress = new InstaUploaderProgress
            {
                Caption = caption ?? string.Empty,
                UploadState = InstaUploadState.Preparing
            };
            try
            {

                var uploadId = ApiRequestMessage.GenerateUnknownUploadId();
                var videoHashCode = Path.GetFileName(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").GetHashCode();
                var waterfallId = ApiRequestMessage.GenerateRandomUploadId();//Guid.NewGuid().ToString();
                var videoEntityName = $"{uploadId}_0_{videoHashCode}";
                var videoUri = UriCreator.GetStoryUploadVideoUri(uploadId, videoHashCode.ToString());
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string videoUploadParams = null;
                string json = null;
                upProgress.UploadId = uploadId;
                progress?.Invoke(upProgress);

                var videoUploadParamsObj = new JObject
                {
                    {"is_igtv_video", "1"},
                    {"upload_media_height", video.Video.Height.ToString()},
                    {"xsharing_user_ids", "[]"},
                    {"upload_media_width", video.Video.Width.ToString()},
                    {"upload_media_duration_ms", video.Video.Length.ToString()},
                    {"upload_id", uploadId},
                    {"retry_context", retryContext},
                    {"media_type", "2"},
                };

                videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Get, videoUri, _deviceInfo);
                request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();


                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }


                // video part
                var videoBytes = video.Video.VideoBytes ?? File.ReadAllBytes(video.Video.Uri);

                var videoContent = new ByteArrayContent(videoBytes);
                //var progressContent = new ProgressableStreamContent(videoContent, 4096, progress)
                //{
                //    UploaderProgress = upProgress
                //};
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, videoUri, _deviceInfo);
                request.Content = videoContent;
                upProgress.UploadState = InstaUploadState.Uploading;
                progress?.Invoke(upProgress);
                var vidExt = Path.GetExtension(video.Video.Uri ?? $"C:\\{13.GenerateRandomString()}.mp4").Replace(".", "").ToLower();
                //if (vidExt == "mov")
                //    request.Headers.AddHeader("X-Entity-Type", "video/quicktime", _instaApi);
                //else
                request.Headers.AddHeader("X-Entity-Type", "video/mp4", _instaApi);

                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", videoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", videoBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                //Debug.WriteLine(json);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    upProgress.UploadState = InstaUploadState.Error;
                    progress?.Invoke(upProgress);
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }
                upProgress.UploadState = InstaUploadState.Uploaded;
                progress?.Invoke(upProgress);
                var photoHashCode = Path.GetFileName(video.VideoThumbnail.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();
                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);
                var photoUploadParamsObj = new JObject
                {
                    {"upload_id", uploadId},
                    {"media_type", "2"},
                    {"retry_context", retryContext},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"95\"}"},
                    {"xsharing_user_ids", "[]"}
                };
                upProgress.UploadState = InstaUploadState.UploadingThumbnail;
                progress?.Invoke(upProgress);
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                var imageBytes = video.VideoThumbnail.ImageBytes ?? File.ReadAllBytes(video.VideoThumbnail.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //upProgress = progressContent?.UploaderProgress;
                    upProgress.UploadState = InstaUploadState.ThumbnailUploaded;
                    progress?.Invoke(upProgress);
                    return await ConfigureIGTVVideo(progress, upProgress, uploadId, title, caption, video, sharePreviewToFeed);
                }
                upProgress.UploadState = InstaUploadState.Error;
                progress?.Invoke(upProgress);
                return Result.UnExpectedResponse<InstaMedia>(response, json);
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

        private async Task<IResult<InstaMedia>> ConfigureIGTVVideo(Action<InstaUploaderProgress> progress,
            InstaUploaderProgress upProgress, string uploadId, string title, string caption, InstaVideoUpload video, bool sharePreviewToFeed = false)
        {
            try
            {
                upProgress.UploadState = InstaUploadState.Configuring;
                progress?.Invoke(upProgress);
                try
                {
                    await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaConfigureToIGTVUri();
                var retryContext = GetRetryContext();
                var clientContext = Guid.NewGuid().ToString();

                const int maxRetries = 10;

                for (var i = 0; i < maxRetries; ++i)
                {
                    var rnd = new Random();
                    var data = new JObject
                    {
                        {"filter_type", "0"},
                        {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                        {"source_type", "4"},
                        {"_uid", _user.LoggedInUser.Pk.ToString()},
                        {"device_id", _deviceInfo.DeviceId},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        {"title", title ?? string.Empty },
                        {"caption", caption ?? string.Empty},
                        {"upload_id", uploadId},
                        {
                            "device", new JObject{
                                {"manufacturer", _deviceInfo.HardwareManufacturer},
                                {"model", _deviceInfo.DeviceModelIdentifier},
                                {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)},
                                {"android_release", _deviceInfo.AndroidVer.VersionNumber}
                            }
                        },
                        {"length", video.Video.Length},
                        {
                            "extra", new JObject
                            {
                                {"source_width", video.Video.Width},
                                {"source_height",video.Video.Height}
                            }
                        },
                        {"audio_muted", false},
                        {"poster_frame_index", 0},
                    };
                    if (!_httpHelper.NewerThan180)
                    {
                        data.Add("_csrftoken", _user.CsrfToken);
                    }

                    if (sharePreviewToFeed)
                        data.Add("igtv_share_preview_to_feed", "1");
                    var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                    request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                    var response = await _httpRequestProcessor.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    // igtv:
                    //{"message": "Transcode error: Video's aspect ratio is too large 1.3333333333333", "status": "fail"}
                    //{"message": "Transcode error: Video's aspect ratio is too large 1.7777777777778", "status": "fail"}
                    //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                    //{"media": {"taken_at": 1536588655, "pk": 1865362680669764409, "id": "1865362680669764409_1647718432", "device_timestamp": 153658858130102, "media_type": 2, "code": "BnjGXmWl3s5", "client_cache_key": "MTg2NTM2MjY4MDY2OTc2NDQwOQ==.2", "filter_type": 0, "comment_likes_enabled": false, "comment_threading_enabled": false, "has_more_comments": false, "max_num_visible_preview_comments": 2, "preview_comments": [], "can_view_more_preview_comments": false, "comment_count": 0, "product_type": "igtv", "nearly_complete_copyright_match": false, "image_versions2": {"candidates": [{"width": 1080, "height": 1680, "url": "https://scontent-lga3-1.cdninstagram.com/vp/59b658bc87fac07bfb12fc493d810147/5B990274/t51.2885-15/e35/40958056_2159975094323981_8136119356155744850_n.jpg?se=7\u0026ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}, {"width": 240, "height": 373, "url": "https://scontent-lga3-1.cdninstagram.com/vp/524297318efe8ac05afbe7c267673f33/5B98BF5D/t51.2885-15/e35/p240x240/40958056_2159975094323981_8136119356155744850_n.jpg?ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}]}, "original_width": 1080, "original_height": 1680, "thumbnails": {}, "video_versions": [{"type": 101, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 103, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 102, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}], "has_audio": true, "video_duration": 122.669, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "caption": {"pk": 17977422871018862, "user_id": 1647718432, "text": "captioooooooooooooooooooon", "type": 1, "created_at": 1536588656, "created_at_utc": 1536588656, "content_type": "comment", "status": "Active", "bit_flags": 0, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "did_report_as_spam": false, "media_id": 1865362680669764409}, "title": "ramtin vid e o", "caption_is_edited": false, "photo_of_you": false, "can_viewer_save": true, "organic_tracking_token": "eyJ2ZXJzaW9uIjo1LCJwYXlsb2FkIjp7ImlzX2FuYWx5dGljc190cmFja2VkIjpmYWxzZSwidXVpZCI6IjgyYzkyZjU0Y2EyMzRhNjM5YzBiOTBlZDAzODcwODlhMTg2NTM2MjY4MDY2OTc2NDQwOSIsInNlcnZlcl90b2tlbiI6IjE1MzY1ODg2NTc4Mzd8MTg2NTM2MjY4MDY2OTc2NDQwOXwxNjQ3NzE4NDMyfGMwZjkxNmNmMjk2NTU4NzQ1MWRlZmU3NTY2NjY3ZDdiNjE4OTMxYjM3NTQ0YjdhYjg1NmUxYWEwZjhmMmM4MWIifSwic2lnbmF0dXJlIjoiIn0="}, "upload_id": "153658858130102", "status": "ok"}

                    if (json.Contains("Transcode not finished"))
                    {
                        if (i == maxRetries - 1)
                        {
                            return Result.UnExpectedResponse<InstaMedia>(response, json);
                        }
                        continue;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var mediaResponse =
                                          JsonConvert.DeserializeObject<InstaMediaItemResponse>(json, new InstaMediaDataConverter());
                        var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);
                        upProgress.UploadState = InstaUploadState.Configured;
                        progress?.Invoke(upProgress);
                        var obj = Result.Success(converter.Convert());
                        upProgress.UploadState = InstaUploadState.Completed;
                        progress?.Invoke(upProgress);
                        return obj;
                    }
                    return Result.UnExpectedResponse<InstaMedia>(response, json);
                }

                throw new Exception("unreachable");
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





        public async Task<IResult<InstaMedia>> SendTVVideoAsync(InstaTVVideoUpload video, string title, string caption, bool sharePreviewToFeed = false)
        {
            try
            {
                var uploadId = ExtensionHelper.GenerateUploadingUploadId()/*ApiRequestMessage.GenerateUnknownUploadId() + new Random().Next(1000,9999)*/;

                var randomId = Guid.NewGuid().ToString();

                var waterfallId = ApiRequestMessage.GenerateRandomUploadId();//Guid.NewGuid().ToString();
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string videoUploadParams = null;
                string json = null;

                var instaUri = UriCreator.GetRUploadVideoStartUri(Guid.NewGuid().ToString());

                var videoUploadParamsObj = new JObject
                {
                    {"is_igtv_video", "1"},
                    {"upload_media_height", video.Height.ToString()},
                    {"xsharing_user_ids", "[]"},
                    {"upload_media_width", video.Width.ToString()},
                    {"upload_media_duration_ms", (video.Length * 1000).ToString()},
                    {"upload_id", uploadId},
                    {"retry_context", retryContext},
                    {"media_type", "2"},
                    {"content_tags", "use_default_cover"}
                };

                videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, new Dictionary<string, string>());
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMedia>(response, json);

                var obJResult = JsonConvert.DeserializeObject<InstaRUploadResponse>(json);

                var streamId = obJResult.StreamId;

                try
                {
                    var files = new Dictionary<string, byte[]>();

                    if (!string.IsNullOrEmpty(video.SegmentedFolderPath))
                    {
                        var f = Directory.GetFiles(video.SegmentedFolderPath);
                        if (f.Length > 0)
                        {
                            foreach (var item in f)
                            {
                                var bytes = File.ReadAllBytes(item);
                                files.Add(item, bytes);
                            }
                        }
                    }
                    else
                        files = video.SegmentedFilesBytes;

                    int offset = 0;
                    foreach (var segment in files)
                    {
                        var buffer = segment.Value;

                        var videoContent = new ByteArrayContent(buffer);

                        instaUri = UriCreator.GetRUploadVideoTransferUri(CryptoHelper.CalculateMd5(segment.Key), 0, buffer.Length);

                        request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                        request.Headers.AddHeader("Stream-Id", streamId, _instaApi);
                        request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                        request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                        request.Headers.AddHeader("Segment-Start-Offset", offset.ToString(), _instaApi);
                        request.Headers.AddHeader("Segment-Type", segment.Key.Contains("xaudiox") ? "1" : "2", _instaApi);
                        request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                        response = await _httpRequestProcessor.SendAsync(request);
                        json = await response.Content.ReadAsStringAsync();


                        request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                        request.Headers.AddHeader("X-Entity-Length", buffer.Length.ToString(), _instaApi);
                        request.Headers.AddHeader("X-Entity-Name", $"{CryptoHelper.CalculateMd5(segment.Key)}_0_{buffer.Length}", _instaApi);
                        request.Headers.AddHeader("Stream-Id", streamId, _instaApi);
                        request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                        request.Headers.AddHeader("X-Entity-Type", "video/mp4", _instaApi);
                        request.Headers.AddHeader("Segment-Start-Offset", offset.ToString(), _instaApi);
                        request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                        request.Headers.AddHeader("Segment-Type", segment.Key.Contains("xaudiox") ? "1" : "2", _instaApi);
                        request.Headers.AddHeader("Offset", "0", _instaApi);
                        request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                        request.Content = videoContent;
                        response = await _httpRequestProcessor.SendAsync(request);
                        json = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return Result.UnExpectedResponse<InstaMedia>(response, json);
                        }
                        offset += buffer.Length;
                    }
                }
                catch { }


                instaUri = UriCreator.GetRUploadVideoEndUri(Guid.NewGuid().ToString());

                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, new Dictionary<string, string>());
                request.Headers.AddHeader("Stream-Id", streamId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMedia>(response, json);


                var photoHashCode = Path.GetFileName(video.VideoThumbnail.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();
                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);
                var photoUploadParamsObj = new JObject
                {
                    {"upload_id", uploadId},
                    {"media_type", "2"},
                    {"retry_context", retryContext},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"87\"}"},
                    {"xsharing_user_ids", "[]"}
                };
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                var imageBytes = video.VideoThumbnail.ImageBytes ?? File.ReadAllBytes(video.VideoThumbnail.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return await ConfigureTVVideo(video, uploadId, title, caption);
                }
                return Result.UnExpectedResponse<InstaMedia>(response, json);
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


        private async Task<IResult<InstaMedia>> ConfigureTVVideo(InstaTVVideoUpload video, string uploadId, string title, string caption,
            bool ignoreMediaDelay = false)
        {
            try
            {
                try
                {
                    if (!ignoreMediaDelay)
                        await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaConfigureToIGTVUri();
                var retryContext = GetRetryContext();
                var clientContext = Guid.NewGuid().ToString();

                var rnd = new Random();
                var data = new JObject
                {
                    {"filter_type", "0"},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"source_type", "4"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"title", title ?? string.Empty },
                    {"caption", caption ?? string.Empty},
                    {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                    {"upload_id", uploadId},
                    {"igtv_composer_session_id", Guid.NewGuid().ToString()},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber}
                        }
                    },
                    {"length", video.Length},
                    {
                        "extra", new JObject
                        {
                            {"source_width", video.Width/*videoUploadOption.SourceWidth*/},
                            {"source_height",video.Height /*videoUploadOption.SourceHeight*/}
                        }
                    },
                    {"audio_muted", video.IsMuted},
                    {"poster_frame_index", 0},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }

                if (video.SharePreviewToFeed)
                    data.Add("igtv_share_preview_to_feed", "1");

                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.AddHeader("is_igtv_video", "1", _instaApi);
                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                // igtv:
                //{"message": "Transcode error: Video's aspect ratio is too large 1.3333333333333", "status": "fail"}
                //{"message": "Transcode error: Video's aspect ratio is too large 1.7777777777778", "status": "fail"}
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                //{"media": {"taken_at": 1536588655, "pk": 1865362680669764409, "id": "1865362680669764409_1647718432", "device_timestamp": 153658858130102, "media_type": 2, "code": "BnjGXmWl3s5", "client_cache_key": "MTg2NTM2MjY4MDY2OTc2NDQwOQ==.2", "filter_type": 0, "comment_likes_enabled": false, "comment_threading_enabled": false, "has_more_comments": false, "max_num_visible_preview_comments": 2, "preview_comments": [], "can_view_more_preview_comments": false, "comment_count": 0, "product_type": "igtv", "nearly_complete_copyright_match": false, "image_versions2": {"candidates": [{"width": 1080, "height": 1680, "url": "https://scontent-lga3-1.cdninstagram.com/vp/59b658bc87fac07bfb12fc493d810147/5B990274/t51.2885-15/e35/40958056_2159975094323981_8136119356155744850_n.jpg?se=7\u0026ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}, {"width": 240, "height": 373, "url": "https://scontent-lga3-1.cdninstagram.com/vp/524297318efe8ac05afbe7c267673f33/5B98BF5D/t51.2885-15/e35/p240x240/40958056_2159975094323981_8136119356155744850_n.jpg?ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}]}, "original_width": 1080, "original_height": 1680, "thumbnails": {}, "video_versions": [{"type": 101, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 103, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 102, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}], "has_audio": true, "video_duration": 122.669, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "caption": {"pk": 17977422871018862, "user_id": 1647718432, "text": "captioooooooooooooooooooon", "type": 1, "created_at": 1536588656, "created_at_utc": 1536588656, "content_type": "comment", "status": "Active", "bit_flags": 0, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "did_report_as_spam": false, "media_id": 1865362680669764409}, "title": "ramtin vid e o", "caption_is_edited": false, "photo_of_you": false, "can_viewer_save": true, "organic_tracking_token": "eyJ2ZXJzaW9uIjo1LCJwYXlsb2FkIjp7ImlzX2FuYWx5dGljc190cmFja2VkIjpmYWxzZSwidXVpZCI6IjgyYzkyZjU0Y2EyMzRhNjM5YzBiOTBlZDAzODcwODlhMTg2NTM2MjY4MDY2OTc2NDQwOSIsInNlcnZlcl90b2tlbiI6IjE1MzY1ODg2NTc4Mzd8MTg2NTM2MjY4MDY2OTc2NDQwOXwxNjQ3NzE4NDMyfGMwZjkxNmNmMjk2NTU4NzQ1MWRlZmU3NTY2NjY3ZDdiNjE4OTMxYjM3NTQ0YjdhYjg1NmUxYWEwZjhmMmM4MWIifSwic2lnbmF0dXJlIjoiIn0="}, "upload_id": "153658858130102", "status": "ok"}
                var defResponse = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                if (response.StatusCode == HttpStatusCode.Accepted && defResponse?.Message != null)
                {
                    if (defResponse.Message.ToLower().Contains("transcode not finished yet"))
                    {
                        await Task.Delay(10000);
                        return await ConfigureTVVideo(video, uploadId, title, caption, true);
                    }
                }
                if (response.IsSuccessStatusCode)
                {
                    var mediaResponse = JsonConvert.DeserializeObject<InstaMediaItemResponse>(json, new InstaMediaDataConverter());
                    var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);

                    return Result.Success(converter.Convert());
                }
                return Result.UnExpectedResponse<InstaMedia>(response, json);

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

        public async Task<IResult<InstaMedia>> SendSegmentedVideoAsync(InstaSegmentedVideoUpload video, string caption, InstaLocationShort location = null)
        {
            try
            {
                var uploadId = ExtensionHelper.GenerateUploadingUploadId() /*ApiRequestMessage.GenerateUnknownUploadId() + new Random().Next(1000, 9999)*/;

                var randomId = Guid.NewGuid().ToString();

                var waterfallId = ExtensionHelper.GenerateUploadingUploadId(); //ApiRequestMessage.GenerateRandomUploadId();//Guid.NewGuid().ToString();
                var retryContext = GetRetryContext();
                HttpRequestMessage request = null;
                HttpResponseMessage response = null;
                string videoUploadParams = null;
                string json = null;

                var instaUri = UriCreator.GetRUploadVideoStartUri(Guid.NewGuid().ToString());

                var videoUploadParamsObj = new JObject
                {
                    {"upload_media_height", video.Height.ToString()},
                    {"xsharing_user_ids", "[]"},
                    {"upload_media_width", video.Width.ToString()},
                    {"upload_media_duration_ms", (video.Length * 1000).ToString()},
                    {"upload_id", uploadId},
                    {"retry_context", retryContext},
                    {"media_type", "2"},
                    {"content_tags", "use_default_cover"}
                };

                videoUploadParams = JsonConvert.SerializeObject(videoUploadParamsObj);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, new Dictionary<string, string>());
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMedia>(response, json);

                var obJResult = JsonConvert.DeserializeObject<InstaRUploadResponse>(json);

                var streamId = obJResult.StreamId;

                try
                {
                    var files = new Dictionary<string, byte[]>();

                    if (!string.IsNullOrEmpty(video.SegmentedFolderPath))
                    {
                        var f = Directory.GetFiles(video.SegmentedFolderPath);
                        if (f.Length > 0)
                        {
                            foreach (var item in f)
                            {
                                var bytes = File.ReadAllBytes(item);
                                files.Add(item, bytes);
                            }
                        }
                    }
                    else
                        files = video.SegmentedFilesBytes;

                    int offset = 0;
                    foreach (var segment in files)
                    {
                        var buffer = segment.Value;

                        var videoContent = new ByteArrayContent(buffer);

                        instaUri = UriCreator.GetRUploadVideoTransferUri(CryptoHelper.CalculateMd5(segment.Key), 0, buffer.Length);

                        request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                        request.Headers.AddHeader("Stream-Id", streamId, _instaApi);
                        request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                        request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                        request.Headers.AddHeader("Segment-Start-Offset", offset.ToString(), _instaApi);
                        request.Headers.AddHeader("Segment-Type", segment.Key.Contains("xaudiox") ? "1" : "2", _instaApi);
                        request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                        response = await _httpRequestProcessor.SendAsync(request);
                        json = await response.Content.ReadAsStringAsync();


                        request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                        request.Headers.AddHeader("X-Entity-Length", buffer.Length.ToString(), _instaApi);
                        request.Headers.AddHeader("X-Entity-Name", $"{CryptoHelper.CalculateMd5(segment.Key)}_0_{buffer.Length}", _instaApi);
                        request.Headers.AddHeader("Stream-Id", streamId, _instaApi);
                        request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                        request.Headers.AddHeader("X-Entity-Type", "video/mp4", _instaApi);
                        request.Headers.AddHeader("Segment-Start-Offset", offset.ToString(), _instaApi);
                        request.Headers.AddHeader("X_FB_VIDEO_WATERFALL_ID", waterfallId, _instaApi);
                        request.Headers.AddHeader("Segment-Type", segment.Key.Contains("xaudiox") ? "1" : "2", _instaApi);
                        request.Headers.AddHeader("Offset", "0", _instaApi);
                        request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                        request.Content = videoContent;
                        response = await _httpRequestProcessor.SendAsync(request);
                        json = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return Result.UnExpectedResponse<InstaMedia>(response, json);
                        }
                        offset += buffer.Length;
                    }
                }
                catch { }


                instaUri = UriCreator.GetRUploadVideoEndUri(Guid.NewGuid().ToString());

                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, new Dictionary<string, string>());
                request.Headers.AddHeader("Stream-Id", streamId, _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", videoUploadParams, _instaApi);
                request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMedia>(response, json);


                var photoHashCode = Path.GetFileName(video.VideoThumbnail.Uri ?? $"C:\\{13.GenerateRandomString()}.jpg").GetHashCode();
                var photoEntityName = $"{uploadId}_0_{photoHashCode}";
                var photoUri = UriCreator.GetStoryUploadPhotoUri(uploadId, photoHashCode);

                //X-Instagram-Rupload-Params: {"upload_id":"271400741803376","media_type":"2","retry_context":"{\"num_reupload\":0,\"num_step_auto_retry\":0,\"num_step_manual_retry\":0}","image_compression":"{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"0\"}","xsharing_user_ids":"[\"1647718432\",\"5318277344\",\"8651542203\",\"14742041103\"]"} 
                //,\"quality\":\"0\"} 
                // quality chera sefre?:|
                var photoUploadParamsObj = new JObject
                {
                    { "upload_id", uploadId},
                    {"media_type", "2"},
                    {"retry_context", retryContext},
                    {"image_compression", "{\"lib_name\":\"moz\",\"lib_version\":\"3.1.m\",\"quality\":\"87\"}"},
                    {"xsharing_user_ids", "[]"}
                };
                var photoUploadParams = JsonConvert.SerializeObject(photoUploadParamsObj);
                var imageBytes = video.VideoThumbnail.ImageBytes ?? File.ReadAllBytes(video.VideoThumbnail.Uri);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.AddHeader("Content-Transfer-Encoding", "binary", _instaApi);
                imageContent.Headers.AddHeader("Content-Type", "application/octet-stream", _instaApi);
                request = _httpHelper.GetDefaultRequest(HttpMethod.Post, photoUri, _deviceInfo);
                request.Content = imageContent;
                request.Headers.AddHeader("X-Entity-Type", "image/jpeg", _instaApi);
                request.Headers.AddHeader("Offset", "0", _instaApi);
                request.Headers.AddHeader("X-Instagram-Rupload-Params", photoUploadParams, _instaApi);
                request.Headers.AddHeader("X-Entity-Name", photoEntityName, _instaApi);
                request.Headers.AddHeader("X-Entity-Length", imageBytes.Length.ToString(), _instaApi);
                request.Headers.AddHeader("X_FB_PHOTO_WATERFALL_ID", waterfallId, _instaApi);
                request.Headers.AddHeader(InstaApiConstants.HEADER_PRIORITY, InstaApiConstants.HEADER_PRIORITY_VALUE_6_I, _instaApi, true);
                response = await _httpRequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return await FinishSegmentedVideo(video, uploadId, caption, location);
                }
                return Result.UnExpectedResponse<InstaMedia>(response, json);
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
        private async Task<IResult<InstaMedia>> FinishSegmentedVideo(InstaSegmentedVideoUpload video, string uploadId, string caption, InstaLocationShort location,
   bool ignoreMediaDelay = false)
        {
            try
            {
                try
                {
                    if (!ignoreMediaDelay)
                        await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaUploadFinishVideoUri();
                var retryContext = GetRetryContext();
                var clientContext = Guid.NewGuid().ToString();

                var rnd = new Random();
                var data = new JObject
                {
                    {"filter_type", "0"},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"source_type", "4"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"caption", caption ?? string.Empty},
                    {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                    {"upload_id", uploadId},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber}
                        }
                    },
                    {"length", video.Length},
                    {
                        "extra", new JObject
                        {
                            {"source_width", video.Width/*videoUploadOption.SourceWidth*/},
                            {"source_height",video.Height /*videoUploadOption.SourceHeight*/}
                        }
                    },
                    {"audio_muted", video.IsMuted},
                    {"poster_frame_index", 0},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
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
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var defResponse = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                if (response.StatusCode == HttpStatusCode.Accepted && defResponse?.Message != null)
                {
                    if (defResponse.Message.ToLower().Contains("transcode not finished yet"))
                    {
                        await Task.Delay(10000);
                        return await FinishSegmentedVideo(video, uploadId, caption, location, true);
                    }
                }
                // igtv:
                //{"message": "Transcode error: Video's aspect ratio is too large 1.3333333333333", "status": "fail"}
                //{"message": "Transcode error: Video's aspect ratio is too large 1.7777777777778", "status": "fail"}
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                //{"media": {"taken_at": 1536588655, "pk": 1865362680669764409, "id": "1865362680669764409_1647718432", "device_timestamp": 153658858130102, "media_type": 2, "code": "BnjGXmWl3s5", "client_cache_key": "MTg2NTM2MjY4MDY2OTc2NDQwOQ==.2", "filter_type": 0, "comment_likes_enabled": false, "comment_threading_enabled": false, "has_more_comments": false, "max_num_visible_preview_comments": 2, "preview_comments": [], "can_view_more_preview_comments": false, "comment_count": 0, "product_type": "igtv", "nearly_complete_copyright_match": false, "image_versions2": {"candidates": [{"width": 1080, "height": 1680, "url": "https://scontent-lga3-1.cdninstagram.com/vp/59b658bc87fac07bfb12fc493d810147/5B990274/t51.2885-15/e35/40958056_2159975094323981_8136119356155744850_n.jpg?se=7\u0026ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}, {"width": 240, "height": 373, "url": "https://scontent-lga3-1.cdninstagram.com/vp/524297318efe8ac05afbe7c267673f33/5B98BF5D/t51.2885-15/e35/p240x240/40958056_2159975094323981_8136119356155744850_n.jpg?ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}]}, "original_width": 1080, "original_height": 1680, "thumbnails": {}, "video_versions": [{"type": 101, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 103, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 102, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}], "has_audio": true, "video_duration": 122.669, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "caption": {"pk": 17977422871018862, "user_id": 1647718432, "text": "captioooooooooooooooooooon", "type": 1, "created_at": 1536588656, "created_at_utc": 1536588656, "content_type": "comment", "status": "Active", "bit_flags": 0, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "did_report_as_spam": false, "media_id": 1865362680669764409}, "title": "ramtin vid e o", "caption_is_edited": false, "photo_of_you": false, "can_viewer_save": true, "organic_tracking_token": "eyJ2ZXJzaW9uIjo1LCJwYXlsb2FkIjp7ImlzX2FuYWx5dGljc190cmFja2VkIjpmYWxzZSwidXVpZCI6IjgyYzkyZjU0Y2EyMzRhNjM5YzBiOTBlZDAzODcwODlhMTg2NTM2MjY4MDY2OTc2NDQwOSIsInNlcnZlcl90b2tlbiI6IjE1MzY1ODg2NTc4Mzd8MTg2NTM2MjY4MDY2OTc2NDQwOXwxNjQ3NzE4NDMyfGMwZjkxNmNmMjk2NTU4NzQ1MWRlZmU3NTY2NjY3ZDdiNjE4OTMxYjM3NTQ0YjdhYjg1NmUxYWEwZjhmMmM4MWIifSwic2lnbmF0dXJlIjoiIn0="}, "upload_id": "153658858130102", "status": "ok"}
                return await ConfigureSegmentedVideo(video, uploadId, caption, location, true);
                //if (response.IsSuccessStatusCode)
                //{
                //    var mediaResponse = JsonConvert.DeserializeObject<InstaMediaItemResponse>(json, new InstaMediaDataConverter());
                //    var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);

                //    return Result.Success(converter.Convert());
                //}
                //return Result.UnExpectedResponse<InstaMedia>(response, json);

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
        private async Task<IResult<InstaMedia>> ConfigureSegmentedVideo(InstaSegmentedVideoUpload video, string uploadId, string caption, InstaLocationShort location,
            bool ignoreMediaDelay = false)
        {
            try
            {
                try
                {
                    if (!ignoreMediaDelay)
                        await Task.Delay(_httpRequestProcessor.ConfigureMediaDelay.Value);
                }
                catch { }
                var instaUri = UriCreator.GetMediaConfigureUri(true);
                var retryContext = GetRetryContext();
                var clientContext = Guid.NewGuid().ToString();

                var rnd = new Random();
                var data = new JObject
                {
                    {"filter_type", "0"},
                    {"timezone_offset", _instaApi.TimezoneOffset.ToString()},
                    {"source_type", "4"},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"device_id", _deviceInfo.DeviceId},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"caption", caption ?? string.Empty},
                    {"date_time_original", DateTime.Now.ToString("yyyy-dd-MMTh:mm:ss-0fffZ")},
                    {"upload_id", uploadId},
                    {
                        "device", new JObject{
                            {"manufacturer", _deviceInfo.HardwareManufacturer},
                            {"model", _deviceInfo.DeviceModelIdentifier},
                            {"android_version", int.Parse(_deviceInfo.AndroidVer.APILevel)},
                            {"android_release", _deviceInfo.AndroidVer.VersionNumber}
                        }
                    },
                    {"length", video.Length},
                    {
                        "extra", new JObject
                        {
                            {"source_width", video.Width/*videoUploadOption.SourceWidth*/},
                            {"source_height",video.Height /*videoUploadOption.SourceHeight*/}
                        }
                    },
                    {"audio_muted", video.IsMuted},
                    {"poster_frame_index", 0},
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
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
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);

                request.Headers.AddHeader("retry_context", retryContext, _instaApi);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var defResponse = JsonConvert.DeserializeObject<InstaDefaultResponse>(json);
                if (response.StatusCode == HttpStatusCode.Accepted && defResponse?.Message != null)
                {
                    if (defResponse.Message.ToLower().Contains("transcode not finished yet"))
                    {
                        await Task.Delay(10000);
                        return await ConfigureSegmentedVideo(video, uploadId, caption, location, true);
                    }
                }
                // igtv:
                //{"message": "Transcode error: Video's aspect ratio is too large 1.3333333333333", "status": "fail"}
                //{"message": "Transcode error: Video's aspect ratio is too large 1.7777777777778", "status": "fail"}
                //{"message": "Uploaded image isn't in an allowed aspect ratio", "status": "fail"}
                //{"media": {"taken_at": 1536588655, "pk": 1865362680669764409, "id": "1865362680669764409_1647718432", "device_timestamp": 153658858130102, "media_type": 2, "code": "BnjGXmWl3s5", "client_cache_key": "MTg2NTM2MjY4MDY2OTc2NDQwOQ==.2", "filter_type": 0, "comment_likes_enabled": false, "comment_threading_enabled": false, "has_more_comments": false, "max_num_visible_preview_comments": 2, "preview_comments": [], "can_view_more_preview_comments": false, "comment_count": 0, "product_type": "igtv", "nearly_complete_copyright_match": false, "image_versions2": {"candidates": [{"width": 1080, "height": 1680, "url": "https://scontent-lga3-1.cdninstagram.com/vp/59b658bc87fac07bfb12fc493d810147/5B990274/t51.2885-15/e35/40958056_2159975094323981_8136119356155744850_n.jpg?se=7\u0026ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}, {"width": 240, "height": 373, "url": "https://scontent-lga3-1.cdninstagram.com/vp/524297318efe8ac05afbe7c267673f33/5B98BF5D/t51.2885-15/e35/p240x240/40958056_2159975094323981_8136119356155744850_n.jpg?ig_cache_key=MTg2NTM2MjY4MDY2OTc2NDQwOQ%3D%3D.2"}]}, "original_width": 1080, "original_height": 1680, "thumbnails": {}, "video_versions": [{"type": 101, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 103, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}, {"type": 102, "width": 480, "height": 746, "url": "https://scontent-lga3-1.cdninstagram.com/vp/04d231154d0d1c95289445a348f26bde/5B98AC98/t50.16885-16/10000000_232772710752607_772643665699930112_n.mp4", "id": "17962568050122118"}], "has_audio": true, "video_duration": 122.669, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "caption": {"pk": 17977422871018862, "user_id": 1647718432, "text": "captioooooooooooooooooooon", "type": 1, "created_at": 1536588656, "created_at_utc": 1536588656, "content_type": "comment", "status": "Active", "bit_flags": 0, "user": {"pk": 1647718432, "username": "kajokoleha", "full_name": "kajokoleha", "is_private": false, "profile_pic_url": "https://scontent-lga3-1.cdninstagram.com/vp/82572ce26b79cec0394c295ff1b486b7/5C203459/t51.2885-19/s150x150/29094366_375967546140243_535690319979610112_n.jpg", "profile_pic_id": "1746518311616597634_1647718432", "has_anonymous_profile_picture": false, "can_boost_post": false, "can_see_organic_insights": false, "show_insights_terms": false, "reel_auto_archive": "on", "is_unpublished": false, "allowed_commenter_type": "any"}, "did_report_as_spam": false, "media_id": 1865362680669764409}, "title": "ramtin vid e o", "caption_is_edited": false, "photo_of_you": false, "can_viewer_save": true, "organic_tracking_token": "eyJ2ZXJzaW9uIjo1LCJwYXlsb2FkIjp7ImlzX2FuYWx5dGljc190cmFja2VkIjpmYWxzZSwidXVpZCI6IjgyYzkyZjU0Y2EyMzRhNjM5YzBiOTBlZDAzODcwODlhMTg2NTM2MjY4MDY2OTc2NDQwOSIsInNlcnZlcl90b2tlbiI6IjE1MzY1ODg2NTc4Mzd8MTg2NTM2MjY4MDY2OTc2NDQwOXwxNjQ3NzE4NDMyfGMwZjkxNmNmMjk2NTU4NzQ1MWRlZmU3NTY2NjY3ZDdiNjE4OTMxYjM3NTQ0YjdhYjg1NmUxYWEwZjhmMmM4MWIifSwic2lnbmF0dXJlIjoiIn0="}, "upload_id": "153658858130102", "status": "ok"}

                if (response.IsSuccessStatusCode)
                {
                    var mediaResponse = JsonConvert.DeserializeObject<InstaMediaItemResponse>(json, new InstaMediaDataConverter());
                    var converter = ConvertersFabric.Instance.GetSingleMediaConverter(mediaResponse);

                    return Result.Success(converter.Convert());
                }
                return Result.UnExpectedResponse<InstaMedia>(response, json);

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
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////// OTHER FUNCTIONS /////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<IResult<InstaMediaList>> GetChannelVideosAsync(Uri instaUri, string firstMediaId,
                                        PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            var mediaList = new InstaMediaList();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaMediaList Convert(InstaMediaListResponse mediaListResponse)
                {
                    return ConvertersFabric.Instance.GetMediaListConverter(mediaListResponse).Convert();
                }

                var mediaResult = await GetHashtagChannelVideos(instaUri, paginationParameters, firstMediaId).ConfigureAwait(false);
                if (!mediaResult.Succeeded)
                {
                    if (mediaResult.Value != null)
                        return Result.Fail(mediaResult.Info, Convert(mediaResult.Value));
                    else
                        return Result.Fail(mediaResult.Info, default(InstaMediaList));
                }
                var mediaResponse = mediaResult.Value;
                if (mediaResponse.Medias?.Count > 0)
                    firstMediaId = mediaResponse.Medias[0].InstaIdentifier;

                mediaList = Convert(mediaResponse);
                mediaList.NextMaxId = paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                paginationParameters.PagesLoaded++;

                while (mediaResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextMedia = await GetHashtagChannelVideos(instaUri, paginationParameters, firstMediaId).ConfigureAwait(false);
                    if (!nextMedia.Succeeded)
                        return Result.Fail(nextMedia.Info, mediaList);
                    if (nextMedia.Value.Medias?.Count > 0)
                        firstMediaId = nextMedia.Value.Medias[0].InstaIdentifier;
                    mediaResponse.MoreAvailable = nextMedia.Value.MoreAvailable;
                    mediaResponse.ResultsCount += nextMedia.Value.ResultsCount;
                    mediaList.NextMaxId = mediaResponse.NextMaxId = paginationParameters.NextMaxId = nextMedia.Value.NextMaxId;
                    mediaList.AddRange(Convert(nextMedia.Value));
                    paginationParameters.PagesLoaded++;
                }

                mediaList.Pages = paginationParameters.PagesLoaded;
                mediaList.PageSize = mediaResponse.ResultsCount;
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
        private async Task<IResult<InstaMediaListResponse>> GetHashtagChannelVideos(Uri instaUri,
                                     PaginationParameters paginationParameters, string mediaId)
        {
            try
            {
                var orgItem = new JObject
                {
                    {"id", mediaId},
                };
                if (!string.IsNullOrEmpty(paginationParameters?.NextMaxId))
                    orgItem.Add("index", 30);
                else
                    orgItem.Add("index", 0);

                var jObj = new JObject
                {
                    {"last_organic_item", orgItem}
                };
                if (!string.IsNullOrEmpty(paginationParameters?.NextMaxId))
                    jObj.Add("total_num_items", 31);
                else
                    jObj.Add("total_num_items", 1);

                var data = new Dictionary<string, string>
                {
                    {"phone_id", _deviceInfo.PhoneGuid.ToString()},
                    {"battery_level", "100"},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"is_charging", "0"},
                    {"will_sound_on", "1"},
                    {"rank_token", Guid.NewGuid().ToString()},
                    {"paging_token", jObj.ToString(Formatting.None)}
                };
                if (!_httpHelper.NewerThan180)
                {
                    data.Add("_csrftoken", _user.CsrfToken);
                }

                if (instaUri.ToString().Contains("/tags/"))
                    data.Add("module", "feed_hashtag");
                else if (instaUri.ToString().Contains("/channels/"))
                    data.Add("module", "explore_popular");

                if (!string.IsNullOrEmpty(paginationParameters?.NextMaxId))
                    data.Add("max_id", paginationParameters.NextMaxId);

                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
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
                return Result.Fail(exception, default(InstaMediaListResponse));
            }
        }




        public static string GetRetryContext()
        {
            return new JObject
                {
                    {"num_step_auto_retry", 0},
                    {"num_reupload", 0},
                    {"num_step_manual_retry", 0}
                }.ToString(Formatting.None);
        }
    }
}

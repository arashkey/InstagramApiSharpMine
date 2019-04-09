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

namespace InstagramApiSharp.API.Processors
{
    internal class VideoCallProcessor : IVideoCallProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public VideoCallProcessor(AndroidDevice deviceInfo, UserSessionData user,
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
        ///     1
        /// </summary>
        public async Task<IResult<InstaVideoCallNtpClock>> GetNtpClockAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetVideoCallNtpClockUri();
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaVideoCallNtpClock>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaVideoCallNtpClock>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaVideoCallNtpClock), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaVideoCallNtpClock>(exception);
            }
        }

        /// <summary>
        ///     2 NOT COMPELETE
        /// </summary>
        public async Task<IResult<InstaVideoCallJoin>> JoinAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetVideoCallJoinUri();


                //{
                //  "product_surface_id": "340282366841710300949128321289542314816",
                //  "sdp_offer": "v=0\r\no=- 2739432498127505809 2 IN IP4 127.0.0.1\r\ns=-\r\nt=0 0\r\na=group:BUNDLE audio video\r\na=msid-semantic: WMS 8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\nm=audio 9 RTP/SAVPF 111 103 9 0 8 105 13 110 113 126\r\nc=IN IP4 0.0.0.0\r\na=rtcp:9 IN IP4 0.0.0.0\r\na=ice-ufrag:1mun\r\na=ice-pwd:T+KDeoMos0r+m87Y71KZztzW\r\na=ice-options:trickle renomination\r\na=mid:audio\r\na=extmap:1 urn:ietf:params:rtp-hdrext:ssrc-audio-level\r\na=sendrecv\r\na=rtcp-mux\r\na=crypto:1 AES_CM_128_HMAC_SHA1_80 inline:RvAQhmyXX2X8EFivICqBq/UdTjjZrpAQy025NV3L\r\na=rtpmap:111 opus/48000/2\r\na=rtcp-fb:111 nack\r\na=fmtp:111 minptime=10;useinbandfec=1\r\na=rtpmap:103 ISAC/16000\r\na=rtpmap:9 G722/8000\r\na=rtpmap:0 PCMU/8000\r\na=rtpmap:8 PCMA/8000\r\na=rtpmap:105 CN/16000\r\na=rtpmap:13 CN/8000\r\na=rtpmap:110 telephone-event/48000\r\na=rtpmap:113 telephone-event/16000\r\na=rtpmap:126 telephone-event/8000\r\na=ssrc:1218941199 cname:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:1218941199 msid:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f 8988d982-c44a-4fe7-8e9a-4cc6f5ad66e5\r\na=ssrc:1218941199 mslabel:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:1218941199 label:8988d982-c44a-4fe7-8e9a-4cc6f5ad66e5\r\nm=video 9 RTP/SAVPF 96 97 100 101 102 127\r\nc=IN IP4 0.0.0.0\r\na=rtcp:9 IN IP4 0.0.0.0\r\na=ice-ufrag:1mun\r\na=ice-pwd:T+KDeoMos0r+m87Y71KZztzW\r\na=ice-options:trickle renomination\r\na=mid:video\r\na=extmap:2 urn:ietf:params:rtp-hdrext:toffset\r\na=extmap:3 http://www.webrtc.org/experiments/rtp-hdrext/abs-send-time\r\na=extmap:4 urn:3gpp:video-orientation\r\na=extmap:5 http://www.ietf.org/id/draft-holmer-rmcat-transport-wide-cc-extensions-01\r\na=extmap:6 http://www.webrtc.org/experiments/rtp-hdrext/playout-delay\r\na=extmap:7 http://www.webrtc.org/experiments/rtp-hdrext/video-content-type\r\na=extmap:8 http://www.webrtc.org/experiments/rtp-hdrext/video-timing\r\na=extmap:10 http://tools.ietf.org/html/draft-ietf-avtext-framemarking-07\r\na=sendrecv\r\na=rtcp-mux\r\na=rtcp-rsize\r\na=crypto:1 AES_CM_128_HMAC_SHA1_80 inline:RvAQhmyXX2X8EFivICqBq/UdTjjZrpAQy025NV3L\r\na=rtpmap:96 VP8/90000\r\na=rtcp-fb:96 goog-remb\r\na=rtcp-fb:96 transport-cc\r\na=rtcp-fb:96 ccm fir\r\na=rtcp-fb:96 nack\r\na=rtcp-fb:96 nack pli\r\na=rtpmap:97 rtx/90000\r\na=fmtp:97 apt=96\r\na=rtpmap:100 red/90000\r\na=rtpmap:101 rtx/90000\r\na=fmtp:101 apt=100\r\na=rtpmap:102 ulpfec/90000\r\na=rtpmap:127 flexfec-03/90000\r\na=rtcp-fb:127 goog-remb\r\na=rtcp-fb:127 transport-cc\r\na=fmtp:127 repair-window=10000000\r\na=ssrc-group:FID 1535069016 564613791\r\na=ssrc-group:FEC-FR 1535069016 1152459681\r\na=ssrc:1535069016 cname:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:1535069016 msid:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f 200afb14-de4d-4f06-bf18-3ba59c48ee25\r\na=ssrc:1535069016 mslabel:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:1535069016 label:200afb14-de4d-4f06-bf18-3ba59c48ee25\r\na=ssrc:564613791 cname:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:564613791 msid:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f 200afb14-de4d-4f06-bf18-3ba59c48ee25\r\na=ssrc:564613791 mslabel:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:564613791 label:200afb14-de4d-4f06-bf18-3ba59c48ee25\r\na=ssrc:1152459681 cname:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:1152459681 msid:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f 200afb14-de4d-4f06-bf18-3ba59c48ee25\r\na=ssrc:1152459681 mslabel:8651542203:8a3ff69e-c9d5-44c1-a8e2-4baaa5f92b4f\r\na=ssrc:1152459681 label:200afb14-de4d-4f06-bf18-3ba59c48ee25\r\n",
                //  2taye balaei ro nazashti


                //  "_csrftoken": "SAR8V58g7jORGU1bVykRYoxTkKbHNCoN",
                //  "_uid": "8651542203",
                //  "device_id": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //  "_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //  "product_surface_type": "thread",
                //  "media_status": {
                //    "200afb14-de4d-4f06-bf18-3ba59c48ee25": false,
                //    "8988d982-c44a-4fe7-8e9a-4cc6f5ad66e5": false
                //  }
                //}
                var mediaStatus = new JObject(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"product_surface_type", "thread"},
                    {"media_status", mediaStatus},
                    {"", ""},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaVideoCallJoin>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaVideoCallJoin>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaVideoCallJoin), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaVideoCallJoin>(exception);
            }
        }

        /// <summary>
        ///     3 NOT COMPELETE
        /// </summary>
        public async Task<IResult<bool>> ConfirmAsync(long videoCallId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetVideoCallConfirmUri(videoCallId);
                //{
                //  "message_type": "CONFERENCE_STATE",
                //  "cur_version": "3",
                //  "_csrftoken": "SAR8V58g7jORGU1bVykRYoxTkKbHNCoN",
                //  "_uid": "8651542203",
                //  "device_id": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //  "_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //  "transaction_id": "1534567430476917024"
                //}
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"message_type", "CONFERENCE_STATE"},
                    {"cur_version", "3"},
                    {"transaction_id", ""},//malom nist chie ya chejori mishe addesh kard
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);

                return obj.Status?.ToLower() == "ok" ? Result.Success(true) : Result.Fail<bool>("");
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
        ///     4
        /// </summary>
        public async Task<IResult<InstaVideoCallInfo>> GetInfoAsync(long videoCallId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetVideoCallInfoUri(videoCallId);
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaVideoCallInfo>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaVideoCallInfo>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaVideoCallInfo), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaVideoCallInfo>(exception);
            }
        }

        /// <summary>
        ///     5
        /// </summary>
        public async Task<IResult<InstaVideoCallAdd>> AddVideoCallToDirectAsync(string threadId, long videoCallId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetAddVideoCallToDirectUri(threadId);
                //{
                //  "video_call_id": "18053591845053696",
                //  "_csrftoken": "SAR8V58g7jORGU1bVykRYoxTkKbHNCoN",
                //  "_uid": "8651542203",
                //  "_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876"
                //}
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                    {"video_call_id", videoCallId.ToString()},
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaVideoCallAdd>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaVideoCallAdd>(json);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaVideoCallAdd), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaVideoCallAdd>(exception);
            }
        }
        /// <summary>
        ///     6
        /// </summary>
        public async Task<IResult<bool>> LeaveAsync(long videoCallId)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetVideoCallLeaveUri(videoCallId);
                var data = new JObject
                {
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"device_id", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken}
                };
                var request = _httpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);

                return obj.Status?.ToLower() == "ok" ? Result.Success(true) : Result.Fail<bool>("");
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
    }
}

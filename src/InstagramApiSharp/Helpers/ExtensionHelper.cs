/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Linq;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using InstagramApiSharp.Enums;
using InstagramApiSharp.API.Versions;
using InstagramApiSharp.Helpers;
using System.Text;
using System.Security.Cryptography;
#if NETSTANDARD
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
#endif
namespace InstagramApiSharp
{
    internal static class ExtensionHelper
    {
        public static string GenerateUserAgent(this AndroidDevice deviceInfo, InstaApiVersion apiVersion)
        {
            if (deviceInfo == null)
                return InstaApiConstants.USER_AGENT_DEFAULT;
            if (deviceInfo.AndroidVer == null)
                deviceInfo.AndroidVer = AndroidVersion.GetRandomAndriodVersion();

            var lang = !string.IsNullOrEmpty(InstaApiConstants.ACCEPT_LANGUAGE) ?
                InstaApiConstants.ACCEPT_LANGUAGE.Replace("-", "_") : "en_US";

            var apiLevel = deviceInfo.AndroidVer.APILevel;

            var versionParts = deviceInfo.AndroidVer.VersionNumber.Split('.');
            var versionNumber = deviceInfo.AndroidVer.VersionNumber.Contains(".") ?
                versionParts[0] : deviceInfo.AndroidVer.VersionNumber;

            if (versionParts?.Length > 1)
                if (apiLevel == "25" || apiLevel == "27")
                    versionNumber = $"{versionParts[0]}.{versionParts[1]}";

            return string.Format(InstaApiConstants.USER_AGENT,
                apiVersion.AppVersion,
                apiLevel,
                versionNumber,
                deviceInfo.Dpi,
                deviceInfo.Resolution,
                deviceInfo.HardwareManufacturer,
                deviceInfo.AndroidBoardName,
                deviceInfo.DeviceModelIdentifier,
                deviceInfo.FirmwareBrand,
                deviceInfo.HardwareModel,
                lang,
                apiVersion.AppApiVersionCode);
        }
        public static string GenerateFacebookUserAgent()
        {
            var deviceInfo = AndroidDeviceGenerator.GetRandomAndroidDevice();
            //Mozilla/5.0 (Linux; Android 7.0; PRA-LA1 Build/HONORPRA-LA1; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36

            return string.Format(InstaApiConstants.FACEBOOK_USER_AGENT,
              deviceInfo.AndroidVer.VersionNumber,deviceInfo.DeviceModelIdentifier,
              $"{deviceInfo.AndroidBoardName}{deviceInfo.DeviceModel}");
        }
        public static bool IsEmpty(this string content)
        {
            return string.IsNullOrEmpty(content);
        }
        public static bool IsNotEmpty(this string content)
        {
            return !string.IsNullOrEmpty(content);
        }
        public static string EncodeList(this long[] listOfValues, bool appendQuotation = true)
        {
            return EncodeList(listOfValues.ToList(), appendQuotation);
        }
        public static string EncodeList(this string[] listOfValues, bool appendQuotation = true)
        {
            return EncodeList(listOfValues.ToList(), appendQuotation);
        }
        public static string EncodeList(this List<long> listOfValues, bool appendQuotation = true)
        {
            if (!appendQuotation)
                return string.Join(",", listOfValues);
            var list = new List<string>();
            foreach (var item in listOfValues)
                list.Add(item.Encode());
            return string.Join(",", list);
        }
        public static string EncodeList(this List<string> listOfValues, bool appendQuotation = true)
        {
            if (!appendQuotation)
                return string.Join(",", listOfValues);
            var list = new List<string>();
            foreach (var item in listOfValues)
                list.Add(item.Encode());
            return string.Join(",", list);
        }
        public static string Encode(this long content)
        {
            return content.ToString().Encode();
        }
        public static string Encode(this string content)
        {
            return "\"" + content + "\"";
        }

        public static string EncodeRecipients(this long[] recipients)
        {
            return EncodeRecipients(recipients.ToList());
        }
        public static string EncodeRecipients(this List<long> recipients)
        {
            var list = new List<string>();
            foreach (var item in recipients)
                list.Add($"[{item}]");
            return string.Join(",", list);
        }

        public static string EncodeUri(this string data)
        {
            return System.Net.WebUtility.UrlEncode(data);
        }

        public static string GenerateJazoest(string phoneid)
        {
            int ix = 0;
            var chars = phoneid.ToCharArray();
            foreach (var ch in chars)
                ix += (int)ch;
            return "2" + ix;
        }
#if NETSTANDARD
        static private readonly SecureRandom secureRandom = new SecureRandom();

        public static string GetEncryptedPassword(this IInstaApi api, string password, long? providedTime = null) 
        {
            var pubKey = api.GetLoggedUser().PublicKey;
            var pubKeyId = api.GetLoggedUser().PublicKeyId;
            byte[] randKey = new byte[32];
            byte[] iv = new byte[12];
            secureRandom.NextBytes(randKey, 0, randKey.Length);
            secureRandom.NextBytes(iv, 0, iv.Length);
            long time = providedTime ?? DateTime.UtcNow.ToUnixTime();
            byte[] associatedData = Encoding.UTF8.GetBytes(time.ToString());
            var pubKEY = Encoding.UTF8.GetString(Convert.FromBase64String(pubKey));
            byte[] encryptedKey;
            using (var rdr = PemKeyUtils.GetRSAProviderFromPemString(pubKEY.Trim()))
                encryptedKey = rdr.Encrypt(randKey, false);

            byte[] plaintext = Encoding.UTF8.GetBytes(password);

            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(randKey), 128, iv, associatedData);
            cipher.Init(true, parameters);

            var ciphertext = new byte[cipher.GetOutputSize(plaintext.Length)];
            var len = cipher.ProcessBytes(plaintext, 0, plaintext.Length, ciphertext, 0);
            cipher.DoFinal(ciphertext, len);

            var con = new byte[plaintext.Length];
            for (int i = 0; i < plaintext.Length; i++)
                con[i] = ciphertext[i];
            ciphertext = con;
            var tag = cipher.GetMac();

            byte[] buffersSize = BitConverter.GetBytes(Convert.ToInt16(encryptedKey.Length));
            byte[] encKeyIdBytes = BitConverter.GetBytes(Convert.ToUInt16(pubKeyId));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(encKeyIdBytes);
            encKeyIdBytes[0] = 1; 
            var payload = Convert.ToBase64String(encKeyIdBytes.Concat(iv).Concat(buffersSize).Concat(encryptedKey).Concat(tag).Concat(ciphertext).ToArray());

            return $"#PWD_INSTAGRAM:4:{time}:{payload}";
        }
#elif NET || NETCOREAPP3_1
        private readonly static RNGCryptoServiceProvider RngProvider = new RNGCryptoServiceProvider();
        private readonly static RSACryptoServiceProvider RsaProvider = new RSACryptoServiceProvider();

        public static string GetEncryptedPassword(this IInstaApi api, string password, long? providedTime = null)
        {
            var pubKey = api.GetLoggedUser().PublicKey;
            var pubKeyId = api.GetLoggedUser().PublicKeyId;
            byte[] randKey = new byte[32];
            byte[] iv = new byte[12];
            RngProvider.GetBytes(randKey);
            RngProvider.GetBytes(iv);

            long time = providedTime ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            byte[] pubKEY = Convert.FromBase64String(pubKey);
            string decodedPubKey = Encoding.UTF8.GetString(pubKEY, 0, pubKEY.Length);
            decodedPubKey = decodedPubKey.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");
            byte[] publicKeyBytes = Convert.FromBase64String(decodedPubKey.Trim());
            RsaProvider.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            byte[] encryptedKey = RsaProvider.Encrypt(randKey, false);

            var plaintext = Encoding.UTF8.GetBytes(password);

            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[16];
            byte[] associatedData = Encoding.UTF8.GetBytes(time.ToString());
            AesGcm aesGsm = new AesGcm(randKey);
            aesGsm.Encrypt(iv, plaintext, ciphertext, tag, associatedData);

            byte[] encKeyIdBytes = BitConverter.GetBytes(Convert.ToUInt16(pubKeyId));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(encKeyIdBytes);
            encKeyIdBytes[0] = 1;
            byte[] buffersSize = BitConverter.GetBytes(Convert.ToInt16(encryptedKey.Length));
            var payload = Convert.ToBase64String(encKeyIdBytes.Concat(iv).Concat(buffersSize).Concat(encryptedKey).Concat(tag).Concat(ciphertext).ToArray());

            return $"#PWD_INSTAGRAM:4:{time}:{payload}";
        }
#endif
        public static string GetThreadToken()
        {
            var str = "";
            // 6600286272511816379
            str += Rnd.Next(0, 9);
            str += Rnd.Next(0, 9);
            str += Rnd.Next(0, 9);
            str += Rnd.Next(1000, 9999);
            str += Rnd.Next(11111, 99999);

            str += Rnd.Next(2222, 6789);

            return $"676{str}";
        }

        public static string GetLiveTransactionToken()
        {
            var str = "";
            str += Rnd.Next(0, 9);
            str += Rnd.Next(0, 9);
            str += Rnd.Next(0, 9);
            str += Rnd.Next(0, 9);
            str += Rnd.Next(1000, 9999);
            str += Rnd.Next(11111, 99999);
            str += Rnd.Next(2222, 6789);
            return $"144{str}";
        }
        static public string GenerateMediaUploadId()
        {
            //192855733414842
            string r = "19";
            for (int i = 0; i < 15; i++)
                r += Rnd.Next(0, 9).ToString();
            return r;
        }
        public static string GetJson(this InstaLocationShort location)
        {
            if (location == null)
                return null;

            return new JObject
                            {
                                {"name", location.Address ?? string.Empty},
                                {"address", location.ExternalId ?? string.Empty},
                                {"lat", location.Lat},
                                {"lng", location.Lng},
                                {"external_source", location.ExternalSource ?? "facebook_places"},
                                {"facebook_places_id", location.ExternalId},
                            }.ToString(Formatting.None);
        }

        public static InstaTVChannelType GetChannelType(this string type)
        {
            if(string.IsNullOrEmpty(type))
                return InstaTVChannelType.User;
            switch (type.ToLower())
            {
                case "chrono_following":
                    return InstaTVChannelType.ChronoFollowing;
                case "continue_watching":
                    return InstaTVChannelType.ContinueWatching;
                case "for_you":
                    return InstaTVChannelType.ForYou;
                case "popular":
                    return InstaTVChannelType.Popular;
                default:
                case "user":
                    return InstaTVChannelType.User;
            }
        }
        public static string GetRealChannelType(this InstaTVChannelType type)
        {
            switch(type)
            {
                case InstaTVChannelType.ChronoFollowing:
                    return "chrono_following";
                case InstaTVChannelType.ContinueWatching:
                    return "continue_watching";
                case InstaTVChannelType.Popular:
                    return "popular";
                case InstaTVChannelType.User:
                    return "user";
                case InstaTVChannelType.ForYou:
                default:
                    return "for_you";

            }
        }

        public static string GetContainerType(this InstaMediaContainerModuleType module)
        {
            switch (module)
            {
                case InstaMediaContainerModuleType.FeedContextualCain:
                    return "feed_contextual_chain";
                case InstaMediaContainerModuleType.FeedContextualProfile:
                    return "feed_contextual_profile";
                case InstaMediaContainerModuleType.FeedTimeline:
                    return "feed_timeline";
                case InstaMediaContainerModuleType.IgtvExplorePinnedNav:
                    return "igtv_explore_pinned_nav";
                case InstaMediaContainerModuleType.PhotoViewOther:
                    return "photo_view_other";
                case InstaMediaContainerModuleType.VideoViewOther:
                    return "video_view_other";
                case InstaMediaContainerModuleType.IgtvProfile:
                    return "igtv_profile";
                default:
                case InstaMediaContainerModuleType.None:
                    return string.Empty;
            }
        }

        public static string GetContainerType(this InstaCommentContainerModuleType module)
        {
            switch (module)
            {
                default:
                case InstaCommentContainerModuleType.FeedTimeline:
                    return "comments_v2_feed_timeline";
                case InstaCommentContainerModuleType.FeedContextualProfile:
                    return "comments_v2_feed_contextual_profile";
                case InstaCommentContainerModuleType.FeedContextualChain:
                    return "comments_v2_feed_contextual_chain";
                case InstaCommentContainerModuleType.ExploreEventViewer:
                    return "comments_v2_explore_event_viewer";
                case InstaCommentContainerModuleType.IgtvExploreViewer:
                    return "comments_v2_igtv_viewer";
                case InstaCommentContainerModuleType.IgtvProfile:
                    return "comments_v2_igtv_profile";
                case InstaCommentContainerModuleType.SelfIgtvProfile:
                    return "self_comments_v2_igtv_profile";
                case InstaCommentContainerModuleType.SelfFeedContextualProfile:
                    return "self_comments_v2_feed_contextual_self_profile";
            }
        }

        public static string GetSurfaceType(this InstaMediaSurfaceType surfaceType)
        {
            switch (surfaceType)
            {
                case InstaMediaSurfaceType.FeedContextualCain:
                    return "feed_contextual_chain";
                case InstaMediaSurfaceType.FeedContextualProfile:
                    return "feed_contextual_profile";
                case InstaMediaSurfaceType.IgtvExplorePinnedNav:
                    return "igtv_explore_pinned_nav";
                case InstaMediaSurfaceType.Profile:
                    return "profile";
                default:
                case InstaMediaSurfaceType.None:
                    return string.Empty;
            }
        }
        public static string GetInvetorySourceType(this InstaMediaInventorySource source)
        {
            switch (source)
            {
                case InstaMediaInventorySource.MediaOrAdd:
                    return "media_or_ad";
                case InstaMediaInventorySource.Clips:
                    return "clips";
                default:
                case InstaMediaInventorySource.None:
                    return string.Empty;
            }
        }
        public static string GetChannelDeviceType(this InstaPushChannelType type)
        {
            switch(type)
            {
                default:
                case InstaPushChannelType.Mqtt:
                    return "android_mqtt";
                case InstaPushChannelType.Gcm:
                    return "android_gcm";
            }
        }
        public static string GetRequestSurface(this InstaGiphyRequestType type)
        {
            switch (type)
            {
                default:
                case InstaGiphyRequestType.Direct:
                    return "direct";
                case InstaGiphyRequestType.Story:
                    return "stories_asset_search_tray";
            }
        }
        public static string GetFollowsOrderByType(this InstaFollowOrderType orderBy)
        {
            switch (orderBy)
            {
                case InstaFollowOrderType.DateFollowedEarliest:
                    return "date_followed_earliest";
                case InstaFollowOrderType.DateFollowedLatest:
                    return "date_followed_late";
                case InstaFollowOrderType.Default:
                default:
                    return "default";
            }
        }
        public static string GetPaginationSource(this InstaFeedPaginationSource source) =>
            source == InstaFeedPaginationSource.PastPosts ? "past_posts" : "feed_recs";

        readonly static Random Rnd = new Random();
        public static string GenerateRandomString(this int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[Rnd.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
        
        public static void PrintInDebug(this object obj)
        {
            System.Diagnostics.Debug.WriteLine(Convert.ToString(obj));
        }
        public static string EncodeTime(this TimeSpan span) => $"{span.Hours.ToString("00")}:{span.Minutes.ToString("00")}:{span.Seconds.ToString("00")}";

        public static InstaImageUpload ConvertToImageUpload(this InstaImage instaImage, InstaUserTagUpload[] userTags = null)
        {
            return new InstaImageUpload
            {
                Height = instaImage.Height,
                ImageBytes = instaImage.ImageBytes,
                Uri = instaImage.Uri,
                Width = instaImage.Width,
                UserTags = userTags?.ToList()
            };
        }
        public static InstaComment ConvertToComment(this InstaCommentShort commentShort)
        {
            return new InstaComment
            {
                ContentType = commentShort.ContentType,
                User=  commentShort.User,
                Pk = commentShort.Pk,
                Text = commentShort.Text,
                Type = commentShort.Type,
                CreatedAt = commentShort.CreatedAt,
                CreatedAtUtc = commentShort.CreatedAtUtc,
                HasLikedComment = commentShort.HasLikedComment
            };
        }
        public static JObject ConvertToJson(this InstaStoryPollUpload poll)
        {
            var jArray = new JArray
            {
                new JObject
                {
                    {"text", poll.Answer1},
                    {"count", 0},
                    {"font_size", poll.Answer1FontSize}
                },
                new JObject
                {
                    {"text", poll.Answer2},
                    {"count", 0},
                    {"font_size", poll.Answer2FontSize}
                },
            };

            return new JObject
            {
                {"x", poll.X},
                {"y", poll.Y},
                {"z", poll.Z},
                {"width", poll.Width},
                {"height", poll.Height},
                {"rotation", poll.Rotation},
                {"question", poll.Question},
                {"viewer_vote", 0},
                {"viewer_can_vote", true},
                {"tallies", jArray},
                {"is_shared_result", false},
                {"finished", false},
                {"is_sticker", poll.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaStoryLocationUpload location)
        {
            return new JObject
            {
                {"x", location.X},
                {"y", location.Y},
                {"z", location.Z},
                {"width", location.Width},
                {"height", location.Height},
                {"rotation", location.Rotation},
                {"location_id", location.LocationId},
                {"is_sticker", location.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaStoryHashtagUpload hashtag)
        {
            return new JObject
            {
                {"x", hashtag.X},
                {"y", hashtag.Y},
                {"z", hashtag.Z},
                {"width", hashtag.Width},
                {"height", hashtag.Height},
                {"rotation", hashtag.Rotation},
                {"tag_name", hashtag.TagName},
                {"is_sticker", hashtag.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaStorySliderUpload slider)
        {
            return new JObject
            {
                {"x", slider.X},
                {"y", slider.Y},
                {"z", slider.Z},
                {"width", slider.Width},
                {"height", slider.Height},
                {"rotation", slider.Rotation},
                {"question", slider.Question},
                {"viewer_can_vote", true},
                {"viewer_vote", -1.0},
                {"slider_vote_average", 0.0},
                {"background_color", slider.BackgroundColor},
                {"emoji", $"{slider.Emoji}"},
                {"text_color", slider.TextColor},
                {"is_sticker", slider.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaMediaStoryUpload mediaStory)
        {
            return new JObject
            {
                {"x", mediaStory.X},
                {"y", mediaStory.Y},
                {"width", mediaStory.Width},
                {"height", mediaStory.Height},
                {"rotation", mediaStory.Rotation},
                {"media_id", mediaStory.MediaPk},
                {"is_sticker", mediaStory.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaStoryMentionUpload storyMention)
        {
            return new JObject
            {
                {"x", storyMention.X},
                {"y", storyMention.Y},
                {"z", storyMention.Z},
                {"width", storyMention.Width},
                {"height", storyMention.Height},
                {"rotation", storyMention.Rotation},
                {"user_id", storyMention.Pk}
            };
        }

        public static JObject ConvertToJson(this InstaStoryQuestionUpload question)
        {
            return new JObject
            {
                {"x", question.X},
                {"y", question.Y},
                {"z", question.Z},
                {"width", question.Width},
                {"height", question.Height},
                {"rotation", question.Rotation},
                {"question", question.Question},
                {"viewer_can_interact", question.ViewerCanInteract},
                {"profile_pic_url", question.ProfilePicture},
                {"question_type", question.QuestionType},
                {"background_color", question.BackgroundColor},
                {"text_color", question.TextColor},
                {"is_sticker", question.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaStoryCountdownUpload countdown)
        {
            return new JObject
            {
                {"x", countdown.X},
                {"y", countdown.Y},
                {"z", countdown.Z},
                {"width", countdown.Width},
                {"height", countdown.Height},
                {"rotation", countdown.Rotation},
                {"text", countdown.Text},
                {"start_background_color", countdown.StartBackgroundColor},
                {"end_background_color", countdown.EndBackgroundColor},
                {"digit_color", countdown.DigitColor},
                {"digit_card_color", countdown.DigitCardColor},
                {"end_ts", countdown.EndTime.ToUnixTime()},
                {"text_color", countdown.TextColor},
                {"following_enabled", countdown.FollowingEnabled},
                {"is_sticker", countdown.IsSticker}
            };
        }

        public static JObject ConvertToJson(this InstaStoryQuizUpload quiz)
        {
            var answers = new JArray();
            if (quiz.Options?.Count > 0)
                foreach (var item in quiz.Options)
                    answers.Add(new JObject
                    {
                        {"text", item.Text},
                        {"count", item.Count}
                    });

            return new JObject
            {
                {"x", quiz.X},
                {"y", quiz.Y},
                {"z", quiz.Z},
                {"width", quiz.Width},
                {"height", quiz.Height},
                {"rotation", quiz.Rotation},
                {"question", quiz.Question},
                {"options", answers},
                {"correct_answer", quiz.CorrectAnswer},
                {"viewer_can_answer", quiz.ViewerCanAnswer},
                {"viewer_answer", quiz.ViewerAnswer},
                {"text_color", quiz.TextColor},
                {"start_background_color", quiz.StartBackgroundColor},
                {"end_background_color", quiz.EndBackgroundColor},
                {"is_sticker", quiz.IsSticker},
            };
        }

        public static JObject ConvertToJson(this InstaStoryChatUpload storyChat)
        {
            return new JObject
            {
                {"x", storyChat.X},
                {"y", storyChat.Y},
                {"z", storyChat.Z},
                {"width", storyChat.Width},
                {"height", storyChat.Height},
                {"rotation", storyChat.Rotation},
                {"text", storyChat.GroupName},
                {"start_background_color", storyChat.StartBackgroundColor},
                {"end_background_color", storyChat.EndBackgroundColor},
                {"has_started_chat", storyChat.HasChatStarted},
                {"is_sticker", storyChat.IsSticker}
            };
        }
    }
}

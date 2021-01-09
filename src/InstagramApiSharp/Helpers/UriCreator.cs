﻿using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.Helpers
{
    internal class UriCreator
    {
        private static readonly Uri BaseInstagramUri = new Uri(InstaApiConstants.INSTAGRAM_URL);

        public static Uri GetAcceptFriendshipUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_APPROVE, userId), out var instaUri))
                throw new Exception("Cant create URI for accept friendship");
            return instaUri;
        }

        public static Uri GetAccount2FALoginAgainUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_2FA_LOGIN_AGAIN, out var instaUri))
                throw new Exception("Cant create URI for Account 2FA Login Again");
            return instaUri;
        }

        public static Uri GetAccountGetCommentFilterUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_GET_COMMENT_FILTER, out var instaUri))
                throw new Exception("Cant create URI for accounts get comment filter");
            return instaUri;
        }

        public static Uri GetAccountRecoverPhoneUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_LOOKUP_PHONE, out var instaUri))
                throw new Exception("Cant create URI for Account Recovery phone");
            return instaUri;
        }

        public static Uri GetAccountRecoveryEmailUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SEND_RECOVERY_EMAIL, out var instaUri))
                throw new Exception("Cant create URI for Account Recovery Email");
            return instaUri;
        }

        public static Uri GetAccountSecurityInfoUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SECURITY_INFO, out var instaUri))
                throw new Exception("Cant create URI for accounts security info");
            return instaUri;
        }

        public static Uri GetAccountSendConfirmEmailUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SEND_CONFIRM_EMAIL, out var instaUri))
                throw new Exception("Cant create URI for accounts send confirm email");
            return instaUri;
        }

        public static Uri GetAccountSendSmsCodeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SEND_SMS_CODE, out var instaUri))
                throw new Exception("Cant create URI for accounts send sms code");
            return instaUri;
        }

        public static Uri GetAccountSetPresenseDisabledUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SET_PRESENCE_DISABLED, out var instaUri))
                throw new Exception("Cant create URI for accounts set presence disabled");
            return instaUri;
        }

        public static Uri GetAccountVerifySmsCodeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_VERIFY_SMS_CODE, out var instaUri))
                throw new Exception("Cant create URI for accounts verify sms code");
            return instaUri;
        }

        public static Uri GetAllowMediaCommetsUri(string mediaId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.ALLOW_MEDIA_COMMENTS, mediaId),
                    out var instaUri))
                throw new Exception("Cant create URI to allow comments on media");
            return instaUri;
        }

        public static Uri GetApprovePendingDirectRequestUri(string threadId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_DIRECT_THREAD_APPROVE, threadId),
                    out var instaUri)) throw new Exception("Cant create URI for approve inbox thread");
            return instaUri;
        }

        public static Uri GetApprovePendingMultipleDirectRequestUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_THREAD_APPROVE_MULTIPLE,
                    out var instaUri)) throw new Exception("Cant create URI for approve multiple inbox threads");
            return instaUri;
        }

        public static Uri GetBlockUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_BLOCK_USER, userId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media likers");
            return instaUri;
        }

        public static Uri GetBroadcastAddToPostLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_ADD_TO_POST_LIVE, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast add to post live");
            return instaUri;
        }

        public static Uri GetBroadcastCommentUri(string broadcastId, string lastcommentts = "")
        {
            if(lastcommentts == "")
            {
                if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_COMMENT, broadcastId), out var instaUri))
                    throw new Exception("Cant create URI for broadcast get comments");
                return instaUri;
            }
            else
            {
                if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_COMMENT_LASTCOMMENTTS, broadcastId, lastcommentts), out var instaUri))
                    throw new Exception("Cant create URI for broadcast get comments");
                return instaUri;
            }
        }

        public static Uri GetBroadcastCreateUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.LIVE_CREATE, out var instaUri))
                throw new Exception("Cant create URI for broadcast create");
            return instaUri;
        }

        public static Uri GetBroadcastDeletePostLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_DELETE_POST_LIVE, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast delete post live");
            return instaUri;
        }

        public static Uri GetBroadcastDisableCommenstUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_MUTE_COMMENTS, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast disable comments");
            return instaUri;
        }

        public static Uri GetBroadcastEnableCommenstUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_UNMUTE_COMMENTS, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast enable comments");
            return instaUri;
        }

        public static Uri GetBroadcastEndUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_END, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast end");
            return instaUri;
        }

        public static Uri GetBroadcastInfoUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_INFO, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for get broadcast info");
            return instaUri;
        }

        public static Uri GetBroadcastJoinRequestsUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_JOIN_REQUESTS, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast join requests");
            return instaUri;
        }

        public static Uri GetBroadcastPinCommentUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_PIN_COMMENT, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast pin comment");
            return instaUri;
        }

        public static Uri GetBroadcastPostCommentUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_COMMENT, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast comments");
            return instaUri;
        }

        public static Uri GetBroadcastPostLiveCommentUri(string broadcastId, int startingOffset, string encodingTag)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_POST_LIVE_COMMENT, broadcastId, startingOffset, encodingTag), out var instaUri))
                throw new Exception("Cant create URI for broadcast post live comment");
            return instaUri;
        }

        public static Uri GetBroadcastPostLiveLikesUri(string broadcastId, int startingOffset, string encodingTag)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_POST_LIVE_LIKES, broadcastId, startingOffset, encodingTag), out var instaUri))
                throw new Exception("Cant create URI for broadcast post live likes");
            return instaUri;
        }
        public static Uri GetDirectThreadBroadcastLikeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_THREAD_LIKE, out var instaUri))
                throw new Exception("Cant create URI for broadcast post live likes");
            return instaUri;
        }

        public static Uri GetBroadcastStartUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_START, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast start");
            return instaUri;
        }

        public static Uri GetBroadcastUnPinCommentUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_UNPIN_COMMENT, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast unpin comments");
            return instaUri;
        }

        public static Uri GetBroadcastViewerListUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_VIEWER_LIST, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for get broadcast viewer list");
            return instaUri;
        }

        public static Uri GetBusinessGraphQLUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GRAPH_QL,
                    out var instaUri))
                throw new Exception("Cant create URI for business graph ql");
            return instaUri;
        }

        public static Uri GetBusinessInstantExperienceUri(string data)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.BUSINESS_INSTANT_EXPERIENCE,
                data, InstaApiConstants.IG_SIGNATURE_KEY_VERSION),
                    out var instaUri))
                throw new Exception("Cant create URI for business instant experience");
            return instaUri;
        }

        public static Uri GetBusinessValidateUrlUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.BUSINESS_VALIDATE_URL,
                    out var instaUri))
                throw new Exception("Cant create URI for business validate url");
            return instaUri;
        }

        public static Uri GetChallengeRequireFirstUri(string apiPath, string guid, string deviceId)
        {
            if (!apiPath.EndsWith("/"))
                apiPath += "/";
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.API_SUFFIX + apiPath +
                $"?guid={guid}&device_id={deviceId}", out var instaUri))
                throw new Exception("Cant create URI for challenge require url");
            return instaUri;
        }
        public static Uri GetChallengeRequireFirstUri(string apiPath, Classes.Android.DeviceInfo. AndroidDevice device, string challengeContext)
        {
            if (!apiPath.EndsWith("/"))
                apiPath += "/";
            string context = "";
            if (!string.IsNullOrEmpty(challengeContext))
                context = "&challenge_context=" + challengeContext;
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.API_SUFFIX + apiPath +
                $"?guid={device.PhoneGuid.ToString()}&device_id={device.DeviceGuid.ToString()}" +
                $"&android_device_id={device.DeviceId.ToString()}&phone_id={device.PhoneGuid.ToString()}" +
                $"&_uuid={device.DeviceGuid.ToString()}{context}", out var instaUri))
                throw new Exception("Cant create URI for challenge require url");
            return instaUri;
        }
        public static Uri GetChallengeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CHALLENGE,
                    out var instaUri))
                throw new Exception("Cant create URI for challenge url");
            return instaUri;
        }

        public static Uri GetChallengeRequireUri(string apiPath)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.API_SUFFIX + apiPath, out var instaUri))
                throw new Exception("Cant create URI for challenge require url");
            return instaUri;
        }

        public static Uri GetChallengeReplayUri(string apiPath)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.API_SUFFIX + apiPath.Replace("challenge/", "challenge/replay/"), out var instaUri))
                throw new Exception("Cant create URI for challenge require url");
            return instaUri;
        }

        public static Uri GetChangePasswordUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CHANGE_PASSWORD, out var instaUri))
                throw new Exception("Can't create URI for changing password");
            return instaUri;
        }

        public static Uri GetChangeProfilePictureUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CHANGE_PROFILE_PICTURE, out var instaUri))
                throw new Exception("Cant create URI for change profile picture");
            return instaUri;
        }

        public static Uri GetCheckEmailUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_CHECK_EMAIL, out var instaUri))
                throw new Exception("Cant create URI for check email");
            return instaUri;
        }

        public static Uri GetCheckPhoneNumberUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CHECK_PHONE_NUMBER, out var instaUri))
                throw new Exception("Cant create URI for check phone number");
            return instaUri;
        }

        public static Uri GetCheckUsernameUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_CHECK_USERNAME, out var instaUri))
                throw new Exception("Cant create URI for check username");
            return instaUri;
        }

        public static Uri GetClearSearchHistoryUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FBSEARCH_CLEAR_SEARCH_HISTORY, out var instaUri))
                throw new Exception("Cant create URI for clear search history");
            return instaUri;
        }

        public static Uri GetCollectionsUri(string nextMaxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_LIST_COLLECTIONS,
                out var instaUri))
                throw new Exception("Can't create URI for getting collections");
            return !string.IsNullOrEmpty(nextMaxId) ? new UriBuilder(instaUri) { Query = $"max_id={nextMaxId}" }.Uri : instaUri;
        }

        public static Uri GetCollectionUri(long collectionId, string nextMaxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_COLLECTION, collectionId),
                out var instaUri))
                throw new Exception("Can't create URI for getting collection");
            return !string.IsNullOrEmpty(nextMaxId) ? new UriBuilder(instaUri) { Query = $"max_id={nextMaxId}" }.Uri : instaUri;
        }

        public static Uri GetConsentNewUserFlowBeginsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CONSENT_NEW_USER_FLOW_BEGINS, out var instaUri))
                throw new Exception("Cant create URI for request for consent new user flow begins.");
            return instaUri;
        }

        public static Uri GetConsentNewUserFlowUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CONSENT_NEW_USER_FLOW, out var instaUri))
                throw new Exception("Cant create URI for request for consent new user flow.");
            return instaUri;
        }

        public static Uri GetCreateAccountUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CREATE, out var instaUri))
                throw new Exception("Cant create URI for user creation");
            return instaUri;
        }

        public static Uri GetCreateCollectionUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CREATE_COLLECTION,
                out var instaUri))
                throw new Exception("Can't create URI for creating collection");
            return instaUri;
        }

        public static Uri GetCreateValidatedUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CREATE_VALIDATED, out var instaUri))
                throw new Exception("Cant create URI for accounbts create validated");
            return instaUri;
        }

        public static Uri GetCurrentUserUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CURRENTUSER, out var instaUri))
                throw new Exception("Cant create URI for current user info");
            return instaUri;
        }

        public static Uri GetDeclineAllPendingDirectRequestsUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_THREAD_DECLINEALL,
                    out var instaUri)) throw new Exception("Cant create URI for decline all pending direct requests");
            return instaUri;
        }

        public static Uri GetDeclineMultplePendingDirectRequestsUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_THREAD_DECLINE_MULTIPLE,
                    out var instaUri)) throw new Exception("Cant create URI for decline all pending direct requests");
            return instaUri;
        }

        public static Uri GetDeclinePendingDirectRequestUri(string threadId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_DIRECT_THREAD_DECLINE, threadId),
                    out var instaUri)) throw new Exception("Cant create URI for decline pending direct request");
            return instaUri;
        }

        public static Uri GetDeleteCollectionUri(long collectionId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DELETE_COLLECTION, collectionId),
                out var instaUri))
                throw new Exception("Can't create URI for deleting collection");
            return instaUri;
        }

        public static Uri GetDeleteCommentUri(string mediaId, string commentId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DELETE_COMMENT, mediaId, commentId),
                    out var instaUri))
                throw new Exception("Cant create URI for delete comment");
            return instaUri;
        }

        public static Uri GetDeleteMediaUri(string mediaId, InstaMediaType mediaType)
        {
            var type = "photo";
            switch (mediaType)
            {
                case InstaMediaType.Video:
                    type = "video";
                    break;
                case InstaMediaType.Carousel:
                    type = InstaMediaType.Carousel.ToString();
                    break;
            }
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.DELETE_MEDIA, mediaId, type.ToUpper()), out var instaUri))
                throw new Exception("Can't create URI for deleting media");
            return instaUri;
        }

        public static Uri GetDeleteMultipleCommentsUri(string mediaId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DELETE_MULTIPLE_COMMENT, mediaId),
                    out var instaUri))
                throw new Exception("Cant create URI for delete multiple comments");
            return instaUri;
        }

        public static Uri GetDeleteStoryMediaUri(string mediaId, InstaSharingType mediaType)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.DELETE_MEDIA, mediaId, mediaType.ToString().ToUpper()), out var instaUri))
                throw new Exception("Can't create URI for deleting media story");
            return instaUri;
        }

        public static Uri GetDenyFriendshipUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_IGNORE, userId), out var instaUri))
                throw new Exception("Cant create URI for deny friendship");
            return instaUri;
        }

        public static Uri GetDirectConfigureVideoUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_CONFIGURE_VIDEO, out var instaUri))
                throw new Exception("Cant create URI for direct config video");
            return instaUri;
        }

        public static Uri GetDirectConfigurePhotoUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_CONFIGURE_PHOTO, out var instaUri))
                throw new Exception("Cant create URI for direct config photo");
            return instaUri;
        }

        public static Uri GetDirectInboxThreadUri(string threadId, string NextId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_DIRECT_THREAD, threadId),
                    out var instaUri)) throw new Exception("Cant create URI for get inbox thread by id");
            return !string.IsNullOrEmpty(NextId)
                ? new UriBuilder(instaUri) { Query = $"use_unified_inbox=true&cursor={NextId}&direction=older" }.Uri
                : new UriBuilder(instaUri) { Query = $"use_unified_inbox=true" }.Uri;
        }

        public static Uri GetDirectInboxUri(string nextId = "", int seqId = 0)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_INBOX, out var instaUri))
                throw new Exception("Cant create URI for get inbox");
            return !string.IsNullOrEmpty(nextId)
                ? new UriBuilder(instaUri) { Query = $"visual_message_return_type=unseen&thread_message_limit=10&limit=20&" +
                $"persistentBadging=true&use_unified_inbox=true&cursor={nextId}&direction=older&seq_id={seqId}" }.Uri

                 : new UriBuilder(instaUri) { Query = "visual_message_return_type=unseen&thread_message_limit=10&limit=20&" +
                 "persistentBadging=true&use_unified_inbox=true" }.Uri;
            //: instaUri;
            //        return instaUri
            ////GET /api/v1/direct_v2/inbox/?visual_message_return_type=unseen&persistentBadging=true&use_unified_inbox=true
            //.AddQueryParameterIfNotEmpty("visual_message_return_type", "unseen")
            //.AddQueryParameterIfNotEmpty("persistentBadging", "true")
            //.AddQueryParameterIfNotEmpty("use_unified_inbox", "true")
            //.AddQueryParameterIfNotEmpty("cursor", NextId);
        }

        public static Uri GetDirectPendingInboxUri(string NextId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_PENDING_INBOX, out var instaUri))
                throw new Exception("Cant create URI for get pending inbox");
            return !string.IsNullOrEmpty(NextId)
                ? new UriBuilder(instaUri) { Query = $"cursor={NextId}" }.Uri
                : instaUri;
        }

        public static Uri GetDirectPresenceUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_PRESENCE,
                    out var instaUri))
                throw new Exception("Cant create URI for direct presence");
            return instaUri;
        }

        public static Uri GetDirectSendMessageUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_TEXT_BROADCAST, out var instaUri))
                throw new Exception("Cant create URI for sending message");
            return instaUri;
        }

        public static Uri GetShareLiveToDirectUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_LIVE_VIEWER_INVITE, out var instaUri))
                throw new Exception("Cant create URI for share live to direct");
            return instaUri;
        }

        public static Uri GetDirectSendPhotoUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_UPLOAD_PHOTO, out var instaUri))
                throw new Exception("Cant create URI for sending photo to direct");
            return instaUri;
        }

        public static Uri GetDirectThreadSeenUri(string threadId, string itemId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_SEEN, threadId, itemId), out var instaUri))
                throw new Exception("Cant create URI for seen thread");
            return instaUri;
        }

        public static Uri GetDirectThreadUpdateTitleUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_UPDATE_TITLE, threadId), out var instaUri))
                throw new Exception("Cant create URI for update thread title");
            return instaUri;
        }

        public static Uri GetDisableMediaCommetsUri(string mediaId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DISABLE_MEDIA_COMMENTS, mediaId),
                    out var instaUri))
                throw new Exception("Cant create URI to disable comments on media");
            return instaUri;
        }

        public static Uri GetDisableSmsTwoFactorUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_DISABLE_SMS_TWO_FACTOR, out var instaUri))
                throw new Exception("Cant create URI for disable sms two factor");
            return instaUri;
        }

        public static Uri GetDiscoverChainingUri(long userId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.DISCOVER_CHAINING, userId), out var instaUri))
                throw new Exception("Cant create URI for discover chaining");
            return instaUri;
        }

        public static Uri GetDiscoverPeopleUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_AYML, out var instaUri))
                throw new Exception("Cant create URI for discover people");
            return instaUri;
        }

        public static Uri GetDiscoverSuggestionDetailsUri(long userId, List<long> chainedIds)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DISCOVER_FETCH_SUGGESTION_DETAILS,
                userId, string.Join(",", chainedIds)), out var instaUri))
                throw new Exception("Cant create URI for discover suggestion details");
            return instaUri;
        }

        public static Uri GetDiscoverTopLiveStatusUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_TOP_LIVE_STATUS, out var instaUri))
                throw new Exception("Cant create URI for discover top live status");
            return instaUri;
        }

        public static Uri GetDiscoverTopLiveUri(string maxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_TOP_LIVE, out var instaUri))
                throw new Exception("Cant create URI for discover top live");
            return instaUri.AddQueryParameterIfNotEmpty("max_id", maxId);
        }

        public static Uri GetEditCollectionUri(long collectionId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.EDIT_COLLECTION, collectionId),
                out var instaUri))
                throw new Exception("Can't create URI for editing collection");
            return instaUri;
        }

        public static Uri GetEditMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.EDIT_MEDIA, mediaId),
                out var instaUri))
                throw new Exception("Can't create URI for editing media");
            return instaUri;
        }

        public static Uri GetEditProfileUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_EDIT_PROFILE, out var instaUri))
                throw new Exception("Cant create URI for edit profile");
            return instaUri;
        }

        public static Uri GetEnableSmsTwoFactorUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_ENABLE_SMS_TWO_FACTOR, out var instaUri))
                throw new Exception("Cant create URI for enable sms two factor");
            return instaUri;
        }

        public static Uri GetExploreUri(string maxId = null, string rankToken = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_EXPLORE, out var instaUri))
                throw new Exception("Cant create URI for explore posts");
            var query = $"is_prefetch=false&is_from_promote=true&timezone_offset={InstaApiConstants.TIMEZONE_OFFSET}&supported_capabilities_new={JsonConvert.SerializeObject(InstaApiConstants.SupportedCapabalities)}";
            if (!string.IsNullOrEmpty(maxId)) query += $"&max_id={maxId}&session_id={rankToken}";
            var uriBuilder = new UriBuilder(instaUri) { Query = query };
            return uriBuilder.Uri;
        }

        public static Uri GetFacebookSignUpUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FB_FACEBOOK_SIGNUP, out var instaUri))
                throw new Exception("Cant create URI for facebook sign up url");
            return instaUri;
        }

        public static Uri GetFollowHashtagUri(string hashtag)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_FOLLOW, hashtag),
                    out var instaUri))
                throw new Exception("Cant create URI for follow hashtag");
            return instaUri;
        }

        public static Uri GetFollowingRecentActivityUri(string maxId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_FOLLOWING_RECENT_ACTIVITY, out var instaUri))
                throw new Exception("Cant create URI (get following recent activity");
            var query = string.Empty;
            if (!string.IsNullOrEmpty(maxId)) query += $"max_id={maxId}";
            var uriBuilder = new UriBuilder(instaUri) { Query = query };
            return uriBuilder.Uri;
        }

        public static Uri GetFollowingTagsInfoUri(long userId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USERS_FOLLOWING_TAG_INFO,
                userId), out var instaUri))
                throw new Exception("Cant create URI for suggested tags");
            return instaUri;
        }

        public static Uri GetFollowUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_FOLLOW_USER, userId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media likers");
            return instaUri;
        }

        public static Uri GetFriendshipPendingRequestsUri(/*string rankToken*/)
        {
            if (!Uri.TryCreate(BaseInstagramUri, /*string.Format(*/InstaApiConstants.FRIENDSHIPS_PENDING_REQUESTS/*, rankToken)*/, out var instaUri))
                throw new Exception("Cant create URI for friendship pending requests");
            return instaUri;
        }

        public static Uri GetFriendshipShowManyUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_SHOW_MANY,
                    out var instaUri))
                throw new Exception("Cant create URI for friendship show many");
            return instaUri;
        }

        public static Uri GetFullUserInfoUri(long userId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USERS_FULL_DETAIL_INFO, userId),
                    out var instaUri))
                throw new Exception("Cant create URI for full user info");
            return instaUri;
        }

        public static Uri GetGraphStatisticsUri(string locale, InstaInsightSurfaceType surfaceType = InstaInsightSurfaceType.Account)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GRAPH_QL_STATISTICS, locale, surfaceType.ToString().ToLower()),
                    out var instaUri))
                throw new Exception("Cant create URI for graph ql statistics");
            return instaUri;
        }

        public static Uri GetHashtagRankedMediaUri(string hashtag, string rankToken = null,
            string nextId = null, int? page = null, IEnumerable<long> nextMediaIds = null)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_RANKED, hashtag.EncodeUri()),
                    out var instaUri))
                throw new Exception("Cant create URI for hashtag ranked(top) media");
            if (!string.IsNullOrEmpty(rankToken))
                instaUri = instaUri.AddQueryParameter("rank_token", rankToken);
            if (!string.IsNullOrEmpty(nextId))
            {
                instaUri = instaUri
                    .AddQueryParameter("max_id", nextId);
            }
            if (page != null && page > 0)
            {
                instaUri = instaUri
                    .AddQueryParameter("page", page.ToString());
            }
            if (nextMediaIds != null && nextMediaIds.Any())
            {
                var mediaIds = $"[{string.Join(",", nextMediaIds)}]";
                instaUri = instaUri
                     .AddQueryParameter("next_media_ids", mediaIds.EncodeUri());
            }
            return instaUri;
        }

        public static Uri GetHashtagRecentMediaUri(string hashtag, string rankToken = null,
            string nextId = null, int? page = null, IEnumerable<long> nextMediaIds = null)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_RECENT, hashtag.EncodeUri()),
                    out var instaUri))
                throw new Exception("Cant create URI for hashtag recent media");
            if (!string.IsNullOrEmpty(nextId))
            {
                instaUri = instaUri
                    .AddQueryParameter("max_id", nextId.EncodeUri());
            }
            if (page != null && page > 0)
            {
                instaUri = instaUri
                    .AddQueryParameter("page", page.ToString());
            }
            if (!string.IsNullOrEmpty(rankToken))
            {
                if (rankToken.Contains("_"))
                    instaUri = instaUri.AddQueryParameter("rank_token", rankToken.Split('_')[1]);
                else
                    instaUri = instaUri.AddQueryParameter("rank_token", rankToken);
            }
            if (nextMediaIds != null && nextMediaIds.Any())
            {
                var mediaIds = $"[{string.Join(",", nextMediaIds)}]";
                instaUri = instaUri
                     .AddQueryParameter("next_media_ids", mediaIds.EncodeUri());
            }
            return instaUri;
        }

        public static Uri GetHashtagStoryUri(string hashtag)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_STORY, hashtag),
                    out var instaUri))
                throw new Exception("Cant create URI for hashtag story");
            return instaUri;
        }

        public static Uri GetHighlightCreateUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.HIGHLIGHT_CREATE_REEL, out var instaUri))
                throw new Exception("Cant create URI for highlight create reel");
            return instaUri;
        }

        public static Uri GetHighlightEditUri(string highlightId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.HIGHLIGHT_EDIT_REEL, highlightId), out var instaUri))
                throw new Exception("Cant create URI for highlight edit reel");
            return instaUri;
        }

        public static Uri GetHighlightFeedsUri(long userId, string phoneId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.HIGHLIGHT_TRAY, userId), out var instaUri))
                throw new Exception("Cant create URI for highlight feeds");
            return instaUri
                .AddQueryParameter(InstaApiConstants.SUPPORTED_CAPABALITIES_HEADER, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None))
               .AddQueryParameter("battery_level", "100")
               .AddQueryParameter("is_charging", "0")
               .AddQueryParameter("will_sound_on", "0")
               .AddQueryParameter("phone_id", phoneId);
        }

        public static Uri GetHighlightsArchiveUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ARCHIVE_REEL_DAY_SHELLS,
                out var instaUri))
                throw new Exception("Cant create URI for highlights archive");
            return instaUri;
        }

        public static Uri GetIGTVChannelUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_CHANNEL, out var instaUri))
                throw new Exception("Cant create URI for igtv channel");
            return instaUri;
        }

        public static Uri GetIGTVGuideUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_TV_GUIDE, out var instaUri))
                throw new Exception("Cant create URI for igtv tv guide");
            return instaUri;
        }

        public static Uri GetIGTVSearchUri(string query)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.IGTV_SEARCH, query), out var instaUri))
                throw new Exception("Cant create URI for igtv search");
            return instaUri;
        }

        public static Uri GetIGTVSuggestedSearchesUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_SUGGESTED_SEARCHES, out var instaUri))
                throw new Exception("Cant create URI for igtv suggested searches");
            return instaUri;
        }

        public static Uri GetLeaveThreadUri(string threadId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_LEAVE, threadId),
                    out var instaUri))
                throw new Exception("Cant create URI for leave group thread");
            return instaUri;
        }

        public static Uri GetLikeCommentUri(string commentId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIKE_COMMENT, commentId),
                    out var instaUri))
                throw new Exception("Cant create URI for like comment");
            return instaUri;
        }

        public static Uri GetLikeLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_LIKE, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for like live");
            return instaUri;
        }

        public static Uri GetLikeMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIKE_MEDIA, mediaId),
                out var instaUri))
                throw new Exception("Cant create URI for like media");
            return instaUri;
        }

        public static Uri GetLikeUnlikeDirectMessageUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_REACTION, out var instaUri))
                throw new Exception("Cant create URI for like direct message");
            return instaUri;
        }

        public static Uri GetLiveFinalViewerListUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_FINAL_VIEWER_LIST, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for get final viewer list");
            return instaUri;
        }

        public static Uri GetLiveHeartbeatAndViewerCountUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_HEARTBEAT_AND_GET_VIEWER_COUNT, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for live heartbeat and get viewer count");
            return instaUri;
        }

        public static Uri GetPushRegisterUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.PUSH_REGISTER), out var instaUri))
                throw new Exception("Cant create URI for registering push notification");
            return instaUri;
        }

        public static Uri GetLiveLikeCountUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_LIKE_COUNT, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for live like count");
            return instaUri;
        }

        public static Uri GetLiveNotifyToFriendsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.LIVE_GET_LIVE_PRESENCE, out var instaUri))
                throw new Exception("Cant create URI for get live presence");
            return instaUri;
        }

        public static Uri GetLoginUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_LOGIN, out var instaUri))
                throw new Exception("Cant create URI for user login");
            return instaUri;
        }

        public static Uri GetLogoutUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_LOGOUT, out var instaUri))
                throw new Exception("Cant create URI for user logout");
            return instaUri;
        }

        public static Uri GetMediaAlbumConfigureUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_ALBUM_CONFIGURE, out var instaUri))
                throw new Exception("Cant create URI for configuring media album");
            return instaUri;
        }

        public static Uri GetMediaCommentsMinIdUri(string mediaId, string nextMinId = "", string targetCommentId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_COMMENTS, mediaId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media comments");
            return instaUri
                .AddQueryParameter("can_support_threading", "true")
                .AddQueryParameter("min_id", nextMinId)
                .AddQueryParameter("target_comment_id", targetCommentId);
        }

        public static Uri GetMediaCommentsUri(string mediaId, string nextMaxId = "", string targetCommentId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_COMMENTS, mediaId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media comments");
            return instaUri
                .AddQueryParameter("can_support_threading", "true")
                .AddQueryParameter("max_id", nextMaxId)
                .AddQueryParameter("target_comment_id", targetCommentId);
        }

        public static Uri GetMediaCommetLikersUri(string mediaId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_COMMENT_LIKERS, mediaId),
                    out var instaUri))
                throw new Exception("Cant create URI to media comments likers");
            return instaUri;
        }

        public static Uri GetMediaConfigureToIGTVUri(bool isVideo = true)
        {
            if (!Uri.TryCreate(BaseInstagramUri, isVideo ? InstaApiConstants.MEDIA_CONFIGURE_TO_IGTV_VIDEO : InstaApiConstants.MEDIA_CONFIGURE_TO_IGTV, out var instaUri))
                throw new Exception("Cant create URI for media configure igtv");
            return instaUri;
        }

        public static Uri GetMediaConfigureUri(bool video = false)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, video ? InstaApiConstants.MEDIA_CONFIGURE_VIDEO : InstaApiConstants.MEDIA_CONFIGURE, out var instaUri))
                throw new Exception("Cant create URI for configuring media");
            return instaUri;
        }

        public static Uri GetMediaUploadFinishVideoUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_UPLOAD_FINISH_VIDEO, out var instaUri))
                throw new Exception("Cant create URI for media upload finish video");
            return instaUri;
        }

        public static Uri GetMediaIdFromUrlUri(Uri uri)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_MEDIAID, uri.AbsoluteUri),
                out var instaUri))
                throw new Exception("Can't create URI for getting media id");
            return instaUri;
        }

        public static Uri GetMediaInlineCommentsUri(string mediaId, string targetCommentId, string nextMaxId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_INLINE_COMMENTS, mediaId, targetCommentId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media comments replies with max id");
            return !string.IsNullOrEmpty(nextMaxId)
                //? new UriBuilder(instaUri) { Query = $"min_id={nextId}" }.Uri
                ? new UriBuilder(instaUri) { Query = $"max_id={nextMaxId}" }.Uri
                : instaUri;
        }

        public static Uri GetMediaInlineCommentsWithMinIdUri(string mediaId, string targetCommentId, string nextMinId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_INLINE_COMMENTS, mediaId, targetCommentId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media comment replies with min id");
            return !string.IsNullOrEmpty(nextMinId)
                ? new UriBuilder(instaUri) { Query = $"min_id={nextMinId}" }.Uri
                : instaUri;
        }

        public static Uri GetMediaInsightsUri(string unixTime)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.INSIGHTS_MEDIA, unixTime),
                    out var instaUri))
                throw new Exception("Cant create URI for media insights");
            return instaUri;
        }

        public static Uri GetMediaLikersUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_LIKERS, mediaId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media likers");
            return instaUri;
        }

        public static Uri GetMediaShareUri(InstaMediaType mediaType)
        {
            var type = "photo";
            switch(mediaType)
            {
                case InstaMediaType.Video:
                    type = "video";
                    break;
            }
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_BROADCAST_MEDIA_SHARE, type),
                    out var instaUri))
                throw new Exception("Cant create URI for media share");
            return instaUri;
        }

        public static Uri GetMediaSingleInsightsUri(string mediaPk)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.INSIGHTS_MEDIA_SINGLE, mediaPk,
                InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION),
                    out var instaUri))
                throw new Exception("Cant create URI for single media insights");
            return instaUri;
        }

        public static Uri GetMediaUri(string mediaId)
        {
            return Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_MEDIA, mediaId),
                out var instaUri)
                ? instaUri
                : null;
        }

        public static Uri GetMuteDirectThreadMessagesUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_MESSAGES_MUTE, threadId), out var instaUri))
                throw new Exception("Cant create URI for mute direct thread");
            return instaUri;
        }

        public static Uri GetOnboardingStepsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DYNAMIC_ONBOARDING_GET_STEPS, out var instaUri))
                throw new Exception("Cant create URI for dynamic onboarding get steps");
            return instaUri;
        }

        public static Uri GetParticipantRecipientUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_PARTICIPANTS_RECIPIENT_USER, userId), out var instaUri))
                throw new Exception("Cant create URI for get participants recipient user");
            return instaUri;
        }

        public static Uri GetPostCommetUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.POST_COMMENT, mediaId),
                out var instaUri))
                throw new Exception("Cant create URI for posting comment");
            return instaUri;
        }

        public static Uri GetPostLiveViewersListUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_POST_LIVE_VIEWERS_LIST, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for get post live viewer list");
            return instaUri;
        }

        public static Uri GetProfileSearchUri(string query, int count)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FBSEARCH_PROFILE_SEARCH, query, count), out var instaUri))
                throw new Exception("Cant create URI for profile search");
            return instaUri;
        }

        public static Uri GetProfileSetPhoneAndNameUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SET_PHONE_AND_NAME, out var instaUri))
                throw new Exception("Cant create URI for sets phone and number");
            return instaUri;
        }

        public static Uri GetPromotableMediaFeedsUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FEED_PROMOTABLE_MEDIA,
                    out var instaUri))
                throw new Exception("Cant create URI for promotable media feeds");
            return instaUri;
        }

        public static Uri GetRankedRecipientsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_RANKED_RECIPIENTS, out var instaUri))
                throw new Exception("Cant create URI (get ranked recipients)");
            return instaUri;
        }

        public static Uri GetRankRecipientsByUserUri(string username, string mode = "raven")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_RANK_RECIPIENTS_BY_USERNAME, username, mode), out var instaUri))
                throw new Exception("Cant create URI for get rank recipients by username");
            return instaUri;
        }

        public static Uri GetRecentActivityUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_RECENT_ACTIVITY, out var instaUri))
                throw new Exception("Cant create URI (get recent activity)");
            //var query = $"activity_module=all";
            //var uriBuilder = new UriBuilder(instaUri) { Query = query };
            return instaUri;
        }

        public static Uri GetRecentRecipientsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_RECENT_RECIPIENTS, out var instaUri))
                throw new Exception("Cant create URI (get recent recipients)");
            return instaUri;
        }

        public static Uri GetRecentSearchUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FBSEARCH_RECENT_SEARCHES, out var instaUri))
                throw new Exception("Cant create URI for facebook recent searches");
            return instaUri;
        }

        public static Uri GetReelMediaUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FEED_REEL_MEDIA,
                out var instaUri))
                throw new Exception("Cant create URI for reel media");
            return instaUri;
        }

        public static Uri GetRegenerateTwoFactorBackUpCodeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_REGEN_BACKUP_CODES, out var instaUri))
                throw new Exception("Cant create URI for regenerate two factor backup codes");
            return instaUri;
        }

        public static Uri GetRemoveProfilePictureUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_REMOVE_PROFILE_PICTURE, out var instaUri))
                throw new Exception("Cant create URI for remove profile picture");
            return instaUri;
        }

        public static Uri GetReportCommetUri(string mediaId, string commentId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_REPORT_COMMENT, mediaId, commentId),
                    out var instaUri))
                throw new Exception("Cant create URI for report comment");
            return instaUri;
        }

        public static Uri GetReportMediaUri(string mediaId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_REPORT, mediaId),
                    out var instaUri))
                throw new Exception("Cant create URI for report media");
            return instaUri;
        }

        public static Uri GetReportUserUri(long userId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USERS_REPORT, userId),
                    out var instaUri))
                throw new Exception("Cant create URI for report user");
            return instaUri;
        }

        public static Uri GetRequestForDownloadDataUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DYI_REQUEST_DOWNLOAD_DATA, out var instaUri))
                throw new Exception("Cant create URI for request for download data.");
            return instaUri;
        }

        public static Uri GetRequestForEditProfileUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_REQUEST_PROFILE_EDIT, out var instaUri))
                throw new Exception("Cant create URI for request editing profile");
            return instaUri;
        }

        public static Uri GetResetChallengeRequireUri(string apiPath)
        {
            apiPath = apiPath.Replace("/challenge/", "/challenge/reset/");
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.API_SUFFIX + apiPath, out var instaUri))
                throw new Exception("Cant create URI for challenge require url");
            return instaUri;
        }

        public static Uri GetSearchTagUri(string tag, int count, IEnumerable<long> excludeList, string rankToken)
        {
            excludeList = excludeList ?? new List<long>();
            var excludeListStr = string.Empty;

            if(excludeList?.Count() > 0)
                excludeListStr = $"[{string.Join(",", excludeList)}]";

            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.SEARCH_TAGS, tag, count),
                out var instaUri))
                throw new Exception("Cant create search tag URI");
            return instaUri
                .AddQueryParameter("exclude_list", excludeListStr)
                .AddQueryParameter("rank_token", rankToken)
                .AddQueryParameter(InstaApiConstants.HEADER_TIMEZONE, InstaApiConstants.TIMEZONE_OFFSET.ToString());
        }

        public static Uri GetSearchUserUri(string text, int count, IEnumerable<long> excludeList, string rankToken)
        {
            excludeList = excludeList ?? new List<long>();
            var excludeListStr = string.Empty;

            if (excludeList?.Count() > 0)
            {
                //excludeListStr = $"[{string.Join(",", excludeList)}]";
                var jARR = new JObject
                {
                    {"users", new JArray(excludeList)}
                };
                excludeListStr = jARR.ToString(Formatting.None);
            }
            //TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalSeconds
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USERS_SEARCH,
                InstaApiConstants.TIMEZONE_OFFSET, text, count), out var instaUri))
                throw new Exception("Cant create URI for search user");
            return instaUri
                .AddQueryParameter("exclude_list", excludeListStr)
                .AddQueryParameter("rank_token", rankToken);
        }

        public static Uri GetSeenMediaStoryUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SEEN_MEDIA_STORY, out var instaUri))
                throw new Exception("Cant create URI for seen media story");
            return instaUri;
        }

        public static Uri GetSeenMediaUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SEEN_MEDIA, out var instaUri))
                throw new Exception("Cant create URI for seen media");
            return instaUri;
        }

        public static Uri GetSendDirectLinkUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_LINK, out var instaUri))
                throw new Exception("Cant create URI for send link to direct thread");
            return instaUri;
        }

        public static Uri GetSendDirectLocationUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_LOCATION, out var instaUri))
                throw new Exception("Cant create URI for send location to direct thread");
            return instaUri;
        }

        public static Uri GetSendDirectProfileUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_PROFILE, out var instaUri))
                throw new Exception("Cant create URI for send profile to direct thread");
            return instaUri;
        }

        public static Uri GetSendDirectHashtagUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_HASHTAG, out var instaUri))
                throw new Exception("Cant create URI for send hashtag to direct thread");
            return instaUri;
        }

        public static Uri GetSendTwoFactorEnableSmsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SEND_TWO_FACTOR_ENABLE_SMS, out var instaUri))
                throw new Exception("Cant create URI for send two factor enable sms");
            return instaUri;
        }

        public static Uri GetSetBiographyUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SET_BIOGRAPHY, out var instaUri))
                throw new Exception("Cant create URI for set biography");
            return instaUri;
        }

        public static Uri GetSetBusinessCategoryUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.BUSINESS_SET_CATEGORY,
                    out var instaUri))
                throw new Exception("Cant create URI for set business category");
            return instaUri;
        }

        public static Uri GetSetReelSettingsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_SET_REEL_SETTINGS, out var instaUri))
                throw new Exception("Cant create URI for set reel settings");
            return instaUri;
        }

        public static Uri GetShareLinkFromMediaId(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_SHARE_LINK, mediaId),
                out var instaUri))
                throw new Exception("Can't create URI for getting share link");
            return instaUri;
        }

        public static Uri GetShareUserUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_DIRECT_SHARE_USER, out var instaUri))
                throw new Exception("Cant create URI for share user");
            return instaUri;
        }

        public static Uri GetSignUpSMSCodeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SEND_SIGNUP_SMS_CODE, out var instaUri))
                throw new Exception("Cant create URI for send signup sms code");
            return instaUri;
        }

        public static Uri GetStarThreadUri(string threadId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_STAR, threadId),
                    out var instaUri))
                throw new Exception("Cant create URI for star thread");
            return instaUri;
        }

        public static Uri GetStoryConfigureUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.STORY_CONFIGURE, out var instaUri))
                throw new Exception("Can't create URI for configuring story media");
            return instaUri;
        }

        public static Uri GetStoryFeedUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.GET_STORY_TRAY, out var instaUri))
                throw new Exception("Can't create URI for getting story tray");
            return instaUri;
        }

        public static Uri GetStoryMediaInfoUploadUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.STORY_MEDIA_INFO_UPLOAD, out var instaUri))
                throw new Exception("Cant create URI for story media info");
            return instaUri;
        }

        public static Uri GetStorySettingsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_REEL_SETTINGS, out var instaUri))
                throw new Exception("Cant create URI for story settings");
            return instaUri;
        }

        public static Uri GetStoryShareUri(string mediaType)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.STORY_SHARE, mediaType), out var instaUri))
                throw new Exception("Cant create URI for story share");
            return instaUri;
        }

        public static Uri GetStoryUploadPhotoUri(string uploadId, int fileHashCode)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.UPLOAD_PHOTO, uploadId, fileHashCode), out var instaUri))
                throw new Exception("Cant create URI for story upload photo");
            return instaUri;
        }

        public static Uri GetStoryUploadVideoUri(string uploadId, int fileHashCode)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.UPLOAD_VIDEO, uploadId, fileHashCode), out var instaUri))
                throw new Exception("Cant create URI for story upload video");
            return instaUri;
        }

        public static Uri GetSuggestedBroadcastsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.LIVE_GET_SUGGESTED_BROADCASTS, out var instaUri))
                throw new Exception("Cant create URI for get suggested broadcasts");
            return instaUri;
        }

        public static Uri GetSuggestedSearchUri(InstaDiscoverSearchType searchType)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FBSEARCH_SUGGESTED_SEARCHS, searchType.ToString().ToLower()), out var instaUri))
                throw new Exception("Cant create URI for suggested search");
            return instaUri;
        }
        public static Uri GetTopSearchUri(string rankToken,string querry = "", InstaDiscoverSearchType searchType = InstaDiscoverSearchType.Users, int timezone_offset = 12600)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FBSEARCH_TOPSEARCH_FALT_PARAMETER,rankToken,timezone_offset,querry, searchType.ToString().ToLower()), out var instaUri))
                throw new Exception("Cant create URI for suggested search");
            return instaUri;
        }

        public static Uri GetSuggestedTagsUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.TAG_SUGGESTED,
                    out var instaUri))
                throw new Exception("Cant create URI for suggested tags");
            return instaUri;
        }

        public static Uri GetSyncContactsUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ADDRESSBOOK_LINK,
                    out var instaUri))
                throw new Exception("Cant create URI for sync contacts");
            return instaUri;
        }

        public static Uri GetTagFeedUri(string tag, string maxId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_TAG_FEED, tag), out var instaUri))
                throw new Exception("Cant create URI for discover tag feed");
            return !string.IsNullOrEmpty(maxId)
                ? new UriBuilder(instaUri) { Query = $"max_id={maxId}" }.Uri
                : instaUri;
        }

        public static Uri GetTagInfoUri(string tag)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_TAG_INFO, tag), out var instaUri))
                throw new Exception("Cant create tag info URI");
            return instaUri;
        }

        public static Uri GetTimelineWithMaxIdUri(string nextId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.TIMELINEFEED, out var instaUri))
                throw new Exception("Cant create search URI for timeline");
            var uriBuilder = new UriBuilder(instaUri) { Query = $"max_id={nextId}" };
            return uriBuilder.Uri;
        }

        public static Uri GetTwoFactorLoginUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_2FA_LOGIN, out var instaUri))
                throw new Exception("Cant create URI for user 2FA login");
            return instaUri;
        }

        public static Uri GetUnBlockUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_UNBLOCK_USER, userId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media likers");
            return instaUri;
        }

        public static Uri GetUnFollowHashtagUri(string hashtag)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_UNFOLLOW, hashtag),
                    out var instaUri))
                throw new Exception("Cant create URI for unfollow hashtag");
            return instaUri;
        }

        public static Uri GetUnFollowUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_UNFOLLOW_USER, userId),
                out var instaUri))
                throw new Exception("Cant create URI for getting media likers");
            return instaUri;
        }

        public static Uri GetUnLikeCommentUri(string commentId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.UNLIKE_COMMENT, commentId),
                    out var instaUri))
                throw new Exception("Cant create URI for unlike comment");
            return instaUri;
        }

        public static Uri GetUnLikeMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.UNLIKE_MEDIA, mediaId),
                out var instaUri))
                throw new Exception("Cant create URI for unlike media");
            return instaUri;
        }

        public static Uri GetUnMuteDirectThreadMessagesUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_MESSAGES_UNMUTE, threadId), out var instaUri))
                throw new Exception("Cant create URI for unmute direct thread");
            return instaUri;
        }

        public static Uri GetUnStarThreadUri(string threadId)
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_UNSTAR, threadId),
                    out var instaUri))
                throw new Exception("Cant create URI for unstar thread");
            return instaUri;
        }

        public static Uri GetUpdateBusinessInfoUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_UPDATE_BUSINESS_INFO,
                    out var instaUri))
                throw new Exception("Cant create URI for update business info");
            return instaUri;
        }

        public static Uri GetUploadPhotoUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.UPLOAD_PHOTO_OLD, out var instaUri))
                throw new Exception("Cant create URI for upload photo");
            return instaUri;
        }

        public static Uri GetUploadVideoUri()
        {
            if (
                !Uri.TryCreate(BaseInstagramUri, InstaApiConstants.UPLOAD_VIDEO_OLD, out var instaUri))
                throw new Exception("Cant create URI for upload video");
            return instaUri;
        }

        public static Uri GetUriSetAccountPrivate()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SET_ACCOUNT_PRIVATE, out var instaUri))
                throw new Exception("Cant create URI for set account private");
            return instaUri;
        }

        public static Uri GetUriSetAccountPublic()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SET_ACCOUNT_PUBLIC, out var instaUri))
                throw new Exception("Cant create URI for set account public");
            return instaUri;
        }

        public static Uri GetUserFeedUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.TIMELINEFEED, out var instaUri))
                throw new Exception("Cant create timeline feed URI");
            return instaUri;
        }

        public static Uri GetUserFollowersUri(long userPk, string rankToken, string searchQuery, bool mutualsfirst = false, 
            string maxId = "", InstaFollowOrderType orderBy = InstaFollowOrderType.Default)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_USER_FOLLOWERS, userPk),
                 out Uri instaUri))
                throw new Exception("Cant create URI for user followers");
            return instaUri
                .AddQueryParameterIfNotEmpty("search_surface", "follow_list_page")
                .AddQueryParameterIfNotEmpty("order", orderBy.GetFollowsOrderByType())
                .AddQueryParameter("query", searchQuery, true)
                .AddQueryParameterIfNotEmpty("enable_groups", "true")
                .AddQueryParameterIfNotEmpty("rank_token", rankToken)
                .AddQueryParameterIfNotEmpty("rank_mutual", mutualsfirst ? "1" : null)
                .AddQueryParameterIfNotEmpty("max_id", maxId);
        }

        public static Uri GetUserFollowingUri(long userPk, string rankToken, string searchQuery, string maxId = "", InstaFollowOrderType orderBy = InstaFollowOrderType.Default)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_USER_FOLLOWING, userPk),
                out var instaUri))
                throw new Exception("Cant create URI for user following");
            return instaUri
                .AddQueryParameterIfNotEmpty("search_surface", "follow_list_page")
                .AddQueryParameter("query", searchQuery, true)
                .AddQueryParameterIfNotEmpty("enable_groups", "true")
                .AddQueryParameter("includes_hashtags", orderBy == InstaFollowOrderType.Default ? "true" : null)
                .AddQueryParameterIfNotEmpty("max_id", maxId)
                .AddQueryParameterIfNotEmpty("order", orderBy == InstaFollowOrderType.Default ? null : orderBy.GetFollowsOrderByType())
                .AddQueryParameterIfNotEmpty("rank_token", rankToken);
        }

        public static Uri GetUserFriendshipUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Concat(InstaApiConstants.FRIENDSHIPSTATUS, userId, "/"),
                out var instaUri))
                throw new Exception("Can't create URI for getting friendship status");
            return instaUri;
        }

        public static Uri GetUserInfoByIdUri(long pk)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_USER_INFO_BY_ID, pk), out var instaUri))
                throw new Exception("Cant create user info by identifier URI");
            return instaUri;
        }

        public static Uri GetUserInfoByUsernameUri(string username)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_USER_INFO_BY_USERNAME, username), out var instaUri))
                throw new Exception("Cant create user info by username URI");
            return instaUri;
        }

        public static Uri GetUserLikeFeedUri(string maxId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.LIKE_FEED, out var instaUri))
                throw new Exception("Can't create URI for getting like feed");
            var query = string.Empty;
            if (!string.IsNullOrEmpty(maxId)) query += $"max_id={maxId}";
            var uriBuilder = new UriBuilder(instaUri) { Query = query };
            return uriBuilder.Uri;
        }

        public static Uri GetUserMediaListUri(long userPk, string nextId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"{InstaApiConstants.USEREFEED}{userPk}/", out var instaUri))
                throw new Exception("Cant create URI for user media retrieval");
            return /*!string.IsNullOrEmpty(nextId)
                ? new UriBuilder(instaUri) { Query = $"max_id={nextId}" }.Uri
                : */ instaUri
                .AddQueryParameter("exclude_comment", "false")
                .AddQueryParameter("max_id", nextId)
                .AddQueryParameter("only_fetch_first_carousel_media", "false");
        }

        public static Uri GetArchivedMediaFeedsListUri(string nextId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FEED_ONLY_ME_FEED, out var instaUri))
                throw new Exception("Cant create URI for arhcived media feeds");
            return !string.IsNullOrEmpty(nextId)
                ? new UriBuilder(instaUri) { Query = $"max_id={nextId}" }.Uri
                : instaUri;
        }

        public static Uri GetUsernameSuggestionsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_USERNAME_SUGGESTIONS, out var instaUri))
                throw new Exception("Cant create URI for username suggestions");
            return instaUri;
        }

        public static Uri GetUserReelFeedUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USER_REEL_FEED, userId),
                out var instaUri))
                throw new Exception("Can't create URI for getting user reel feed");
            return instaUri;
        }

        public static Uri GetUserSearchByLocationUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FBSEARCH_TOPSEARCH_FALT, out var instaUri))
                throw new Exception("Cant create URI for user search by location");
            return instaUri;
        }

        public static Uri GetUserStoryUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_USER_STORY, userId),
                out var instaUri))
                throw new Exception("Can't create URI for getting user's story");
            return instaUri;
        }

        public static Uri GetUserTagsUri(long userPk, string rankToken, string maxId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_USER_TAGS, userPk),
                out var instaUri))
                throw new Exception("Cant create URI for get user tags");
            var query = $"rank_token={rankToken}&ranked_content=true";
            if (!string.IsNullOrEmpty(maxId)) query += $"&max_id={maxId}";
            var uriBuilder = new UriBuilder(instaUri) { Query = query };
            return uriBuilder.Uri;
        }

        public static Uri GetUserUri(string username)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SEARCH_USERS, out var instaUri))
                throw new Exception("Cant create search user URI");
            var userUriBuilder = new UriBuilder(instaUri) { Query = $"q={username}" };
            return userUriBuilder.Uri;
        }
        public static Uri GetValidateReelLinkAddressUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_VALIDATE_REEL_URL, out var instaUri))
                throw new Exception("Cant create URI for request for validate reel url");
            return instaUri;
        }

        public static Uri GetValidateSignUpSMSCodeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_VALIDATE_SIGNUP_SMS_CODE, out var instaUri))
                throw new Exception("Cant create URI for validate signup sms code");
            return instaUri;
        }

        public static Uri GetVideoStoryConfigureUri(bool isVideo = false)
        {
            if (!Uri.TryCreate(BaseInstagramUri, isVideo ? InstaApiConstants.STORY_CONFIGURE_VIDEO : InstaApiConstants.STORY_CONFIGURE_VIDEO2, out var instaUri))
                throw new Exception("Can't create URI for configuring story media");
            return instaUri;
        }
        public static Uri GetAddUserToDirectThreadUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_ADD_USER, threadId), out var instaUri))
                throw new Exception("Cant create URI for add users to direct thread");
            return instaUri;
        }

        public static Uri GetBusinessBrandedSettingsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.BUSINESS_BRANDED_GET_SETTINGS, out var instaUri))
                throw new Exception("Cant create URI for business branded settings");
            return instaUri;
        }

        public static Uri GetBusinessBrandedSearchUserUri(string query, int count)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.BUSINESS_BRANDED_USER_SEARCH, query, count), out var instaUri))
                throw new Exception("Cant create URI for business branded user search");
            return instaUri;
        }

        public static Uri GetBusinessBrandedUpdateSettingsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.BUSINESS_BRANDED_UPDATE_SETTINGS, out var instaUri))
                throw new Exception("Cant create URI for business branded update settings");
            return instaUri;
        }

        public static Uri GetMediaNametagConfigureUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_CONFIGURE_NAMETAG, out var instaUri))
                throw new Exception("Cant create URI for media nametag configure");
            return instaUri;
        }

        public static Uri GetUsersNametagLookupUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_NAMETAG_LOOKUP, out var instaUri))
                throw new Exception("Cant create URI for users nametag lookup");
            return instaUri;
        }

        public static Uri GetUsersNametagConfigUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_NAMETAG_CONFIG, out var instaUri))
                throw new Exception("Cant create URI for users nametag config");
            return instaUri;
        }

        public static Uri GetRemoveFollowerUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.FRIENDSHIPS_REMOVE_FOLLOWER, userId), out var instaUri))
                throw new Exception("Cant create URI for remove follower");
            return instaUri;
        }

        public static Uri GetTranslateBiographyUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.LANGUAGE_TRANSLATE, userId), out var instaUri))
                throw new Exception("Cant create URI for translate bio");
            return instaUri;
        }

        public static Uri GetTranslateCommentsUri(string commendIds)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.LANGUAGE_TRANSLATE_COMMENT, commendIds), out var instaUri))
                throw new Exception("Cant create URI for translate comments");
            return instaUri;
        }

        public static Uri GetSearchPlacesUri(string query, string rankToken, List<long> excludeList, double? lat = null, double? lng = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FBSEARCH_PLACES, out var instaUri))
                throw new Exception("Cant create URI for search places");

            var parameters = $"timezone_offset={InstaApiConstants.TIMEZONE_OFFSET}&";

            if(lat!= null && lng != null)
                parameters += $"lat={lat.Value.ToString(CultureInfo.InvariantCulture)}&lng={lng.Value.ToString(CultureInfo.InvariantCulture)}";

            if (!string.IsNullOrEmpty(query))
                parameters += $"&query={query}";

            if (!string.IsNullOrEmpty(rankToken))
                parameters += $"&rank_token={rankToken}";

            if (excludeList?.Count > 0)
                parameters += $"&exclude_list=[{string.Join(",", excludeList)}]";

            return new UriBuilder(instaUri) { Query = parameters }.Uri;
        }

        public static Uri GetBroadcastReelShareUri(InstaSharingType sharingType)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_BROADCAST_REEL_SHARE,
                sharingType.ToString().ToLower()), out var instaUri))
                throw new Exception("Cant create URI for direct broadcast reel share");
            return instaUri;
        }

        public static Uri GetUserShoppableMediaListUri(long userPk, string nextId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USER_SHOPPABLE_MEDIA, userPk), out var instaUri))
                throw new Exception("Cant create URI for user shoppable media");
            return !string.IsNullOrEmpty(nextId)
                ? new UriBuilder(instaUri) { Query = $"max_id={nextId}" }.Uri
                : instaUri;
        }

        public static Uri GetProductInfoUri(long productId, string mediaPk, int deviceWidth)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.COMMERCE_PRODUCT_INFO,
                productId, mediaPk, deviceWidth), out var instaUri))
                throw new Exception("Cant create URI for product info");
            return instaUri;
        }

        public static Uri GetMarkUserOverageUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_MARK_USER_OVERAGE, userId), out var instaUri))
                throw new Exception("Cant create URI for mark user overage");
            return instaUri;
        }

        public static Uri GetFavoriteUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_FAVORITE, userId), out var instaUri))
                throw new Exception("Cant create URI for favorite user");
            return instaUri;
        }

        public static Uri GetUnFavoriteUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_UNFAVORITE, userId), out var instaUri))
                throw new Exception("Cant create URI for unfavorite user");
            return instaUri;
        }

        public static Uri GetFavoriteForUserStoriesUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_FAVORITE_FOR_STORIES, userId), out var instaUri))
                throw new Exception("Cant create URI for favorite user stories");
            return instaUri;
        }

        public static Uri GetUnFavoriteForUserStoriesUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_UNFAVORITE_FOR_STORIES, userId), out var instaUri))
                throw new Exception("Cant create URI for unfavorite user stories");
            return instaUri;
        }

        public static Uri GetMuteUserMediaStoryUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_MUTE_POST_STORY, out var instaUri))
                throw new Exception("Cant create URI for mute user media or story");
            return instaUri;
        }

        public static Uri GetUnMuteUserMediaStoryUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_UNMUTE_POST_STORY, out var instaUri))
                throw new Exception("Cant create URI for unmute user media or story");
            return instaUri;
        }

        public static Uri GetHideMyStoryFromUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_BLOCK_FRIEND_REEL, userId),
                out var instaUri))
                throw new Exception("Cant create URI for hide my story from specific user");
            return instaUri;
        }

        public static Uri GetUnHideMyStoryFromUserUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_UNBLOCK_FRIEND_REEL, userId),
                out var instaUri))
                throw new Exception("Cant create URI for unhide my story from from specific user");
            return instaUri;
        }

        public static Uri GetMuteFriendStoryUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_MUTE_FRIEND_REEL, userId),
                out var instaUri))
                throw new Exception("Cant create URI for mute friend story");
            return instaUri;
        }

        public static Uri GetUnMuteFriendStoryUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FRIENDSHIPS_UNMUTE_FRIEND_REEL, userId),
                out var instaUri))
                throw new Exception("Cant create URI for unmute friend story");
            return instaUri;
        }

        public static Uri GetBlockedStoriesUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_BLOCKED_REEL, out var instaUri))
                throw new Exception("Cant create URI for blocked stories");
            return instaUri;
        }


        public static Uri GetVerifyEmailUri(Uri uri)
        {
            var u = uri.ToString();
            if (u.Contains("?"))
                u = u.Substring(0, u.IndexOf("?"));
            u = u.Substring(u.IndexOf("/accounts/"));

            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.API_SUFFIX + u, out var instaUri))
                throw new Exception("Cant create URI for verify email");
            return instaUri;
        }

        public static Uri GetHideDirectThreadUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_HIDE, threadId),
                out var instaUri))
                throw new Exception("Cant create URI for hide direct thread");
            return instaUri;
        }
        public static Uri GetDeleteDirectMessageUri(string threadId, string itemId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_DELETE_MESSAGE, threadId, itemId),
                out var instaUri))
                throw new Exception("Cant create URI for delete direct message");
            return instaUri;
        }

        public static Uri GetLocationInfoUri(string externalId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LOCATIONS_INFO, externalId),
                out var instaUri))
                throw new Exception("Cant create URI for location info");
            return instaUri;
        }

        public static Uri GetStoryMediaViewersUri(string storyMediaId, string nextId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_STORY_VIEWERS, storyMediaId), out var instaUri))
                throw new Exception("Cant create URI for story media viewers");
            return !string.IsNullOrEmpty(nextId)
                ? new UriBuilder(instaUri) { Query = $"max_id={nextId}" }.Uri
                : instaUri;
        }

        public static Uri GetBlockedMediaUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_BLOCKED, out var instaUri))
                throw new Exception("Cant create URI for blocked media");
            return instaUri;
        }

        public static Uri GetMediaInfoByMultipleMediaIdsUri(string[] mediaIds, string uuid, string csrfToken)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_INFOS, 
                uuid, csrfToken, string.Join("," , mediaIds)), out var instaUri))
                throw new Exception("Cant create URI for media info by multiple media ids");
            return instaUri;
        }

        public static Uri GetBlockedUsersUri(string maxId = "")
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_BLOCKED_LIST, out var instaUri))
                throw new Exception("Cant create URI for blocked users");
            return !string.IsNullOrEmpty(maxId)
                ? new UriBuilder(instaUri) { Query = $"max_id={maxId}" }.Uri
                : instaUri;
        }

        public static Uri GetConvertToPersonalAccountUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CONVERT_TO_PERSONAL, out var instaUri))
                throw new Exception("Cant create URI for account convert to personal account");
            return instaUri;
        }

        public static Uri GetCreateBusinessInfoUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CREATE_BUSINESS_INFO, out var instaUri))
                throw new Exception("Cant create URI for account create business info");
            return instaUri;
        }

        public static Uri GetConvertToBusinessAccountUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.BUSINESS_CONVERT_TO_BUSINESS_ACCOUNT, out var instaUri))
                throw new Exception("Cant create URI for convert to business account");
            return instaUri;
        }

        public static Uri GetUsersLookupUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERS_LOOKUP, out var instaUri))
                throw new Exception("Cant create URI for user lookup");
            return instaUri;
        }

        public static Uri GetArchiveMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_ARCHIVE, mediaId), out var instaUri))
                throw new Exception("Cant create URI for archive an post");
            return instaUri;
        }

        public static Uri GetUnArchiveMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_UNARCHIVE, mediaId), out var instaUri))
                throw new Exception("Cant create URI for unarchive an post");
            return instaUri;
        }

        public static Uri GetPresenceUri(string signedKey)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_GET_PRESENCE, out var instaUri))
                throw new Exception("Cant create URI for get presence disabled");
            //?signed_body=b941ff07b83716087710019790b3529ab123c8deabfb216e056651e9cf4b4ca7.{}&ig_sig_key_version=4
            var signedBody = signedKey + ".{}";
            var query = $"{InstaApiConstants.HEADER_IG_SIGNATURE}={signedBody}&{InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION}={InstaApiConstants.IG_SIGNATURE_KEY_VERSION}";
            var uriBuilder = new UriBuilder(instaUri) { Query = query };
            return uriBuilder.Uri;
        }

        public static Uri GetBlockedCommentersUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_GET_BLOCKED_COMMENTERS, out var instaUri))
                throw new Exception("Cant create URI for blocked commenters");
            return instaUri;
        }

        public static Uri GetSetBlockedCommentersUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SET_BLOCKED_COMMENTERS, out var instaUri))
                throw new Exception("Cant create URI for set blocked commenters");
            return instaUri;
        }

        public static Uri GetStoryPollVotersUri(string storyMediaId, string pollId, string maxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, 
                string.Format(InstaApiConstants.MEDIA_STORY_POLL_VOTERS, storyMediaId, pollId), out var instaUri))
                throw new Exception("Cant create URI for get story poll voters list");
            return !string.IsNullOrEmpty(maxId)
                ? new UriBuilder(instaUri) { Query = $"max_id={maxId}" }.Uri
                : instaUri;
        }

        public static Uri GetStoryPollVoteUri(string storyMediaId, string pollId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.MEDIA_STORY_POLL_VOTE, storyMediaId, pollId), out var instaUri))
                throw new Exception("Cant create URI for get story poll vote");
            return instaUri;
        }

        public static Uri GetVoteStorySliderUri(string storyMediaId, string pollId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.MEDIA_STORY_SLIDER_VOTE, storyMediaId, pollId), out var instaUri))
                throw new Exception("Cant create URI for vote story slider");
            return instaUri;
        }

        public static Uri GetSaveMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.MEDIA_SAVE, mediaId), out var instaUri))
                throw new Exception("Cant create URI for save media");
            return instaUri;
        }

        public static Uri GetUnSaveMediaUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.MEDIA_UNSAVE, mediaId), out var instaUri))
                throw new Exception("Cant create URI for unsave media");
            return instaUri;
        }

        public static Uri GetSavedFeedUri(string maxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FEED_SAVED, out var instaUri))
                throw new Exception("Cant create URI for get saved feed");
            return !string.IsNullOrEmpty(maxId)
                ? new UriBuilder(instaUri) { Query = $"max_id={maxId}" }.Uri
                : instaUri;
        }

        public static Uri GetBestFriendsUri(string maxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_BESTIES, out var instaUri))
                throw new Exception("Cant create URI for user besties");
            return !string.IsNullOrEmpty(maxId)
                ? new UriBuilder(instaUri) { Query = $"max_id={maxId}" }.Uri
                : instaUri;
        }

        public static Uri GetBestiesSuggestionUri(string maxId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_BESTIES_SUGGESTIONS, out var instaUri))
                throw new Exception("Cant create URI for user besties suggestions");
            return !string.IsNullOrEmpty(maxId)
                ? new UriBuilder(instaUri) { Query = $"max_id={maxId}" }.Uri
                : instaUri;
        }

        public static Uri GetSetBestFriendsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                InstaApiConstants.FRIENDSHIPS_SET_BESTIES, out var instaUri))
                throw new Exception("Cant create URI for set best friends");
            return instaUri;
        }

        public static Uri GetLocationFeedUri(string locationId, string maxId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.LOCATION_FEED, locationId), out var instaUri))
                throw new Exception("Cant create URI for get location feed");

            return instaUri
                .AddQueryParameterIfNotEmpty("max_id", maxId);
        }

        public static Uri GetLocationSectionUri(string locationId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.LOCATION_SECTION, locationId), out var instaUri))
                throw new Exception("Cant create URI for get location section");
            return instaUri;
        }

        public static Uri GetLocationSearchUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, 
                InstaApiConstants.LOCATION_SEARCH, out var instaUri))
                throw new Exception("Cant create URI for location search");
            return instaUri;
        }

        public static Uri GetAccountDetailsUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.USERS_ACCOUNT_DETAILS, userId), out var instaUri))
                throw new Exception("Cant create URI for account details");
            return instaUri;
        }

        public static Uri GetStoryQuestionResponseUri(string storyId, long questionid)
        {
            if (!Uri.TryCreate(BaseInstagramUri,
                string.Format(InstaApiConstants.MEDIA_STORY_QUESTION_RESPONSE, storyId, questionid), out var instaUri))
                throw new Exception("Cant create URI for story question answer");
            return instaUri;
        }

        public static Uri GetStoryCountdownMediaUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_STORY_COUNTDOWNS, out var instaUri))
                throw new Exception("Cant create URI for story countdown media");
            return instaUri;
        }

        public static Uri GetStoryFollowCountdownUri(long countdownId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_FOLLOW_COUNTDOWN, countdownId), out var instaUri))
                throw new Exception("Cant create URI for story follow countdown");
            return instaUri;
        }

        public static Uri GetStoryUnFollowCountdownUri(long countdownId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_UNFOLLOW_COUNTDOWN, countdownId), out var instaUri))
                throw new Exception("Cant create URI for story unfollow countdown");
            return instaUri;
        }

        public static Uri GetMediaUploadFinishUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_UPLOAD_FINISH, out var instaUri))
                throw new Exception("Cant create URI for media upload finish");
            return instaUri;
        }

        public static Uri GetBroadcastVoiceUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_SHARE_VOICE, out var instaUri))
                throw new Exception("Cant create URI for broadcast share voice");
            return instaUri;
        }

        public static Uri GetHashtagSectionUri(string hashtag)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_SECTION, hashtag), out var instaUri))
                throw new Exception("Cant create URI for hashtag section");
            return instaUri;
        }

        public static Uri GetTopicalExploreUri(string sessionId, string maxId = null, string clusterId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_TOPICAL_EXPLORE, out var instaUri))
                throw new Exception("Cant create URI for topical explore");

            instaUri = instaUri
                .AddQueryParameter("is_prefetch", "false")
                .AddQueryParameter("omit_cover_media", "true");
            if (!string.IsNullOrEmpty(maxId))
                instaUri = instaUri.AddQueryParameter("max_id", maxId);
            instaUri = instaUri
                .AddQueryParameter("module", "explore_popular")
                .AddQueryParameter("use_sectional_payload", "true")
                .AddQueryParameter("timezone_offset", InstaApiConstants.TIMEZONE_OFFSET.ToString())
                .AddQueryParameter("session_id", sessionId)
                .AddQueryParameter("include_fixed_destinations", "true");
            if (clusterId.ToLower() == "explore_all:0" || clusterId.ToLower() == "explore_all%3A0")
            {
                if (!string.IsNullOrEmpty(maxId))
                {
                    instaUri = instaUri.AddQueryParameter("cluster_id", "explore_all%3A0");
                }
            }
            else
            {
                instaUri = instaUri.AddQueryParameter("cluster_id", Uri.EscapeDataString(clusterId));
            }

            instaUri = instaUri
                .AddQueryParameter("session_id", sessionId)
                .AddQueryParameter("include_fixed_destinations", "true");
            return instaUri;
        }

        public static Uri GetBroadcastAnimatedMediaUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_ANIMATED_MEDIA, out var instaUri))
                throw new Exception("Cant create URI for broadcast animated media");
            return instaUri;
        }

        public static Uri GetBroadcastFelixShareUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_FELIX_SHARE, out var instaUri))
                throw new Exception("Cant create URI for broadcast felix share");
            return instaUri;
        }

        public static Uri GetVideoCallNtpClockUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"/video_call/ntp_clock/", out var instaUri))
                throw new Exception("Cant create URI for GetVideoCallNtpClockUri");

            return instaUri
                .AddQueryParameter("client_time", DateTime.UtcNow.ToUnixTimeMiliSeconds().ToString());
        }

        public static Uri GetVideoCallJoinUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"/video_call/join/", out var instaUri))
                throw new Exception("Cant create URI for GetVideoCallJoinUri");

            return instaUri;
        }

        public static Uri GetVideoCallConfirmUri(long videoCallId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"/video_call/{videoCallId}/confirm/", out var instaUri))
                throw new Exception("Cant create URI for GetVideoCallConfirmUri");

            return instaUri;
        }

        public static Uri GetVideoCallInfoUri(long videoCallId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"/video_call/{videoCallId}/info/", out var instaUri))
                throw new Exception("Cant create URI for GetVideoCallInfoUri");

            return instaUri;
        }

        public static Uri GetAddVideoCallToDirectUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"/direct_v2/threads/{threadId}/add_video_call/", out var instaUri))
                throw new Exception("Cant create URI for GetAddVideoCallToDirectUri");

            return instaUri;
        }
        public static Uri GetVideoCallLeaveUri(long videoCallId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, $"/video_call/{videoCallId}/leave/", out var instaUri))
                throw new Exception("Cant create URI for GetVideoCallLeaveUri");

            return instaUri;
        }

        public static Uri GetLauncherSyncUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.LAUNCHER_SYNC, out var instaUri))
                throw new Exception("Cant create URI for launcher sync");
            return instaUri;
        }
        public static Uri GetDiscoverDismissSuggestionUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_DISMISS_SUGGESTION, out var instaUri))
                throw new Exception("Cant create URI for discover dismiss suggestion");
            return instaUri;
        }
        public static Uri GetHashtagMediaReportUri() 
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.TAG_MEDIA_REPORT, out var instaUri))
                throw new Exception("Cant create URI for hashtag media report");
            return instaUri;
        }
        public static Uri GetExploreReportUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_EXPLORE_REPORT, out var instaUri))
                throw new Exception("Cant create URI for discover explore report");
            return instaUri;
        }

        public static Uri GetMediaTagsUri(string mediaId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.MEDIA_TAG, mediaId), out var instaUri))
                throw new Exception("Cant create URI for media hashtags");
            return instaUri;
        }
        public static Uri GetDiscoverSurfaceWithSuUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_SURFACE_WITH_SU, out var instaUri))
                throw new Exception("Cant create URI for discover surface with su");
            return instaUri;
        }
        public static Uri GetBanyanUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.BANYAN, out var instaUri))
                throw new Exception("Cant create URI for banyan");
            return instaUri;
        }
        public static Uri GetHideSearchEntitiesUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FBSEARCH_HIDE_SEARCH_ENTITIES, out var instaUri))
                throw new Exception("Cant create URI for hide search entities");
            return instaUri;
        }
        public static Uri GetDynamicSearchUri(InstaDiscoverSearchType searchType)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.FBSEARCH_DYNAMIC_SEARCH, searchType.ToString().ToLower()), out var instaUri))
                throw new Exception("Cant create URI for fbsearch dynamic search");
            return instaUri;
        }
        public static Uri GetAnswerToStoryQuizUri(long storyItemPk, long quizId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.STORY_QUIZ_ANSWER, storyItemPk, quizId), out var instaUri))
                throw new Exception("Cant create URI for answer to quiz");
            return instaUri;
        }
        public static Uri GetNotificationBadgeUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.NOTIFICATION_BADGE, out var instaUri))
                throw new Exception("Cant create URI for notification badge");
            return instaUri;
        }
        public static Uri GetContactPointPrefillUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_CONTACT_POINT_PREFILL, out var instaUri))
                throw new Exception("Cant create URI for contact point prefill");
            return instaUri;
        }
        public static Uri GetReadMsisdnHeaderUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_READ_MSISDN_HEADER, out var instaUri))
                throw new Exception("Cant create URI for read msisdn header");
            return instaUri;
        }
        public static Uri GetPrefillCandidatesUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_GET_PREFILL_CANDIDATES, out var instaUri))
                throw new Exception("Cant create URI for get prefill candidates");
            return instaUri;
        }
        public static Uri GetQeSyncUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.QE_SYNC, out var instaUri))
                throw new Exception("Cant create URI for qe sync");
            return instaUri;
        }
        public static Uri GetConsentExistingUserFlowUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CONSENT_EXISTING_USER_FLOW, out var instaUri))
                throw new Exception("Cant create URI for consent existing user flow");
            return instaUri;
        }

        public static Uri GetMuteDirectThreadVideoCallsUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_VIDEOCALLS_MUTE, threadId), out var instaUri))
                throw new Exception("Cant create URI for mute direct thread video calls");
            return instaUri;
        }

        public static Uri GetUnMuteDirectThreadVideoCallsUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_VIDEOCALLS_UNMUTE, threadId), out var instaUri))
                throw new Exception("Cant create URI for unmute direct thread video calls");
            return instaUri;
        }

        public static Uri GetHashtagChannelVideosUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.TAG_CHANNEL_VIEWER, threadId), out var instaUri))
                throw new Exception("Cant create URI for hashtag channel videos");
            return instaUri;
        }

        public static Uri GetExploreChannelVideosUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.EXPLORE_CHANNEL_VIEWER, threadId), out var instaUri))
                throw new Exception("Cant create URI for explore channel videos");
            return instaUri;
        }

        public static Uri GetRUploadVideoStartUri(string guid)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.RUPLOAD_IGVIDEO_START, guid), out var instaUri))
                throw new Exception("Cant create URI for rupload segmented video start");
            return instaUri;
        }
        public static Uri GetRUploadVideoTransferUri(string md5, int start, int fileSize)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.RUPLOAD_IGVIDEO_TRANSFER, md5, start, fileSize), out var instaUri))
                throw new Exception("Cant create URI for rupload segmented video transfer");
            return instaUri;
        }

        public static Uri GetRUploadVideoEndUri(string guid)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.RUPLOAD_IGVIDEO_END, guid), out var instaUri))
                throw new Exception("Cant create URI for rupload segmented video end");
            return instaUri;
        }

        public static Uri GetBroadcastReelReactUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_REEL_REACT, out var instaUri))
                throw new Exception("Cant create URI for direct broadcast reel react");
            return instaUri;
        }

        public static Uri GetStoryChatRequestUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_STORY_CHAT_REQUEST, out var instaUri))
                throw new Exception("Cant create URI for story chat request");
            return instaUri;
        }

        public static Uri GetStoryChatCancelRequestUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.MEDIA_STORY_CHAT_CANCEL_REQUEST, out var instaUri))
                throw new Exception("Cant create URI for story chat cancel request");
            return instaUri;
        }

        public static Uri GetRemoveTrustedDeviceUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_REMOVE_TRUSTED_DEVICE, out var instaUri))
                throw new Exception("Cant create URI for remove trusted device");
            return instaUri;
        }

        public static Uri GetTVBrowseFeedUri(string maxId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_BROWSE_FEED, out var instaUri))
                throw new Exception("Cant create URI for browse igtv");
            if (string.IsNullOrEmpty(maxId))
                instaUri = instaUri.AddQueryParameter("prefetch", "1");

            return instaUri
                .AddQueryParameter("max_id", maxId);
        }

        public static Uri GetSeenTVUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_WRITE_SEEN, out var instaUri))
                throw new Exception("Cant create URI for tv write seen");
            return instaUri;
        }

        public static Uri GetRecentFollowersUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.FRIENDSHIPS_RECENT_FOLLOWERS, out var instaUri))
                throw new Exception("Cant create URI for recent followers");
            return instaUri;
        }
        public static Uri GetDismissDiscoverUserSuggestionUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_DISMISS_USER_SUGGESTION, out var instaUri))
                throw new Exception("Cant create URI for discover dismiss user suggestion");
            return instaUri;
        }

        public static Uri GetDirectEndChatUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_REMOVE_ALL_USERS, threadId), out var instaUri))
                throw new Exception("Cant create URI for direct thread end chat");
            return instaUri;
        }

        public static Uri GetDirectThreadApprovalRequiredUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_APPROVAL_REQUIRED_FOR_NEW_MEMBERS, threadId), out var instaUri))
                throw new Exception("Cant create URI for direct thread approval required for new members");
            return instaUri;
        }

        public static Uri GetDirectThreadApprovalNotRequiredUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_APPROVAL_NOT_REQUIRED_FOR_NEW_MEMBERS, threadId), out var instaUri))
                throw new Exception("Cant create URI for direct thread approval NOT required for new members");
            return instaUri;
        }

        public static Uri GetDirectThreadAddAdminUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_ADD_ADMINS, threadId), out var instaUri))
                throw new Exception("Cant create URI for direct thread add admins");
            return instaUri;
        }

        public static Uri GetDirectThreadRemoveAdminUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_REMOVE_ADMINS, threadId), out var instaUri))
                throw new Exception("Cant create URI for direct thread remove admins");
            return instaUri;
        }
        public static Uri GetDirectThreadRemoveUserUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_REMOVE_USERS, threadId), out var instaUri))
                throw new Exception("Cant create URI for direct thread remove user");
            return instaUri;
        }
        public static Uri GetCreateGroupThreadUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_CREATE_GROUP, out var instaUri))
                throw new Exception("Cant create URI for create group thread");
            return instaUri;
        }
        public static Uri GetDirectThreadByParticipantsUri(string users, int seqId, int limit = 10)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_BY_PARTICIPANTS, users, seqId, limit), out var instaUri))
                throw new Exception("Cant create URI for direct thread by participants users");
            return instaUri;
        }
        public static Uri GetUserStoryAndLivesUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.GET_USER_STORY_WITH_LIVES, userId, InstaApiConstants.SupportedCapabalities.ToString(Formatting.None)),
                out var instaUri))
                throw new Exception("Can't create URI for getting user's story");
            return instaUri;
        }
        public static Uri GetUserTagsPendingReviewCountUri(long userId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USERTAGS_GET_PENDING_REVIEW_COUNT, userId), out var instaUri))
                throw new Exception("Cant create URI for usertag pending review count");
            return instaUri;
        }
        public static Uri GetUnSyncContactsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ADDRESSBOOK_UNLINK, out var instaUri))
                throw new Exception("Cant create URI for unlink addressbook");
            return instaUri;
        }
        public static Uri GetUserTagsReviewUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERTAGS_REVIEW, out var instaUri))
                throw new Exception("Cant create URI for usertags review");
            return instaUri;
        }
        public static Uri GetUserTagsReviewPreferenceUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.USERTAGS_REVIEW_PREFERENCE, out var instaUri))
                throw new Exception("Cant create URI for usertags review preference");
            return instaUri;
        }
        public static Uri GetUserTagsPendingReviewMediaUri(long userId, string maxId = null)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.USERTAGS_GET_PENDING_REVIEW_MEDIA, userId), out var instaUri))
                throw new Exception("Cant create URI for usertag pending review media");
            return instaUri
                .AddQueryParameter("max_id", maxId);
        }
        public static Uri GetSessionLoginActivityUri(string deviceGuid)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.SESSION_LOGIN_ACTIVITY_WITH_DEVICEID, deviceGuid), out var instaUri))
                throw new Exception("Cant create URI for session login activity");
            return instaUri;
        }
        public static Uri GetAcceptThisIsMeSessionLoginActivityUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SESSION_LOGIN_ACTIVITY_AVOW_LOGIN, out var instaUri))
                throw new Exception("Cant create URI for session activity this is me");
            return instaUri;
        }
        public static Uri GetLogoutSessionLoginActivityUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.SESSION_LOGIN_ACTIVITY_LOGOUT_SESSION, out var instaUri))
                throw new Exception("Cant create URI for logout session");
            return instaUri;
        }

        public static Uri GetSendDirectProductUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_PRODUCT_SHARE, out var instaUri))
                throw new Exception("Cant create URI for send product to direct thread");
            return instaUri;
        }
        public static Uri GetBroadcastArEffectUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DIRECT_BROADCAST_AR_EFFECT, out var instaUri))
                throw new Exception("Cant create URI for direct broadcast ar effect");
            return instaUri;
        }
        public static Uri GetNotificationsSettingsUri(string contentType)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.NOTIFICATION_GET_NOTIFICATION_SETTINGS, contentType),
                out var instaUri))
                throw new Exception("Can't create URI for getting notification settings");
            return instaUri;
        }
        public static Uri GetChangeNotificationsSettingsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.NOTIFICATION_CHANGE_NOTIFICATION_SETTINGS,
                out var instaUri))
                throw new Exception("Can't create URI for change notification settings");
            return instaUri;
        }
        public static Uri GetMarkVisualThreadsAsSeenUri(string threadId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.DIRECT_THREAD_ITEM_SEEN, threadId),
                    out var instaUri)) throw new Exception("Cant create URI for mark visual threads as seen");
            return instaUri;
        }
        public static Uri GetNewsInboxSeenUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.NEWS_INBOX_SEEN,
                out var instaUri))
                throw new Exception("Can't create URI for news inbox seen");
            return instaUri;
        }
        public static Uri GetDiscoverMarkSuSeenUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.DISCOVER_MARK_SU_SEEN,
                out var instaUri))
                throw new Exception("Can't create URI for discover mark su seen");
            return instaUri;
        }
        public static Uri GetSetAccountBirthdayUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SET_BIRTHDAY,
                out var instaUri))
                throw new Exception("Can't create URI for setting account birthday");
            return instaUri;
        }
        public static Uri GetSetAccountGenderUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.ACCOUNTS_SET_GENDER,
                out var instaUri))
                throw new Exception("Can't create URI for setting account gender");
            return instaUri;
        }
        public static Uri GetCheckOffensiveTextUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.WARNING_CHECK_OFFENSIVE_TEXT,
                out var instaUri))
                throw new Exception("Can't create URI for check offensive text");
            return instaUri;
        }
        public static Uri GetCreateTvSeriesUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_SERIES_CREATE,
                out var instaUri))
                throw new Exception("Can't create URI for creating TV series");
            return instaUri;
        }
        public static Uri GetDeleteTvSeriesUri(string seriesId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.IGTV_SERIES_DELETE, seriesId),
                    out var instaUri)) throw new Exception("Can't create URI for deleting TV series");
            return instaUri;
        }
        public static Uri GetUpdateTvSeriesUri(string seriesId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.IGTV_SERIES_UPDATE, seriesId),
                    out var instaUri)) throw new Exception("Can't create URI for updating TV series");
            return instaUri;
        }
        public static Uri GetAddEpisodeToTvSeriesUri(string seriesId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.IGTV_SERIES_ADD_EPISODE, seriesId),
                    out var instaUri)) throw new Exception("Can't create URI for add espisode TV series");
            return instaUri;
        }
        public static Uri GetRemoveEpisodeFromTvSeriesUri(string seriesId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.IGTV_SERIES_REMOVE_EPISODE, seriesId),
                    out var instaUri)) throw new Exception("Can't create URI for remove espisode TV series");
            return instaUri;
        }
        public static Uri GetUserTVSeriesUri(long userId, HttpHelper httpHelper)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.IGTV_SERIES_ALL_USER_SERIES, userId),
                    out var instaUri)) throw new Exception("Can't create URI for user TV series");

            var data = "{}";
            var hash = CryptoHelper.CalculateHash(httpHelper._apiVersion.SignatureKey, JsonConvert.SerializeObject(data));
            var signature = $"{(httpHelper.IsNewerApis ? httpHelper._apiVersion.SignatureKey : hash)}.{data}";

            instaUri = instaUri.AddQueryParameter(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!httpHelper.IsNewerApis)
                instaUri = instaUri.AddQueryParameter(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);

            return instaUri;
        }
        public static Uri GetPostLiveThumbnailsUri(string broadcastId, HttpHelper httpHelper)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_GET_POST_LIVE_THUMBNAILS, broadcastId),
                    out var instaUri)) throw new Exception("Can't create URI for post live thumbnails");

            var data = "{}";
            var hash = CryptoHelper.CalculateHash(httpHelper._apiVersion.SignatureKey, JsonConvert.SerializeObject(data));
            var signature = $"{(httpHelper.IsNewerApis ? httpHelper._apiVersion.SignatureKey : hash)}.{data}";

            instaUri = instaUri.AddQueryParameter(InstaApiConstants.HEADER_IG_SIGNATURE, signature);

            if (!httpHelper.IsNewerApis)
                instaUri = instaUri.AddQueryParameter(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);

            return instaUri;
        }
        public static Uri GetIgTvCreationToolsUri()
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.IGTV_CREATION_TOOLS,
                out var instaUri))
                throw new Exception("Can't create URI for igtv creating tools");
            return instaUri;
        }
        public static Uri GetGiphyUri(InstaGiphyRequestType type, string query)
        {
            if (!Uri.TryCreate(BaseInstagramUri, InstaApiConstants.CREATIVES_STORY_MEDIA_SEARCH_KEYED_FORMAT,
                    out var instaUri)) throw new Exception("Can't create URI for creatives story media search keyed format");

            return instaUri
                .AddQueryParameter("request_surface", type.GetRequestSurface())
                .AddQueryParameter("q", query, true)
                .AddQueryParameter("media_types", "[\"giphy\"]");
        }
        public static Uri GetBroadcastJoinLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_JOIN, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast join");
            return instaUri;
        }
        public static Uri GetBroadcastConfirmLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_CONFIRM, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast confirm");
            return instaUri;
        }
        public static Uri GetBroadcastInviteLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_INVITE, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast invite");
            return instaUri;
        }
        public static Uri GetBroadcastLeaveLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_LEAVE, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast leave");
            return instaUri;
        }
        public static Uri GetBroadcastEventLiveUri(string broadcastId)
        {
            if (!Uri.TryCreate(BaseInstagramUri, string.Format(InstaApiConstants.LIVE_BROADCAST_EVENT, broadcastId), out var instaUri))
                throw new Exception("Cant create URI for broadcast event");
            return instaUri;
        }
    }
}
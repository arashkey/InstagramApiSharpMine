/*  
 *  
 *  
 *  All endpoints and headers is here
 *  
 *  
 *        IRANIAN DEVELOPERS
 *        
 *        
 *               2019
 *  
 *  
 */

using Newtonsoft.Json.Linq;
using System;

namespace InstagramApiSharp.API
{
    /// <summary>
    ///     Place of every endpoints, headers and other constants and variables.
    /// </summary>
    internal static class InstaApiConstants
    {
        #region New

        public const string BANYAN = API_SUFFIX + "/banyan/banyan/?views=[\"story_share_sheet\",\"reshare_share_sheet\"]";
        public const string FBSEARCH_DYNAMIC_SEARCH = API_SUFFIX + "/fbsearch/nullstate_dynamic_sections/?type={0}";
        public const string STORY_QUIZ_ANSWER = API_SUFFIX + "/media/{0}/{1}/story_quiz_answer/";


        public const string ACCOUNTS_GET_PREFILL_CANDIDATES = API_SUFFIX + "/accounts/get_prefill_candidates/";
        public const string QE_SYNC = API_SUFFIX + "/qe/sync/";


        public const string DIRECT_THREAD_VIDEOCALLS_MUTE = API_SUFFIX + "/direct_v2/threads/{0}/mute_video_call/";
        public const string DIRECT_THREAD_VIDEOCALLS_UNMUTE = API_SUFFIX + "/direct_v2/threads/{0}/unmute_video_call/";

        // push 
        public const string FACEBOOK_OTA_FIELDS = "update%7Bdownload_uri%2Cdownload_uri_delta_base%2Cversion_code_delta_base%2Cdownload_uri_delta%2Cfallback_to_full_update%2Cfile_size_delta%2Cversion_code%2Cpublished_date%2Cfile_size%2Cota_bundle_type%2Cresources_checksum%7D";
        public const int FACEBOOK_ORCA_PROTOCOL_VERSION = 20150314;
        public const string FACEBOOK_ORCA_APP_ID = "124024574287414";
        public const string FACEBOOK_ANALYTICS_APP_ID = "567067343352427";
        public const string INSTAGRAM_PACKAGE_NAME = "com.instagram.android";

        #endregion New


        #region Main
        public const string HEADER_PIGEON_SESSION_ID = "X-Pigeon-Session-Id";
        public const string HEADER_PIGEON_RAWCLINETTIME = "X-Pigeon-Rawclienttime";
        public const string HEADER_X_IG_CONNECTION_SPEED = "X-IG-Connection-Speed";
        public const string HEADER_X_IG_BANDWIDTH_SPEED_KBPS = "X-IG-Bandwidth-Speed-KBPS";
        public const string HEADER_X_IG_BANDWIDTH_TOTALBYTES_B = "X-IG-Bandwidth-TotalBytes-B";
        public const string HEADER_X_IG_BANDWIDTH_TOTALTIME_MS = "X-IG-Bandwidth-TotalTime-MS";



        public const string ACCEPT_ENCODING = "gzip, deflate, sdch";
        public const string API = "/api";
        public const string API_SUFFIX = API + API_VERSION;
        public const string API_SUFFIX_V2 = API + API_VERSION_V2;
        public const string API_VERSION = "/v1";
        public const string API_VERSION_V2 = "/v2";
        public const string BASE_INSTAGRAM_API_URL = INSTAGRAM_URL + API_SUFFIX + "/";
        public const string COMMENT_BREADCRUMB_KEY = "iN4$aGr0m";
        public const string CSRFTOKEN = "csrftoken";
        public const string HEADER_ACCEPT_ENCODING = "gzip, deflate, sdch";
        public const string HEADER_ACCEPT_LANGUAGE = "Accept-Language";
        public const string HEADER_COUNT = "count";
        public const string HEADER_EXCLUDE_LIST = "exclude_list";
        public const string HEADER_IG_APP_ID = "X-IG-App-ID";
        public const string HEADER_IG_CAPABILITIES = "X-IG-Capabilities";
        public const string HEADER_IG_CONNECTION_TYPE = "X-IG-Connection-Type";
        public const string HEADER_IG_SIGNATURE = "signed_body";
        public const string HEADER_IG_SIGNATURE_KEY_VERSION = "ig_sig_key_version";
        public const string HEADER_MAX_ID = "max_id";
        public const string HEADER_PHONE_ID = "phone_id";
        public const string HEADER_QUERY = "q";
        public const string HEADER_RANK_TOKEN = "rank_token";
        public const string HEADER_TIMEZONE = "timezone_offset";
        public const string HEADER_USER_AGENT = "User-Agent";
        public const string HEADER_X_INSTAGRAM_AJAX = "X-Instagram-AJAX";
        public const string HEADER_X_REQUESTED_WITH = "X-Requested-With";
        public const string HEADER_XCSRF_TOKEN = "X-CSRFToken";
        public const string HEADER_XGOOGLE_AD_IDE = "X-Google-AD-ID";
        public const string HEADER_XML_HTTP_REQUEST = "XMLHttpRequest";
        public const string IG_APP_ID = "567067343352427";
        public const string IG_CONNECTION_TYPE = "WIFI";
        public const string IG_SIGNATURE_KEY_VERSION = "4";
        public const string INSTAGRAM_URL = "https://i.instagram.com";
        public const string P_SUFFIX = "p/";
        public const string SUPPORTED_CAPABALITIES_HEADER = "supported_capabilities_new";

        public static string TIMEZONE = "Asia/Tehran";

        public static int TIMEZONE_OFFSET = 16200;

        public const string USER_AGENT =
                                    "Instagram {6} Android ({7}/{8}; {0}; {1}; {2}/{10}; {3}; {4}; {5}; en_US; {9})";
        public const string USER_AGENT_DEFAULT =
        "Instagram 94.0.0.22.116 Android (24/7.0; 480dpi; 1080x1794; HUAWEI/HONOR; PRA-LA1; HWPRA-H; hi6250; en_US; 155374104)";
        public static readonly JArray SupportedCapabalities = new JArray
        {
            new JObject
            {
                {"name","SUPPORTED_SDK_VERSIONS"},
                {"value","13.0,14.0,15.0,16.0,17.0,18.0,19.0,20.0," +
                    "21.0,22.0,23.0,24.0,25.0,26.0,27.0,28.0,29.0,30.0,31.0,32.0,33.0," +
                    "34.0,35.0,36.0,37.0,38.0,39.0,40.0,41.0,42.0,43.0,44.0,45.0,46.0," +
                    "47.0,48.0,49.0,50.0,51.0,52.0,53.0,54.0,55.0,56.0,57.0,58.0,59.0,60.0,61.0," +
                    "62.0,63.0,64.0"}
            },
            new JObject
            {
                {"name","FACE_TRACKER_VERSION"},
                {"value","12"}
            },
            //new JObject
            //{
            //    {"name","segmentation"},
            //    {"value","segmentation_enabled"}
            //},
            new JObject
            {
                {"name","COMPRESSION"},
                {"value","ETC2_COMPRESSION"}
            },
            new JObject
            {
                {"name","world_tracker"},
                {"value","world_tracker_enabled"}
            },
            new JObject
            {
                {"name","gyroscope"},
                {"value","gyroscope_enabled"}
            }
        };

        public const string LOGIN_EXPERIMENTS_CONFIGS = "ig_android_fci_onboarding_friend_search,ig_android_device_detection_info_upload," +
            "ig_android_sms_retriever_backtest_universe,ig_android_direct_add_direct_to_android_native_photo_share_sheet," +
            "ig_growth_android_profile_pic_prefill_with_fb_pic_2,ig_account_identity_logged_out_signals_global_holdout_universe," +
            "ig_android_login_identifier_fuzzy_match,ig_android_reliability_leak_fixes_h1_2019,ig_android_push_fcm," +
            "ig_android_show_login_info_reminder_universe,ig_android_email_fuzzy_matching_universe," +
            "ig_android_one_tap_aymh_redesign_universe,ig_android_direct_send_like_from_notification," +
            "ig_android_suma_landing_page,ig_android_direct_main_tab_universe,ig_android_login_forgot_password_universe," +
            "ig_android_smartlock_hints_universe,ig_android_account_switch_infra_universe,ig_android_multi_tap_login_new," +
            "ig_android_caption_typeahead_fix_on_o_universe,ig_android_save_pwd_checkbox_reg_universe," +
            "ig_android_nux_add_email_device,ig_username_suggestions_on_username_taken,ig_android_analytics_accessibility_event," +
            "ig_android_editable_username_in_reg,ig_android_ingestion_video_support_hevc_decoding,direct_app_deep_linking_universe," +
            "ig_android_account_recovery_auto_login,ig_android_feed_cache_device_universe2,ig_android_sim_info_upload," +
            "ig_android_mobile_http_flow_device_universe,ig_account_recovery_via_whatsapp_universe," +
            "ig_android_hide_fb_button_when_not_installed_universe,ig_android_targeted_one_tap_upsell_universe," +
            "ig_android_gmail_oauth_in_reg,ig_android_native_logcat_interceptor,ig_android_hide_typeahead_for_logged_users," +
            "ig_android_vc_interop_use_test_igid_universe,ig_android_reg_modularization_universe,ig_android_phone_edit_distance_universe," +
            "ig_android_device_verification_separate_endpoint,ig_android_universe_noticiation_channels,ig_android_account_linking_universe," +
            "ig_android_hsite_prefill_new_carrier,ig_android_retry_create_account_universe,ig_android_family_apps_user_values_provider_universe," +
            "ig_android_reg_nux_headers_cleanup_universe,ig_android_ci_fb_reg,ig_android_device_info_foreground_reporting," +
            "ig_fb_invite_entry_points,ig_android_device_verification_fb_signup,ig_android_onetaplogin_optimization," +
            "ig_video_debug_overlay,ig_android_ask_for_permissions_on_reg,ig_assisted_login_universe," +
            "ig_android_display_full_country_name_in_reg_universe,ig_android_security_intent_switchoff," +
            "ig_android_passwordless_auth,ig_circularimageview_outlineprovider,ig_android_direct_main_tab_account_switch," +
            "ig_android_modularized_dynamic_nux_universe,ig_android_li_use_object_pool_holdout," +
            "ig_android_fb_account_linking_sampling_freq_universe,ig_android_fix_sms_read_lollipop,ig_android_access_flow_prefill";


        public const string AFTER_LOGIN_EXPERIMENTS_CONFIGS = "ig_android_push_notifications_settings_redesign_universe,ig_hashtag_display_universe," +
            "ig_android_video_ssim_fix_pts_universe,coupon_price_test_ad4ad_instagram_resurrection_universe," +
            "ig_android_live_rendering_looper_universe,android_ig_camera_ar_asset_manager_improvements_universe," +
            "ig_android_direct_autoscroll_to_first_unread_universe,ig_android_mqtt_cookie_auth_memcache_universe," +
            "ig_android_video_player_memory_leaks,ig_android_stories_seen_state_serialization,ig_stories_photo_time_duration_universe," +
            "ig_android_bitmap_cache_executor_size,ig_android_stories_music_search_typeahead,ig_android_remove_fb_nux_universe,i" +
            "g_android_delayed_comments,ig_android_direct_mutation_manager_media_3,ig_smb_ads_holdout_2019_h1_universe,ig_fb_graph_d" +
            "ifferentiation,ig_android_stories_share_extension_video_segmentation,ig_android_switch_back_option,ig_andro" +
            "id_interactions_realtime_typing_indicator_and_live_comments,ig_android_stories_create_flow_favorites_too" +
            "ltip,ig_android_direct_reshare_chaining,ig_android_direct_app_reel_grid_search,ig_android_stories_no_inflation_o" +
            "n_app_start,ig_android_stories_viewer_viewpoint_universe,ig_android_direct_newer_single_line_composer_u" +
            "niverse,ig_direct_holdout_h1_2019,ig_android_story_viewpoint_impression_universe,ig_explore_2019_h1_destinat" +
            "ion_cover,ig_android_direct_stories_in_direct_inbox,ig_android_stories_music_precapture,ig_android_" +
            "vc_service_crash_fix_universe,ig_android_direct_double_tap_to_like_hearts,ig_fb_graph_differentiati" +
            "on_no_fb_data,ig_camera_android_api_rewrite_universe,ig_android_direct_realtime_timeout,ig_android" +
            "_growth_fci_team_holdout_universe,android_camera_core_cpu_frames_sync,ig_android_stories_gallery_r" +
            "ecyclerview_kit_universe,ig_android_story_ads_instant_sub_impression_universe,ig_business_signup_biz" +
            "_id_universe,ig_android_save_all,ig_android_ttcp_improvements,ig_android_camera_ar_platform_profile_" +
            "universe,ig_android_separate_sms_n_email_invites_setting_universe,ig_explore_2018_topic_channel_nav" +
            "igation_android_universe,ig_shopping_bag_universe,ig_ar_shopping_camera_android_universe,ig_android_rec" +
            "yclerview_binder_group_enabled_universe,ig_android_stories_viewer_tall_android_cap_media_universe,ig_" +
            "android_direct_share_sheet_custom_fast_scroller,ig_android_video_exoplayer_2,native_contact_invites_u" +
            "niverse,ig_android_stories_seen_state_processing_universe,ig_android_dash_script,ig_android_insights_" +
            "media_hashtag_insight_universe,ig_camera_fast_tti_universe,ig_android_stories_whatsapp_share,ig_andro" +
            "id_stories_music_filters,ig_android_render_thread_memory_leak_holdout,ig_android_direct_group_creati" +
            "on_flow_redesign,ig_android_2018_h1_hashtag_report_universe,ig_share_to_story_toggle_include_shoppin" +
            "g_product,ig_android_camera_disable_hands_free_countdown_universe,ig_android_interactions_verified_" +
            "badge_on_comment_details,ig_android_stories_fix_story_ring_for_deleted_story,ig_android_camera_reduce_file_exif_reads" +
            ",ig_payments_billing_address,ig_android_fs_new_gallery_hashtag_prompts,ig_camera_remove_display_rotation_cb" +
            "_universe,ig_android_interactions_migrate_inline_composer_to_viewpoint_universe,ig_android_ufiv3_holdout" +
            ",ig_camera_android_async_post_capture_controls_inflate_universe,ig_android_direct_fix_playing_invalid_vis" +
            "ual_message,ig_android_deeplink_invites_url_to_profile_universe,ig_android_enable_zero_rating,ig_andro" +
            "id_story_ads_carousel_performance_universe_1,ig_android_import_page_post_after_biz_conversion,ig_camera" +
            "_ar_effect_attribution_position,ig_promote_add_payment_navigation_universe,ig_android_story_ads_carous" +
            "el_performance_universe_2,ig_android_main_feed_refresh_style_universe,ig_stories_engagement_holdout_2019_" +
            "h1_universe,ig_android_story_ads_performance_universe_1,ig_android_stories_viewer_modal_activity,ig_android_" +
            "story_ads_performance_universe_2,ig_android_publisher_stories_migration,ig_android_story_ads_performance_uni" +
            "verse_3,ig_android_quick_conversion_universe,ig_android_story_import_intent,ig_android_story_ads_performance_" +
            "universe_4,ig_android_ads_paid_branded_content_stories,ig_android_feed_seen_state_with_view_info,ig_biz_grap" +
            "h_connection_universe,ig_android_disable_verification,ig_android_ads_profile_cta_feed_universe,ig_android_dj" +
            "ango_push_phase_logginig,ig_android_vc_cowatch_universe,ig_android_nametag,ig_hashtag_creation_universe,ig_an" +
            "droid_igtv_chaining,ig_android_live_qa_viewer_v1_universe,ig_shopping_insights_wc_copy_update_android,ig_andr" +
            "oid_stories_music_lyrics_pre_capture,ig_android_igtv_reshare,ig_android_direct_serialize_to_sqlite_memory_i" +
            "mprovements,ig_android_wellbeing_timeinapp_v1_universe,ig_android_profile_cta_v3,ig_end_of_feed_universe,ig_android_" +
            "vc_shareable_moments_universe,ig_camera_text_overlay_controller_opt_universe,ig_android_video_qp_logger_u" +
            "niverse,ig_android_cache_video_autoplay_checker,ig_android_follow_request_button_improvements_universe,ig_" +
            "android_vc_start_from_direct_inbox_universe,ig_android_separate_network_executor,ig_perf_android_holdout," +
            "ig_fb_graph_differentiation_only_fb_candidates,ig_android_media_streaming_sdk_universe,ig_android_direct_mutation_m" +
            "anager_queue_thread_universe,ig_android_direct_reshares_from_thread,ig_android_stories_v" +
            "ideo_prefetch_kb,ig_android_wellbeing_timeinapp_v1_migration,ig_android_camera_post_smile_face_fir" +
            "st_universe,ig_android_maintabfragment,ig_android_cookie_injection_retry_universe,ig_inventory_connecti" +
            "ons,ig_stories_injection_tool_enabled_universe,ig_android_canvas_cookie_universe,ig_android_stor" +
            "ies_disable_highlights_media_preloading,ig_android_expired_build_lockout,ig_android_branded_conten" +
            "t_ads_universe,ig_promote_lotus_universe,ig_android_video_streaming_upload_universe,ig_camera_andr" +
            "oid_attribution_bottomsheet_universe,ig_android_explore_viewpoint_autoplay,ig_android_product_tag_hint_dot" +
            "s,ig_interactions_h1_2019_team_holdout_universe,ig_android_music_story_fb_crosspost_universe,ig_andro" +
            "id_disable_scroll_listeners,ig_android_ad_async_ads_universe,ig_android_vc_participants_grid_refactor_uni" +
            "verse,ig_camera_android_camera_filter_downsample_fix_universe,ig_android_persistent_nux,ig_camera_android_effect_i" +
            "nfo_bottom_sheet_universe,ig_android_igtv_audio_always_on,ig_android_sorting_on_self_following_universe," +
            "ig_android_edit_location_page_info,ig_camera_android_segmentation_v106_igdjango_universe,ig_promote_are_you_sur" +
            "e_universe,ig_android_persistent_duplicate_notif_checker_user_based,ig_android_interactions_feed_label_belo" +
            "w_comments_refactor_universe,ig_android_li_session_chaining,ig_android_camera_platform_effect_share_universe,ig" +
            "_android_rate_limit_mediafeedviewablehelper,ig_android_camera_ar_platform_logging,ig_android_direct_quick_rep" +
            "lies,ig_direct_android_mentions_receiver,ig_camera_android_device_capabilities_experiment,ig_android_stories_" +
            "viewer_drawable_cache_universe,ig_camera_android_qcc_constructor_opt_universe,ig_direct_android_reply_modal_u" +
            "niverse,ig_android_stories_alignment_guides_universe,ig_android_stories_music_question_response,ig_android_r" +
            "n_ads_manager_universe,ig_android_video_visual_quality_score_based_abr,ig_explore_2018_post_chaining_accoun" +
            "t_recs_dedupe_universe,ig_android_stories_reel_media_item_automatic_retry,ig_android_stories_video_seeking_audio_bug_fix,ig_a" +
            "ndroid_insights_holdout,ig_fb_notification_universe,ig_android_feed_post_sticker,ig_android_inline_" +
            "editing_local_prefill,ig_android_low_data_mode_backup_2,ig_android_invite_xout_universe,ig_android_" +
            "low_data_mode_backup_3,ig_android_low_data_mode_backup_4,ig_android_low_data_mode_backup_5,ig_android_stories_" +
            "unregister_decor_listener_universe,ig_android_search_condensed_search_icons,ig_android_video_abr_univ" +
            "erse,ig_android_blended_inbox_split_button_v2_universe,ig_android_low_data_mode_backup_1,ig_android_n" +
            "elson_v0_universe,ig_android_scroll_audio_priority,ig_android_own_profile_sharing_universe,ig_andro" +
            "id_vc_cowatch_media_share_universe,ig_biz_graph_unify_assoc_universe,ig_challenge_general_v2,ig_an" +
            "droid_place_signature_universe,ig_android_direct_inbox_cache_universe,ig_android_ig_branding_in_fb" +
            "_universe,ig_android_business_promote_tooltip,ig_android_shopping_product_appeals_universe,ig_andr" +
            "oid_tap_to_capture_universe,ig_android_follow_requests_ui_improvements,ig_android_story_camera_sh" +
            "are_to_feed_universe,ig_android_fb_follow_server_linkage_universe,ig_android_direct_updated_story_" +
            "reference_ui,ig_android_sorting_on_self_following_new_posts_universe,ig_android_direct_composer_ov" +
            "erflow_tray_test,ig_android_biz_reorder_value_props,ig_android_music_continuous_capture,ig_android_direct_" +
            "view_more_qe,ig_android_churned_find_friends_redirect_to_discover_people,ig_android_main_feed_new_post" +
            "s_indicator_universe,ig_vp9_hd_blacklist,ig_camera_android_ar_effect_stories_deeplink,ig_android_client_side_delivery_universe,ig" +
            "_ios_queue_time_qpl_universe,ig_android_stories_send_client_reels_on_tray_fetch_universe,ig_android_storie" +
            "s_viewer_responsiveness_universe,ig_android_felix_prefetch_thumbnail_sprite_sheet,ig_android_live_use_" +
            "rtc_upload_universe,ig_android_vc_always_process_mws_events_universe,ig_android_multi_dex_class_load" +
            "er_v2,ig_android_live_ama_viewer_universe,ig_android_business_id_conversion_universe,ig_smb_ads_holdout_2018_h2_u" +
            "niverse,ig_android_camera_post_smile_low_end_universe,ig_android_profile_follow_tab_hashtag_row_universe,ig_" +
            "android_watch_and_more_redesign,igtv_feed_previews,ig_android_direct_group_admin_tools,ig_android_live_real" +
            "time_comments_universe,ig_android_vc_renderer_type_universe,ig_android_purx_native_checkout_universe,ig_ca" +
            "mera_android_filter_optmizations,change_invite_settings_copy_universe,ig_android_downloadable_slam,ig_androi" +
            "d_igds_edit_profile_fields,ig_android_business_transaction_in_stories_creator,ig_android_rounded_corner_frame" +
            "layout_perf_fix,android_cameracore_ard_ig_integration,ig_video_experimental_encoding_consumption_universe,ig_a" +
            "ndroid_iab_autofill,ig_android_location_page_intent_survey,ig_camera_android_segmentation_async_universe,ig_camera_andro" +
            "id_target_recognition_universe,ig_camera_android_skip_camera_initialization_open_to_post_capture,ig" +
            "_android_stories_samsung_sharing_integration,ig_android_create_page_on_top_universe,ig_android_camera_focus_v2,ig_android" +
            "_hashtag_header_display,android_ig_ard_integration_load_assetmanager_async,ig_discovery_holdout_2019_h1_univ" +
            "erse,ig_android_wellbeing_support_frx_comment_reporting,ig_android_direct_active_now_section_in_share_sheet,ig_androi" +
            "d_user_url_deeplink_fbpage_endpoint,ig_android_ad_holdout_watchandmore_universe,ig_android_shopping_hero_carousel_load_an" +
            "imation,ig_android_explore_use_shopping_endpoint,ig_android_image_upload_skip_queue_only_on_wifi,ig_android_ad_wat" +
            "chbrowse_carousel_universe,ig_android_camera_new_post_smile_universe,ig_android_stories_browser_warmup_background_" +
            "universe,ig_android_interactions_show_verified_badge_for_preview_comments_universe,ig_android_shopping_signup_red" +
            "esign_universe,ig_android_direct_hide_inbox_header,allow_publish_page_universe,ig_android_experimental_onetap_dia" +
            "logs_universe,ig_promote_ppe_v2_universe,ig_android_direct_multi_upload_universe,ig_camera_text_mode_composer_con" +
            "troller_opt_universe,ig_explore_2019_h1_video_autoplay_resume,ig_android_multi_capture_camera,ig_android_video_up" +
            "load_quality_qe1,ig_android_fb_family_navigation_badging_user,ig_android_follow_requests_copy_improvements,ig_android" +
            "_save_collaborative_collections,ig_camera_android_profile_ar_notification_universe,coupon_price_test_boost_instagr" +
            "am_media_acquisition_universe,ig_media_geo_gating,ig_android_video_outputsurface_handlerthread_universe,ig_android_d" +
            "irect_sort_thread_members_list,ig_android_country_code_fix_universe,ig_perf_android_holdout_2018_h1,ig_android_storie" +
            "s_music_overlay,ig_android_enable_lean_crash_reporting_universe,ig_android_resumable_downloads_logging_universe,ig_an" +
            "droid_comments_notifications_universe,ig_android_low_latency_consumption_universe,ig_android_render_output_surface_t" +
            "imeout_universe,ig_android_unified_iab_logging_universe,ig_android_aggressive_cookie,ig_android_offline_mode_holdout" +
            ",ig_android_realtime_mqtt_logging,ig_android_rainbow_hashtags,ig_android_react_native_universe_kill_switch,ig_android" +
            "_video_ssim_report_universe,ig_android_logged_in_delta_migration,ig_android_stories_gallery_video_segmentation,ig_and" +
            "roid_ad_screenshot_detection,ig_android_cache_network_util,ig_android_stories_in_feed_preview_notify_fix_universe,ig_" +
            "biz_graph_remove_unowned_page_v2,ig_android_direct_business_holdout,ig_android_xposting_upsell_directly_after_sharing" +
            "_to_story,ig_android_camera_focus_low_end_universe,ig_android_interactions_new_comment_like_pos_universe,ig_android_s" +
            "uggested_highlights,ig_android_direct_combine_action_logs,ig_android_leak_detector_upload_universe,ig_android_carousel_pr" +
            "efetch_bumping,ig_android_branded_content_access_upsell,ig_android_follow_button_in_story_viewers_list,ig_android_vc_bac" +
            "kground_call_toast_universe,ig_hashtag_following_holdout_universe,ig_promote_default_destination_universe,ig_android_di" +
            "rect_reel_options_entry_point_2_universe,mi_viewpoint_viewability_universe,enable_creator_account_conversion_v0_anima" +
            "tion,ig_android_location_page_info_page_upsell,ig_android_direct_organize_thread_details_members,ig_android_storie" +
            "s_cta_animation_fix,android_ard_ig_download_manager_v2,ig_direct_reshare_sharesheet_ranking,ig_music_dash,ig_andro" +
            "id_fb_url_universe,ig_android_reel_raven_video_segmented_upload_universe,ig_android_promote_native_migration_universe,invite_friends_by_messenger_in_setting_universe,ig_android_fb_sync_options_universe,ig_android_stories_skip_seen_state_update_for_direct_stories,ig_camera_android_badge_face_effects_universe,ig_android_recommend_accounts_destination_routing_fix,ig_android_fix_prepare_direct_push,ig_direct_android_larger_media_reshare_style,ig_android_enable_automated_instruction_text_ar,ig_android_multi_author_story_reshare_universe,ig_android_building_aymf_universe,ig_android_internal_sticker_universe,ig_traffic_routing_universe,ig_camera_async_gallerycontroller_universe,ig_android_igtv_always_show_browse_ui,ig_android_page_claim_deeplink_qe,ig_android_video_controls_universe,ig_explore_2018_h2_account_rec_deduplication_android,ig_android_logging_metric_universe_v2,ig_android_xposting_newly_fbc_people,ig_android_visualcomposer_inapp_notification_universe,ig_android_do_not_show_social_context_on_follow_list_universe,ig_android_contact_point_upload_rate_limit_killswitch,ig_android_webrtc_encoder_factory_universe,ig_android_search_impression_logging,ig_android_handle_username_in_media_urls_universe,ig_android_qpl_class_marker,ig_android_fix_profile_pic_from_fb_universe,ig_android_sso_kototoro_app_universe,ig_android_profile_picture_change_fbupload,ig_android_shopping_pdp_hero_carousel,ig_android_direct_unread_count_badge,ig_android_profile_thumbnail_impression,ig_android_igtv_autoplay_on_prepare,ig_file_based_session_handler_2_universe,ig_branded_content_tagging_upsell,ig_android_clear_inflight_image_request,ig_android_direct_close_friends_reshare_sheet,ig_android_stories_infeed_lower_threshold_launch,ig_android_direct_cold_start_improvements,ig_android_main_feed_video_countdown_timer,ig_android_live_ama_universe,ig_android_external_gallery_import_affordance,ig_search_hashtag_content_advisory_remove_snooze,ig_android_category_search_edit_profile,ig_android_updatelistview_on_loadmore,ig_payment_checkout_info,ig_android_optic_new_zoom_controller,ig_android_photos_qpl,ig_stories_ads_delivery_rules,ig_android_video_upload_iframe_interval,ig_business_new_value_prop_universe,ig_android_power_metrics,ig_android_show_profile_picture_upsell_in_reel_universe,ig_discovery_holdout_universe,ig_android_direct_import_google_photos2,ig_direct_feed_media_sticker_universe,ig_android_stories_collapse_seen_segments,ig_android_suggested_users_background,ig_android_fetch_xpost_setting_in_camera_fully_open,ig_android_hashtag_discover_tab,ig_android_netgo_cta,ig_android_viewpoint_netego_universe,ig_android_stories_separate_overlay_creation,ig_android_ads_bottom_sheet_report_flow,ig_android_login_onetap_upsell_universe,ig_android_iris_improvements,enable_creator_account_conversion_v0_universe,ig_android_biz_conversion_naming_test,ig_android_test_not_signing_address_book_unlink_endpoint,ig_android_direct_tabbed_media_picker,ig_ei_option_setting_universe,ig_camera_android_ar_platform_universe,ig_android_stories_viewer_prefetch_improvements,ig_android_livewith_liveswap_optimization_universe,ig_android_camera_leak,ig_android_feed_core_unified_tags_universe,ig_android_optic_camera_warmup,ig_camera_android_supported_capabilities_api_universe,ig_stories_rainbow_ring,ig_android_place_search_profile_image,ig_android_interactions_in_feed_comment_view_universe,ig_story_ads_reel_position_fix,android_cameracore_safe_makecurrent_ig,ig_android_oreo_hardware_bitmap,ig_android_analytics_diagnostics_universe,ig_direct_android_mentions_sender,ig_android_whats_app_contact_invite_universe,ig_android_video_scrubber_thumbnail_universe,ig_android_insights_creative_tutorials_universe,ig_graph_management_holdout_universe,ig_android_stories_reaction_setup_delay_universe,ig_shopping_hero_carousel_modal_permalink,ig_android_insights_creation_growth_universe,ig_android_profile_unified_follow_view,ig_android_direct_admin_tools_lite,ig_android_collect_os_usage_events_universe,ig_android_vc_face_effects_universe,ig_android_fbpage_on_profile_side_tray,ig_android_igtv_refresh_tv_guide_interval,ig_android_direct_thread_content_picker,ig_android_notif_improvement_universe,ig_android_hashtag_remove_share_hashtag,ig_android_stories_music_broadcast_receiver,ig_android_fb_profile_integration_fbnc_universe,ig_android_low_data_mode,ig_android_business_profile_entrypoint,ig_android_direct_bump_active_threads,ig_fb_graph_differentiation_control,ig_android_show_create_content_pages_universe,ig_android_igsystrace_universe,ig_feed_content_universe,ig_android_keep_browser_process_alive_universe,ig_android_disk_usage_logging_universe,ig_android_new_contact_invites_entry_points_universe,ig_android_search_without_typed_hashtag_autocomplete,ig_android_video_product_specific_abr,ig_android_ad_redesign_iab_universe,ig_android_viewmaster_phase_3_cap_effects,ig_android_stories_loading_automatic_retry,ig_android_fb_connect_follow_invite_flow,ig_android_invite_list_button_redesign_universe,ig_android_react_native_email_sms_settings_universe,ig_hero_player,ag_family_bridges_2018_h2_holdout,ig_promote_net_promoter_score_universe,ig_android_save_auto_sharing_to_fb_option_on_server,ig_android_direct_last_seen_message_indicator,aymt_instagram_promote_flow_abandonment_ig_universe,ig_android_whitehat_options_universe,ig_android_keyword_media_serp_page,ig_android_delete_ssim_compare_img_soon,ig_android_felix_video_upload_length,ig_story_ads_tap_logging,android_cameracore_preview_frame_listener2_ig_universe,ig_android_direct_message_follow_button,ig_android_biz_conversion_suggest_biz_nux,ig_android_direct_remove_visual_messages_nuxs,ig_explore_2018_finite_chain_android_universe,ig_android_analytics_background_uploader_schedule,ig_camera_android_boomerang_attribution_universe,ig_android_feed_survey_viewpoint,ig_android_igtv_browse_long_press,ig_android_profile_neue_infra_rollout_universe,ig_android_instacrash_detection,ig_android_stories_weblink_creation,ig_profile_company_holdout_h2_2018,ig_android_ads_manager_pause_resume_ads_universe,ig_android_vc_capture_universe,ig_android_direct_smaller_permanent_media_in_thread_test,ig_nametag_local_ocr_universe,ig_android_stories_media_seen_batching_universe,ig_branded_content_share_to_facebook,ig_android_interactions_nav_to_permalink_followup_universe,ig_camera_discovery_surface_universe,ig_android_dismiss_sheet_on_pop_to_root_universe,ig_android_direct_segmented_video,ig_android_shopping_filters,instagram_stories_time_fixes,ig_android_direct_mark_as_read_notif_action,ig_android_stories_async_view_inflation_universe,ig_android_stories_recently_captured_universe,ig_android_interactions_comment_like_for_all_feed_universe,ig_cameracore_android_new_optic_camera2,ig_fb_graph_differentiation_top_k_fb_coefficients,ig_android_fbc_upsell_on_dp_first_load,ig_android_rename_share_option_in_dialog_menu_universe,ig_android_video_call_participant_state_caller_universe,ig_android_privacy_and_security_settings_redesign,ig_android_business_attribute_sync,ig_camera_android_bg_processor,ig_android_view_and_likes_cta_universe,ig_android_redirect_to_web_on_oembed_fail_universe,ig_android_optic_new_focus_controller,ig_android_direct_presence_for_groups,ig_android_optic_new_features_implementation,ig_android_search_hashtag_badges,ig_android_stories_reel_interactive_tap_target_size,ig_android_video_live_trace_universe,ig_android_igtv_browse_with_pip_v2,ig_android_igtv_feed_banner_universe,ig_android_unfollow_from_main_feed_v2,ig_android_add_invite_friends_icon_universe,ig_android_igtv_garfield,ig_android_self_story_setting_option_in_menu,ig_android_render_cleanup,ig_android_camera_ar_platform_details_view_universe,ig_android_story_real_time_ad,ig_android_contact_invites,ig_android_hybrid_bitmap_v4,ufi_share,ig_android_camera_should_lazy_init_persistent_dial,ig_android_direct_remix_visual_messages,ig_quick_story_placement_validation_universe,ig_android_custom_story_import_intent,ig_android_stories_mention_bootstrap_surface,ig_android_live_qa_broadcaster_v1_universe,ig_android_biz_new_choose_category,ig_android_view_info_universe,ig_android_camera_upsell_dialog,ig_android_direct_albums,ig_android_business_transaction_in_stories_consumer,ig_android_dead_code_detection,ig_android_stories_time_elapsed_logging_fix,ig_android_scroll_stories_tray_to_front_when_stories_ready,ig_android_ad_watchbrowse_universe,ig_android_pbia_proxy_profile_universe,ig_android_qp_kill_switch,instagram_android_stories_sticker_tray_redesign,ig_android_branded_content_access_tag,ig_camera_android_superzoom_icon_position_universe,ig_android_gap_rule_enforcer_universe,ig_android_business_cross_post_with_biz_id_infra,ig_android_direct_delete_or_block_from_message_requests,ig_android_photo_invites,ig_android_reel_tray_item_impression_logging_viewpoint,ig_account_identity_2018_h2_lockdown_phone_global_holdout,ig_android_direct_left_aligned_navigation_bar,ig_android_high_res_gif_stickers,ig_android_feed_load_more_viewpoint_universe,ig_android_stories_reshare_reply_msg,ig_close_friends_v4,ig_android_ads_history_universe,ig_android_pigeon_sampling_runnable_check,ig_android_comments_composer_newline_universe,ig_rtc_use_dtls_srtp,ig_promote_media_picker_universe,ig_direct_holdout_h2_2018,ig_android_pending_media_file_registry,ig_android_wab_adjust_resize_universe,ig_camera_android_facetracker_v12_universe,ig_android_bitmap_attribution_check,ig_android_camera_ar_effects_low_storage_universe,ig_android_direct_media_forwarding,ig_android_profile_add_profile_pic_universe,ig_android_ig_to_fb_sync_universe,ig_android_reel_viewer_data_buffer_size,ig_android_audience_control,ig_android_direct_presence_digest_improvements,ig_android_stories_cross_sharing_to_fb_holdout_universe,ig_android_enable_main_feed_reel_tray_preloading,ig_android_ad_view_ads_native_universe,ig_android_profile_neue_universe,ig_android_igtv_whitelisted_for_web,ig_company_profile_holdout,ig_rti_inapp_notifications_universe,ig_android_vc_join_timeout_universe,ig_android_add_ci_upsell_in_normal_account_chaining_universe,ig_android_feed_core_ads_2019_h1_holdout_universe,ig_close_friends_v4_global,ig_android_share_publish_page_universe,ig_android_new_camera_design_universe,ig_direct_max_participants,ig_promote_hide_local_awareness_universe,ar_engine_audio_service_fba_decoder_ig,ig_android_shopping_home_universe,ar_engine_audio_fba_integration_instagram,ig_android_igtv_save,ig_android_explore_lru_cache,ig_android_graphql_survey_new_proxy_universe,ig_camera_android_try_on_camera_universe,ig_android_follower_following_whatsapp_invite_universe,ig_android_fs_creation_flow_tweaks,ig_android_ad_watchbrowse_cta_universe,ig_android_camera_new_tray_behavior_universe,ig_direct_blocking_redesign_universe,ig_android_downloadable_vp8_module,ig_android_claim_location_page,ig_android_stories_gutter_width_universe,ig_android_story_ads_2019_h1_holdout_universe,ig_android_3pspp,ig_android_cache_timespan_objects,ig_timestamp_public_test,ig_android_fb_profile_integration_universe,ig_android_feed_auto_share_to_facebook_dialog,ig_android_skip_button_content_on_connect_fb_universe,ig_android_network_perf_qpl_ppr,ig_android_post_live,ig_android_interactions_inline_composer_extensions_universe,ig_camera_android_focus_attribution_universe,ig_camera_async_space_validation_for_ar,ig_android_su_follow_back,ig_android_prefetch_notification_data,ig_android_iab_clickid_universe,ig_story_camera_reverse_video_experiment,ig_android_live_use_timestamp_normalizer,ig_android_profile_lazy_load_carousel_media,ig_android_webrtc_icerestart_on_failure_universe,ig_android_stories_question_sticker_music_format,ig_android_vpvd_impressions_universe,ig_android_payload_based_scheduling,ig_pacing_overriding_universe,ig_android_direct_allow_multiline_composition,ig_android_interactions_emoji_extension_followup_universe,ig_android_story_ads_direct_cta_universe,ig_android_q3lc_transparency_control_settings,ig_stories_selfie_sticker,ig_android_sso_use_trustedapp_universe,ig_android_ad_increase_story_adpreload_priority_universe,ig_android_direct_search_bar_redesign,ig_android_stories_music_lyrics,ig_android_stories_music_awareness_universe,ig_android_stories_reels_tray_media_count_check,ig_android_new_fb_page_selection,ig_video_holdout_h2_2017,ig_background_prefetch,ig_camera_android_focus_in_post_universe,ig_android_time_spent_dashboard,ig_android_story_sharing_universe,ig_nelson_strings,ig_android_direct_vm_activity_sheet,ig_promote_political_ads_universe,ig_android_stories_auto_retry_reels_media_and_segments,ig_android_camera_effects_initialization_universe,ig_promote_post_insights_entry_universe,ig_android_direct_thread_store_rewrite,ig_branded_content_paid_branded_content,ig_android_direct_presence_media_viewer,ig_android_ad_iab_qpl_kill_switch_universe,ig_android_live_subscribe_user_level_universe,ig_android_igtv_creation_flow,ig_android_vc_sounds_universe,ig_android_video_call_finish_universe,ig_camera_android_cache_format_picker_children,ig_android_direct_autofocus_threadcomposer,ig_android_vc_early_attempt_reporting_universe,direct_unread_reminder_qe,ig_android_direct_mqtt_send,ig_android_self_story_button_non_fbc_accounts,ig_android_vc_directapp_integration_universe,ig_android_explore_discover_people_entry_point_universe,ig_android_live_webrtc_livewith_params,ig_android_direct_inbox_typing_indicator,ig_feed_experience,ig_android_direct_activator_cards,ig_android_vc_codec_settings,ig_promote_prefill_destination_universe,ig_android_appstate_logger,ig_android_profile_leaks_holdouts,ig_android_video_cached_bandwidth_estimate,ig_promote_insights_video_views_universe,ig_android_discover_interests_universe,ig_android_camera_gallery_upload_we_universe,ig_android_business_category_sticky_header_qe,ig_android_dismiss_recent_searches,ig_android_fb_link_ui_polish_universe,ig_stories_music_sticker,ig_android_tags_unification_universe,ig_android_stories_mixed_attribution_universe,ig_android_nametag_effect_deeplink_universe,ig_android_direct_reshare_media_preview_in_share_sheet,ig_android_optic_surface_texture_cleanup,ig_android_vc_use_timestamp_normalizer,ig_android_business_remove_unowned_fb_pages,ig_android_stories_combined_asset_search,ig_promote_enter_error_screens_universe,ig_stories_allow_camera_actions_while_recording,ig_android_analytics_mark_events_as_offscreen,ig_shopping_checkout_mvp_experiment,ig_android_video_fit_scale_type_igtv,ig_android_direct_pending_media,ig_android_scroll_main_feed,ig_android_optic_feature_testing,ig_android_intialization_chunk_410,ig_android_story_ads_default_long_video_duration,ig_android_vc_start_call_minimized_universe,ig_android_recognition_tracking_thread_prority_universe,ig_android_stories_music_sticker_position,ig_android_optic_photo_cropping_fixes,ig_android_direct_character_limit,ig_camera_regiontracking_use_similarity_tracker_for_scaling,ig_android_interactions_media_breadcrumb,ig_android_vc_cowatch_config_universe,ig_android_nametag_save_experiment_universe,ig_android_refreshable_list_view_check_spring,ig_android_biz_endpoint_switch,ig_android_direct_continuous_capture,ig_android_comments_direct_reply_to_author,ig_android_fs_new_gallery,ig_android_stories_hide_retry_button_during_loading_launch,ig_android_shopping_hero_carousel_prefetching,ig_android_remove_follow_all_fb_list,ig_android_vc_webrtc_params,ig_android_specific_story_sharing,ig_android_claim_or_connect_page_on_xpost,ig_android_anr,ig_android_optic_new_architecture,ig_androi" +
            "d_stories_viewer_as_modal_high_end_launch,ig_android_local_info_page,ig_new_eof_demarcator_universe";

        public static string ACCEPT_LANGUAGE = "en-US";

        public const string FACEBOOK_LOGIN_URI = "https://m.facebook.com/v2.3/dialog/oauth?access_token=&client_id=124024574287414&e2e={0}&scope=email&default_audience=friends&redirect_uri=fbconnect://success&display=touch&response_type=token,signed_request&return_scopes=true";
        public const string FACEBOOK_TOKEN = "https://graph.facebook.com/me?access_token={0}&fields=id,is_employee,name";
        public const string FACEBOOK_TOKEN_PICTURE = "https://graph.facebook.com/me?access_token={0}&fields=picture";

        public const string FACEBOOK_USER_AGENT = "Mozilla/5.0 (Linux; Android {0}; {1} Build/{2}; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36";
        public const string FACEBOOK_USER_AGENT_DEFAULT = "Mozilla/5.0 (Linux; Android 7.0; PRA-LA1 Build/HONORPRA-LA1; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36";

        public const string WEB_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 OPR/57.0.3098.116";

        public const string ERROR_OCCURRED = "Oops, an error occurred";

        public static readonly Uri BaseInstagramUri = new Uri(BASE_INSTAGRAM_API_URL);

        #endregion Main

        #region Account endpoints constants

        public const string ACCOUNTS_2FA_LOGIN = API_SUFFIX + "/accounts/two_factor_login/";
        public const string ACCOUNTS_2FA_LOGIN_AGAIN = API_SUFFIX + "/accounts/send_two_factor_login_sms/";
        public const string ACCOUNTS_CHANGE_PROFILE_PICTURE = API_SUFFIX + "/accounts/change_profile_picture/";
        public const string ACCOUNTS_CHECK_PHONE_NUMBER = API_SUFFIX + "/accounts/check_phone_number/";
        public const string ACCOUNTS_CONTACT_POINT_PREFILL = API_SUFFIX + "/accounts/contact_point_prefill/";
        public const string ACCOUNTS_CREATE = API_SUFFIX + "/accounts/create/";
        public const string ACCOUNTS_CREATE_VALIDATED = API_SUFFIX + "/accounts/create_validated/";
        public const string ACCOUNTS_DISABLE_SMS_TWO_FACTOR = API_SUFFIX + "/accounts/disable_sms_two_factor/";
        public const string ACCOUNTS_EDIT_PROFILE = API_SUFFIX + "/accounts/edit_profile/";
        public const string ACCOUNTS_ENABLE_SMS_TWO_FACTOR = API_SUFFIX + "/accounts/enable_sms_two_factor/";
        public const string ACCOUNTS_GET_COMMENT_FILTER = API_SUFFIX + "/accounts/get_comment_filter/";
        public const string ACCOUNTS_LOGIN = API_SUFFIX + "/accounts/login/";
        public const string ACCOUNTS_LOGOUT = API_SUFFIX + "/accounts/logout/";
        public const string ACCOUNTS_READ_MSISDN_HEADER = API_SUFFIX + "/accounts/read_msisdn_header/";
        public const string ACCOUNTS_REGEN_BACKUP_CODES = API_SUFFIX + "/accounts/regen_backup_codes/";
        public const string ACCOUNTS_REMOVE_PROFILE_PICTURE = API_SUFFIX + "/accounts/remove_profile_picture/";
        public const string ACCOUNTS_REQUEST_PROFILE_EDIT = API_SUFFIX + "/accounts/current_user/?edit=true";
        public const string ACCOUNTS_SECURITY_INFO = API_SUFFIX + "/accounts/account_security_info/";
        public const string ACCOUNTS_SEND_CONFIRM_EMAIL = API_SUFFIX + "/accounts/send_confirm_email/";
        public const string ACCOUNTS_SEND_RECOVERY_EMAIL = API_SUFFIX + "/accounts/send_recovery_flow_email/";
        public const string ACCOUNTS_SEND_SIGNUP_SMS_CODE = API_SUFFIX + "/accounts/send_signup_sms_code/";
        public const string ACCOUNTS_SEND_SMS_CODE = API_SUFFIX + "/accounts/send_sms_code/";
        public const string ACCOUNTS_SEND_TWO_FACTOR_ENABLE_SMS = API_SUFFIX + "/accounts/send_two_factor_enable_sms/";
        public const string ACCOUNTS_SET_BIOGRAPHY = API_SUFFIX + "/accounts/set_biography/";
        public const string ACCOUNTS_SET_PHONE_AND_NAME = API_SUFFIX + "/accounts/set_phone_and_name/";
        public const string ACCOUNTS_SET_PRESENCE_DISABLED = API_SUFFIX + "/accounts/set_presence_disabled/";
        public const string ACCOUNTS_UPDATE_BUSINESS_INFO = API_SUFFIX + "/accounts/update_business_info/";
        public const string ACCOUNTS_USERNAME_SUGGESTIONS = API_SUFFIX + "/accounts/username_suggestions/";
        public const string ACCOUNTS_VALIDATE_SIGNUP_SMS_CODE = API_SUFFIX + "/accounts/validate_signup_sms_code/";
        public const string ACCOUNTS_VERIFY_SMS_CODE = API_SUFFIX + "/accounts/verify_sms_code/";
        public const string CHANGE_PASSWORD = API_SUFFIX + "/accounts/change_password/";
        public const string CURRENTUSER = API_SUFFIX + "/accounts/current_user/?edit=true";
        public const string SET_ACCOUNT_PRIVATE = API_SUFFIX + "/accounts/set_private/";
        public const string SET_ACCOUNT_PUBLIC = API_SUFFIX + "/accounts/set_public/";
        public const string ACCOUNTS_CONVERT_TO_PERSONAL = API_SUFFIX + "/accounts/convert_to_personal/";
        public const string ACCOUNTS_CREATE_BUSINESS_INFO = API_SUFFIX + "/accounts/create_business_info/";
        public const string ACCOUNTS_GET_PRESENCE = API_SUFFIX + "/accounts/get_presence_disabled/";
        public const string ACCOUNTS_GET_BLOCKED_COMMENTERS = API_SUFFIX + "/accounts/get_blocked_commenters/";
        public const string ACCOUNTS_SET_BLOCKED_COMMENTERS = API_SUFFIX + "/accounts/set_blocked_commenters/";

        #endregion Account endpoint constants

        #region Business endpoints constants

        /// <summary>
        /// /api/v1/business/instant_experience/get_ix_partners_bundle/?signed_body=b941ff07b83716087710019790b3529ab123c8deabfb216e056651e9cf4b4ca7.{}&ig_sig_key_version=4
        /// </summary>
        public const string BUSINESS_INSTANT_EXPERIENCE = API_SUFFIX + "/business/instant_experience/get_ix_partners_bundle/?signed_body={0}&ig_sig_key_version={1}";

        public const string BUSINESS_SET_CATEGORY = API_SUFFIX + "/business/account/set_business_category/";
        public const string BUSINESS_VALIDATE_URL = API_SUFFIX + "/business/instant_experience/ix_validate_url/";
        public const string BUSINESS_BRANDED_GET_SETTINGS = API_SUFFIX + "/business/branded_content/get_whitelist_settings/";
        public const string BUSINESS_BRANDED_USER_SEARCH = API_SUFFIX + "/users/search/?q={0}&count={1}&branded_content_creator_only=true";
        public const string BUSINESS_BRANDED_UPDATE_SETTINGS = API_SUFFIX + "/business/branded_content/update_whitelist_settings/";
        public const string BUSINESS_CONVERT_TO_BUSINESS_ACCOUNT = API_SUFFIX + "/business_conversion/get_business_convert_social_context/";

        #endregion Business endpoints constants

        #region Collection endpoints constants

        public const string COLLECTION_CREATE_MODULE = API_SUFFIX + "/collection_create/";
        public const string CREATE_COLLECTION = API_SUFFIX + "/collections/create/";
        public const string DELETE_COLLECTION = API_SUFFIX + "/collections/{0}/delete/";
        public const string EDIT_COLLECTION = API_SUFFIX + "/collections/{0}/edit/";
        public const string FEED_SAVED_ADD_TO_COLLECTION_MODULE = "feed_saved_add_to_collection/";
        public const string GET_LIST_COLLECTIONS = API_SUFFIX + "/collections/list/";

        #endregion Collection endpoints constants

        #region Consent endpoints constants

        public const string CONSENT_NEW_USER_FLOW = API_SUFFIX + "/consent/new_user_flow/";
        public const string CONSENT_NEW_USER_FLOW_BEGINS = API_SUFFIX + "/consent/new_user_flow_begins/";
        public const string CONSENT_EXISTING_USER_FLOW = API_SUFFIX + "/consent/existing_user_flow/";



        #endregion Consent endpoints constants

        #region Direct endpoints constants

        public const string DIRECT_BROADCAST_CONFIGURE_VIDEO = API_SUFFIX + "/direct_v2/threads/broadcast/configure_video/";
        public const string DIRECT_BROADCAST_CONFIGURE_PHOTO = API_SUFFIX + "/direct_v2/threads/broadcast/configure_photo/";
        public const string DIRECT_BROADCAST_LINK = API_SUFFIX + "/direct_v2/threads/broadcast/link/";
        public const string DIRECT_BROADCAST_THREAD_LIKE = API_SUFFIX + "/direct_v2/threads/broadcast/like/";
        public const string DIRECT_BROADCAST_LOCATION = API_SUFFIX + "/direct_v2/threads/broadcast/location/";
        public const string DIRECT_BROADCAST_MEDIA_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/media_share/?media_type={0}";
        public const string DIRECT_BROADCAST_PROFILE = API_SUFFIX + "/direct_v2/threads/broadcast/profile/";
        public const string DIRECT_BROADCAST_REACTION = API_SUFFIX + "/direct_v2/threads/broadcast/reaction/";
        public const string DIRECT_BROADCAST_REEL_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/reel_share/?media_type={0}";
        public const string DIRECT_BROADCAST_UPLOAD_PHOTO = API_SUFFIX + "/direct_v2/threads/broadcast/upload_photo/";
        public const string DIRECT_BROADCAST_HASHTAG = API_SUFFIX + "/direct_v2/threads/broadcast/hashtag/";
        public const string DIRECT_BROADCAST_LIVE_VIEWER_INVITE = API_SUFFIX + "/direct_v2/threads/broadcast/live_viewer_invite/";
        public const string DIRECT_BROADCAST_SHARE_VOICE = API_SUFFIX + "/direct_v2/threads/broadcast/share_voice/";
        public const string DIRECT_BROADCAST_ANIMATED_MEDIA = API_SUFFIX + "/direct_v2/threads/broadcast/animated_media/";
        public const string DIRECT_BROADCAST_FELIX_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/felix_share/";

        /// <summary>
        /// post data:
        /// <para>use_unified_inbox=true</para>
        /// <para>recipient_users= user ids split with comma.: ["user id1","user id2","..."]</para>
        /// </summary>
        public const string DIRECT_CREATE_GROUP = API_SUFFIX + "/direct_v2/create_group_thread/";

        public const string DIRECT_PRESENCE = API_SUFFIX + "/direct_v2/get_presence/";
        public const string DIRECT_SHARE = API_SUFFIX + "/direct_share/inbox/";
        public const string DIRECT_STAR = API_SUFFIX + "/direct_v2/threads/{0}/label/";
        public const string DIRECT_THREAD_HIDE = API_SUFFIX + "/direct_v2/threads/{0}/hide/";
        public const string DIRECT_THREAD_ADD_USER = API_SUFFIX + "/direct_v2/threads/{0}/add_user/";
        /// <summary>
        ///  deprecated
        /// </summary>
        public const string DIRECT_THREAD_ITEM_SEEN = API_SUFFIX + "/direct_v2/visual_threads/{0}/item_seen/";
        public const string DIRECT_THREAD_SEEN = API_SUFFIX + "/direct_v2/threads/{0}/items/{1}/seen/";
        public const string DIRECT_THREAD_LEAVE = API_SUFFIX + "/direct_v2/threads/{0}/leave/";
        public const string DIRECT_THREAD_MESSAGES_MUTE = API_SUFFIX + "/direct_v2/threads/{0}/mute/";
        public const string DIRECT_THREAD_MESSAGES_UNMUTE = API_SUFFIX + "/direct_v2/threads/{0}/unmute/";
        public const string DIRECT_THREAD_UPDATE_TITLE = API_SUFFIX + "/direct_v2/threads/{0}/update_title/";
        public const string DIRECT_UNSTAR = API_SUFFIX + "/direct_v2/threads/{0}/unlabel/";
        public const string GET_DIRECT_INBOX = API_SUFFIX + "/direct_v2/inbox/";
        public const string GET_DIRECT_PENDING_INBOX = API_SUFFIX + "/direct_v2/pending_inbox/";
        public const string GET_DIRECT_SHARE_USER = API_SUFFIX + "/direct_v2/threads/broadcast/profile/";
        public const string GET_DIRECT_TEXT_BROADCAST = API_SUFFIX + "/direct_v2/threads/broadcast/text/";
        public const string GET_DIRECT_THREAD = API_SUFFIX + "/direct_v2/threads/{0}/";
        public const string GET_DIRECT_THREAD_APPROVE = API_SUFFIX + "/direct_v2/threads/{0}/approve/";
        public const string GET_DIRECT_THREAD_APPROVE_MULTIPLE = API_SUFFIX + "/direct_v2/threads/approve_multiple/";
        public const string GET_DIRECT_THREAD_DECLINE = API_SUFFIX + "/direct_v2/threads/{0}/decline/";
        public const string GET_DIRECT_THREAD_DECLINE_MULTIPLE = API_SUFFIX + "/direct_v2/threads/decline_multiple/";
        public const string GET_DIRECT_THREAD_DECLINEALL = API_SUFFIX + "/direct_v2/threads/decline_all/";
        public const string GET_PARTICIPANTS_RECIPIENT_USER = API_SUFFIX + "/direct_v2/threads/get_by_participants/?recipient_users=[{0}]";
        public const string GET_RANK_RECIPIENTS_BY_USERNAME = API_SUFFIX + "/direct_v2/ranked_recipients/?mode={1}&show_threads=true&query={0}";
        public const string GET_RANKED_RECIPIENTS = API_SUFFIX + "/direct_v2/ranked_recipients/";
        public const string GET_RECENT_RECIPIENTS = API_SUFFIX + "/direct_share/recent_recipients/";
        public const string STORY_SHARE = API_SUFFIX + "/direct_v2/threads/broadcast/story_share/?media_type={0}";
        public const string DIRECT_THREAD_DELETE_MESSAGE = API_SUFFIX + "/direct_v2/threads/{0}/items/{1}/delete/"; 

        #endregion Direct endpoints constants

        #region Discover endpoints constants

        public const string DISCOVER_AYML = API_SUFFIX + "/discover/ayml/";
        public const string DISCOVER_CHAINING = API_SUFFIX + "/discover/chaining/?target_id={0}";
        public const string DISCOVER_EXPLORE = API_SUFFIX + "/discover/explore/";
        public const string DISCOVER_TOPICAL_EXPLORE = API_SUFFIX + "/discover/topical_explore/";
        public const string DISCOVER_FETCH_SUGGESTION_DETAILS = API_SUFFIX + "/discover/fetch_suggestion_details/?target_id={0}&chained_ids={1}&media_info_count=0&include_social_context=1&use_full_media_info=0";
        public const string DISCOVER_TOP_LIVE = API_SUFFIX + "/discover/top_live/";
        public const string DISCOVER_TOP_LIVE_STATUS = API_SUFFIX + "/discover/top_live_status/";
        public const string DISCOVER_DISMISS_SUGGESTION = API_SUFFIX + "/discover/dismiss_suggestion/";
        public const string DISCOVER_EXPLORE_REPORT = API_SUFFIX + "/discover/explore_report/";


        public const string DISCOVER_SURFACE_WITH_SU = API_SUFFIX + "/discover/surface_with_su/";


        #endregion Discover endpoints constants

        #region FBSearch endpoints constants

        public const string FBSEARCH_CLEAR_SEARCH_HISTORY = API_SUFFIX + "/fbsearch/clear_search_history/";
        public const string FBSEARCH_GET_HIDDEN_SEARCH_ENTITIES = API_SUFFIX + "/fbsearch/get_hidden_search_entities/";

        public const string FBSEARCH_HIDE_SEARCH_ENTITIES = API_SUFFIX + "/fbsearch/hide_search_entities/";

        /// <summary>
        /// get nearby places
        /// </summary>
        public const string FBSEARCH_PLACES = API_SUFFIX + "/fbsearch/places/";
        
        public const string FBSEARCH_PROFILE_SEARCH = API_SUFFIX + "/fbsearch/profile_link_search/?q={0}&count={1}";
        public const string FBSEARCH_RECENT_SEARCHES = API_SUFFIX + "/fbsearch/recent_searches/";
        public const string FBSEARCH_SUGGESTED_SEARCHS = API_SUFFIX + "/fbsearch/suggested_searches/?type={0}";
        public const string FBSEARCH_TOPSEARCH = API_SUFFIX + "/fbsearch/topsearch/";
        public const string FBSEARCH_TOPSEARCH_FALT = API_SUFFIX + "/fbsearch/topsearch_flat/";
        public const string FBSEARCH_TOPSEARCH_FALT_PARAMETER = API_SUFFIX + "/fbsearch/topsearch_flat/?rank_token={0}&timezone_offset={1}&query={2}&context={3}";

        #endregion FBSearch endpoints constants

        #region FB endpoints constants

        public const string FB_ENTRYPOINT_INFO = API_SUFFIX + "/fb/fb_entrypoint_info/";
        public const string FB_FACEBOOK_SIGNUP = API_SUFFIX + "/fb/facebook_signup/";
        public const string FB_GET_INVITE_SUGGESTIONS = API_SUFFIX + "/fb/get_invite_suggestions/";

        #endregion FB endpoints constants

        #region Feed endpoints constants

        public const string FEED_ONLY_ME_FEED = API_SUFFIX + "/feed/only_me_feed/";
        /// <summary>
        /// {0} = rank token <<<<< this endpoint is deprecated
        /// </summary>
        public const string FEED_POPULAR = API_VERSION + "/feed/popular/?people_teaser_supported=1&rank_token={0}&ranked_content=true";

        public const string FEED_PROMOTABLE_MEDIA = API_SUFFIX + "/feed/promotable_media/";
        public const string FEED_REEL_MEDIA = API_SUFFIX + "/feed/reels_media/";
        public const string FEED_SAVED = API_SUFFIX + "/feed/saved/";
        public const string GET_COLLECTION = API_SUFFIX + "/feed/collection/{0}/";
        public const string GET_STORY_TRAY = API_SUFFIX + "/feed/reels_tray/";
        public const string GET_TAG_FEED = API_SUFFIX + "/feed/tag/{0}/";
        public const string GET_USER_STORY = API_SUFFIX + "/feed/user/{0}/reel_media/";
        public const string GET_USER_TAGS = API_SUFFIX + "/usertags/{0}/feed/";
        public const string LIKE_FEED = API_SUFFIX + "/feed/liked/";
        public const string TIMELINEFEED = API_SUFFIX + "/feed/timeline/";
        public const string USER_REEL_FEED = API_SUFFIX + "/feed/user/{0}/reel_media/";
        public const string USEREFEED = API_SUFFIX + "/feed/user/";

        #endregion Feed endpoints constants

        #region Friendship endpoints constants

        public const string FRIENDSHIPS_APPROVE = API_SUFFIX + "/friendships/approve/{0}/";
        public const string FRIENDSHIPS_AUTOCOMPLETE_USER_LIST = API_SUFFIX + "/friendships/autocomplete_user_list/";
        public const string FRIENDSHIPS_BLOCK_USER = API_SUFFIX + "/friendships/block/{0}/";
  
        public const string FRIENDSHIPS_FOLLOW_USER = API_SUFFIX + "/friendships/create/{0}/";
        public const string FRIENDSHIPS_IGNORE = API_SUFFIX + "/friendships/ignore/{0}/";


        public const string FRIENDSHIPS_PENDING_REQUESTS = API_SUFFIX + "/friendships/pending/?rank_mutual=0&rank_token={0}";
        public const string FRIENDSHIPS_REMOVE_FOLLOWER = API_SUFFIX + "/friendships/remove_follower/{0}/";
        /// <summary>
        /// hide your stories from specific users
        /// </summary>
        public const string FRIENDSHIPS_SET_REEL_BLOCK_STATUS = API_SUFFIX + "/friendships/set_reel_block_status/";

        public const string FRIENDSHIPS_SHOW_MANY = API_SUFFIX + "/friendships/show_many/";


        public const string FRIENDSHIPS_UNBLOCK_USER = API_SUFFIX + "/friendships/unblock/{0}/";


        public const string FRIENDSHIPS_FAVORITE = API_SUFFIX + "/friendships/favorite/{0}/";
        public const string FRIENDSHIPS_UNFAVORITE = API_SUFFIX + "/friendships/unfavorite/{0}/";
        public const string FRIENDSHIPS_FAVORITE_FOR_STORIES = API_SUFFIX + "/friendships/favorite_for_stories/{0}/";
        public const string FRIENDSHIPS_UNFAVORITE_FOR_STORIES = API_SUFFIX + "/friendships/unfavorite_for_stories/{0}/";
        public const string FRIENDSHIPS_UNFOLLOW_USER = API_SUFFIX + "/friendships/destroy/{0}/";
        public const string FRIENDSHIPS_USER_FOLLOWERS = API_SUFFIX + "/friendships/{0}/followers/?rank_token={1}";
        public const string FRIENDSHIPS_USER_FOLLOWERS_MUTUALFIRST = API_SUFFIX + "/friendships/{0}/followers/?rank_token={1}&rank_mutual={2}";
        public const string FRIENDSHIPS_USER_FOLLOWING = API_SUFFIX + "/friendships/{0}/following/?rank_token={1}";
        public const string FRIENDSHIPSTATUS = API_SUFFIX + "/friendships/show/";
        public const string FRIENDSHIPS_MARK_USER_OVERAGE = API_SUFFIX + "/friendships/mark_user_overage/{0}/feed/";
        public const string FRIENDSHIPS_MUTE_POST_STORY = API_SUFFIX + "/friendships/mute_posts_or_story_from_follow/";
        public const string FRIENDSHIPS_UNMUTE_POST_STORY = API_SUFFIX + "/friendships/unmute_posts_or_story_from_follow/";
        public const string FRIENDSHIPS_BLOCK_FRIEND_REEL = API_SUFFIX + "/friendships/block_friend_reel/{0}/";
        public const string FRIENDSHIPS_UNBLOCK_FRIEND_REEL = API_SUFFIX + "/friendships/unblock_friend_reel/{0}/";
        public const string FRIENDSHIPS_MUTE_FRIEND_REEL = API_SUFFIX + "/friendships/mute_friend_reel/{0}/";
        public const string FRIENDSHIPS_UNMUTE_FRIEND_REEL = API_SUFFIX + "/friendships/unmute_friend_reel/{0}/";
        public const string FRIENDSHIPS_BLOCKED_REEL = API_SUFFIX + "/friendships/blocked_reels/";
        public const string FRIENDSHIPS_BESTIES = API_SUFFIX + "/friendships/besties/";
        public const string FRIENDSHIPS_BESTIES_SUGGESTIONS = API_SUFFIX + "/friendships/bestie_suggestions/";
        public const string FRIENDSHIPS_SET_BESTIES = API_SUFFIX + "/friendships/set_besties/";

        #endregion Friendships endpoints constants

        #region Graphql, insights [statistics] endpoints constants

        public const string GRAPH_QL = API_SUFFIX + "/ads/graphql/";
        public const string GRAPH_QL_STATISTICS = GRAPH_QL + "?locale={0}&vc_policy=insights_policy&surface={1}";
        public const string INSIGHTS_MEDIA = API_SUFFIX + "/insights/account_organic_insights/?show_promotions_in_landing_page=true&first={0}";
        public const string INSIGHTS_MEDIA_SINGLE = API_SUFFIX + "/insights/media_organic_insights/{0}?{1}={2}";

        #endregion Graphql, insights [statistics] endpoints constants

        #region Highlight endpoints constants

        public const string HIGHLIGHT_CREATE_REEL = API_SUFFIX + "/highlights/create_reel/";
        public const string HIGHLIGHT_DELETE_REEL = API_SUFFIX + "/highlights/{0}/delete_reel/";
        public const string HIGHLIGHT_EDIT_REEL = API_SUFFIX + "/highlights/{0}/edit_reel/";
        public const string HIGHLIGHT_TRAY = API_SUFFIX + "/highlights/{0}/highlights_tray/";

        #endregion Highlight endpoints constants

        #region IgTv (instagram tv) endpoints constants

        public const string IGTV_CHANNEL = API_SUFFIX + "/igtv/channel/";
        public const string IGTV_SEARCH = API_SUFFIX + "/igtv/search/?query={0}";
        public const string IGTV_SUGGESTED_SEARCHES = API_SUFFIX + "/igtv/suggested_searches/";
        public const string IGTV_TV_GUIDE = API_SUFFIX + "/igtv/tv_guide/";
        public const string MEDIA_CONFIGURE_TO_IGTV = API_SUFFIX + "/media/configure_to_igtv/";

        #endregion IgTv (instagram tv) endpoints constants

        #region Language endpoints constants

        public const string LANGUAGE_TRANSLATE = API_SUFFIX + "/language/translate/?id={0}&type=3";
        public const string LANGUAGE_TRANSLATE_COMMENT = API_SUFFIX + "/language/bulk_translate/?comment_ids={0}";

        #endregion Language endpoints constants

        #region Live endpoints constants

        public const string LIVE_ADD_TO_POST_LIVE = API_SUFFIX + "/live/{0}/add_to_post_live/";
        public const string LIVE_COMMENT = API_SUFFIX + "/live/{0}/comment/";
        public const string LIVE_CREATE = API_SUFFIX + "/live/create/";
        public const string LIVE_DELETE_POST_LIVE = API_SUFFIX + "/live/{0}/delete_post_live/";
        public const string LIVE_END = API_SUFFIX + "/live/{0}/end_broadcast/";
        public const string LIVE_GET_COMMENT = API_SUFFIX + "/live/{0}/get_comment/";
        public const string LIVE_GET_COMMENT_LASTCOMMENTTS = API_SUFFIX + "/live/{0}/get_comment/?last_comment_ts={1}";
        public const string LIVE_GET_FINAL_VIEWER_LIST = API_SUFFIX + "/live/{0}/get_final_viewer_list/";
        public const string LIVE_GET_JOIN_REQUESTS = API_SUFFIX + "/live/{0}/get_join_requests/";
        public const string LIVE_GET_LIKE_COUNT = API_SUFFIX + "/live/{0}/get_like_count/";
        public const string LIVE_GET_LIVE_PRESENCE = API_SUFFIX + "/live/get_live_presence/?presence_type=30min";
        public const string LIVE_GET_POST_LIVE_COMMENT = API_SUFFIX + "/live/{0}/get_post_live_comments/?starting_offset={1}&encoding_tag={2}";
        public const string LIVE_GET_POST_LIVE_VIEWERS_LIST = API_SUFFIX + "/live/{0}/get_post_live_viewers_list/";
        public const string LIVE_GET_SUGGESTED_BROADCASTS = API_SUFFIX + "/live/get_suggested_broadcasts/";
        public const string LIVE_GET_VIEWER_LIST = API_SUFFIX + "/live/{0}/get_viewer_list/";
        public const string LIVE_HEARTBEAT_AND_GET_VIEWER_COUNT = API_SUFFIX + "/live/{0}/heartbeat_and_get_viewer_count/";
        public const string LIVE_INFO = API_SUFFIX + "/live/{0}/info/";
        public const string LIVE_LIKE = API_SUFFIX + "/live/{0}/like/";
        public const string LIVE_MUTE_COMMENTS = API_SUFFIX + "/live/{0}/mute_comment/";
        public const string LIVE_PIN_COMMENT = API_SUFFIX + "/live/{0}/pin_comment/";
        public const string LIVE_POST_LIVE_LIKES = API_SUFFIX + "/live/{0}/get_post_live_likes/?starting_offset={1}&encoding_tag={2}";
        public const string LIVE_START = API_SUFFIX + "/live/{0}/start/";
        public const string LIVE_UNMUTE_COMMENTS = API_SUFFIX + "/live/{0}/unmute_comment/";
        public const string LIVE_UNPIN_COMMENT = API_SUFFIX + "/live/{0}/unpin_comment/";

        #endregion Live endpoints constants

        #region Location endpoints constants
        /// <summary>
        /// It seems deprecated and can't get feeds, only stories will recieve
        /// </summary>
        public const string LOCATION_FEED = API_SUFFIX + "/feed/location/{0}/";
        public const string LOCATION_SECTION = API_SUFFIX + "/locations/{0}/sections/";

        public const string LOCATION_SEARCH = API_SUFFIX + "/location_search/";

        public const string LOCATIONS_INFO = API_SUFFIX + "/locations/{0}/info/";
        /// <summary>
        /// {0} => external id, NOT WORKING
        /// </summary>
        public const string LOCATIONS_RELEATED = API_SUFFIX + "/locations/{0}/related/";

        #endregion Location endpoints constants

        #region Media endpoints constants

        public const string ALLOW_MEDIA_COMMENTS = API_SUFFIX + "/media/{0}/enable_comments/";
        public const string DELETE_COMMENT = API_SUFFIX + "/media/{0}/comment/{1}/delete/";
        public const string DELETE_MEDIA = API_SUFFIX + "/media/{0}/delete/?media_type={1}";
        public const string DELETE_MULTIPLE_COMMENT = API_SUFFIX + "/media/{0}/comment/bulk_delete/";
        public const string DISABLE_MEDIA_COMMENTS = API_SUFFIX + "/media/{0}/disable_comments/";
        public const string EDIT_MEDIA = API_SUFFIX + "/media/{0}/edit_media/";
        public const string GET_MEDIA = API_SUFFIX + "/media/{0}/info/";
        public const string GET_SHARE_LINK = API_SUFFIX + "/media/{0}/permalink/";
        public const string LIKE_COMMENT = API_SUFFIX + "/media/{0}/comment_like/";
        public const string LIKE_MEDIA = API_SUFFIX + "/media/{0}/like/";
        public const string MAX_MEDIA_ID_POSTFIX = "/media/?max_id=";
        public const string MEDIA = "/media/";
        public const string MEDIA_ALBUM_CONFIGURE = API_SUFFIX + "/media/configure_sidecar/";
        public const string MEDIA_COMMENT_LIKERS = API_SUFFIX + "/media/{0}/comment_likers/";
        public const string MEDIA_COMMENTS = API_SUFFIX + "/media/{0}/comments/";//?can_support_threading=true";
        public const string MEDIA_CONFIGURE = API_SUFFIX + "/media/configure/";
        public const string MEDIA_CONFIGURE_VIDEO = API_SUFFIX + "/media/configure/?video=1";
        public const string MEDIA_UPLOAD_FINISH_VIDEO = API_SUFFIX + "/media/upload_finish/?video=1";
        public const string MEDIA_UPLOAD_FINISH = API_SUFFIX + "/media/upload_finish/";
        public const string MEDIA_INFOS = API_SUFFIX + "/media/infos/?_uuid={0}&_csrftoken={1}&media_ids={2}&ranked_content=true&include_inactive_reel=true";
        public const string MEDIA_CONFIGURE_NAMETAG = API_SUFFIX + "/media/configure_to_nametag/";
        public const string MEDIA_INLINE_COMMENTS = API_SUFFIX + "/media/{0}/comments/{1}/inline_child_comments/";
        public const string MEDIA_LIKERS = API_SUFFIX + "/media/{0}/likers/";
        public const string MEDIA_REPORT = API_SUFFIX + "/media/{0}/flag_media/";
        public const string MEDIA_REPORT_COMMENT = API_SUFFIX + "/media/{0}/comment/{1}/flag/";
        public const string MEDIA_SAVE = API_SUFFIX + "/media/{0}/save/";
        public const string MEDIA_UNSAVE = API_SUFFIX + "/media/{0}/unsave/";

        public const string MEDIA_VALIDATE_REEL_URL = API_SUFFIX + "/media/validate_reel_url/";
        public const string POST_COMMENT = API_SUFFIX + "/media/{0}/comment/";
        public const string SEEN_MEDIA = API_SUFFIX + "/media/seen/";
        public const string SEEN_MEDIA_STORY = API_SUFFIX_V2 + "/media/seen/?reel=1&live_vod=0";
        public const string STORY_CONFIGURE = API_SUFFIX + "/media/configure_to_reel/";
        public const string STORY_CONFIGURE_VIDEO = API_SUFFIX + "/media/configure_to_story/?video=1";
        public const string STORY_CONFIGURE_VIDEO2 = API_SUFFIX + "/media/configure_to_story/";
        public const string STORY_MEDIA_INFO_UPLOAD = API_SUFFIX + "/media/mas_opt_in_info/";
        public const string UNLIKE_COMMENT = API_SUFFIX + "/media/{0}/comment_unlike/";
        public const string UNLIKE_MEDIA = API_SUFFIX + "/media/{0}/unlike/";
        public const string MEDIA_STORY_VIEWERS = API_SUFFIX + "/media/{0}/list_reel_media_viewer/";
        public const string MEDIA_BLOCKED = API_SUFFIX + "/media/blocked/";
        public const string MEDIA_ARCHIVE = API_SUFFIX + "/media/{0}/only_me/";
        public const string MEDIA_UNARCHIVE = API_SUFFIX + "/media/{0}/undo_only_me/";
        public const string MEDIA_STORY_POLL_VOTERS = API_SUFFIX + "/media/{0}/{1}/story_poll_voters/";
        public const string MEDIA_STORY_POLL_VOTE = API_SUFFIX + "/media/{0}/{1}/story_poll_vote/";
        public const string MEDIA_STORY_SLIDER_VOTE = API_SUFFIX + "/media/{0}/{1}/story_slider_vote/";
        public const string MEDIA_STORY_QUESTION_RESPONSE = API_SUFFIX + "/media/{0}/{1}/story_question_response/";
        public const string MEDIA_STORY_COUNTDOWNS = API_SUFFIX + "/media/story_countdowns/";
        public const string MEDIA_FOLLOW_COUNTDOWN = API_SUFFIX + "/media/{0}/follow_story_countdown/";
        public const string MEDIA_UNFOLLOW_COUNTDOWN = API_SUFFIX + "/media/{0}/unfollow_story_countdown/";



        public const string MEDIA_TAG = API_SUFFIX + "/media/{0}/tags/";

        #endregion Media endpoints constants

        #region News endpoints constants

        public const string GET_FOLLOWING_RECENT_ACTIVITY = API_SUFFIX + "/news/";
        public const string GET_RECENT_ACTIVITY = API_SUFFIX + "/news/inbox/";
        /// <summary>
        /// post params:
        /// <para>"action":"click"</para>
        /// </summary>
        public const string NEWS_LOG = API_SUFFIX + "/news/log/";

        #endregion News endpoints constants

        #region Notification endpoints constants

        public const string NOTIFICATION_BADGE = API_SUFFIX + "/notifications/badge/";
        public const string PUSH_REGISTER = API_SUFFIX + "/push/register/";

        #endregion Notification endpoints constants

        #region Shopping endpoints constants

        public const string USER_SHOPPABLE_MEDIA = API_SUFFIX + "/feed/user/{0}/shoppable_media/";

        public const string COMMERCE_PRODUCT_INFO = API_SUFFIX + "/commerce/products/{0}/?media_id={1}&device_width={2}";

        #endregion Shopping endpoints constants

        #region Tags endpoints constants

        public const string GET_TAG_INFO = API_SUFFIX + "/tags/{0}/info/";
        public const string SEARCH_TAGS = API_SUFFIX + "/tags/search/?q={0}&count={1}";
        public const string TAG_FOLLOW = API_SUFFIX + "/tags/follow/{0}/";
        public const string TAG_RANKED = API_SUFFIX + "/tags/{0}/ranked_sections/";
        public const string TAG_RECENT = API_SUFFIX + "/tags/{0}/recent_sections/";
        public const string TAG_SECTION = API_SUFFIX + "/tags/{0}/sections/";
        /// <summary>
        /// queries:
        /// <para>visited = [{"id":"TAG ID","type":"hashtag"}]</para>
        /// <para>related_types = ["location","hashtag"]</para>
        /// </summary>
        public const string TAG_RELATED = API_SUFFIX + "/tags/{0}/related/";

        public const string TAG_STORY = API_SUFFIX + "/tags/{0}/story/";
        public const string TAG_SUGGESTED = API_SUFFIX + "/tags/suggested/";
        public const string TAG_UNFOLLOW = API_SUFFIX + "/tags/unfollow/{0}/";
        public const string TAG_MEDIA_REPORT = API_SUFFIX + "/tags/hashtag_media_report/";

        #endregion Tags endpoints constants

        #region Users endpoints constants

        public const string ACCOUNTS_LOOKUP_PHONE = API_SUFFIX + "/users/lookup_phone/";
        public const string GET_USER_INFO_BY_ID = API_SUFFIX + "/users/{0}/info/";
        public const string GET_USER_INFO_BY_USERNAME = API_SUFFIX + "/users/{0}/usernameinfo/";
        public const string SEARCH_USERS = API_SUFFIX + "/users/search/";
        public const string USERS_CHECK_EMAIL = API_SUFFIX + "/users/check_email/";
        public const string USERS_CHECK_USERNAME = API_SUFFIX + "/users/check_username/";
        public const string USERS_LOOKUP = API_SUFFIX + "/users/lookup/";
        public const string USERS_NAMETAG_CONFIG = API_SUFFIX + "/users/nametag_config/";
        public const string USERS_REEL_SETTINGS = API_SUFFIX + "/users/reel_settings/";
        public const string USERS_REPORT = API_SUFFIX + "/users/{0}/flag_user/";
        public const string USERS_SEARCH = API_SUFFIX + "/users/search/?timezone_offset={0}&q={1}&count={2}";
        public const string USERS_SET_REEL_SETTINGS = API_SUFFIX + "/users/set_reel_settings/";
        public const string USERS_FOLLOWING_TAG_INFO = API_SUFFIX + "/users/{0}/following_tags_info/";
        public const string USERS_FULL_DETAIL_INFO = API_SUFFIX + "/users/{0}/full_detail_info/";
        public const string USERS_NAMETAG_LOOKUP = API_SUFFIX + "/users/nametag_lookup/";
        public const string USERS_BLOCKED_LIST = API_SUFFIX + "/users/blocked_list/";
        public const string USERS_ACCOUNT_DETAILS = API_SUFFIX + "/users/{0}/account_details/";

        #endregion Users endpoints constants

        #region Upload endpoints constants

        public const string UPLOAD_PHOTO = INSTAGRAM_URL + "/rupload_igphoto/{0}_0_{1}";
        public const string UPLOAD_PHOTO_OLD = API_SUFFIX + "/upload/photo/";
        public const string UPLOAD_VIDEO = INSTAGRAM_URL + "/rupload_igvideo/{0}_0_{1}";
        public const string UPLOAD_VIDEO_OLD = API_SUFFIX + "/upload/video/";

        #endregion Upload endpoints constants

        #region Other endpoints constants

        public const string ADDRESSBOOK_LINK = API_SUFFIX + "/address_book/link/?include=extra_display_name,thumbnails";
        public const string ARCHIVE_REEL_DAY_SHELLS = API_SUFFIX + "/archive/reel/day_shells/?include_cover=0";
        public const string DYI_REQUEST_DOWNLOAD_DATA = API_SUFFIX + "/dyi/request_download_data/";
        public const string DYI_CHECK_DATA_STATE = API_SUFFIX + "/dyi/check_data_state/";
        public const string DYNAMIC_ONBOARDING_GET_STEPS = API_SUFFIX + "/dynamic_onboarding/get_steps/";
        public const string GET_MEDIAID = API_SUFFIX + "/oembed/?url={0}";
        public const string MEGAPHONE_LOG = API_SUFFIX + "/megaphone/log/";

        public const string QE_EXPOSE = API_SUFFIX + "/qe/expose/";

        public const string CHALLENGE = API_SUFFIX + "/challenge/";

        public const string LAUNCHER_SYNC = API_SUFFIX + "/launcher/sync/";

        #endregion Other endpoints constants

        #region Web endpoints constants

        public static string WEB_ADDRESS = "https://www.instagram.com";
        public static string WEB_ACCOUNTS = "/accounts/";
        public static string WEB_ACCOUNT_DATA = WEB_ACCOUNTS + "access_tool";
        public static string WEB_CURRENT_FOLLOW_REQUESTS = WEB_ACCOUNT_DATA + "/current_follow_requests";
        public static string WEB_FORMER_EMAILS = WEB_ACCOUNT_DATA + "/former_emails";
        public static string WEB_FORMER_PHONES = WEB_ACCOUNT_DATA + "/former_phones";
        public static string WEB_FORMER_USERNAMES = WEB_ACCOUNT_DATA + "/former_usernames";
        public static string WEB_FORMER_FULL_NAMES = WEB_ACCOUNT_DATA + "/former_full_names";
        public static string WEB_FORMER_BIO_TEXTS = WEB_ACCOUNT_DATA + "/former_bio_texts";
        public static string WEB_FORMER_BIO_LINKS = WEB_ACCOUNT_DATA + "/former_links_in_bio";


        public static string WEB_CURSOR = "__a=1&cursor={0}";

        public static readonly Uri InstagramWebUri = new Uri(WEB_ADDRESS);
        #endregion
    }
}
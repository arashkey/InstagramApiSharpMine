/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Classes
{
    public static class InstaNavigationChain
    {
        public const string FeedTimeline = "1aj:feed_timeline";
        public const string FeedTimeline1 = FeedTimeline + ":1";
        public const string FeedTimeline6 = FeedTimeline + ":6";
        public const string FeedTimeline8 = FeedTimeline + ":8";

        public const string DirectShareSheetFragmentDirectReshareSheet = "DirectShareSheetFragment:direct_reshare_sheet";
        public const string DirectShareSheetFragmentDirectReshareSheet5 = DirectShareSheetFragmentDirectReshareSheet + ":5";

        public const string ModalCommentComposerFeedTimeline = "5ub:modal_comment_composer_feed_timeline";
        public const string ModalCommentComposerFeedTimeline7 = ModalCommentComposerFeedTimeline + ":7";

        public const string ExplorePopular = "1sW:explore_popular";
        public const string ExplorePopular2 = ExplorePopular + ":2";
        public const string ExplorePopular3 = ExplorePopular + ":3";
        public const string ExplorePopular4 = ExplorePopular + ":4";

        public const string FeedContextualChain = "5p7:feed_contextual_chain";
        public const string FeedContextualChain10 = FeedContextualChain + ":10";

        public const string UserDetailFragmentProfile = "UserDetailFragment:profile";
        public const string UserDetailFragmentProfile11 = UserDetailFragmentProfile + ":11";

        public const string ProfileMediaTabFragmentProfile = "ProfileMediaTabFragment:profile";
        public const string ProfileMediaTabFragmentProfile12 = ProfileMediaTabFragmentProfile + ":12";

        public const string FeedContextualProfile = "9tO:feed_contextual_profile";
        public const string FeedContextualProfile13 = FeedContextualProfile + ":13";

        public const string IGTVProfileTabFragmentIgTvProfile = "IGTVProfileTabFragment:igtv_profile";
        public const string IGTVProfileTabFragmentIgTvProfile15 = IGTVProfileTabFragmentIgTvProfile + ":15";

        public const string AiIgTvMiniProfile = "Ai3:igtv_mini_profile";
        public const string AiIgTvMiniProfile16 = AiIgTvMiniProfile + ":16";
        public const string AiIgTvMiniProfile17 = AiIgTvMiniProfile + ":17";

        public const string DirectInbox = "4R2:direct_inbox";
        public const string DirectInbox7 = DirectInbox + ":7";
        public const string DirectInbox8 = DirectInbox + ":8";

        public const string DirectThread = "62Y:direct_thread";
        public const string DirectThread9 = DirectThread + ":9";
        public const string DirectThread11 = DirectThread + ":11";
        public const string DirectThread13 = DirectThread + ":13";

        public const string DirectQuickReplyFragment = "6kr:direct_quick_reply_fragment";
        public const string DirectQuickReplyFragment10 = DirectQuickReplyFragment + ":10";

        public const string DirectMediaPickerPhotosFragment = "722:direct_media_picker_photos_fragment";
        public const string DirectMediaPickerPhotosFragment12 = DirectMediaPickerPhotosFragment + ":12";

        public const string Truncated = "TRUNCATED";
        public const string TruncatedX5 = Truncated + "x5";

    }

    // DM TEXT TIMELINE => nav_chain=1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5&
    // LIKE POST TIMELINE => "nav_chain": "1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5,1aj:feed_timeline:6",
    // CM POST TIMELINE => 1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5,1aj:feed_timeline:6,5ub:modal_comment_composer_feed_timeline:7
    // LIKE CM POST TIMELINE => 1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5,1aj:feed_timeline:6,5ub:modal_comment_composer_feed_timeline:7,1aj:feed_timeline:8
    // EXPLORE LIKE => 1sW:explore_popular:2,1sW:explore_popular:3,1sW:explore_popular:4,5p7:feed_contextual_chain:10
    // EXPLORE=> 1sW:explore_popular:2,1sW:explore_popular:3,1sW:explore_popular:4,5p7:feed_contextual_chain:10,UserDetailFragment:profile:11,ProfileMediaTabFragment:profile:12,9tO:feed_contextual_profile:13
    // 1sW:explore_popular:2,1sW:explore_popular:3,1sW:explore_popular:4,5p7:feed_contextual_chain:10,UserDetailFragment:profile:11,ProfileMediaTabFragment:profile:12,IGTVProfileTabFragment:igtv_profile:15,Ai3:igtv_mini_profile:16,Ai3:igtv_mini_profile:17
    //
    //
    //
    //
    // Like/unlike post timeline => 1aj:feed_timeline:1,1aj:feed_timeline:2
    // CM timeline => 1aj:feed_timeline:1,1aj:feed_timeline:2,5ub:modal_comment_composer_feed_timeline:3
    // CM timeline dakhele comments page => 1aj:feed_timeline:1,1aj:feed_timeline:2,5ub:modal_comment_composer_feed_timeline:3,1aj:feed_timeline:4,CommentThreadFragment:comments_v2_feed_timeline:5
    // DM SEND PIC => 1aj:feed_timeline:1,1aj:feed_timeline:2,5ub:modal_comment_composer_feed_timeline:3,TRUNCATEDx2,4R2:direct_inbox:7,4R2:direct_inbox:8,62Y:direct_thread:9,6kr:direct_quick_reply_fragment:10,62Y:direct_thread:11,722:direct_media_picker_photos_fragment:12,62Y:direct_thread:13
    // 1aj:feed_timeline:1,1aj:feed_timeline:2,5ub:modal_comment_composer_feed_timeline:3,TRUNCATEDx5,6kr:direct_quick_reply_fragment:10,62Y:direct_thread:11,722:direct_media_picker_photos_fragment:12,62Y:direct_thread:13,UserDetailFragment:profile:14,ProfileMediaTabFragment:profile:15,9tO:feed_contextual_profile:16
    // 1aj:feed_timeline:1,1aj:feed_timeline:2,5ub:modal_comment_composer_feed_timeline:3,1aj:feed_timeline:4,1aj:feed_timeline:6
    //
    //
    //
    //
    //

}

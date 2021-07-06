/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */
using static InstagramApiSharp.Classes.InstaNavigationChain;

namespace InstagramApiSharp.Classes
{
    /// <summary>
    ///     A bunch of navigation chains generated in instagram app
    /// </summary>
    public static class InstaNavigationChainGenerator
    {
        /// <summary>
        ///     DM TEXT TIMELINE
        /// </summary>
        /// <returns>
        ///     1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5
        /// </returns>
        public static string GetForTimelineDirectShare() => $"{FeedTimeline1},{DirectShareSheetFragmentDirectReshareSheet5}";

        /// <summary>
        ///     LIKE POST TIMELINE 
        /// </summary>
        /// <returns>
        ///     1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5,1aj:feed_timeline:6
        /// </returns>
        public static string GetForTimelineLikeUnlikePost() => $"{FeedTimeline1},{DirectShareSheetFragmentDirectReshareSheet5},{FeedTimeline6}";

        /// <summary>
        ///     CM POST TIMELINE
        /// </summary>
        /// <returns>
        ///     1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5,1aj:feed_timeline:6,5ub:modal_comment_composer_feed_timeline:7
        /// </returns>
        public static string GetForTimelineComment() => $"{FeedTimeline1},{DirectShareSheetFragmentDirectReshareSheet5},{FeedTimeline6},{ModalCommentComposerFeedTimeline7}";

        /// <summary>
        ///     LIKE CM POST TIMELINE
        /// </summary>
        /// <returns>
        ///     1aj:feed_timeline:1,DirectShareSheetFragment:direct_reshare_sheet:5,1aj:feed_timeline:6,5ub:modal_comment_composer_feed_timeline:7,1aj:feed_timeline:8
        /// </returns>
        public static string GetForTimelineLikeUnlikeComment() => $"{FeedTimeline1},{DirectShareSheetFragmentDirectReshareSheet5},{FeedTimeline6},{ModalCommentComposerFeedTimeline7},{FeedTimeline8}";

        /// <summary>
        ///     LIKE EXPLORE POST
        /// </summary>
        /// <returns>
        ///     1sW:explore_popular:2,1sW:explore_popular:3,1sW:explore_popular:4,5p7:feed_contextual_chain:10
        /// </returns>
        public static string GetForExploreLikePost() => $"{ExplorePopular2},{ExplorePopular3},{ExplorePopular4},{FeedContextualChain10}";

    }
}

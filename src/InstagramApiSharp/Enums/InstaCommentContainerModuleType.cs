/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

namespace InstagramApiSharp.Enums
{
    public enum InstaCommentContainerModuleType
    {
        FeedTimeline,               // comments_v2_feed_timeline                            Timeline
        FeedContextualProfile,      // comments_v2_feed_contextual_profile                  People profile comment
        FeedContextualChain,        // comments_v2_feed_contextual_chain                    Explore 
        ExploreEventViewer,         // comments_v2_explore_event_viewer                     Explore cluster
        IgtvExploreViewer,          // comments_v2_igtv_viewer                              Igtv viewer [ explore ]
        IgtvProfile,                // comments_v2_igtv_profile                             Igtv profile
        SelfIgtvProfile,            // self_comments_v2_igtv_profile                        Self igtv profile
        SelfFeedContextualProfile,  // self_comments_v2_feed_contextual_self_profile        Self comment [media, user tags]
    }
}

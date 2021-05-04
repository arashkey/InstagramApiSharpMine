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
    public enum InstaMediaInventorySource
    {
        None = 0,
        MediaOrAdd,
        Clips
    }
    public enum InstaMediaContainerModuleType
    {
        None,
        FeedTimeline,
        FeedContextualProfile,
        FeedContextualCain,
        IgtvExplorePinnedNav,
        VideoViewOther,
        PhotoViewOther,
        IgtvProfile,
        ClipsViewerClipsTab
    }
    public enum InstaMediaSurfaceType
    {
        None,
        FeedContextualProfile,
        FeedContextualCain,
        IgtvExplorePinnedNav,
        Profile
    }
}

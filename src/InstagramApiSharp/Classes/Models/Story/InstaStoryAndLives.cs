/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
namespace InstagramApiSharp.Classes.Models
{
    public class InstaStoryAndLives
    {
        public InstaStory Reel { get; set; }
        public InstaBroadcast Broadcast { get; set; }
        public InstaBroadcastPostLive PostLiveItems { get; set; }
    }
}

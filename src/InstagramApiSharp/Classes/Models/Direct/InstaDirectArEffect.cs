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
    public class InstaDirectArEffect
    {
        public InstaDirectTargetEffectPreview Data { get; set; } = new InstaDirectTargetEffectPreview();
        public InstaStory PreviewVideo { get; set; }
    }

    public class InstaDirectTargetEffectPreview
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public InstaDirectAttributionUser AttributionUser { get; set; }
        public string ThumbnailImage { get; set; }
    }

    public class InstaDirectAttributionUser
    {
        public string InstagramUserId { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
    }

}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaTranslate
    {
        public long Id { get; set; }

        public string Translation { get; set; }
    }
    public class InstaTranslateList : List<InstaTranslate> { }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaDynamicSearchConverter : IObjectConverter<InstaDynamicSearch, InstaDynamicSearchResponse>
    {
        public InstaDynamicSearchResponse SourceObject { get; set; }

        public InstaDynamicSearch Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var dynamicSearch = new InstaDynamicSearch();
            if (SourceObject.Sections?.Count > 0)
            {
                foreach (var section in SourceObject.Sections)
                {
                    try
                    {
                        dynamicSearch.Sections.Add(ConvertersFabric.Instance.GetDynamicSearchSectionConverter(section).Convert());
                    }
                    catch { }
                }
            }
            return dynamicSearch;
        }
    }
}

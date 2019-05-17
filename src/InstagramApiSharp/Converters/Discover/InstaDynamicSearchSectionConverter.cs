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
    internal class InstaDynamicSearchSectionConverter : IObjectConverter<InstaDynamicSearchSection, InstaDynamicSearchSectionResponse>
    {
        public InstaDynamicSearchSectionResponse SourceObject { get; set; }

        public InstaDynamicSearchSection Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var dynamicSearchSection = new InstaDynamicSearchSection
            {
                Title = SourceObject.Title
            };
            try
            {
                dynamicSearchSection.Type = (InstaDynamicSearchSectionType)Enum.Parse(typeof(InstaDynamicSearchSectionType), SourceObject.Type, true);
            }
            catch { }
            if (SourceObject.Items?.Count > 0)
            {
                foreach (var search in SourceObject.Items)
                {
                    try
                    {
                        dynamicSearchSection.Items.Add(ConvertersFabric.Instance.GetDiscoverRecentSearchesItemConverter(search).Convert());
                    }
                    catch { }
                }
            }
            return dynamicSearchSection;
        }
    }
}

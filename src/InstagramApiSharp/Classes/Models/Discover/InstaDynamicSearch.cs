/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaDynamicSearch
    {
        public List<InstaDynamicSearchSection> Sections { get; set; } = new List<InstaDynamicSearchSection>();
    }

    public class InstaDynamicSearchSection : INotifyPropertyChanged
    {
        public InstaDynamicSearchSectionType Type { get; set; }

        string Title_ { get; set; } = string.Empty;
        public string Title { get { return Title_; } set { Title_ = value; OnPropertyChanged("Title"); } }

        public ObservableCollection<InstaDiscoverRecentSearchesItem> Items { get; set; } = new ObservableCollection<InstaDiscoverRecentSearchesItem>();
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}

﻿using InstagramApiSharp.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaFeed : IInstaBaseList
    {
        public int MediaItemsCount => Medias.Count;
        public int StoriesItemsCount => Stories.Count;
        public int PostsItemsCount => Posts.Count;

        public List<InstaMedia> Medias { get; set; } = new List<InstaMedia>();
        public List<InstaStory> Stories { get; set; } = new List<InstaStory>();
        public string NextMaxId { get; set; }
        public bool MoreAvailable { get; set; }

        public List<InstaSuggestionItem> SuggestedUserItems { get; set; } = new List<InstaSuggestionItem>();

        public List<InstaPost> Posts { get; set; } = new List<InstaPost>();
    }

    
    public class InstaPost : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public InstaFeedsType Type { get; set; }
        public InstaMedia Media { get; set; }
        
        public ObservableCollection<InstaReelFeed> Stories { get; set; } = new ObservableCollection<InstaReelFeed>();
        //public List<InstaStory> Stories { get; set; } = new List<InstaStory>();
        public List<InstaSuggestionItem> SuggestedUserItems { get; set; } = new List<InstaSuggestionItem>();
        public InstaAllCatchedUp EndOfFeedDemarcator { get; set; }
        public List<InstaHashtagMedia> Hashtags { get; set; } = new List<InstaHashtagMedia>();
        public InstaStoriesNetego StoriesNetego { get; set; }
    }
}
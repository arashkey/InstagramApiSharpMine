using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaComment : INotifyPropertyChanged
    {
        public ObservableCollection<InstaComment> ChildComments { get; private set; } = new ObservableCollection<InstaComment>();

        public int Type { get; set; }

        public int BitFlags { get; set; }

        public long UserId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public int LikesCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public InstaContentType ContentType { get; set; }
        public InstaUserShort User { get; set; }
        public long Pk { get; set; }
        public string Text { get; set; }

        public bool DidReportAsSpam { get; set; }

        private bool _haslikedcm;
        public bool HasLikedComment { get => _haslikedcm; set { _haslikedcm = value; Update("HasLikedComment"); } }

        public int ChildCommentCount { get; set; }

        int _numTailChildComments = 0;
        public int NumTailChildComments
        {
            get => _numTailChildComments;
            set
            {
                _numTailChildComments = value;
                Update("NumTailChildComments");
                if (value > 0)
                    NumTailChildCommentsText = ViewPrevComText + $" ({value})";
            }
        }

        const string ViewPrevComText = "View previous comments";
        string _numTailChildCommentsText = ViewPrevComText;
        public string NumTailChildCommentsText { get => _numTailChildCommentsText; set { _numTailChildCommentsText = value; Update("NumTailChildCommentsText"); } }

        bool _hasMoreTailChildComments = false;
        public bool HasMoreTailChildComments { get => _hasMoreTailChildComments; set { _hasMoreTailChildComments = value; Update("HasMoreTailChildComments"); } }
        bool _hasMoreHeadChildComments = false;
        public bool HasMoreHeadChildComments { get => _hasMoreHeadChildComments; set { _hasMoreHeadChildComments = value; Update("HasMoreHeadChildComments"); } }

        //public string NextMaxChildCursor { get; set; }
        public List<InstaCommentShort> PreviewChildComments { get; set; } = new List<InstaCommentShort>();

        public List<InstaUserShort> OtherPreviewUsers { get; set; } = new List<InstaUserShort>();


        bool _hasMorePaginationParameters = false;
        public bool HasMorePaginationParameters { get => _hasMorePaginationParameters; set { _hasMorePaginationParameters = value; Update("HasMorePaginationParameters"); } }

        public bool ShareEnabled { get; set; }
        public int CommentIndex { get; set; }

        public long ParentCommentId { get; set; }

        public bool HasTranslation { get; set; }
        private bool _isCommentsDisabled = false;
        public bool IsCommentsDisabled { get => _isCommentsDisabled; set { _isCommentsDisabled = value; Update("IsCommentsDisabled"); } }
        public PaginationParameters PaginationParameters { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void Update(string PName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PName)); }

        public bool Equals(InstaComment comment)
        {
            return Pk == comment?.Pk;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InstaComment);
        }

        public override int GetHashCode()
        {
            return Pk.GetHashCode();
        }
    }
}
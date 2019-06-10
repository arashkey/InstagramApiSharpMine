using System.Collections.Generic;
using System.ComponentModel;

namespace InstagramApiSharp.Classes.Models
{
    public class InstaInboxMedia : INotifyPropertyChanged
    {
        public List<InstaImage> Images { get; set; } = new List<InstaImage>();
        public long OriginalWidth { get; set; }
        public long OriginalHeight { get; set; }
        public InstaMediaType MediaType { get; set; }
        public List<InstaVideo> Videos { get; set; } = new List<InstaVideo>();


        private double _percentage = 0;
        public double Percentage { get { return _percentage; } set { _percentage = value; OnPropertyChanged("Percentage"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
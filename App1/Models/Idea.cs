using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class Idea : INotifyPropertyChanged
    {
        private string _shortContent;

        public string ShortContent
        {
            get { return _shortContent; }
            set
            {
                if (_shortContent == value) return;
                _shortContent = value;
                OnPropertyChanged("ShortContent");
            }
        }

        private int _voteCount;

        public int VoteCount
        {
            get { return _voteCount; }
            set
            {
                if (_voteCount == value) return;
                _voteCount = value;
                OnPropertyChanged("VoteCount");
            }
        }

        private string _publisher;

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                if (_publisher == value) return;
                _publisher = value;
                OnPropertyChanged("Publisher");
            }
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value) return;
                _date = value;
                OnPropertyChanged("Date");
            }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
}

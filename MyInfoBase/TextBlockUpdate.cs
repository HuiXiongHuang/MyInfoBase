using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MyInfoBase
{
    public class TextBlockUpdate : INotifyPropertyChanged
    {
        private String _textMessage = "";
        public String TextMessage
        {
            get
            {
                return _textMessage;
            }
            set
            {
                _textMessage = value;
                OnPropertyChanged("TextMessage");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}

using System;
using System.ComponentModel;

namespace BlitzChat
{
    [Serializable]
    public class  ChatSettingsXML : INotifyPropertyChanged
    {
        #region Members
        private string _foreColor;
        private string _backgroundColor;
        private string _nicknameColor;
        private string _dateColor;
        private double _chatOpacity;
        private string _textFont;
        private double _textFontSize;
        private bool _topMost;
        private double _smileScale;
        private bool _nicknameBold;
        private bool _textBold;
        private bool _dateEnabled;
        private bool _transparencyEnabled;
        private bool _backgroundMode;
        private double _height;
        private double _width;
        private double _left;
        private double _top;
        #endregion

        #region Properties
        public string ForeColor {
            get { return _foreColor; } 
            set {
                _foreColor = value;
                OnPropertyChanged("ForeColor");
        } }
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }
        public string NicknameColor
        {
            get { return _nicknameColor; }
            set
            {
                _nicknameColor = value;
                OnPropertyChanged("NicknameColor");
            }
        }
        public string DateColor
        {
            get { return _dateColor; }
            set
            {
                _dateColor = value;
                OnPropertyChanged("DateColor");
            }
        }
        public double ChatOpacity
        {
            get { return _chatOpacity; }
            set
            {
                _chatOpacity = value;
                OnPropertyChanged("ChatOpacity");
            }
        }
        public string TextFont
        {
            get { return _textFont; }
            set
            {
                _textFont = value;
                OnPropertyChanged("TextFont");
            }
        }
        public double TextFontSize
        {
            get { return _textFontSize; }
            set
            {
                _textFontSize = value;
                OnPropertyChanged("TextFontSize");
            }
        }
        public bool TopMost
        {
            get { return _topMost; }
            set
            {
                _topMost = value;
                OnPropertyChanged("TopMost");
            }
        }
        public double SmileScale
        {
            get { return _smileScale; }
            set
            {
                _smileScale = value;
                OnPropertyChanged("SmileSize");
            }
        }
        public bool NicknameBold
        {
            get { return _nicknameBold; }
            set
            {
                _nicknameBold = value;
                OnPropertyChanged("NicknameBold");
            }
        }
        public bool TextBold
        {
            get { return _textBold; }
            set
            {
                _textBold = value;
                OnPropertyChanged("TextBold");
            }
        }
        public bool DateEnabled
        {
            get { return _dateEnabled; }
            set
            {
                _dateEnabled = value;
                OnPropertyChanged("DateEnabled");
            }
        }
        public bool TransparencyEnabled
        {
            get { return _transparencyEnabled; }
            set
            {
                _transparencyEnabled = value;
                OnPropertyChanged("TransparencyEnabled");
            }
        }
        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged("Left");
            }
        }
        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged("Top");
            }
        }
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }
        public bool BackgroundMode
        {
            get { return _backgroundMode; }
            set
            {
                _backgroundMode = value;
                OnPropertyChanged("BackgroundMode");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public ChatSettingsXML()
        {
            BackgroundColor = "#FF444141";
            ForeColor = "#FFFFFFFF";
            NicknameColor = "#FF48D1CC";
            DateColor = "#FFFFFFFF";
            ChatOpacity = 1;
            TextFont = "Arial";
            TextFontSize = 14.00;
            TopMost = true;
            SmileScale = 15.0;
            NicknameBold = true;
            TextBold = false;
            DateEnabled = false;
            TransparencyEnabled = false;
            BackgroundMode = false;
        }

        // Create the OnPropertyChanged method to raise the event 
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

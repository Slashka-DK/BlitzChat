using System;

namespace BlitzChat
{
    [Serializable]
    public class  ChatSettingsXML
    {
        public string ForeColor { get; set; }
        public string BackgroundColor { get; set; }
        public string NicknameColor { get; set; }
        public string DateColor { get; set; }
        public double ChatOpacity { get; set; }
        public string TextFont { get; set; }
        public double TextFontSize { get; set; }
        public bool TopMost { get; set; }
        public double SmileSize { get; set; }
        public bool NicknameBold { get; set; }
        public bool TextBold { get; set; }
        public bool DateEnabled { get; set; }
        public bool TransparencyEnabled { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width {get; set;}
        public double Height { get; set; }

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
            SmileSize = 15.0;
            NicknameBold = true;
            TextBold = false;
            DateEnabled = false;
            TransparencyEnabled = false;
        }
    }
}

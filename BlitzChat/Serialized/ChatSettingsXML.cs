using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ChatOpacity = 0.85;
            TextFont = "Verdana";
            TextFontSize = 16.00;
            TopMost = true;
            SmileSize = 0.0;
            NicknameBold = true;
            TextBold = false;
            DateEnabled = true;
        }
    }
}

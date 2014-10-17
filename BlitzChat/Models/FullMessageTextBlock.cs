using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BlitzChat.Models
{
    public class MessageTextBlock : TextBlock
    {
        public Run ToName;
        public Run Date;
        public InlineUIContainer chatImage;
        public Run Nickname;
        public Run Message;
        public ResourceDictionary resDict; 
        public MessageTextBlock(ResourceDictionary rd) {
            resDict = rd;
            Date = new Run("");
            chatImage = null;
            Nickname = new Run("");
           
            ToName = new Run("");
            Message = new Run("");
        }

        public void createTextBlock(){
            this.Inlines.Add(Date);
            //Date.Style = resDict["styleDateTime"] as Style;
            this.Inlines.Add(chatImage);
            this.Inlines.Add(new Run(" "));
            this.Inlines.Add(Nickname);
            //Nickname.Style = resDict["styleNickname"] as Style;
            this.Inlines.Add(ToName);
            //ToName.Style = resDict["styleToName"] as Style;
            this.Inlines.Add(Message);
            //Message.Style = resDict["styleText"] as Style;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlitzChat
{
    public class Sc2Tv
    {
        public dotSC2TV.Sc2Chat chat;
        public Sc2Tv(string channel) {
            dotSC2TV.JSEvaluator eva = new dotSC2TV.JSEvaluator();
            chat = new dotSC2TV.Sc2Chat(10);
            chat.ChannelId = HTMLParsing.getSc2TVChannelID("http://sc2tv.ru/channel/" + channel);
        }
                
    }
}

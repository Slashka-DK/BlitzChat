﻿
namespace BlitzChat
{
    public class ChannelsSaveXML
    {
        public string Twitch {get;set;}
        public string SC2TV { get; set; }
        public int SC2TVLastMessageId { get; set; }
        public string Cybergame { get; set; }
        public string GoodGame { get; set; }
        public string Hitbox { get; set; }
        public string Empire { get; set; }
        public string GohaTV { get; set; }
        public string GamersTV { get; set; }
        public int GamersTVLastMessageId { get; set; }
        public ChannelsSaveXML() {
            SC2TVLastMessageId = 0;
            GamersTVLastMessageId = 0;
        }
    }
}

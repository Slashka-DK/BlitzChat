using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bliGamersTV
{
    public class GamersTVMessage
    {
        public int chat_id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public int user_id { get; set; }
        public string time { get; set; }
        public int to_user_id { get; set; }
        public string toName { get; set; }
        public bool toStreamer { get; set; }
    }
}

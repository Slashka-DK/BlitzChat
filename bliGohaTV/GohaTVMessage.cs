using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bliGohaTV
{
    public class GohaTVMessage
    {
        public string Name { get; set; }
        public string Text {get;set;}
        public bool ToStreamer { get; set; }

        public GohaTVMessage() {
            ToStreamer = false;
        }
    }
}

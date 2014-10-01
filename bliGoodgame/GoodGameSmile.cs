using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bliGoodgame
{
    public class GoodGameSmile
    {
        public string code { get; set; }
        public int width { get; set; }
        public int heght { get; set; }
        public string path { get; set; }
        public int x { get; set; }
        public int y {get;set;}

        public GoodGameSmile(string code, string path, int x, int y, int width = 81, int heght = 35)
        {
            this.code = code;
            this.path = path;
            this.x = x;
            this.y = y;
            this.width = width;
            this.heght = heght;
        }
    }
}

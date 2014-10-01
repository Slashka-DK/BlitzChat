using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bliWebClient
{
    public class WebClientTimeOut : WebClient
    {

                //time in milliseconds
        private int timeout;
    
        public int Timeout
       {
               get {
                   return timeout;
               }
               set {
                   timeout = value;
               }
        }

        public WebClientTimeOut()
        {
               this.timeout = 60000;
        }

        public WebClientTimeOut(int timeout)
        {
               this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
               var result = base.GetWebRequest(address);
               result.Timeout = this.timeout;
               return result;
        }
    }
}

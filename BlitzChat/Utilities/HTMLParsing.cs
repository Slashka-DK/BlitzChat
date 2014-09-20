using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;
namespace BlitzChat
{
    public class HTMLParsing
    {
        WebBrowser webBrowserControl = new WebBrowser();
        public static uint getSc2TVChannelID(string address)
        {
            uint channel = 0;
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc;
            try
            {
               doc = web.Load(address);
            }
            catch(WebException){
                throw  new WrongChannelNameException();
            }
            var result = from link in doc.DocumentNode.SelectNodes("//link[@rel='canonical']")
                         select link.Attributes["href"].Value;
            foreach (string r in result) { 
                if(r.Contains("node")){
                    string[] arr = r.Split('/');
                    channel = Convert.ToUInt32(arr[arr.Length - 1]);
                    break;
                }
            }
            return channel;
        }

        public string getGoodgameChatPage(string address)
        {
            Uri uri = new Uri(address);
            
            webBrowserControl.AllowNavigation = true;
            // optional but I use this because it stops javascript errors breaking your scraper
            webBrowserControl.ScriptErrorsSuppressed = true;
            // you want to start scraping after the document is finished loading so do it in the function you pass to this handler
            webBrowserControl.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowserControl_DocumentCompleted);
            webBrowserControl.Navigate(uri);
            return "";
        }

        private void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection divs = webBrowserControl.Document.GetElementsByTagName("div");

            foreach (HtmlElement div in divs)
            {
                if (div.Id != null && div.Id.Contains("messages"))
                {
                   string html = div.InnerHtml;
                }
            }
        }
    }
}

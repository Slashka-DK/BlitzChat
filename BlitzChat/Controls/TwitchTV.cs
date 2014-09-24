using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotIRC;
using System.Threading;
using System.Collections;
using System.Drawing;
namespace BlitzChat
{
    public class TwitchTV
    {
        string strNickname;
        string strPassword;
        IrcRegistrationInfo regInf;
        public Irc irc { get; set; }
        

        public TwitchTV(string channelname)
        {
            try
            {
                using (var client = new NewWebClient())
                {
                    client.HeadOnly = true;
                    // fine, no content downloaded
                    // throws 404
                    client.DownloadString("https://api.twitch.tv/kraken/channels/"+channelname);
                }
            }
            catch {
                throw new WrongChannelNameException();
            }
            irc = new Irc();
            Random random = new Random();

            strNickname = "justinfan"+random.Next(99999999);
            strPassword = "blah";
            connectTwitch(channelname);
        }

        public void connectTwitch(string channelname)
        {
            irc.Connect("irc.twitch.tv", 6667, strNickname, strPassword);
            irc.channelConnect(channelname.ToLower());
       
        }

        public bool channelExists(string channel) {

            return true;
        }
               

    }
}

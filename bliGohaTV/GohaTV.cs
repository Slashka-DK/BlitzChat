using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotIRC;
using System.Threading;
using System.Diagnostics;
using bliWebClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace bliGohaTV
{
    public class GohaTV
    {
        private string channelName;
        private const string channelInfoUrl = "http://www.goha.tv/app/tv/data-v2.php/stream/getInfo/{0}/gohaTV.mvc.js";
        private string ircAdress = "i.gohanet.ru";
        private int ircPort = 6667;
        public event EventHandler<GohaTVMessage> messageReceived;
        private IrcClient ircClient;
        private bool isStopped = false;
        public GohaTV(string name, string[] urls) {
            if (urls != null)
            {
                ircAdress = urls[0];
                ircPort = int.Parse(urls[1]);
            }
            channelName = name;
        }

        public string getChannelName
        {
            get { return channelName; }
        }

        private void connectGohaTV(){
            ircClient = new IrcClient();
            Random random = new Random();
            string strNickname = "guest" + random.Next(99999999);
            string strPassword = "";
            ircClientConnect(ircAdress, ircPort, strNickname, strPassword);
            ircChannelConnect(channelName);
            setEvents();
        }

        private void ircChannelConnect(string channelName)
        {
            IrcChannelCollection channels = ircClient.Channels;
            try
            {
                channels.Join("#"+channelName.ToLower());
            }
            catch (Exception e)
            {
                Debug.Print("Channel Join Exception: " + e.Message);
            }
            do { Thread.Sleep(100); } while (ircClient.Channels.Count <= 0);
        }

        private void ircClientConnect(string ircAdress, int ircPort, string strNickname, string strPassword)
        {
            IrcUserRegistrationInfo regInf = new IrcUserRegistrationInfo()
            {
                NickName = strNickname,
                UserName = strNickname,
                Password = strPassword,
                RealName = strNickname
            };
            try
            {
                ircClient.Connect(ircAdress, ircPort, false, regInf);
                do { Thread.Sleep(1000); } while (!ircClient.IsConnected);
            }
            catch (Exception e)
            {
                Debug.Print("IrcClient Connection Exception: " + e.Message);
            }
        }

        private void setEvents()
        {
            ircClient.Error += ircClient_Error;
            ircClient.ErrorMessageReceived += ircClient_ErrorMessageReceived;
            ircClient.ConnectFailed += ircClient_ConnectFailed;
            ircClient.Disconnected += ircClient_Disconnected;
            ircClient.ProtocolError += ircClient_ProtocolError;
            ircClient.Channels[0].MessageReceived += GohaTV_MessageReceived;
        }

        private void GohaTV_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            if (this.messageReceived != null)
            {
                GohaTVMessage msg = new GohaTVMessage();
                msg.Name = e.Source.Name;
                msg.Text = e.Text;
                if (e.Text.Contains(channelName))
                    msg.ToStreamer = true;
                else
                    msg.ToStreamer = false;
                messageReceived(sender, msg);
            }
        }

        private void unsetEvents()
        {
            ircClient.Error -= ircClient_Error;
            ircClient.ErrorMessageReceived -= ircClient_ErrorMessageReceived;
            ircClient.ConnectFailed -= ircClient_ConnectFailed;
            ircClient.Disconnected -= ircClient_Disconnected;
            ircClient.ProtocolError -= ircClient_ProtocolError;
            ircClient.Channels[0].MessageReceived -= GohaTV_MessageReceived;
        }

        private void ircClient_ProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            Debug.Print("IrcClient Protocol Error Exception: " + e.Message);
            Reconnect();
        }

        private void ircClient_Disconnected(object sender, EventArgs e)
        {
            Debug.Print("IrcClient Disconnected");
            if (!isStopped)
                Reconnect();
        }

        private void ircClient_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            Debug.Print("IrcClient Connection Failed: " + e.Error.Message);
            Reconnect();
        }

        private void ircClient_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            Debug.Print("IrcClient Error Message Received: " + e.Message);
            Reconnect();
        }

        private void ircClient_Error(object sender, IrcErrorEventArgs e)
        {
            Debug.Print("IrcClient Error: " + e.Error.Message);
            Reconnect();
        }

        private void Reconnect()
        {
            stopIrc();
            connectGohaTV();
        }

        private void stopIrc()
        {
            unsetEvents();
            if (ircClient.IsConnected)
            {
                ircClient.Disconnect();
            }
        }

        public void Start() {
            isStopped = false;
            connectGohaTV();
        }

        public void Stop() {
            isStopped = true;
            stopIrc();
        }

        public string getViewers() {
            try
            {
                using (WebClientTimeOut wc = new WebClientTimeOut(1000))
                {
                    string info = wc.DownloadString(String.Format(channelInfoUrl, channelName));
                    int startIndex = info.IndexOf("{\"userinfo\"");
                    info = info.Substring(startIndex, info.Length - startIndex - 1);
                    JObject jObj = JsonConvert.DeserializeObject<JObject>(info);
                    return jObj["viewers"]["cnt"].ToString();
                }
            }
            catch (Exception e)
            {
                Debug.Print("Exception on parsing viewers on gohaTV: " + e.Message);
                return "0";
            }
        }

        public static bool channelExists(string channel) {
            try
            {
                using (var client = new NewWebClient())
                {
                   string s = client.DownloadString(String.Format(channelInfoUrl, channel));
                   if (s.Contains("mvc.js', null)"))
                       return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        
        }
    }
}

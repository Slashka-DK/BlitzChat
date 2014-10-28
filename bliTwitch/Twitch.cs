using System;
using dotIRC;
using bliWebClient;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
namespace bliTwitch
{
    public class Twitch
    {
        public event EventHandler<TwitchMessage> messageReceived;
        public event EventHandler smilesLoaded;
        private IrcClient ircClient;
        private string channelName;
        private string ircUrl;
        private int port;
        private bool isStopped = false;
        private string[] regexSignsEmotions;
        Dictionary<string, TwitchSmile> smiles;
        public string getChannelName {
            get { return channelName; }
        }

        public Twitch(string channel, string ircAdress, int port) {
            smiles = new Dictionary<string, TwitchSmile>();
            channelName = channel.Trim().ToLower();
            ircUrl = ircAdress;
            this.port = port;
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += bg_DoWork;
            bg.RunWorkerAsync();
            regexSignsEmotions = new string[]{"B-?\\)",
                                            "\\:-?[z|Z|\\|]",
                                            "\\:-?\\)",
                                            "\\:-?\\(",
                                            "\\:-?(p|P)",
                                            "\\;-?(p|P)",
                                            "\\&lt\\;3",
                                            "\\:-?(?:\\/|\\\\)(?!\\/)",
                                            "\\;-?\\)",
                                            "R-?\\)",
                                            "[o|O](_|\\.)[o|O]",
                                            "\\:-?D",
                                            "\\:-?(o|O)",
                                            "\\&gt\\;\\(",
                                            "\\:\\&gt\\;",
                                            "\\:-?(S|s)",
                                            "#-?[\\\\/]",
                                            "\\&lt\\;\\]",
                                            ":-?(?:7|L)"};
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            loadSmiles();
        }


        public bool Start(){
            isStopped = false;
            connectTwitch();
            return true;
        }

        public bool Stop() {
            isStopped = true;
            stopIrc();
            return true;
        }

        private void stopIrc() {
            unsetEvents();
            if (ircClient.IsConnected)
            {
                ircClient.Disconnect();
            }
        }

        public bool Reconnect() {
            stopIrc();
            connectTwitch();
            return true;
        }
        
        public static bool channelExists(string channel) {
            try
            {
                using (var client = new NewWebClient())
                {
                    client.HeadOnly = true;
                    // fine, no content downloaded
                    // throws 404
                    client.DownloadString("https://api.twitch.tv/kraken/channels/" + channel);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool connectTwitch() {
            ircClient = new IrcClient();
            Random random = new Random();
            string strNickname = "justinfan"+random.Next(99999999);
            string strPassword = "blah";
            ircClientConnect(ircUrl, port, strNickname, strPassword);
            ircChannelConnect(channelName);
            setEvents();
            return true;
        }

        private void ircChannelConnect(string channelname)
        {
            IrcChannelCollection channels = ircClient.Channels;
            try
            {
                channels.Join("#" + channelname.ToLower());
            }
            catch(Exception e)
            {
                Debug.Print("Channel Join Exception: "+e.Message);
            }
            do { Thread.Sleep(100); } while (ircClient.Channels.Count <= 0);
        }

        private void ircClientConnect(string adress, int port, string nickname, string password)
        {
            IrcUserRegistrationInfo regInf = new IrcUserRegistrationInfo()
            {
                NickName = nickname,
                UserName = nickname,
                Password = password,
                RealName = nickname
            };
            try
            {
                ircClient.Connect(adress, port, false, regInf);
                do { Thread.Sleep(1000); } while (!ircClient.IsConnected);
            }
            catch(Exception e)
            {
                Debug.Print("IrcClient Connection Exception: "+e.Message);
            }
        }

        private void setEvents()
        {
            ircClient.Error += ircClient_Error;
            ircClient.ErrorMessageReceived += ircClient_ErrorMessageReceived;
            ircClient.ConnectFailed += ircClient_ConnectFailed;
            ircClient.Disconnected += ircClient_Disconnected;
            ircClient.ProtocolError += ircClient_ProtocolError;
            ircClient.Channels[0].MessageReceived += Twitch_MessageReceived;
        }

        private void Twitch_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            if (this.messageReceived != null)
            {
                TwitchMessage tm = new TwitchMessage();
                tm.date = DateTime.Now;
                tm.Name = e.Source.Name;
                tm.ToName = "";
                tm.Text = e.Text;
                messageReceived(sender, tm);
            }
            Thread.Sleep(100);
        }

        private void unsetEvents() {
            ircClient.Error -= ircClient_Error;
            ircClient.ErrorMessageReceived -= ircClient_ErrorMessageReceived;
            ircClient.ConnectFailed -= ircClient_ConnectFailed;
            ircClient.Disconnected -= ircClient_Disconnected;
            ircClient.ProtocolError -= ircClient_ProtocolError;
            ircClient.Channels[0].MessageReceived -= Twitch_MessageReceived;
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

        public int getViewersCount() {
            int viewers = 0;
            using (var wc = new WebClient())
            {

                string json_data = wc.DownloadString("https://api.twitch.tv/kraken/streams/" + channelName);
                JObject stream = JObject.Parse(json_data);
                if (stream["stream"].HasValues)
                {
                    int.TryParse(stream["stream"]["viewers"].ToString(), out viewers);
                    return viewers;
                }
            }
            return viewers;
        }


        private void loadSmiles()
        {
            using (var wc = new WebClient())
            {
                wc.DownloadStringCompleted += (o, a) =>
                {
                    if (a.Error == null) {
                        JObject stream = (JObject)JsonConvert.DeserializeObject(a.Result.ToString());
                        if (stream["emoticons"].HasValues)
                        {
                            foreach (JObject jobj in stream["emoticons"])
                            {
                                TwitchSmile smile = new TwitchSmile();
                                smile.regex = jobj["regex"].ToString();
                                JArray imgopt = (JArray)JsonConvert.DeserializeObject(jobj["images"].ToString());
                                smile.width = int.Parse(imgopt[0]["width"].ToString());
                                smile.height = int.Parse(imgopt[0]["height"].ToString());
                                smile.uri = new Uri(imgopt[0]["url"].ToString());
                                if(imgopt[0]["emoticon_set"]!=null)
                                    smile.emoticon_set = imgopt[0]["emoticon_set"].ToString();
                                smile.key = smile.regex;
                                smiles.Add(smile.regex, smile);
                            }

                        }
                        if (smilesLoaded != null)
                            smilesLoaded(this, new EventArgs());
                    }
                };
                wc.DownloadStringAsync(new Uri(@"https://api.twitch.tv/kraken/chat/emoticons"));
            }
        }

        public TwitchSmile checkSmiles(string message) {
            string[] arrWords = message.Split(' ');
            if (smiles.Count <= 0)
                return null;
            foreach (string word in arrWords) {
                if (smiles.ContainsKey(word)) {
                    return smiles[word];
                }
                else{
                    Regex reg = null;
                    foreach(string pattern in regexSignsEmotions){
                        string newWord = word.Replace("<","&lt;");
                        newWord = newWord.Replace(">", "&gt;");
                        reg = new Regex(pattern);
                        if (reg.IsMatch(newWord))
                        {
                            smiles[pattern].key = word;
                            return smiles[pattern];
                        }
                    }
                }
            }
            return null;
        }
    }

    public class TwitchMessage
    {
        public DateTime date { get; set; }
        public string Name { get; set; }
        public string ToName { get; set; }
        public string Text { get; set; }
    }

    public class TwitchSmile
    {
        public Uri uri { get; set; }
        public string regex { get; set; }
        public string key { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string emoticon_set { get; set; }
    }
}

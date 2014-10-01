using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using bliWebClient;
using System.Text.RegularExpressions;
namespace bliGoodgame
{
    public class Goodgame
    {
        private WebSocket socket;
        private string channelName;
        private const string socketAdress = @"ws://chat.goodgame.ru:8080/chat/websocket";
        private const string smileSpriteUri = @"http://goodgame.ru/images/chat/new-smiles-big-sprite.png";
        private int viewers = 0;
        private int lastMessageId = 0;
        private object locker = new object();
        private bool loadHistory;
        private uint channelId = 0;
        private string token = "";
        private Dictionary<string, GoodGameSmile> smiles;
        public Goodgame(string channel, bool loadHistory = false) {
            this.channelName = channel;
            this.loadHistory = loadHistory;
            smiles = new Dictionary<string, GoodGameSmile>();
            loadSmiles();
        }

        public static bool channelExists(string channel) {
            try
            {
                using (var client = new NewWebClient())
                {
                    client.HeadOnly = true;
                    // fine, if channel exists
                    // throws Exception on 404
                    client.DownloadString("http://goodgame.ru/chat/" + channel);
                }
                return true;
            }
            catch(WebException)
            {
                return false;
            }
        }

        private void loadSmiles(){
            string[] array = new string[]{"peka",
                                          "fp",
                                          "bobr",
                                          "pirat",
                                          "gg",
                                          "crab",
                                          "gta",
                                          "grin",
                                          "shoked",
                                          "smile",
                                          "cry",
                                          "rage",
                                          "cool",
                                          "slow",
                                          "flame",
                                          "car",
                                          "love",
                                          "tort",
                                          "muta",
                                          "bane",
                                          "dm",
                                          "bratok",
                                          "grumpy",
                                          "cat",
                                          "zerg",
                                          "thup",
                                          "terran",
                                          "toss",
                                          "gend",
                                          "rocket",
                                          "rail",
                                          "shaft",
                                          "dog1",
                                          "dog2",
                                          "daun",
                                          "getout",
                                          "fireext",
                                          "fry",
                                          "bender",
                                          "jackie",
                                          "moscow",
                                          "gabe",
                                          "dendi",
                                          "gglord",
                                          "ggwp"
            };
            foreach(string code in array){
                    smiles.Add(":" + code + ":", new GoodGameSmile(":" + code + ":", code + ".png", 0, 0));
            }
            
        }

        public Dictionary<string, GoodGameSmile> checkSmiles(string msg) {
            Dictionary<string,GoodGameSmile> dictSmiles = new Dictionary<string,GoodGameSmile>();
            string[] arrWords = msg.Split(' ');

            foreach (string word in arrWords)
            {
                if (smiles.ContainsKey(word))
                {
                    dictSmiles.Add(word, smiles[word]);
                }
            }

            return dictSmiles;
        }

        #region GoodGame Events
        public event EventHandler<Message> OnMessageReceived;
        #endregion

        public void Start() {
            if(init_goodgame())
                Debug.Print("Goodgame socket started");
        }

        private bool init_goodgame()
        {
            string uri = "http://goodgame.ru/chat/" + channelName;
            CookieContainer cookieContainer = new CookieContainer();
            CookieAwareWebClient cookieAwareWebClient = new CookieAwareWebClient(cookieContainer);
            cookieAwareWebClient.Headers.Add("user-agent", "BlitzChat");
            cookieAwareWebClient.Encoding = Encoding.UTF8;
            try
            {
                string input = cookieAwareWebClient.DownloadString(new Uri(uri, UriKind.RelativeOrAbsolute)).Replace('\n', ' ').Replace('\r', ' ');
                Regex regex = new Regex("BPC=(.*?)\"");
                Match match = regex.Match(input);
                cookieContainer.Add(new Cookie("BPC", match.Groups[1].Value, "/", "goodgame.ru"));
                uri += "?attempt=1";
                input = cookieAwareWebClient.DownloadString(new Uri(uri, UriKind.RelativeOrAbsolute)).Replace('\n', ' ').Replace('\r', ' ');
                regex = new Regex("token.*?\\'(.*?)\\'.*?channelId\\:(.*?)\\,.*?\\'(.*?)\\'", RegexOptions.Multiline);
                match = regex.Match(input);
                if (!match.Success)
                {
                    channelId = 0;
                    token = "";
                    return false;
                }
                token = match.Groups[1].Value;
                channelId = uint.Parse(match.Groups[2].Value);
                //this.channelName = match.Groups[3].Value;
            }
            catch (Exception)
            {
                channelId = 0;
                return false;
            }
            connectSocket();
            return true;
        }

        public void Stop() {
            socket_Dispose();
            Debug.Print("Goodgame socket stopped");
        }

        private void connectSocket() {
            socket = new WebSocket(socketAdress, "", WebSocketVersion.Rfc6455);
            socket.EnableAutoSendPing = true;
            socket.Closed += new EventHandler(socket_Closed);
            socket.Error += new EventHandler<ErrorEventArgs>(socket_Error);
            socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
            socket.Open();
        }

        private void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                JObject jObj = JsonConvert.DeserializeObject(e.Message) as JObject;
                string type = (string)jObj["type"];
                switch (type)
                {
                    case "welcome":
                        socket.Send(("{'type':'auth','data':{'user_id':0,'token':'" + token + "'}}").Replace("'", "\""));
                        break;
                    case "success_auth":
                        socket.Send(("{'type':'join','data':{'channel_id':" + channelId + "}}").Replace("'", "\""));
                        break;
                    case "success_join":
                        socket.Send(("{'type':'get_channel_history','data':{'channel_id':" + channelId + "}}").Replace("'", "\""));
                        break;
                    case "message":
                        lock (locker)
                        {
                            OnMessage((JObject)jObj["data"]);
                        }
                        break;
                    case "channel_history":
                        lock (locker)
                        {
                            OnHistory((JObject)jObj["data"]);
                        }
                        break;
                    default:
                        Debug.Print("Goodgame unparsed message: " + e.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Exception in socket_MessageReceived: " + ex.Message);
                Reconnect();
            }
        }

        private void OnHistory(JObject history)
        {
            JArray jArray = (JArray)history["messages"];
            using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)jArray).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    JObject msg = (JObject)enumerator.Current;
                    if (loadHistory)
                        OnMessage(msg);
                    else
                        lastMessageId = (int)msg["message_id"];
                }
            }
        }

        private void OnMessage(JObject msg)
        {
            int msgId = (int)msg["message_id"];
            if (lastMessageId < msgId)
            {
                lastMessageId = msgId;
                string nickname = (string)msg["user_name"];
                string text = (string)msg["text"];
                
                Message newMessage = new Message
                {
                    Name = nickname,
                    Text = text,
                };
                if (text.Length > channelName.Length && text.Substring(0, channelName.Length).ToLower() + "," == channelName.ToLower() + ",")
                {
                    newMessage.ToName = text.Substring(0, channelName.Length);
                    newMessage.Text = text.Substring(channelName.Length+1, text.Length-channelName.Length-1);
                }
                if(newMessage.Text.Contains("href=")){
                    int linkStartIndex = newMessage.Text.IndexOf("<a");
                    int linkStartLength = newMessage.Text.IndexOf("href=\"")+6 - linkStartIndex;
                    string linkStart = newMessage.Text.Substring(linkStartIndex, linkStartLength);
                    newMessage.Text = newMessage.Text.Replace(linkStart, "");
                    int linkEndIndex = newMessage.Text.IndexOf("\">");
                    int linkEndLength = newMessage.Text.IndexOf("</a>") + 4 - linkEndIndex;
                    string linkEnd = newMessage.Text.Substring(linkEndIndex, linkEndLength);
                    newMessage.Text = newMessage.Text.Replace(linkEnd, "");
                }
                if (OnMessageReceived != null)
                    OnMessageReceived(this, newMessage);
            }
        }

        public int getViewersCount() {
            Random random = new Random();
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadStringCompleted += delegate(object b, DownloadStringCompletedEventArgs a)
                {
                    if (a.Error == null)
                    {
                        int value;
                        if (int.TryParse(a.Result, out value))
                        {
                            viewers = value;
                        }
                    }
                };
                wc.DownloadStringAsync(new Uri(string.Format("http://ftp.goodgame.ru/counter/{0}.txt?rnd={1}", channelId, random.NextDouble()), UriKind.RelativeOrAbsolute));
                return viewers;
            }
            catch {
                Debug.Print("Cannot get Viewers count");
                return 0;
            }
        }

        private void socket_Error(object sender, ErrorEventArgs e)
        {
            Debug.Print("Socket Error Exception: " + e.Exception.Message);
            Reconnect();
        }

        private void socket_Closed(object sender, EventArgs e)
        {
            Debug.Print("Goodgame socket closed");
            Reconnect();
        }

        private void Reconnect()
        {
            socket_Dispose();
            if(lastMessageId > 0)
                loadHistory = true;
            Start();
            
        }

        private void socket_Dispose() {
            if (socket != null) {
                socket.Closed -= new EventHandler(socket_Closed);
                socket.Error -= new EventHandler<ErrorEventArgs>(socket_Error);
                socket.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
            }
            if (socket.State == WebSocketState.Open)
                socket.Close();
            socket = null;
        }

        public class Message : EventArgs {
            public string Name{get;set;}
            public string Text{get;set;}
            public string ToName { get; set; }
        }
    }

}

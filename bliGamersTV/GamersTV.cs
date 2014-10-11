using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bliWebClient;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
namespace bliGamersTV
{
    public class GamersTV
    {
        private string chatId;
        private const string chatUrl = "http://gamerstv.ru/modules/ajax/chat_cache/room_{0}.js";
        private const string chatUsers = "http://gamerstv.ru/modules/ajax/get_chat_online.php?room_id={0}";
        private const string smilesUrl = "http://gamerstv.ru/smiles/smiles.js";
        private const string imageUri = "http://gamerstv.ru/smiles/";
        public int lastMsgId { get; set; }
        public int userId {get;set;}
        public string streamerName { get; set; }
        private Dictionary<string, GamersTVSmile> smiles;
        public event EventHandler<GamersTVMessage> messageReceived;
        public GamersTV(string url) {
            chatId = getIdFromLink(url);
            parseStreamerName();
            smiles = new Dictionary<string, GamersTVSmile>();
            lastMsgId = 0;
            loadSmiles();
        }

        public static string getIdFromLink(string url) {
            string[] arr = url.Split('/');
            string pageName = arr[arr.Length - 1];
            pageName = pageName.Replace("i", "");
            return pageName.Replace(".html", "");
        }

        private void parseStreamerName() {
            string uri = "http://gamerstv.ru/video/i" + chatId+".html";
            CookieContainer cookieContainer = new CookieContainer();
            CookieAwareWebClient cookieAwareWebClient = new CookieAwareWebClient(cookieContainer);
            cookieAwareWebClient.Headers.Add("user-agent", "BlitzChat");
            cookieAwareWebClient.Encoding = Encoding.GetEncoding("Windows-1251");
            try
            {
                string input = cookieAwareWebClient.DownloadString(new Uri(uri, UriKind.RelativeOrAbsolute)).Replace('\n', ' ').Replace('\r', ' ');
                Regex regex = new Regex("<span class=\"name\">(.*?)</span>", RegexOptions.Multiline);
                Match match = regex.Match(input);
                if (!match.Success)
                {
                    streamerName = "";
                }
                streamerName = match.Groups[1].Value;
            }
            catch (Exception e)
            {
                Debug.Print("Exception on parsing userId from gamersTV: "+e.Message);
            }
        }

        public void loadChat()
        {
            bool completed = false;
            using (WebClientTimeOut wc = new WebClientTimeOut(2000))
            {

                wc.Headers.Add("user-agent", "BlitzChat");
                wc.DownloadStringCompleted += delegate(object a, DownloadStringCompletedEventArgs b)
                {
                    if (b.Error == null)
                    {
                        try
                        {
                            JObject messages = JsonConvert.DeserializeObject<JObject>(b.Result);
                            if (messages != null)
                            {
                                updateMessages(messages);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Print("Chat messages deserializing exception: " + e.Message);
                        }
                    }
                    completed = true;
                };
                wc.DownloadStringAsync(new Uri(string.Format(chatUrl, chatId)));
                while (!completed) { 
                    
                    Thread.Sleep(20); 
                }

            }
        }

        private void loadSmiles()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string text = webClient.DownloadString(smilesUrl);

                    JObject emotions = JsonConvert.DeserializeObject<JObject>(text);

                    foreach (JObject emote in emotions["img"])
                    {
                        GamersTVSmile s = new GamersTVSmile();
                        s.Code = emote["code"].ToString();
                        s.Source = imageUri + emote["src"].ToString();
                        s.Width = int.Parse(emote["width"].ToString());
                        s.Height = int.Parse(emote["height"].ToString());
                        smiles.Add(emote["code"].ToString(), s);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print("Exception on loading smiles: " + e.Message);
            }
        }

        public Dictionary<string, GamersTVSmile> checkSmiles(string msg)
        {
            string[] arrWords = msg.Split(' ');
            Dictionary<string, GamersTVSmile> dictSmiles = new Dictionary<string, GamersTVSmile>();

            foreach (string word in arrWords)
            {
                if (smiles.ContainsKey(word) && !dictSmiles.ContainsKey(word))
                {
                    GamersTVSmile s = new GamersTVSmile();
                    dictSmiles.Add(word, smiles[word]);
                }
            }
            return dictSmiles;
        }

        private void updateMessages(JObject messages)
        {
            foreach (JObject message in messages["text"]) {
                if (lastMsgId < int.Parse(message["chat_id"].ToString()))
                {
                    GamersTVMessage msg = new GamersTVMessage();
                    msg.chat_id = int.Parse(message["chat_id"].ToString());
                    msg.user_id = int.Parse(message["id"].ToString());
                    msg.to_user_id = int.Parse(message["to_id"].ToString());
                    msg.name = message["name"].ToString();
                    msg.text = message["text"].ToString();
                    if (msg.text.ToLower().Contains(streamerName.ToLower()))
                        msg.toStreamer = true;
                    else
                        msg.toStreamer = false;
                    if (msg.text.StartsWith("<u>"))
                    {
                        int startIndex = 0;
                        int endIndex = msg.text.IndexOf("</u>,", startIndex) + 5;
                        msg.toName = msg.text.Substring(startIndex, endIndex - startIndex);
                        msg.text = msg.text.Replace(msg.toName, "");
                        msg.toName = msg.toName.Replace("<u>", "");
                        msg.toName = msg.toName.Replace("</u>,", "");
                    }
                    lastMsgId = msg.chat_id;
                    if (messageReceived != null)
                        messageReceived(this, msg);
                }
            }
        }

        public string getViewers() {
            string uri = "http://gamerstv.ru/video/i" + chatId + ".html";
            CookieContainer cookieContainer = new CookieContainer();
            CookieAwareWebClient cookieAwareWebClient = new CookieAwareWebClient(cookieContainer);
            cookieAwareWebClient.Headers.Add("user-agent", "BlitzChat");
            cookieAwareWebClient.Headers.Add("Accept-Encoding", "");
            cookieAwareWebClient.Encoding = Encoding.GetEncoding("Windows-1251");
            int count = 0;
            try
            {
                string input = cookieAwareWebClient.DownloadString(new Uri(uri, UriKind.RelativeOrAbsolute)).Replace('\n', ' ').Replace('\r', ' ');
                Regex regex = new Regex("<div class=\"count_users\">(.*?)</div>", RegexOptions.Multiline);
                Match match = regex.Match(input);
                if (!match.Success)
                {
                    count = 0;
                }
                if (match.Groups.Count > 0 && match.Groups[1].Value != "")
                    count = int.Parse(match.Groups[1].Value);
                regex = new Regex("<div class=\"count_guests\">(.*?)</div>", RegexOptions.Multiline);
                match = regex.Match(input);
                if (!match.Success)
                {
                    count = 0;
                }
                if(match.Groups.Count > 0 && match.Groups[1].Value != "")
                    count += int.Parse(match.Groups[1].Value);
                return count.ToString();
            }
            catch (Exception e)
            {
                Debug.Print("Exception on parsing userId from gamersTV: " + e.Message);
                return "0";
            }
        }
        
        public static bool channelExists(string url) {
            try
            {
                using (var client = new NewWebClient())
                {
                    client.HeadOnly = true;
                    // fine, if channel exists
                    // throws Exception on 404
                    client.DownloadString(url);
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

    }
}

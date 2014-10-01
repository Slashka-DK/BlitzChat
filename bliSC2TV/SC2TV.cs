using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bliSC2TV
{
    public class SC2TV
    {
        private string channelName;
        private uint channelId;
        private Dictionary<string, SC2TVSmile> smiles;
        private const string smilesUri = @"http://chat.sc2tv.ru/js/smiles.js";
        private const string channelUri = @"http://sc2tv.ru/channel/";
        private const string chatUri = @"http://chat.sc2tv.ru/memfs/channel-{0}.json";
        private const string imageUri = @"http://chat.sc2tv.ru/img/";
        private int lastMsgId = 0;
        private int lastMsgIndex = 0;
        public event EventHandler<SC2TVMessage> messageReceived;
        public SC2TV(string channelName) {
            smiles = new Dictionary<string, SC2TVSmile>();
            this.channelName = channelName.ToLower();
            loadSmiles();
            Start();
        }

        public void Start() {
            connectSC2TV();
        }


        public void Stop() { 
            
        }

        public void connectSC2TV() {
            channelId = getChannelId(channelName);
        }

        public static bool channelExists() {
            return false;
        }

        private void updateMessages(JObject messages) {
            for (int i = messages["messages"].Count()-1; i >= 0; i--)
            {
                int Id = int.Parse(messages["messages"][i]["id"].ToString());
                if (lastMsgId < Id)
                {
                    lastMsgId = int.Parse(messages["messages"][i]["id"].ToString());
                    SC2TVMessage message = new SC2TVMessage();
                    message.Name = messages["messages"][i]["name"].ToString();
                    message.Text = messages["messages"][i]["message"].ToString();
                    if (message.Text.StartsWith("[b]"))
                    {
                        int startIndex = 0;
                        int endIndex = message.Text.IndexOf("[/b],", startIndex) + 5;
                        message.ToName = message.Text.Substring(startIndex, endIndex - startIndex);
                        message.Text = message.Text.Replace(message.ToName, "");
                        message.ToName = message.ToName.Replace("[b]", "");
                        message.ToName = message.ToName.Replace("[/b],", "");
                    }
                    message.Text = message.Text.Replace("[url]", "");
                    message.Text = message.Text.Replace("[/url]", "");
                    messageReceived(this, message);
                    lastMsgId = Id;
                }
            }
        }

        private void loadSmiles() {
            try
            {
                using(WebClient webClient = new WebClient()){
                    string text = webClient.DownloadString(smilesUri);
                    int startIndex = text.IndexOf('[');
                    int endIndex = text.IndexOf(';');
                    string array = text.Substring(startIndex, endIndex - startIndex);
                    JArray emotions = JsonConvert.DeserializeObject<JArray>(array);
                    
                    foreach (JObject emote in emotions)
                    {
                        SC2TVSmile s = new SC2TVSmile();
                        s.code = ":s" + emote["code"].ToString();
                        s.uri = new Uri(imageUri + emote["img"].ToString());
                        s.width = int.Parse(emote["width"].ToString());
                        s.height = int.Parse(emote["height"].ToString());
                        smiles.Add(":s" + emote["code"].ToString(), s);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Print("Exception on loading smiles: " + e.Message);
            }
        }

        public Dictionary<string, SC2TVSmile> checkSmiles(string msg) { 
            string[] arrWords = msg.Split(' ');
            Dictionary<string, SC2TVSmile> dictSmiles = new Dictionary<string, SC2TVSmile>();

            foreach (string word in arrWords) {
                if (smiles.ContainsKey(word)) {
                    SC2TVSmile s = new SC2TVSmile();
                    dictSmiles.Add(word, smiles[word]);
                }
            }
            return dictSmiles;
        }

        public void loadChat() {
            bool completed = false;
            using (WebClient wc = new WebClient())
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
                        finally {
                            completed = true;
                        }
                    }
                };
                wc.DownloadStringAsync(new Uri(string.Format(chatUri, channelId)));
                while (!completed) { Thread.Sleep(20); }
            }
        }

        private uint getChannelId(string channelName) {
            uint channelId = 0;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = null;
            try
            {
                doc = web.Load(channelUri+channelName );
            }
            catch (Exception e)
            {
                Debug.Print("Exception on loading channelId: "+e.Message);
                return 0;
            }
            var result = from link in doc.DocumentNode.SelectNodes("//link[@rel='canonical']")
                         select link.Attributes["href"].Value;
            foreach (string r in result)
            {
                if (r.Contains("node"))
                {
                    string[] arr = r.Split('/');
                    channelId = Convert.ToUInt32(arr[arr.Length - 1]);
                    break;
                }
            }
            return channelId;
        }
    }
    public class SC2TVMessage
    {
        public string Name { get; set; }
        public string ToName { get; set; }
        public string Text { get; set; }
    }

    public class SC2TVSmile {
        public Uri uri { get; set; }
        public string code { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}

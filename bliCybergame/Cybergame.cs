using bliWebClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using System;
using System.Diagnostics;
using WebSocket4Net;
namespace bliCybergame
{
    public class Cybergame
    {
        private string channelName;
        private WebSocket socket;
        private const string socketAdress = "ws://cybergame.tv:9090/websocket";
        private const string infoUrl = "http://api.cybergame.tv/p/statusv2/?channel={0}";
        public event EventHandler<CybergameMessage> messageReceived;

        public Cybergame(string channel, bool start = false) {
            channelName = channel.ToLower();
            if (start)
                this.Start();
        }

        public void Start() {
            connectSocket();
            Debug.WriteLine("Cybergame started");
        }

        public static bool channelExists(string channel)
        {
            try
            {
                using (var client = new NewWebClient())
                {
                    client.HeadOnly = true;
                    // fine, no content downloaded
                    // throws 404
                    client.DownloadString("http://cybergame.tv/" + channel);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void connectSocket()
        {
            socket = new WebSocket(socketAdress, "", WebSocketVersion.Rfc6455);
            if (socket.State == WebSocketState.Open)
                socket.Close();
            socket.EnableAutoSendPing = true;
            socket.Closed += new EventHandler(socket_Closed);
            socket.Error += new EventHandler<ErrorEventArgs>(socket_Error);
            socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
            socket.Opened += socket_Opened;
            socket.Open();
            Debug.WriteLine("Cybergame socket connected.");
        }

        private void disableEventListeners() {
            socket.Closed -= new EventHandler(socket_Closed);
            socket.Error -= new EventHandler<ErrorEventArgs>(socket_Error);
            socket.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
            socket.Opened -= socket_Opened;
        }

        private void socket_Opened(object sender, EventArgs e)
        {
            try
            {
                socket.Send("{\"command\":\"login\",\"message\":\"{\\\"login\\\":\\\"\\\",\\\"password\\\":\\\"\\\",\\\"channel\\\":\\\"#" + channelName + "\\\"}\"}");
            }
            catch (Exception exc) {
                Debug.WriteLine("Connect to channel socket Exception: " + exc.Message);     
            }
        }

        private void socket_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket closed. Reconnecting...");
            Reconnect();
        }

        private void socket_Error(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine("Socket error. Reconnecting...");
            Reconnect();
        }

        private void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JObject jObject;
            try
            {
                jObject = JsonConvert.DeserializeObject<JObject>(e.Message);
            }
            catch(Exception ex) {
                Debug.WriteLine("Socket message exception: " + ex.Message);
                return;
            }
            string command = (string)jObject["command"];
            Debug.WriteLine(command);
            if (command != null && command == "chatMessage")
            {
                onMessage(((JValue)jObject["message"]).Value as string);
            }
        }

        public void onMessage(string msg){
            CybergameSocketMessage socketMessage = JsonConvert.DeserializeObject<CybergameSocketMessage>(msg);
            CybergameMessage chatMessage = new CybergameMessage
            {
                Name = socketMessage.From,
                Text = socketMessage.Text,
                ToName = socketMessage.Text.Contains(channelName) ? channelName : ""
            };
            messageReceived(this, chatMessage);
        }

        public void Stop()
        {
            if(socket.State == WebSocketState.Open)
                socket.Close();
            disableEventListeners();
            Debug.WriteLine("Cybergame stopped.");
        }

        public void Reconnect() {
            Stop();
            Start();
            Debug.WriteLine("Cybergame reconnected.");
        }

        private class CybergameSocketMessage
		{
			[JsonProperty("when")]
			public long When
			{
				get;
				set;
			}
			[JsonProperty("from")]
			public string From
			{
				get;
				set;
			}
			[JsonProperty("text")]
			public string Text
			{
				get;
				set;
			}
		}
    }
}

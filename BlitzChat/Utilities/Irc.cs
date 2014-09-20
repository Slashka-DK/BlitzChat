using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotIRC;
using System.Threading;
using System.Windows;
namespace BlitzChat
{
    public class Irc
    {
        public IrcClient client{get;set;}

        public Irc() {
            client = new IrcClient();
            client.Error += new EventHandler<IrcErrorEventArgs>(client_Error);
            client.ErrorMessageReceived += client_ErrorMessageReceived;
        }

        void client_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        void client_Error(object sender, IrcErrorEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void Connect(string adress, int port, string nickname, string password) {
            IrcUserRegistrationInfo regInf = new IrcUserRegistrationInfo()
            {
                NickName = nickname,
                UserName = nickname,
                Password = password,
                RealName = nickname
            };
            try
            {
                client.Connect(adress, port, false, regInf);
                do{Thread.Sleep(1000);} while (!client.IsConnected);
                //client.LocalUser.SetModes("+i");
            }
            catch
            {
                MessageBox.Show("Can not connect to IRC channel "+adress+":"+port+". Please try again later.");
            }
        }

        public void channelConnect(string channelname) { 
            IrcChannelCollection channels = client.Channels;
            try
            {
                channels.Join("#"+channelname);
            }
            catch {
                throw new WrongChannelNameException();
            }
            do { Thread.Sleep(1000); } while (client.Channels.Count <= 0);
            client.Channels[0].SetModes("+i");
        }
    }
}

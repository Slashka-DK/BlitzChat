using dotCybergame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlitzChat
{
    public class CybergameTV
    {
        public Cybergame cb;
        string channel;
        public CybergameTV(string channel) {
            this.channel = channel;
            cb = new Cybergame("blitzchat", "zkyhQ7tQPVzuxGhlX6hH", channel);
        }

        public void connectCybergame()
        {
            cb.Live += new EventHandler<EventArgs>(cybergame_Live);
            cb.Offline += new EventHandler<EventArgs>(cybergame_Offline);
            cb.OnLogin += new EventHandler<EventArgs>(cybergame_OnLogin);
            cb.Login();
        }

        private void cybergame_Live(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void cybergame_Offline(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void cybergame_OnLogin(object sender, EventArgs e)
        {
            cb.GetDescription();
        }
    }
}

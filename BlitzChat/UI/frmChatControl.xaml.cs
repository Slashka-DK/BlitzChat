using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class ChatControl : Window
    {
        public ChatControl()
        {
            InitializeComponent();
            
            cmbAddChat.SelectedIndex = 0;
            //txtChannel.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(txtChannel_MouseLeftButtonDown), true);
        }


        private void bttnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void frmAddChat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void txtChannel_GotFocus(object sender, RoutedEventArgs e)
        {
            txtChannel.Clear();
        }

        private void cmbAddChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddChat.SelectedValue == null)
            {
                txtChannel.Text = "Nothing to add";
                return;
            }
            switch (cmbAddChat.SelectedValue.ToString()) { 
                case Constants.TWITCH:
                    txtChannel.Text = "Twitch streamer name";
                    break;
                case Constants.SC2TV:
                    txtChannel.Text = "SC2TV streamer name";
                    break;
                case Constants.CYBERGAME:
                    txtChannel.Text = "Cybergame streamer name";
                    break;
                case Constants.GOODGAME:
                    txtChannel.Text = "Goodgame streamer name";
                    break;
                case Constants.GAMERSTV:
                    txtChannel.Text = "GamersTV stream URL";
                    break;
                case Constants.GOHATV:
                    txtChannel.Text = "GohaTV streamer name";
                    break;
                case Constants.HITBOX:
                    txtChannel.Text = "Hitbox streamer name";
                    break;
                case Constants.EMPIRE:
                    txtChannel.Text = "EmpireTV streamer name";
                    break;
                default:
                    txtChannel.Text = "Nothing to add";
                    break;
            }
        }
    }
}

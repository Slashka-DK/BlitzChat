using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class AddChat : Window
    {
        public AddChat()
        {
            InitializeComponent();
            cmbAddChat.Items.Add(Constants.TWITCH);
            cmbAddChat.Items.Add(Constants.SC2TV);
            cmbAddChat.Items.Add(Constants.CYBERGAME);
            cmbAddChat.Items.Add(Constants.GOODGAME);
            cmbAddChat.Items.Add(Constants.HITBOX);
            cmbAddChat.Items.Add(Constants.EMPIRE);
            cmbAddChat.SelectedIndex = 0;
            txtChannel.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(txtChannel_MouseLeftButtonDown), true);
        }

        public static int Compare(String left, String right)
        {
            return left.GetHashCode() - right.GetHashCode();
        } 
                
        private void txtChannel_MouseEnter(object sender, MouseEventArgs e)
        {
            
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

        private void txtChannel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtChannel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(txtChannel.Text == "StreamerName")
                txtChannel.Clear();
        }
    }
}

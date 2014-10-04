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

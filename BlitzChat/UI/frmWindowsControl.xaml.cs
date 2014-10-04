using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlitzChat.UI
{
    /// <summary>
    /// Interaktionslogik für frmWindowsControl.xaml
    /// </summary>
    public partial class frmWindowsControl : Window
    {
        public frmWindowsControl()
        {
            InitializeComponent();
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

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            bttnAdd.IsEnabled = true;
        }

        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
        }
    }
}

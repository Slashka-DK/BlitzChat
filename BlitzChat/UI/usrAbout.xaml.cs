using System.Windows;
using System.Windows.Controls;

namespace BlitzChat.UI
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class usrAbout : UserControl
    {
        public usrAbout()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            UrlTools.Hyperlink_Click(sender, e);
        }
    }
}

using System.Windows.Controls;
using System.Windows.Media;

namespace BlitzChat.UI
{
    /// <summary>
    /// Interaktionslogik für Header.xaml
    /// </summary>
    public partial class usrHeader : UserControl
    {
        public usrHeader()
        {
            InitializeComponent();
            SolidColorBrush brush = new SolidColorBrush(Colors.Transparent);
            brush.Opacity = 0;
            Background = brush;
            
        }
    }
}

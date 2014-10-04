using System.Windows;
using System.Windows.Input;
namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class frmViewers : Window
    {
        //private StickyWindow _stickyWindow;
        public frmViewers()
        {
            InitializeComponent();
        }

        private void frmViewers_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


    }
}

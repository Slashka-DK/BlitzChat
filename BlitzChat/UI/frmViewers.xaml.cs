using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class frmViewers : Window
    {
        //private StickyWindow _stickyWindow;

        #region WinAPI
        private int _normalStyle;
        private const int GWL_EXSTYLE = -20;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public void setBackgroundMode(bool b){
            if (b)
            {
                WindowInteropHelper helper = new WindowInteropHelper(this);
                SetWindowLong(helper.Handle, GWL_EXSTYLE,
                    GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT);
                this.IsHitTestVisible = false;
            }
            else
            {
                WindowInteropHelper helper = new WindowInteropHelper(this);
                SetWindowLong(helper.Handle, GWL_EXSTYLE, _normalStyle);
                this.IsHitTestVisible = true;
            }
        }
        #endregion
        public frmViewers()
        {
            InitializeComponent();
        }

        private void frmViewers_Loaded(object sender, RoutedEventArgs e)
        {
            _normalStyle = GetWindowLong(new WindowInteropHelper(this).Handle, GWL_EXSTYLE); 
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


    }
}

using System.Windows.Controls;

namespace BlitzChat.UI
{
    /// <summary>
    /// Interaktionslogik für usrRichTextBox.xaml
    /// </summary>
    public partial class usrRichTextBox : UserControl
    {
        public usrRichTextBox()
        {
            InitializeComponent();
            this.richChat.Document.TextAlignment = System.Windows.TextAlignment.Left;
        }
    }
}

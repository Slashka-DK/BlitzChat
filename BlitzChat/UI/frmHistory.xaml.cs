using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace BlitzChat.UI
{
    /// <summary>
    /// Interaktionslogik für frmHistory.xaml
    /// </summary>
    public partial class frmHistory : Window
    {
        string windowName;
        public frmHistory(string name)
        {
            InitializeComponent();
            windowName = name;
            preloadHistoryList();
        }

        private void preloadHistoryList()
        {
            if (Directory.Exists(Constants.HISTORYDIR))
            {
                foreach (string file in Directory.GetFileSystemEntries(Constants.HISTORYDIR)) {
                    string name = file.Substring(file.Length-15, 10);
                    cmbHistoryList.Items.Add(name);
                }
                cmbHistoryList.SelectedIndex = cmbHistoryList.Items.Count - 1;
            }
            else
                return;
        }

        private void bttnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void frmHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void bttnLoad_Click(object sender, RoutedEventArgs e)
        {
            Paragraph p = new Paragraph();

            usrRTB.richChat.Width = col0.ActualWidth + col1.ActualWidth;
            p.Inlines.Add("Loading...");
            
            usrRTB.richChat.Document.Blocks.Add(p);
            usrRTB.richChat.Document.TextAlignment = TextAlignment.Left;
            usrRTB.richChat.Document.FontSize = 20;
            usrRTB.richChat.Document.Foreground = new SolidColorBrush(Colors.White); 

            Thread thr = new Thread(thr_load);
            thr.Start();
        }

        private void thr_load(object obj)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                string date = cmbHistoryList.SelectedValue.ToString();
                string[] files = Directory.GetFiles(Constants.HISTORYDIR, "*" +windowName+"_"+date + ".hist");
                string file = files[0];
                TextRange tr = new TextRange(usrRTB.richChat.Document.ContentStart, usrRTB.richChat.Document.ContentEnd);
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true))
                {
                    if (tr.CanLoad(DataFormats.XamlPackage))
                    {
                        tr.Load(fs, DataFormats.XamlPackage);
                    }
                }
                foreach (Block b in usrRTB.richChat.Document.Blocks) {
                    b.TextAlignment = TextAlignment.Left;
                }
            }));  
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            usrRTB.richChat.Width = col0.ActualWidth + col1.ActualWidth;
        }
    }
}

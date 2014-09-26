using BlitzChat.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlitzChat.Forms;
using System.Windows;
using System.IO;
using NCrash.WinForms;
using NCrash;
using NCrash.Plugins;
using NCrash.WPF;
namespace BlitzChat
{
    class WindowControl
    {
        Dictionary<string,MainWindow> listMainWindows;
        Windows windows;
        frmWindowsControl frmControl;
        public WindowControl(){

            listMainWindows = new Dictionary<string, MainWindow>();
            windows = new Windows();
            deserializeWindows();
            foreach (string window in windows.listWindows) {
                addWindow(window);
            }
        }

        private void newWindow_OnAdditionalWindows(object sender, MainWindow e)
        {
            if (frmControl != null && frmControl.IsLoaded)
                return;
            
            frmControl = new frmWindowsControl();
            frmControl.bttnAdd.Click += bttnAdd_Click;
            frmControl.bttnDelete.Click += bttnDelete_Click;
            frmControl.listWindows.SelectionChanged += listWindows_SelectionChanged;
            foreach (KeyValuePair<string, MainWindow> kv in listMainWindows)
            {
                frmControl.listWindows.Items.Add(kv.Value.WindowName);
            }
            frmControl.Show();
        }

        void listWindows_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            frmControl.bttnDelete.IsEnabled = true;
        }

        void bttnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (frmControl.listWindows.SelectedItem.ToString() == "Main") {
                MessageBox.Show("You can not delete Main Window");
                return;
            }
            string windowToDelete = frmControl.listWindows.SelectedItem.ToString();
            listMainWindows[windowToDelete].Close();
            listMainWindows.Remove(windowToDelete);
            frmControl.listWindows.Items.Remove(windowToDelete);
            windows.listWindows.Remove(windowToDelete);
            string fileChannels = Constants.CONFIGPATH + windowToDelete + "_" + Constants.CHANNELSFILE;
            string fileSettings = Constants.CONFIGPATH + windowToDelete + "_" + Constants.SETTINGSFILE;
            if (File.Exists(fileChannels))
                File.Delete(fileChannels);
            if (File.Exists(fileSettings))
                File.Delete(fileSettings);
            serializeWindows();
        }

        void bttnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(frmControl.txtName.Text) || frmControl.txtName.Text == "Main" || frmControl.txtName.Text == "WindowName")
            {
                MessageBox.Show("Wrong name. It can not be set as empty or \"Main\" or \"WindowName\". Try again.");
                return;
            }
            string windowName = frmControl.txtName.Text;
            addWindow(windowName);
            frmControl.listWindows.Items.Add(windowName);
            windows.listWindows.Add(windowName);
            serializeWindows();
        }

        private void addWindow(string name){
            MainWindow newWindow = new MainWindow(name);
            newWindow.OnAdditionalWindows += newWindow_OnAdditionalWindows;
            listMainWindows.Add(name,newWindow);
            newWindow.Show();
        }
        
        private void serializeWindows(){
            XMLSerializer.serialize(windows, Constants.CONFIGPATH+"Windows.xml");
        }

        private void deserializeWindows() {
            Windows newWindows = null;
            if (File.Exists(Constants.CONFIGPATH + "Windows.xml"))
               newWindows  = (Windows)XMLSerializer.deserialize(windows,Constants.CONFIGPATH+ "Windows.xml");
               if (newWindows != null)
               {
                   windows = newWindows;
               }
               else
                   windows.listWindows.Add("Main");
        }
    }
}

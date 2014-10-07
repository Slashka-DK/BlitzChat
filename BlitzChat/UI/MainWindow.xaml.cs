using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media.Animation;
using System.Reflection;
using System.Net;
using HtmlAgilityPack;
using bliGoodgame;
using System.Text.RegularExpressions;
using System.Diagnostics;
using BlitzChat.UI;
using bliTwitch;
using bliSC2TV;
using bliCybergame;
using System.ComponentModel;
using BlitzChat.Utilities;
namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members and Fields
        SolidColorBrush mainbackBrush;
        SolidColorBrush contextBrush;
        SolidColorBrush textBrush;
        SolidColorBrush nicknameBrush;
        SolidColorBrush dateBrush;
        SolidColorBrush quoteBrush;
        frmSettings frmSettings;
        Color nicknameColor { get; set; }
        ChatControl frmAddChat;
        ChatSettingsXML settingsChat;
        ChannelsSaveXML channels;
        Twitch twitch;
        SC2TV sc2tv;
        Cybergame cyberg;
        Goodgame goodgame;
        private volatile bool bTwitchEnd;
        private volatile bool bSC2TVEnd;
        private volatile bool bCybergameEnd;
        private volatile bool bGoodgameEnd;
        private volatile bool bViewersEnd = true;
        private volatile bool bSaveScreenEnd = false;
        bool reload = false;
        FlowDocument historydoc;
        public string WindowName { get; set; }
        List<string> listChats;
        FontWeight nicknameWeight { get; set; }
        FontWeight textWeight { get; set; }
        FontFamily font { get; set; }
        private List<TextBlock> listMessages;
        public frmViewers viewers { get; set; }
        public event EventHandler<MainWindow> OnAdditionalWindows;
        public event EventHandler<MainWindow> OnTransparencyChanged;
        ListBox listChat;
        #endregion

        #region Constructors
        public MainWindow(string name)
        {
            header = new usrHeader();
            header.lblWindowName.Content = name;
            InitializeComponent();
            historydoc = new FlowDocument();
            listMessages = new List<TextBlock>();
            WindowName = name;
            listChats = new List<string>();
            listChat = usrLstChat.listChat;
            settingsChat = new ChatSettingsXML();
            channels = new ChannelsSaveXML();
            bTwitchEnd = false;
            Height = settingsChat.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 150;
            Width = settingsChat.Width = 300;
            Top = settingsChat.Top = 0;
            Left = settingsChat.Left = System.Windows.SystemParameters.PrimaryScreenWidth - Width;
            Topmost = true;
            textWeight = FontWeights.Normal;
            nicknameWeight = FontWeights.Bold;
            header.Width = Column0.ActualWidth;
            header.Height = double.NaN;
            listChat.Height = Row1.ActualHeight;
            listChat.Width = Column0.ActualWidth;
            listChat.MouseEnter += richChat_MouseEnter;
            listChat.MouseLeave += richChat_MouseLeave;
            var bc = new BrushConverter();
            mainbackBrush = new SolidColorBrush(fromHexColor(settingsChat.BackgroundColor));
            mainbackBrush.Opacity = settingsChat.ChatOpacity;
            contextBrush = new SolidColorBrush(Colors.Black);
            textBrush = new SolidColorBrush(Colors.White);
            quoteBrush = new SolidColorBrush(Colors.Orange);
            dateBrush = new SolidColorBrush(Colors.White);
            dateBrush.Opacity = 0.7;
            nicknameColor = Colors.BlueViolet;
            nicknameBrush = new SolidColorBrush(nicknameColor);
            listChat.Foreground = textBrush;
            Background = mainbackBrush;
            mainContextMenu.Background = contextBrush;
            mainContextMenu.Foreground = new SolidColorBrush(Colors.White);
            deserializeSettings();
            frmSettings = new frmSettings();
            //listChat.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            deserializeChannels();
            this.ContextMenu = mainContextMenu;
            listChat.ContextMenu = this.ContextMenu;
            header.ContextMenu = mainContextMenu;
            Version vers = Assembly.GetExecutingAssembly().GetName().Version;
            header.lblProgramName.Content = "BlitzСhat v." + vers.ToString() + " Alpha";
            preSetSettings();
            //RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            loadHistory();
            listChat.LayoutUpdated += richChat_LayoutUpdated;
        }

        void richChat_LayoutUpdated(object sender, EventArgs e)
        {

            if ((ScrollBarVisibility)listChat.GetValue(ScrollViewer.VerticalScrollBarVisibilityProperty) != ScrollBarVisibility.Visible)
                listChat.ScrollIntoView(listChat.Items[listChat.Items.Count - 1]);
        }

        #endregion

        #region UI Events
        private void history_Click(object sender, RoutedEventArgs e)
        {
            Thread historyThread = new Thread(delegate()
            {
                frmHistory history = new frmHistory(WindowName);
                history.Width = this.ActualWidth;
                history.Height = this.ActualHeight;
                history.ShowDialog();
            });

            historyThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
            historyThread.Name = "History thread";
            historyThread.Start();
        }

        private void addWindows_Click(object sender, RoutedEventArgs e)
        {
            if (OnAdditionalWindows != null)
                OnAdditionalWindows(sender, this);
        }

        private void chkDateEnabled_Click(object sender, RoutedEventArgs e)
        {
            settingsChat.DateEnabled = frmSettings.chkDateEnabled.IsChecked.Value;
            serializeSettings();
            applySettingsToBlocks();
        }

        void ClrPcker_Date_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Color newColor = frmSettings.ClrPcker_Date.SelectedColor;
            dateBrush.Color = newColor;
            settingsChat.DateColor = ToHexColor(newColor);
            serializeSettings();
        }

        void chkNicknameBold_Click(object sender, RoutedEventArgs e)
        {
            settingsChat.NicknameBold = frmSettings.chkNicknameBold.IsChecked.Value;
            if (settingsChat.NicknameBold)
            {
                nicknameWeight = FontWeights.Bold;
            }
            else
            {
                nicknameWeight = FontWeights.Normal;
            }
            applySettingsToBlocks();
            serializeSettings();
        }

        private void chkTextBold_Click(object sender, RoutedEventArgs e)
        {
            settingsChat.TextBold = frmSettings.chkTextBold.IsChecked.Value;
            if (settingsChat.TextBold)
            {
                textWeight = FontWeights.Bold;
            }
            else
            {
                textWeight = FontWeights.Normal;
            }
            applySettingsToBlocks();
            serializeSettings();
        }
        void bttnSmileSmaller_Click(object sender, RoutedEventArgs e)
        {
            settingsChat.SmileSize--;
            frmSettings.lblSmileSize.Content = settingsChat.SmileSize;
            foreach (TextBlock block in listChat.Items)
            {
                foreach (Inline inline in block.Inlines)
                {
                    if (inline.Name == "Smile" && inline is InlineUIContainer)
                    {
                        InlineUIContainer uiContainer = (InlineUIContainer)inline;
                        if (uiContainer.Child is Image)
                        {
                            Image image = (Image)uiContainer.Child;
                            image.Width--;
                            image.Height--;
                        }
                    }
                    
                }   
            }
            
            serializeSettings();
        }
        private void bttnSmileBigger_Click(object sender, RoutedEventArgs e)
        {
            settingsChat.SmileSize++;
            frmSettings.lblSmileSize.Content = settingsChat.SmileSize;
            foreach (TextBlock block in listChat.Items)
            {
                foreach (Inline inline in block.Inlines)
                {
                    if (inline.Name == "Smile" && inline is InlineUIContainer)
                    {
                        InlineUIContainer uiContainer = (InlineUIContainer)inline;
                        if (uiContainer.Child is Image)
                        {
                            Image image = (Image)uiContainer.Child;
                            image.Width++;
                            image.Height++;
                        }
                    }
                    
                }   
            }
            serializeSettings();
        }
        private void richChat_MouseEnter(object sender, MouseEventArgs e)
        {
            listChat.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
            listChat.Focus();
        }


        private void richChat_MouseLeave(object sender, MouseEventArgs e)
        {
            listChat.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
        }
        private void clear_Chat_Click(object sender, RoutedEventArgs e)
        {
            listChat.Items.Clear();

        }

        private void frmChat_Loaded(object sender, RoutedEventArgs e)
        {
            //StickyWindow.RegisterExternalReferenceForm(this);
            header.lblWindowName.Content = WindowName;
            Thread threadHist = new Thread(threadSaveHistory);
            threadHist.Name = "Save history";
            threadHist.IsBackground = true;
            //threadHist.Start();
            ThreadPool.QueueUserWorkItem(threadSaveScreenshot);
        }


        private void show_Viewers_Click(object sender, RoutedEventArgs e)
        {
            if (bViewersEnd)
            {
                viewers = new frmViewers();
                viewers.Topmost = settingsChat.TopMost;
                viewers.AllowsTransparency = settingsChat.TransparencyEnabled;
                viewers.Background = mainbackBrush;
                viewers.Opacity = settingsChat.ChatOpacity;
                viewers.Top = this.Top + this.ActualHeight;
                viewers.Width = this.Width;
                viewers.Left = this.Left;

                viewers.Show();
                bViewersEnd = false;
                viewersMenuItem.Header = "Close viewers";
                Thread threadViewers = new Thread(threadViewersShow);
                threadViewers.IsBackground = true;
                threadViewers.Name = "Viewers thread";
                threadViewers.Start();
            }
            else
            {
                bViewersEnd = true;
                viewersMenuItem.Header = "Show viewers";
                viewers.Hide();
                viewers.Close();
            }
        }

        private void frmChat_LocationChanged(object sender, EventArgs e)
        {
            settingsChat.Left = RestoreBounds.Left;
            settingsChat.Top = RestoreBounds.Top;
            if (viewers != null)
            {
                viewers.Top = this.Top + this.ActualHeight;
                viewers.Left = this.Left;
            }
            if(this.WindowState != System.Windows.WindowState.Minimized)
                serializeSettings();
            
        }

        private void frmChat_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ResizeMode == System.Windows.ResizeMode.NoResize)
            {
                // restore resize grips
                this.ResizeMode = System.Windows.ResizeMode.CanResize;
                this.UpdateLayout();
            }
        }

        private void listStreamers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            frmAddChat.bttnRemove.IsEnabled = true;
        }


        private void chat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // this prevents win7 aerosnap
                if (this.ResizeMode != System.Windows.ResizeMode.NoResize)
                {
                    this.ResizeMode = System.Windows.ResizeMode.NoResize;
                    this.UpdateLayout();
                }
                frmChat.DragMove();
            }
        }

        private void contextMenu_Settings_Click(object sender, RoutedEventArgs e)
        {
            frmSettings = new frmSettings();
            settingsInit();
            if (this.AllowsTransparency == false)
            {
                this.frmSettings.sliderOpacity.IsEnabled = false;
                mainbackBrush.Opacity = 1;
            }
            else
                this.frmSettings.sliderOpacity.IsEnabled = true;
            frmSettings.Show();
        }

        private void cmbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!frmSettings.IsLoaded)
                return;
            //TODO: Maybe need loop change all blocks
            string newFont = frmSettings.cmbFonts.SelectedItem.ToString();
            font = new FontFamily(newFont);
            listChat.FontFamily = font;
            settingsChat.TextFont = newFont;
            serializeSettings();
            applySettingsToBlocks();
        }

        private void numSizeText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!frmSettings.IsLoaded)
                return;
            double newSize = Convert.ToDouble(frmSettings.numSizeText.Value.Value);
            //TODO: Maybe need loop change all blocks
            settingsChat.TextFontSize = newSize;
            serializeSettings();
            applySettingsToBlocks();
        }

        private void ClrPcker_Nickname_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (!frmSettings.IsLoaded)
                return;
            Color newColor = frmSettings.ClrPcker_Nickname.SelectedColor;
            nicknameColor = newColor;
            settingsChat.NicknameColor = ToHexColor(newColor);
            nicknameBrush.Color = nicknameColor;
            serializeSettings();
        }

        private void ClrPcker_Text_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (!frmSettings.IsLoaded)
                return;
            Color newColor = frmSettings.ClrPcker_Text.SelectedColor;
            textBrush.Color = newColor;
            listChat.Foreground = textBrush;
            settingsChat.ForeColor = ToHexColor(newColor);
            serializeSettings();
            //applySettingsToBlocks();
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (!frmSettings.IsLoaded)
                return;
            Color newColor = frmSettings.ClrPcker_Background.SelectedColor;
            mainbackBrush.Color = newColor;
            settingsChat.BackgroundColor = ToHexColor(newColor);
            serializeSettings();
        }

        private void chkOnTop_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void chkOnTop_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void sliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double newOpacity = e.NewValue / 100;
            mainbackBrush.Opacity = newOpacity;
            settingsChat.ChatOpacity = newOpacity;
            //header.Opacity = settingsChat.ChatOpacity;
            serializeSettings();
        }

        private void contextMenu_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bttnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void frmChat_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            listChat.Width = Column0.ActualWidth;
            listChat.Height = Row1.ActualHeight;
            header.Width = Column0.ActualWidth;
            settingsChat.Width = RestoreBounds.Width;
            settingsChat.Height = RestoreBounds.Height;
            if (viewers != null)
            {
                viewers.Width = this.ActualWidth;
                viewers.Top = this.Top + this.ActualHeight;
            }
            serializeSettings();
        }

        private void MenuItem_AddChat_Click(object sender, RoutedEventArgs e)
        {
            if (frmAddChat != null && frmAddChat.IsLoaded)
                return;

            frmAddChat = new ChatControl();
            foreach (object item in listChats)
                frmAddChat.listStreamers.Items.Add(item);
            frmAddChat.bttnAddChat.Click += bttnAddChat_Click;
            frmAddChat.listStreamers.SelectionChanged += listStreamers_SelectionChanged;
            frmAddChat.bttnRemove.Click += bttnRemove_Click;
            initCmbAddChat();
            frmAddChat.cmbAddChat.SelectedIndex = 0;
            frmAddChat.Show();
        }

        private void initCmbAddChat()
        {
            frmAddChat.cmbAddChat.Items.Add(Constants.TWITCH);
            frmAddChat.cmbAddChat.Items.Add(Constants.SC2TV);
            frmAddChat.cmbAddChat.Items.Add(Constants.CYBERGAME);
            frmAddChat.cmbAddChat.Items.Add(Constants.GOODGAME);
            frmAddChat.cmbAddChat.Items.Add(Constants.HITBOX);
            frmAddChat.cmbAddChat.Items.Add(Constants.EMPIRE);
            if (channels != null && !String.IsNullOrEmpty(channels.Twitch))
            {
                frmAddChat.cmbAddChat.Items.Remove(Constants.TWITCH);
            }
            if (channels != null && !String.IsNullOrEmpty(channels.SC2TV))
            {
                frmAddChat.cmbAddChat.Items.Remove(Constants.SC2TV);

            }
            if (channels != null && !String.IsNullOrEmpty(channels.Cybergame))
            {
                frmAddChat.cmbAddChat.Items.Remove(Constants.CYBERGAME);

            }
            if (channels != null && !String.IsNullOrEmpty(channels.GoodGame))
            {
                frmAddChat.cmbAddChat.Items.Remove(Constants.GOODGAME);

            }
        }

        private void bttnRemove_Click(object sender, RoutedEventArgs e)
        {
            removeChannel();
        }

        private void bttnAddChat_Click(object sender, RoutedEventArgs e)
        {
            addChannel();
        }

        private void frmChat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.None;
            if (reload == true && (result = MessageBox.Show("Do you want to reload this window to apply settings?", "Reload window?", MessageBoxButton.YesNo)) == MessageBoxResult.Yes)
            {
                return;
            }
            else if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            else if (reload == false)
            {
                result = MessageBox.Show("Do you want to save your chat history?", "Save history?", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    saveHistory();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (WindowName == "Main")
                {
                    System.Environment.Exit(0);
                }
            }
        }

        private void contextMenu_hideHeader_Click(object sender, RoutedEventArgs e)
        {
            if (header.Visibility != Visibility.Hidden)
            {
                header.Visibility = Visibility.Hidden;
                menuHeader.Header = "Show header";
            }
            else
            {
                header.Visibility = Visibility.Visible;
                menuHeader.Header = "Hide header";
            }

        }
        #endregion

        #region Functional Events

        private void goodgame_OnMessageReceived(object sender, Goodgame.Message e)
        {
            addMessageToChat(3, e.Name, e.Text, e.ToName);
        }


        private void cybergame_MessageReceived(object sender, CybergameMessage e)
        {
            addMessageToChat(2, e.Name, e.Text, e.ToName);
        }

        private void sc2tv_MessageReceived(object sender, SC2TVMessage e)
        {
            string to = e.ToName;
            if (to == null)
                to = "";
            string msg = e.Text;
            addMessageToChat(1, e.Name, msg, to);
        }

        private void twitch_MessageReceived(object sender, TwitchMessage e)
        {
            string nick = e.Name;
            string message = e.Text;
            string to = "";
            addMessageToChat(0, nick, message, to);
        }
        #endregion

        #region Threads
        private void threadGoodgame(object obj)
        {
            try
            {
                goodgame = new Goodgame(channels.GoodGame);
                goodgame.OnMessageReceived += goodgame_OnMessageReceived;
                goodgame.Start();
            }
            catch (WrongChannelNameException)
            {
                MessageBox.Show("Wrong channelname was saved to the file. If it happens again. Please delete data from file Channels.xml or use Add chat form to delete it.");
            }
            while (!bGoodgameEnd) { Thread.Sleep(100); }
            goodgame.Stop();
            goodgame = null;
        }

        private void threadCybergame(object obj)
        {
            try
            {
                cyberg = new Cybergame(channels.Cybergame, true);
            }
            catch (WrongChannelNameException)
            {
                MessageBox.Show("Wrong channelname was saved to the file. If it happens again. Please delete data from file Channels.xml or use Add chat form to delete it.");
            }
            cyberg.messageReceived += new EventHandler<CybergameMessage>(cybergame_MessageReceived);
            while (!bCybergameEnd) { Thread.Sleep(100); }
            cyberg.Stop();
            cyberg = null;
        }

        private void threadSC2TV(object obj)
        {
            try
            {
                sc2tv = new SC2TV(channels.SC2TV);
                sc2tv.lastMsgId = channels.SC2TVLastMessageId;
                sc2tv.messageReceived += sc2tv_MessageReceived;

                while (!bSC2TVEnd)
                {
                    sc2tv.loadChat();
                    channels.SC2TVLastMessageId = sc2tv.lastMsgId;
                    serializeChannels();
                    Thread.Sleep(100);
                }
            }
            catch (WrongChannelNameException)
            {
                MessageBox.Show("Wrong channelname was saved to the file. If it happens again. Please delete data from file Channels.xml or use Add chat form to delete it.");
            }
            finally
            {
                sc2tv.Stop();
                sc2tv = null;
            }
        }

        private void threadSaveScreenshot(object obj) {
            while (!bSaveScreenEnd) {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    Screenshots.createScreenshot(this, "");
                }));
                Thread.Sleep(500);
            }
        }

        private void threadTwitch(object obj)
        {
            try
            {
                twitch = new Twitch(channels.Twitch, "irc.twitch.tv", 6667);
                twitch.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show("Exception: "+e.Message);
            }
            twitch.messageReceived += twitch_MessageReceived;
            twitch.smilesLoaded += twitch_smilesLoaded;
            
            while (!bTwitchEnd) {

                Thread.Sleep(100); 
            }
            twitch.Stop();
        }

        private void twitch_smilesLoaded(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                foreach (TextBlock block in listChat.Items)
                {
                    foreach (Inline inl in block.Inlines)
                    {
                        if (inl.Name == "Message") {

                                TextRange trMessage = new TextRange(inl.ContentStart, inl.ContentEnd);
                                replaceAllSmiles(0, trMessage.Text, trMessage);
                        }
                    }
                }
                foreach (Paragraph p in historydoc.Blocks)
                {
                    foreach (Inline inl in p.Inlines)
                    {
                        if (inl.Name == "Message")
                        {
                            TextRange trMessage = new TextRange(inl.ContentStart, inl.ContentEnd);
                            replaceAllSmiles(0, trMessage.Text, trMessage);
                        }
                    }
                }
            }));
        }

        private void threadViewersShow(object obj)
        {
            while (!bViewersEnd)
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (twitch != null)
                    {
                        viewers.twitchName.Content = twitch.getChannelName;
                        viewers.lblViewersTwitch.Content = twitch.getViewersCount();
                    }

                    if (sc2tv != null)
                    {
                        //TODO Cannot parse from webpage, because no viewer counter available
                    }
                    if (cyberg != null)
                        //viewers.lblViewersCyber.Content = cyberg.cb.Viewers;
                    if (goodgame != null)
                        viewers.lblViewersGG.Content = goodgame.getViewersCount();

                }));
                Thread.Sleep(5000);
            }
        }

        public void threadSaveHistory(object obj)
        {
            //while(true){
            //    Thread.Sleep(60000);
            //    saveHistory();
            //}
        }
        #endregion

        #region Methods
        private void removeChannel()
        {
            if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.TWITCH))
            {
                notificateToChat(String.Format("-{0} Channel: \"{1}\" disconnected;", Constants.TWITCH, channels.Twitch), Brushes.Red);
                channels.Twitch = "";
                bTwitchEnd = true;
            }
            else if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.SC2TV))
            {
                notificateToChat(String.Format("-{0} Channel: \"{1}\" disconnected;", Constants.SC2TV, channels.SC2TV), Brushes.Red);
                channels.SC2TV = "";
                channels.SC2TVLastMessageId = 0;
                bSC2TVEnd = true;
            }
            else if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.CYBERGAME))
            {
                notificateToChat(String.Format("-{0} Channel: \"{1}\" disconnected;", Constants.CYBERGAME, channels.Cybergame), Brushes.Red); 
                channels.Cybergame = "";
                bCybergameEnd = true;
            }
            else if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.GOODGAME))
            {
                notificateToChat(String.Format("-{0} Channel: \"{1}\" disconnected;", Constants.GOODGAME, channels.GoodGame), Brushes.Red); 
                channels.GoodGame = "";
                bGoodgameEnd = true;
            }
            listChats.RemoveAt(frmAddChat.listStreamers.SelectedIndex);
            frmAddChat.listStreamers.Items.Remove(frmAddChat.listStreamers.SelectedItem);
            frmAddChat.cmbAddChat.Items.Clear();
            initCmbAddChat();
            frmAddChat.cmbAddChat.SelectedIndex = 0;
            if (frmAddChat.listStreamers.Items.Count <= 0)
            {
                frmAddChat.bttnRemove.IsEnabled = false;
            }
            serializeChannels();
        }

        private void addChannel()
        {
            if (frmAddChat.cmbAddChat.SelectedItem.ToString() != "")
            {
                switch (frmAddChat.cmbAddChat.SelectedItem.ToString())
                {
                    case Constants.TWITCH:

                        if (Twitch.channelExists(frmAddChat.txtChannel.Text))
                        {
                            channels.Twitch = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.TWITCH + ": " + channels.Twitch);
                            frmAddChat.cmbAddChat.Items.Clear();
                            initCmbAddChat();
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addTwitch();
                        }
                        else
                            MessageBox.Show("Twitch channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case Constants.SC2TV:
                        try
                        {
                            HtmlWeb webcl = new HtmlWeb();
                            webcl.Load("http://sc2tv.ru/channel/" + frmAddChat.txtChannel.Text);
                            channels.SC2TV = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.SC2TV + ": " + channels.SC2TV);
                            frmAddChat.cmbAddChat.Items.Clear();
                            initCmbAddChat();
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addSC2TV();
                        }
                        catch (WebException)
                        {
                            MessageBox.Show("SC2TV channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        break;
                    case Constants.CYBERGAME:
                        if(Cybergame.channelExists(frmAddChat.txtChannel.Text))
                        {
                            channels.Cybergame = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.CYBERGAME + ": " + channels.Cybergame);
                            frmAddChat.cmbAddChat.Items.Clear();
                            initCmbAddChat();
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addCybergame();
                        }
                        else
                        {
                            MessageBox.Show("Cybergame channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case Constants.GOODGAME:
                        if (Goodgame.channelExists(frmAddChat.txtChannel.Text))
                        {
                            channels.GoodGame = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.GOODGAME + ": " + channels.GoodGame);
                            frmAddChat.cmbAddChat.Items.Clear();
                            initCmbAddChat();
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addGoodgame();
                        }
                        else
                            MessageBox.Show("Goodgame channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case Constants.HITBOX:
                    case Constants.EMPIRE:
                    default:
                        break;
                }
                serializeChannels();
            }
        }

        private void applySettingsToBlocks()
        {
            if (frmSettings.IsLoaded)
            {
                foreach (TextBlock block in listChat.Items)
                {
                    foreach (Inline inline in block.Inlines) 
                    {
                        if (inline.Name == "Nickname")
                            inline.FontWeight = nicknameWeight;
                        if (inline.Name == "Message")
                            inline.FontWeight = textWeight;
                        inline.FontFamily = new FontFamily(settingsChat.TextFont);
                        inline.FontSize = settingsChat.TextFontSize;
                    }
                }
                foreach (Paragraph p in historydoc.Blocks)
                {
                    foreach (Inline inline in p.Inlines)
                    {
                        if (inline.Name == "Nickname")
                            inline.FontWeight = nicknameWeight;
                        if (inline.Name == "Message")
                            inline.FontWeight = textWeight;
                        inline.FontFamily = new FontFamily(settingsChat.TextFont);
                        inline.FontSize = settingsChat.TextFontSize;
                    }
                }
                //TextRange tr = new TextRange(listChat.Conte.ContentStart, usrRTB.richChat.Document.ContentEnd);
                //tr.ApplyPropertyValue(FontFamilyProperty, font);
                //tr.ApplyPropertyValue(FontSizeProperty, settingsChat.TextFontSize);
            }

        }

        private void setEvents()
        {
            frmSettings.sliderOpacity.ValueChanged += sliderOpacity_ValueChanged;
            frmSettings.ClrPcker_Background.SelectedColorChanged += ClrPcker_Background_SelectedColorChanged;
            frmSettings.chkOnTop.Checked += chkOnTop_Checked;
            frmSettings.chkOnTop.Unchecked += chkOnTop_Unchecked;
            frmSettings.ClrPcker_Text.SelectedColorChanged += ClrPcker_Text_SelectedColorChanged;
            frmSettings.ClrPcker_Nickname.SelectedColorChanged += ClrPcker_Nickname_SelectedColorChanged;
            frmSettings.numSizeText.ValueChanged += numSizeText_ValueChanged;
            frmSettings.cmbFonts.SelectionChanged += cmbFonts_SelectionChanged;
            frmSettings.bttnSmileBigger.Click += bttnSmileBigger_Click;
            frmSettings.bttnSmileSmaller.Click += bttnSmileSmaller_Click;
            frmSettings.chkDateEnabled.Click += chkDateEnabled_Click;
            frmSettings.chkTextBold.Click += chkTextBold_Click;
            frmSettings.chkNicknameBold.Click += chkNicknameBold_Click;
            frmSettings.ClrPcker_Date.SelectedColorChanged += ClrPcker_Date_SelectedColorChanged;
            frmSettings.cbTransparency.Checked += cbTransparency_Checked;
            frmSettings.cbTransparency.Unchecked += cbTransparency_Checked;
        }

        private void cbTransparency_Checked(object sender, RoutedEventArgs e)
        {
            if (!frmSettings.IsLoaded)
                return;
            settingsChat.TransparencyEnabled = frmSettings.cbTransparency.IsChecked.Value;
            serializeSettings();
            stopAll();
            reload = true;
            frmSettings.Close();
            if(OnTransparencyChanged != null)
                OnTransparencyChanged(sender, this);
        }

        private void setTransparency()
        {
            this.AllowsTransparency = settingsChat.TransparencyEnabled;
        }



        private void settingsInit()
        {
            frmSettings.Topmost = settingsChat.TopMost;
            frmSettings.ClrPcker_Background.SelectedColor = mainbackBrush.Color;
            frmSettings.chkOnTop.IsChecked = frmChat.Topmost;
            frmSettings.ClrPcker_Text.SelectedColor = textBrush.Color;
            frmSettings.ClrPcker_Nickname.SelectedColor = fromHexColor(settingsChat.NicknameColor);
            frmSettings.numSizeText.Value = Convert.ToInt32(settingsChat.TextFontSize);
            frmSettings.cmbFonts.SelectedValue = font.Source;
            frmSettings.sliderOpacity.Value = mainbackBrush.Opacity * 100;
            frmSettings.lblOpacity.Content = Convert.ToInt32(mainbackBrush.Opacity * 100) + "%";
            frmSettings.lblSmileSize.Content = settingsChat.SmileSize;
            frmSettings.chkNicknameBold.IsChecked = settingsChat.NicknameBold;
            frmSettings.chkTextBold.IsChecked = settingsChat.TextBold;
            frmSettings.chkDateEnabled.IsChecked = settingsChat.DateEnabled;
            frmSettings.ClrPcker_Date.SelectedColor = dateBrush.Color;
            frmSettings.cbTransparency.IsChecked = settingsChat.TransparencyEnabled;
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                frmSettings.cmbFonts.Items.Add(fontFamily.Source);
            }
            frmSettings.cmbFonts.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("",
            System.ComponentModel.ListSortDirection.Ascending));
            setEvents();
        }

        private void preSetSettings()
        {
            this.nicknameColor = fromHexColor(settingsChat.NicknameColor);
            nicknameBrush.Color = nicknameColor;
            textBrush = new SolidColorBrush(fromHexColor(settingsChat.ForeColor));
            listChat.Foreground = textBrush;
            mainbackBrush.Color = fromHexColor(settingsChat.BackgroundColor);
            mainbackBrush.Opacity = settingsChat.ChatOpacity;
            //header.Background = mainbackBrush;
            Topmost = settingsChat.TopMost;
            font = new FontFamily(settingsChat.TextFont);
            textWeight = settingsChat.TextBold ? FontWeights.Bold : FontWeights.Normal;
            nicknameWeight = settingsChat.NicknameBold ? FontWeights.Bold : FontWeights.Normal;
            dateBrush = new SolidColorBrush(fromHexColor(settingsChat.DateColor));
            this.Width = settingsChat.Width;
            this.Height = settingsChat.Height;
            this.Top = settingsChat.Top;
            this.Left = settingsChat.Left;
            setTransparency();
        }

        void Handle(CheckBox checkBox)
        {
            // Use IsChecked.
            bool flag = checkBox.IsChecked.Value;

            frmChat.Topmost = flag;
            settingsChat.TopMost = flag;
            serializeSettings();
        }

        private void addGoodgame()
        {
            listChats.Add(Constants.GOODGAME + ": " + channels.GoodGame);
            Thread threadGoodg = new Thread(threadGoodgame);
            threadGoodg.Name = "Goodgame Thread";
            threadGoodg.IsBackground = true;
            bGoodgameEnd = false;
            notificateToChat(String.Format("+{0} Channel: \"{1}\" connected;", Constants.GOODGAME, channels.GoodGame), Brushes.Green);
            threadGoodg.Start();
        }

        private void addCybergame()
        {
            listChats.Add(Constants.CYBERGAME + ": " + channels.Cybergame);
            Thread threadCyber = new Thread(threadCybergame);
            threadCyber.Name = "Cybergame Thread";
            threadCyber.IsBackground = true;
            bCybergameEnd = false;
            notificateToChat(String.Format("+{0} Channel: \"{1}\" connected;", Constants.CYBERGAME, channels.Cybergame), Brushes.Green);
            threadCyber.Start();
        }

        private void addSC2TV()
        {
            listChats.Add(Constants.SC2TV + ": " + channels.SC2TV);
            Thread thrSC2TV = new Thread(threadSC2TV);
            thrSC2TV.IsBackground = true;
            thrSC2TV.Name = "SC2TV Thread";
            bSC2TVEnd = false;
            notificateToChat(String.Format("+{0} Channel: \"{1}\" connected;", Constants.SC2TV, channels.SC2TV), Brushes.Green);
            thrSC2TV.Start();
        }

        private void addTwitch()
        {
            listChats.Add(Constants.TWITCH + ": " + channels.Twitch);
            Thread twitchThread = new Thread(threadTwitch);
            twitchThread.IsBackground = true;
            twitchThread.Name = "Twitch Thread";
            bTwitchEnd = false;
            notificateToChat(String.Format("+{0} Channel: \"{1}\" connected;", Constants.TWITCH, channels.Twitch), Brushes.Green);
            twitchThread.Start();
        }

        private void notificateToChat(string msg, Brush brush) {
            TextBlock b = new TextBlock();
            b.Foreground = brush;
            b.FontSize = settingsChat.TextFontSize;
            b.FontFamily = new FontFamily(settingsChat.TextFont);
            b.Inlines.Add(msg);
            listChat.Items.Add(b);
        }

        /**
         * @param name="chattype" 0-Twitch, 1-Sc2TV, 2-Cybergame, 3-Goodgame, 4-Hitbox, 5-Empire 
         * 
         * */
        private void addMessageToChat(int chattype, string nickname, string msg, string to)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                string path = "pack://application:,,,/BlitzChat;component/images/";
                TextBlock block = new TextBlock();
                Paragraph parHist = new Paragraph();
                block.TextWrapping = TextWrapping.Wrap;
                if (settingsChat.DateEnabled)
                {
                    block.Inlines.Add(new Run(DateTime.Now.ToShortTimeString() + " ") { FontWeight = FontWeights.Normal, Foreground = dateBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                    parHist.Inlines.Add(new Run(DateTime.Now.ToShortTimeString() + " ") { FontWeight = FontWeights.Normal, Foreground = dateBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                }
                else{
                    block.Inlines.Add("");
                    parHist.Inlines.Add("");
                }
                string imagesource = "";
                double imagewidth = 16;
                double imageheight = 16;
                bool quoteColor = false;

                switch (chattype)
                {
                    case 0:
                        if (msg.ToLower().Contains(channels.Twitch.ToLower()) && !String.IsNullOrEmpty(channels.Twitch))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "twitch.png";
                        nickname = UppercaseFirst(nickname);
                        break;
                    case 1:
                        if (to != "" && to.ToLower().Contains(channels.SC2TV.ToLower()) && !String.IsNullOrEmpty(channels.SC2TV))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "sc2tv.png";
                        break;
                    case 2:
                        if (msg.ToLower().Contains(channels.Cybergame.ToLower()) && !String.IsNullOrEmpty(channels.Cybergame))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "cybergame.png";
                        break;
                    case 3:
                        if (!String.IsNullOrEmpty(to) && to.ToLower().Contains(channels.GoodGame.ToLower()) && !String.IsNullOrEmpty(channels.GoodGame))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "goodgame.png";
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
                Run r = new Run();
                if (!String.IsNullOrEmpty(imagesource))
                {
                    AddImage(block.Inlines, imagesource, imagewidth, imageheight);
                    AddImage(parHist.Inlines, imagesource, imagewidth, imageheight);
                    block.Inlines.Add(" ");
                    parHist.Inlines.Add(" ");
                }
                block.Inlines.Add(new Run(nickname + ": ") { FontWeight = nicknameWeight, Foreground = nicknameBrush, FontSize = settingsChat.TextFontSize, FontFamily = font, Name = "Nickname" });
                parHist.Inlines.Add(new Run(nickname + ": ") { FontWeight = nicknameWeight, Foreground = nicknameBrush, FontSize = settingsChat.TextFontSize, FontFamily = font, Name = "Nickname" });
                if (!String.IsNullOrEmpty(to) && quoteColor)
                {  
                    block.Inlines.Add(new Run(to + ", ") { FontWeight = FontWeights.Bold, Foreground = quoteBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                    parHist.Inlines.Add(new Run(to + ", ") { FontWeight = FontWeights.Bold, Foreground = quoteBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                }
                else if (!quoteColor && !String.IsNullOrEmpty(to))
                {
                    block.Inlines.Add(new Run(to + ", ") { FontWeight = FontWeights.Bold, Foreground = textBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                    parHist.Inlines.Add(new Run(to + ", ") { FontWeight = FontWeights.Bold, Foreground = textBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                }
                if (quoteColor)
                {
                    block.Inlines.Add(new Run(msg) { FontWeight = this.textWeight, Foreground = quoteBrush, FontSize = settingsChat.TextFontSize, FontFamily = font, Name="Message" });
                    parHist.Inlines.Add(new Run(msg) { FontWeight = this.textWeight, Foreground = quoteBrush, FontSize = settingsChat.TextFontSize, FontFamily = font, Name = "Message" });
                }
                else
                {
                    block.Inlines.Add(new Run(msg) { FontWeight = this.textWeight, Foreground = textBrush, FontSize = settingsChat.TextFontSize, FontFamily = font, Name = "Message" });
                    parHist.Inlines.Add(new Run(msg) { FontWeight = this.textWeight, Foreground = textBrush, FontSize = settingsChat.TextFontSize, FontFamily = font, Name = "Message" });
                }
                //smiles.checkEmotions(ref paragraph, settingsChat.SmileSize);
                listChat.Items.Add(block);
                historydoc.Blocks.Add(parHist);
                string uri = "";
                TextRange trMessage = FindWordFromPosition(block.ContentStart, msg);
                TextRange trMessageHist = FindWordFromPosition(parHist.ContentStart, msg);
                double lineheight = settingsChat.TextFontSize;
                lineheight = replaceAllSmiles(chattype, msg, trMessage);
                replaceAllSmiles(chattype, msg, trMessageHist);
                while (!String.IsNullOrEmpty(uri = UrlTools.DetectURLs(trMessage)))
                {
                    TextRange trUri = FindWordFromPosition(trMessage.Start, uri);
                    TextRange trUriHist = FindWordFromPosition(trMessageHist.Start, uri);
                    replaceLink(trUri, uri);
                    replaceLink(trUriHist, uri);
                }
                DoubleAnimation da = new DoubleAnimation();
                da.From = 0;
                da.To = 1;
                da.RepeatBehavior = (RepeatBehavior)new RepeatBehaviorConverter().ConvertFromString("0:0:0.7");
                da.Duration = TimeSpan.FromSeconds(0.7);
                block.BeginAnimation(OpacityProperty, da);
            }));
        }

        private double replaceAllSmiles(int type, string msg, TextRange range)
        {
            double maxHeight = 0; 
            Dispatcher.BeginInvoke(new Action(delegate
            {
                TextRange tr = FindWordFromPosition(range.Start, msg);
                if (tr == null)
                    return;
                switch (type)
                {
                    case 0:
                        TwitchSmile twitchsmile;
                        while ((twitchsmile = twitch.checkSmiles(tr.Text)) != null)
                        {
                            double imgHeight = replaceTwitchSmile(twitchsmile, tr);
                            if (imgHeight > maxHeight)
                            {
                                maxHeight = imgHeight;
                            }
                        }
                        break;
                    case 1:
                        foreach (KeyValuePair<string, SC2TVSmile> smile in sc2tv.checkSmiles(msg))
                        {
                            while (tr.Text.Contains(smile.Key))
                            {
                                double imgHeight = replaceSC2TVSmile(smile.Value, tr);
                                if (imgHeight > maxHeight)
                                {
                                    maxHeight = imgHeight;
                                }
                            }
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        foreach (KeyValuePair<string, GoodGameSmile> smile in goodgame.checkSmiles(msg))
                        {
                            while(tr.Text.Contains(smile.Key)){
                                double imgHeight = replaceGoodgameSmile(smile.Value, tr);
                                if (imgHeight > maxHeight)
                                {
                                    maxHeight = imgHeight;
                                }
                            }
                        }
                        break;
                    case 4:
                    case 5:
                    default:
                        break;
                }
            }));
            return maxHeight;
        }

        private double replaceGoodgameSmile(GoodGameSmile smile, TextRange range)
        {
            double h = 0;
            TextRange tr = FindWordFromPosition(range.Start, smile.code);
            if (tr != null)
            {
                tr.Text = "";
                Image img = new Image();
                BitmapImage bitmapImage = new BitmapImage(new Uri(@"pack://application:,,,/BlitzChat;component/Smiles/goodgame/"+ smile.path));
                
                // Create a CroppedBitmap based off of a xaml defined resource.
                //CroppedBitmap cb = new CroppedBitmap(bitmapImage,
                //   new Int32Rect(0, smile.y, smile.width, smile.heght));       //select region rect

                img.Source = bitmapImage;
                //img.Stretch = Stretch.Fill;
                img.Width = bitmapImage.Width/2 + settingsChat.SmileSize;
                img.Height = bitmapImage.Height/2 + settingsChat.SmileSize;
                h = img.Height;
                new InlineUIContainer(img, tr.Start).Name = "Smile";
            }
            return h;
        }

        private double replaceSC2TVSmile(SC2TVSmile smile, TextRange range)
        {
            double h = 0;
            TextRange tr = FindWordFromPosition(range.Start, smile.code);
            if (tr != null)
            {
                tr.Text = "";
                Image img = new Image();
                BitmapImage bitmapImage = new BitmapImage(smile.uri);
                img.Source = bitmapImage;
                //img.Stretch = Stretch.Fill;
                img.Width = smile.width+ settingsChat.SmileSize;
                img.Height = smile.height + settingsChat.SmileSize;
                h = img.Height;
                new InlineUIContainer(img, tr.Start).Name = "Smile";
            }
            return h;
        }

        private double replaceTwitchSmile(TwitchSmile smile, TextRange range)
        {
            double h = 0;
            TextRange tr = FindWordFromPosition(range.Start, smile.key);
            if (tr != null)
            {
                tr.Text = "";
                Image img = new Image();
                BitmapImage bitmapImage = new BitmapImage(smile.uri);
                img.Source = bitmapImage;
                //img.Stretch = Stretch.Fill;
                img.Width = smile.width + settingsChat.SmileSize;
                img.Height = smile.height + settingsChat.SmileSize;
                h = img.Height;
                new InlineUIContainer(img, tr.Start).Name = "Smile";
            }
            return h = 0;
        }

        private void saveHistory()
        {
            //if (Dispatcher.CheckAccess())
            //{
            //    if (!File.Exists(Constants.HISTORYDIR + WindowName + "_" + DateTime.Now.ToString("dd.MM.yyyy") + ".hist"))
            //        historydoc.Blocks.Clear();
            //    AddDocument(usrRTB.richChat.Document, historydoc);
            //}
            //else
            //    Dispatcher.BeginInvoke(new Action(delegate
            //{
            //    if (!File.Exists(Constants.HISTORYDIR + WindowName + "_" + DateTime.Now.ToString("dd.MM.yyyy") + ".hist"))
            //        historydoc.Blocks.Clear();
            //    AddDocument(usrRTB.richChat.Document, historydoc);
            //}));
            
            if (!Directory.Exists(Constants.HISTORYDIR)) {
                Directory.CreateDirectory(Constants.HISTORYDIR);
            }
            if (Dispatcher.CheckAccess())
            {
                using (FileStream fs = new FileStream(Constants.HISTORYDIR + WindowName + "_" + DateTime.Now.ToString("dd.MM.yyyy") + ".hist", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    TextRange tr = new TextRange(historydoc.ContentStart, historydoc.ContentEnd);
                    if (tr.CanSave(DataFormats.XamlPackage))
                    {
                        tr.Save(fs, DataFormats.XamlPackage, true);
                    }
                }
            }else
            Dispatcher.BeginInvoke(new Action(delegate
            {
                using (FileStream fs = new FileStream(Constants.HISTORYDIR + WindowName + "_" + DateTime.Now.ToString("dd.MM.yyyy") + ".hist", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    TextRange tr = new TextRange(historydoc.ContentStart, historydoc.ContentEnd);
                    if (tr.CanSave(DataFormats.XamlPackage))
                    {
                        tr.Save(fs, DataFormats.XamlPackage, true);
                    }
                }
            }));
        }

        public static void AddBlock(TextRange range, FlowDocument to)
        {
            if (range != null)
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream())
                    {

                        System.Windows.Markup.XamlWriter.Save(range, stream);

                        range.Save(stream, DataFormats.XamlPackage);

                        TextRange textRange2 = new TextRange(to.ContentEnd, to.ContentEnd);

                        textRange2.Load(stream, DataFormats.XamlPackage);
                        to.Blocks.Add(new Paragraph());
                        to.TextAlignment = TextAlignment.Left;
                    }
                }catch(Exception){
                
                }
            }
        }

        public void stopAll() {
            bTwitchEnd = true;
            bSC2TVEnd = true;
            bCybergameEnd = true;
            bGoodgameEnd = true;
            bSaveScreenEnd = true;
        }

        public static void AddDocument(FlowDocument from, FlowDocument to)
        {
            TextRange range = new TextRange(from.ContentStart, from.ContentEnd);

            MemoryStream stream = new MemoryStream();

            System.Windows.Markup.XamlWriter.Save(range, stream);

            range.Save(stream, DataFormats.XamlPackage);

            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);

            range2.Load(stream, DataFormats.XamlPackage);
        }

        private void loadHistory() {
            string file = Constants.HISTORYDIR + WindowName + "_" + DateTime.Now.ToString("dd.MM.yyyy") + ".hist";
            TextRange tr = new TextRange(historydoc.ContentStart, historydoc.ContentEnd);
            if (File.Exists(file))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (tr.CanLoad(DataFormats.XamlPackage))
                    {
                        try
                        {
                            tr.Load(fs, DataFormats.XamlPackage);
                        }
                        catch (Exception e) {
                            Debug.WriteLine(e.Message);
                            fs.Close();
                            File.Move(file, file + "corrupted/");
                        }
                     }
                }
            }
            else
                tr.Text = "";
        }
        private void replaceLink(TextRange tr, string uri)
        {
            tr.Text = "";
            string link = uri;
            
            Hyperlink hl = new Hyperlink(tr.Start, tr.End);
            hl.Inlines.Add("Link");
            hl.ToolTip = uri;
            Regex regex = new Regex("href=\"([^\"]*)\"");
            if (regex.IsMatch(uri))
            {
                MatchCollection m = regex.Matches(uri);
                link = m[0].Groups[1].Value;
            }
            try
            {
                if(!link.Contains("http")){
                    link = "http://"+link;
                }
                hl.NavigateUri = new Uri(link);
                hl.FontSize = settingsChat.TextFontSize;
                hl.FontFamily = font;
                hl.Foreground = new SolidColorBrush(fromHexColor("#FFB2DFEE"));
                hl.RequestNavigate += UrlTools.Hyperlink_Click;
            }
            catch {
                Debug.Print("Error found in link");
            }
        }

        TextRange FindWordFromPosition(TextPointer position, string word)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(word);
                    if (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        return new TextRange(start, end);
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "word" is not found.
            return null;
        }

        private void AddImage(InlineCollection inlines, string path, double w, double h)
        {
            BitmapImage bi = new BitmapImage(new Uri(@path));
            Image image = new Image();
            image.Source = bi;
            image.Width = w;
            image.Height = h;
            InlineUIContainer container = new InlineUIContainer(image);
            container.BaselineAlignment = BaselineAlignment.Center;
            inlines.Add(container);
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        #endregion

        #region Deserializing
        private void deserializeSettings()
        {
            if (File.Exists("Config/" + WindowName + "_" + "ChatSettings.xml"))
            {
                ChatSettingsXML newSettings = (ChatSettingsXML)XMLSerializer.deserialize(settingsChat, "Config/" + WindowName + "_" + "ChatSettings.xml");
                if (newSettings == null)
                    return;
                settingsChat = newSettings;
                //settingsChat.BackgroundColor = newSettings.BackgroundColor;
                //settingsChat.ChatOpacity = newSettings.ChatOpacity;
                //settingsChat.ForeColor = newSettings.ForeColor;
                //settingsChat.NicknameColor = newSettings.NicknameColor;
                //settingsChat.TextFont = newSettings.TextFont;
                //settingsChat.TextFontSize = newSettings.TextFontSize;
                //settingsChat.TopMost = newSettings.TopMost;
                //settingsChat.NicknameBold = newSettings.NicknameBold;
                //settingsChat.TextBold = newSettings.TextBold;
                //settingsChat.SmileSize = newSettings.SmileSize;
                //settingsChat.DateEnabled = newSettings.DateEnabled;
                //settingsChat.DateColor = newSettings.DateColor;
                //settingsChat.Width = newSettings.Width;
                //settingsChat.Height = newSettings.Height;
                //settingsChat.Top = newSettings.Top;
                //settingsChat.Left = newSettings.Left;
                preSetSettings();
            }
        }

        private void deserializeChannels()
        {
            if (File.Exists("Config/" + WindowName + "_" + "Channels.xml"))
            {
                ChannelsSaveXML desChannels = (ChannelsSaveXML)XMLSerializer.deserialize(channels, "Config/" + WindowName + "_" + "Channels.xml");
                if(desChannels == null)
                    return;
                channels = desChannels;
                //channels.Twitch = desChannels.Twitch;
                //channels.SC2TV = desChannels.SC2TV;
                //channels.Cybergame = desChannels.Cybergame;
                //channels.GoodGame = desChannels.GoodGame;
                //channels.SC2TVLastMessageId = desChannels.SC2TVLastMessageId;
                if (!String.IsNullOrEmpty(channels.Twitch))
                    addTwitch();
                if (!String.IsNullOrEmpty(channels.SC2TV))
                    addSC2TV();
                if (!String.IsNullOrEmpty(channels.Cybergame))
                    addCybergame();
                if (!String.IsNullOrEmpty(channels.GoodGame))
                    addGoodgame();

            }

        }
        #endregion

        #region Serializing
        private void serializeSettings(){
            XMLSerializer.serialize(settingsChat, "Config/" + WindowName + "_" + "ChatSettings.xml");
        }
        private void serializeChannels() {
            XMLSerializer.serialize(channels, "Config/" + WindowName + "_" + "Channels.xml");
        }
        #endregion

        #region Color methods
        private Color getColorFromInt(int c)
        {

            byte[] bytes = BitConverter.GetBytes(c);
            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        private int colorToInt(Color c)
        {

            return (c.A << 24) | (c.R << 16) | (c.G << 8) | c.B;
        }

        public static string ToHexColor(Color color)
        {
            return String.Format("#{0}{1}{2}{3}",
                                 color.A.ToString("X2"),
                                 color.R.ToString("X2"),
                                 color.G.ToString("X2"),
                                 color.B.ToString("X2"));
        }

        public Color fromHexColor(string hex)
        {
            if (hex != null)
                return (Color)ColorConverter.ConvertFromString(hex);
            else
                return Colors.Transparent;
        }
        #endregion

        #region String manipulation methods
        private string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        #endregion
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlitzChat;
using System.IO;
using System.Threading;
using System.Windows.Media.Animation;
using Blue.Windows;
using System.Reflection;
using dotCybergame;
using System.Net;
using HtmlAgilityPack;
using bliGoodgame;
using System.Text.RegularExpressions;
using System.Diagnostics;
using BlitzChat.Forms;
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
        Settings frmSettings;
        Color nicknameColor { get; set; }
        ChatControl frmAddChat;
        ChatSettingsXML settingsChat;
        ChannelsSaveXML channels;
        TwitchTV twitch;
        Sc2Tv sc2tv;
        CybergameTV cyberg;
        Goodgame goodgame;
        private volatile bool bTwitchEnd;
        private volatile bool bSC2TVEnd;
        private volatile bool bCybergameEnd;
        private volatile bool bGoodgameEnd;
        private volatile bool bViewersEnd = true;
        public string WindowName { get; set; }
        List<string> listChats;
        FontWeight nicknameWeight { get; set; }
        FontWeight textWeight { get; set; }
        FontFamily font { get; set; }
        private Smiles smiles { get; set; }
        public frmViewers viewers { get; set; }
        public event EventHandler<MainWindow> OnAdditionalWindows; 
        #endregion

        #region Constructors
        public MainWindow(string name)
        {
            header = new usrHeader();
            header.lblProgramName.Content = name;
            header.Width = double.NaN;
            header.Height = double.NaN;
            WindowName = name;
            this.smiles = new Smiles(this);
            listChats = new List<string>();
            InitializeComponent();
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
            richChat.Height = Row1.ActualHeight;
            richChat.Width = Column0.ActualWidth;
            richChat.IsReadOnly = true;
            richChat.SelectionOpacity = 0;
            mainbackBrush = new SolidColorBrush(Colors.Transparent);
            contextBrush = new SolidColorBrush(Colors.Black);
            textBrush = new SolidColorBrush(Colors.White);
            quoteBrush = new SolidColorBrush(Colors.Orange);
            dateBrush = new SolidColorBrush(Colors.White);
            dateBrush.Opacity = 0.7;
            nicknameColor = Colors.BlueViolet;
            nicknameBrush = new SolidColorBrush(nicknameColor);
            richChat.Foreground = textBrush;
            Background = mainbackBrush;
            mainContextMenu.Background = contextBrush;
            mainContextMenu.Foreground = new SolidColorBrush(Colors.White);
            deserializeSettings();
            frmSettings = new Settings();
            richChat.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            setEvents();
            deserializeChannels();
            richChat.ContextMenu = this.ContextMenu;
            Version vers = Assembly.GetExecutingAssembly().GetName().Version;
            header.lblProgramName.Content = "BlitzСhat v." + vers.ToString() + " Alpha";
            preSetSettings();
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }
        #endregion

        #region UI Events
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
            bool changed = false;
            foreach (Block block in richChat.Document.Blocks)
            {
                if (block is Paragraph)
                {
                    int index = 0;
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (index > 3 && inline is InlineUIContainer)
                        {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            if (uiContainer.Child is Image)
                            {
                                Image image = (Image)uiContainer.Child;
                                if (image.Width - 1 > 0 && image.Height - 1 > 0)
                                {
                                    image.Width--;
                                    image.Height--;
                                    changed = true;
                                }
                            }
                        }
                        index++;
                    }
                }
            }
            if (changed)
            {
                settingsChat.SmileSize--;
                frmSettings.lblSmileSize.Content = settingsChat.SmileSize;
            }
            serializeSettings();
        }
        private void bttnSmileBigger_Click(object sender, RoutedEventArgs e)
        {
            bool changed = false;
            foreach (Block block in richChat.Document.Blocks)
            {
                if (block is Paragraph)
                {
                    int index = 0;
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (index > 3 && inline is InlineUIContainer)
                        {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            if (uiContainer.Child is Image)
                            {
                                Image image = (Image)uiContainer.Child;
                                image.Width++;
                                image.Height++;
                                changed = true;
                            }
                        }
                        index++;
                    }
                }
            }
            if (changed)
            {
                settingsChat.SmileSize++;
                frmSettings.lblSmileSize.Content = settingsChat.SmileSize;
            }
            serializeSettings();
        }
        private void richChat_MouseEnter(object sender, MouseEventArgs e)
        {
            richChat.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            richChat.Document.Focus();
        }

        private void richChat_MouseLeave(object sender, MouseEventArgs e)
        {
            richChat.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
        private void clear_Chat_Click(object sender, RoutedEventArgs e)
        {
            richChat.Document.Blocks.Clear();

        }

        private void frmChat_Loaded(object sender, RoutedEventArgs e)
        {
            StickyWindow.RegisterExternalReferenceForm(this);
            header.lblWindowName.Content = WindowName;
        }


        private void show_Viewers_Click(object sender, RoutedEventArgs e)
        {
            if (bViewersEnd)
            {
                viewers = new frmViewers();
                viewers.Topmost = settingsChat.TopMost;
                viewers.borderViewers.Background = mainbackBrush;
                viewers.Top = this.Top + this.ActualHeight;
                viewers.Width = this.Width;
                viewers.Left = this.Left;
                viewers.Show();
                bViewersEnd = false;
                viewersMenuItem.Header = "Close viewers";
                Thread threadViewers = new Thread(threadViewersShow);
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
            if (e.ChangedButton == MouseButton.Left)
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
            settingsInit();
            frmSettings.Show();
        }

        private void cmbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: Maybe need loop change all blocks
            string newFont = frmSettings.cmbFonts.SelectedItem.ToString();
            font = new FontFamily(newFont);
            richChat.FontFamily = font;
            richChat.Document.FontFamily = font;
            settingsChat.TextFont = newFont;
            serializeSettings();
            applySettingsToBlocks();
        }

        private void numSizeText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            double newSize = Convert.ToDouble(frmSettings.numSizeText.Value.Value);
            //TODO: Maybe need loop change all blocks
            richChat.Document.Blocks.LastBlock.FontSize = newSize;
            settingsChat.TextFontSize = newSize;
            serializeSettings();
            applySettingsToBlocks();
        }

        private void ClrPcker_Nickname_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Color newColor = frmSettings.ClrPcker_Nickname.SelectedColor;
            nicknameColor = newColor;
            settingsChat.NicknameColor = ToHexColor(newColor);
            nicknameBrush.Color = nicknameColor;
            serializeSettings();
        }

        private void ClrPcker_Text_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Color newColor = frmSettings.ClrPcker_Text.SelectedColor;
            textBrush.Color = newColor;
            richChat.Foreground = textBrush;
            settingsChat.ForeColor = ToHexColor(newColor);
            serializeSettings();
            //applySettingsToBlocks();
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Color newColor = frmSettings.ClrPcker_Background.SelectedColor;
            mainbackBrush.Color = newColor;
            //Background = mainBrush;

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
            serializeSettings();
        }

        private void contextMenu_Close_Click(object sender, RoutedEventArgs e)
        {
            if (WindowName == "Main")
                System.Environment.Exit(0);
            else
                this.Close();
        }

        private void bttnClose_Click(object sender, RoutedEventArgs e)
        {
            if (WindowName == "Main")
                System.Environment.Exit(0);
            else
                this.Close();
        }

        private void frmChat_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            richChat.Width = Column0.ActualWidth;
            richChat.Height = Row1.ActualHeight;
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
            frmAddChat.bttnAddChat.Click += bttnAddChat_Click;
            frmAddChat.listStreamers.SelectionChanged += listStreamers_SelectionChanged;
            frmAddChat.bttnRemove.Click += bttnRemove_Click;
            foreach (object item in listChats)
                frmAddChat.listStreamers.Items.Add(item);
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
            if (channels != null && !String.IsNullOrEmpty(channels.Cybergame))
            {
                frmAddChat.cmbAddChat.Items.Remove(Constants.GOODGAME);

            }
            frmAddChat.cmbAddChat.SelectedIndex = 0;
            frmAddChat.Show();
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


        private void cybergame_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string nick = e.Message.alias;
            string msg = e.Message.message;
            addMessageToChat(2, nick, msg, "");
        }

        private void sc2tv_MessageReceived(object sender, dotSC2TV.Sc2Chat.Sc2MessageEvent e)
        {
            string to = e.message.to;
            if (to == null)
                to = "";
            string msg = e.message.message;
            if (msg.Contains(":s:"))
                msg = msg.Replace(":s:", ":");
            addMessageToChat(1, e.message.name, msg, to);
        }

        private void twitch_MessageReceived(object sender, dotIRC.IrcMessageEventArgs e)
        {
            string nick = e.Source.Name;
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
                cyberg = new CybergameTV(channels.Cybergame);
                cyberg.cb.Start();
                cyberg.connectCybergame();
            }
            catch (WrongChannelNameException)
            {
                MessageBox.Show("Wrong channelname was saved to the file. If it happens again. Please delete data from file Channels.xml or use Add chat form to delete it.");
            }
            cyberg.cb.OnChatMessage += new EventHandler<MessageReceivedEventArgs>(cybergame_MessageReceived);
            while (!bCybergameEnd) { Thread.Sleep(100); }
            cyberg.cb.Stop();
            cyberg.cb.Close();
            cyberg = null;
        }

        private void threadSC2TV(object obj)
        {
            try
            {
                //do
                //{
                sc2tv = new Sc2Tv(channels.SC2TV);
                sc2tv.chat.MessageReceived += sc2tv_MessageReceived;
                //    if (sc2tv.chat.chat.messages == null)
                //    {
                //        //MessageBox.Show("SC2TV chat is not loaded. There are some problems with server or you have not loaded chat on sc2tv. Try write something in channel.");
                //        Thread.Sleep(1000);
                //    }
                //} while (sc2tv.chat.chat.messages == null);
                while (!bSC2TVEnd)
                {
                    sc2tv.chat.DownloadChat(false);
                    Thread.Sleep(100);
                }
            }
            catch (WrongChannelNameException)
            {
                MessageBox.Show("Wrong channelname was saved to the file. If it happens again. Please delete data from file Channels.xml or use Add chat form to delete it.");
            }
            finally
            {
                sc2tv.chat.Stop();
                sc2tv = null;
            }
        }

        private void threadTwitch(object obj)
        {
            try
            {
                twitch = new TwitchTV(channels.Twitch);
            }
            catch (WrongChannelNameException)
            {
                MessageBox.Show("Wrong channelname was saved to the file. If it happens again. Please delete data from file Channels.xml or use Add chat form to delete it.");
            }
            twitch.irc.client.Channels[0].MessageReceived += twitch_MessageReceived;
            while (!bTwitchEnd) { Thread.Sleep(100); }
            twitch.irc.client.Disconnect();
        }

        private void threadViewersShow(object obj)
        {
            while (!bViewersEnd)
            {
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (twitch != null && twitch.irc.client.Users.Count >= 2)
                    {
                        viewers.lblViewersTwitch.Content = twitch.irc.client.Users.Count - 2;
                    }

                    if (sc2tv != null)
                    {
                        //TODO Cannot parse from webpage, because no viewer counter available
                    }
                    if (cyberg != null)
                        viewers.lblViewersCyber.Content = cyberg.cb.Viewers;
                    if (goodgame != null)
                        viewers.lblViewersGG.Content = goodgame.getViewersCount();

                }));
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Methods
        private void removeChannel()
        {
            if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.TWITCH))
            {
                frmAddChat.cmbAddChat.Items.Add(Constants.TWITCH);
                channels.Twitch = "";
                bTwitchEnd = true;
            }
            else if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.SC2TV))
            {
                frmAddChat.cmbAddChat.Items.Add(Constants.SC2TV);
                channels.SC2TV = "";
                bSC2TVEnd = true;
            }
            else if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.CYBERGAME))
            {
                frmAddChat.cmbAddChat.Items.Add(Constants.CYBERGAME);
                channels.Cybergame = "";
                bCybergameEnd = true;
            }
            else if (frmAddChat.listStreamers.SelectedItem.ToString().Contains(Constants.GOODGAME))
            {
                frmAddChat.cmbAddChat.Items.Add(Constants.GOODGAME);
                channels.GoodGame = "";
                bGoodgameEnd = true;
            }
            listChats.RemoveAt(frmAddChat.listStreamers.SelectedIndex);
            frmAddChat.listStreamers.Items.Remove(frmAddChat.listStreamers.SelectedItem);
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
                        try
                        {
                            new TwitchTV(frmAddChat.txtChannel.Text);
                            channels.Twitch = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.TWITCH + ": " + channels.Twitch);
                            frmAddChat.cmbAddChat.Items.Remove(Constants.TWITCH);
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addTwitch();
                        }
                        catch (WrongChannelNameException)
                        {
                            MessageBox.Show("Twitch channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case Constants.SC2TV:
                        try
                        {
                            HtmlWeb webcl = new HtmlWeb();
                            webcl.Load("http://sc2tv.ru/channel/" + frmAddChat.txtChannel.Text);
                            channels.SC2TV = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.SC2TV + ": " + channels.SC2TV);
                            frmAddChat.cmbAddChat.Items.Remove(Constants.SC2TV);
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addSC2TV();
                        }
                        catch (WebException)
                        {
                            MessageBox.Show("SC2TV channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        break;
                    case Constants.CYBERGAME:
                        try
                        {
                            //new CybergameTV(frmAddChat.txtChannel.Text);
                            using (var client = new NewWebClient())
                            {
                                client.HeadOnly = true;
                                // fine, no content downloaded
                                // throws 404
                                client.DownloadString("http://cybergame.tv/" + frmAddChat.txtChannel.Text);
                            }
                            channels.Cybergame = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.CYBERGAME + ": " + channels.Cybergame);
                            frmAddChat.cmbAddChat.Items.Remove(Constants.CYBERGAME);
                            frmAddChat.cmbAddChat.SelectedIndex = 0;
                            addCybergame();
                        }
                        catch (WebException)
                        {
                            MessageBox.Show("Cybergame channel " + frmAddChat.txtChannel.Text + " not exists! Please try again!", "Channel not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case Constants.GOODGAME:
                        if (Goodgame.channelExists(frmAddChat.txtChannel.Text))
                        {
                            channels.GoodGame = frmAddChat.txtChannel.Text;
                            frmAddChat.listStreamers.Items.Add(Constants.GOODGAME + ": " + channels.GoodGame);
                            frmAddChat.cmbAddChat.Items.Remove(Constants.GOODGAME);
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
                foreach (Paragraph p in richChat.Document.Blocks)
                {
                    if (p is Paragraph)
                    {
                        int index = 0;
                        foreach (Inline inl in p.Inlines)
                        {
                            if (index == 3)
                            {
                                inl.FontWeight = nicknameWeight;
                            }
                            else if (index > 3)
                            {
                                inl.FontWeight = textWeight;
                            }
                            index++;
                        }
                    }

                }
                TextRange tr = new TextRange(richChat.Document.ContentStart, richChat.Document.ContentEnd);
                tr.ApplyPropertyValue(FontFamilyProperty, font);
                tr.ApplyPropertyValue(FontSizeProperty, settingsChat.TextFontSize);
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
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                frmSettings.cmbFonts.Items.Add(fontFamily.Source);
            }
            frmSettings.cmbFonts.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("",
            System.ComponentModel.ListSortDirection.Ascending));

        }

        private void preSetSettings()
        {
            this.nicknameColor = fromHexColor(settingsChat.NicknameColor);
            nicknameBrush.Color = nicknameColor;
            textBrush = new SolidColorBrush(fromHexColor(settingsChat.ForeColor));
            this.richChat.Foreground = textBrush;
            mainbackBrush.Color = fromHexColor(settingsChat.BackgroundColor);
            Topmost = settingsChat.TopMost;
            mainbackBrush.Opacity = settingsChat.ChatOpacity;
            font = new FontFamily(settingsChat.TextFont);
            textWeight = settingsChat.TextBold ? FontWeights.Bold : FontWeights.Normal;
            nicknameWeight = settingsChat.NicknameBold ? FontWeights.Bold : FontWeights.Normal;
            dateBrush = new SolidColorBrush(fromHexColor(settingsChat.DateColor));
            this.Width = settingsChat.Width;
            this.Height = settingsChat.Height;
            this.Top = settingsChat.Top;
            this.Left = settingsChat.Left;
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
            threadGoodg.Start();
        }

        private void addCybergame()
        {
            listChats.Add(Constants.CYBERGAME + ": " + channels.Cybergame);
            Thread threadCyber = new Thread(threadCybergame);
            threadCyber.Name = "Cybergame Thread";
            threadCyber.IsBackground = true;
            bCybergameEnd = false;
            threadCyber.Start();
        }

        private void addSC2TV()
        {
            listChats.Add(Constants.SC2TV + ": " + channels.SC2TV);
            Thread thrSC2TV = new Thread(threadSC2TV);
            thrSC2TV.IsBackground = true;
            thrSC2TV.Name = "SC2TV Thread";
            bSC2TVEnd = false;
            thrSC2TV.Start();
        }

        private void addTwitch()
        {
            listChats.Add(Constants.TWITCH + ": " + channels.Twitch);
            Thread twitchThread = new Thread(threadTwitch);
            twitchThread.IsBackground = true;
            twitchThread.Name = "Twitch Thread";
            bTwitchEnd = false;
            twitchThread.Start();
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
                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Left;
                if (settingsChat.DateEnabled)
                    paragraph.Inlines.Add(new Run(DateTime.Now.ToShortTimeString() + " ") { FontWeight = FontWeights.Normal, Foreground = dateBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                else
                    paragraph.Inlines.Add("");
                string imagesource = "";
                double imagewidth = 0;
                double imageheight = 0;
                bool quoteColor = false;

                switch (chattype)
                {
                    case 0:
                        if (msg.ToLower().Contains(channels.Twitch.ToLower()))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "twitch.png";
                        imageheight = settingsChat.TextFontSize;
                        imagewidth = settingsChat.TextFontSize;
                        nickname = UppercaseFirst(nickname);
                        break;
                    case 1:
                        if (to != "" && to.ToLower().Contains(channels.SC2TV.ToLower()))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "sc2tv.png";
                        imageheight = settingsChat.TextFontSize;
                        imagewidth = settingsChat.TextFontSize;
                        break;
                    case 2:
                        if (msg.ToLower().Contains(channels.Cybergame.ToLower()))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "cybergame.png";
                        imageheight = settingsChat.TextFontSize;
                        imagewidth = settingsChat.TextFontSize;
                        break;
                    case 3:
                        if (!String.IsNullOrEmpty(to) && to.ToLower().Contains(channels.GoodGame.ToLower()))
                        {
                            quoteColor = true;
                        }
                        imagesource = path + "goodgame.png";
                        imageheight = settingsChat.TextFontSize;
                        imagewidth = settingsChat.TextFontSize;
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
                if (!String.IsNullOrEmpty(imagesource))
                {
                    AddImage(paragraph, imagesource, imagewidth, imageheight);
                    paragraph.Inlines.Add(" ");
                }
                paragraph.Inlines.Add(new Run(nickname + ": ") { FontWeight = nicknameWeight, Foreground = nicknameBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                if (!String.IsNullOrEmpty(to) && quoteColor)
                {
                    paragraph.Inlines.Add(new Run(to + ", ") { FontWeight = FontWeights.Bold, Foreground = quoteBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                }
                else if (!quoteColor && !String.IsNullOrEmpty(to))
                {
                    paragraph.Inlines.Add(new Run(to + ", ") { FontWeight = FontWeights.Bold, Foreground = textBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });
                }
                if (quoteColor)
                    paragraph.Inlines.Add(new Run(msg) { FontWeight = this.textWeight, Foreground = quoteBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });

                else
                    paragraph.Inlines.Add(new Run(msg) { FontWeight = this.textWeight, Foreground = textBrush, FontSize = settingsChat.TextFontSize, FontFamily = font });


                smiles.checkEmotions(ref paragraph, settingsChat.SmileSize);
                richChat.Document.Blocks.Add(paragraph);
                string uri = "";
                while (!String.IsNullOrEmpty(uri = UrlTools.DetectURLs(ref paragraph)))
                {
                    replaceLink(paragraph, uri);
                }
                if (richChat.VerticalScrollBarVisibility != ScrollBarVisibility.Visible)
                    richChat.ScrollToEnd();
            }));
        }

        private void replaceLink(Paragraph paragraph, string uri)
        {
            TextRange tr = FindWordFromPosition(paragraph.ContentStart, uri);
            tr.Text = "";
            string link = uri;
            Run r = new Run("Link");
            Hyperlink hl = new Hyperlink(r, tr.Start);
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

        private void AddImage(Paragraph paragraph, string path, double w, double h)
        {
            BitmapImage bi = new BitmapImage(new Uri(@path));
            Image image = new Image();
            image.Source = bi;
            image.Width = w;
            image.Height = h;
            InlineUIContainer container = new InlineUIContainer(image);
            paragraph.Inlines.Add(container);
        }
        #endregion

        #region Deserializing
        private void deserializeSettings()
        {
            if (File.Exists("Config/" + WindowName + "_" + "ChatSettings.xml"))
            {
                ChatSettingsXML newSettings = (ChatSettingsXML)XMLSerializer.deserialize(settingsChat, "Config/" + WindowName + "_" + "ChatSettings.xml");
                settingsChat.BackgroundColor = newSettings.BackgroundColor;
                settingsChat.ChatOpacity = newSettings.ChatOpacity;
                settingsChat.ForeColor = newSettings.ForeColor;
                settingsChat.NicknameColor = newSettings.NicknameColor;
                settingsChat.TextFont = newSettings.TextFont;
                settingsChat.TextFontSize = newSettings.TextFontSize;
                settingsChat.TopMost = newSettings.TopMost;
                settingsChat.NicknameBold = newSettings.NicknameBold;
                settingsChat.TextBold = newSettings.TextBold;
                settingsChat.SmileSize = newSettings.SmileSize;
                settingsChat.DateEnabled = newSettings.DateEnabled;
                settingsChat.DateColor = newSettings.DateColor;
                settingsChat.Width = newSettings.Width;
                settingsChat.Height = newSettings.Height;
                settingsChat.Top = newSettings.Top;
                settingsChat.Left = newSettings.Left;
                preSetSettings();
            }
        }

        private void deserializeChannels()
        {
            if (File.Exists("Config/" + WindowName + "_" + "Channels.xml"))
            {
                ChannelsSaveXML desChannels = (ChannelsSaveXML)XMLSerializer.deserialize(channels, "Config/" + WindowName + "_" + "Channels.xml");
                channels.Twitch = desChannels.Twitch;
                channels.SC2TV = desChannels.SC2TV;
                channels.Cybergame = desChannels.Cybergame;
                channels.GoodGame = desChannels.GoodGame;
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

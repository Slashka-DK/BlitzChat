﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34014
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BlitzChat.Settings {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class URLs : global::System.Configuration.ApplicationSettingsBase {
        
        private static URLs defaultInstance = ((URLs)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new URLs())));
        
        public static URLs Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("irc.twitch.tv:80")]
        public string Twitch {
            get {
                return ((string)(this["Twitch"]));
            }
            set {
                this["Twitch"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.twitch.tv/kraken/chat/emoticons")]
        public string TwitchSmiles {
            get {
                return ((string)(this["TwitchSmiles"]));
            }
            set {
                this["TwitchSmiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://chat.sc2tv.ru/memfs/channel-{0}.json")]
        public string SC2TV {
            get {
                return ((string)(this["SC2TV"]));
            }
            set {
                this["SC2TV"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ws://cybergame.tv:9090/websocket")]
        public string Cybergame {
            get {
                return ((string)(this["Cybergame"]));
            }
            set {
                this["Cybergame"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ws://chat.goodgame.ru:8081/chat/websocket")]
        public string Goodgame {
            get {
                return ((string)(this["Goodgame"]));
            }
            set {
                this["Goodgame"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://chat.sc2tv.ru/js/smiles.js")]
        public string SC2TVSmiles {
            get {
                return ((string)(this["SC2TVSmiles"]));
            }
            set {
                this["SC2TVSmiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.twitch.tv/kraken/streams/")]
        public string TwitchViewers {
            get {
                return ((string)(this["TwitchViewers"]));
            }
            set {
                this["TwitchViewers"] = value;
            }
        }
    }
}

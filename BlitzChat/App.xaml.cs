using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        ErrorLog Logger;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger = new ErrorLog("Crashes");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 20 });
            WindowControl wc = new WindowControl();
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            Thread.Sleep(1000);
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.Exception;
                string LogFile = Logger.LogError(ex);

                MessageBox.Show(
                    "The application encountered a fatal error and must exit. This error has been logged and should be reported using the Error Report utility.\n\n" +
                        "Error:\n" +
                        ex.Message +
                        "\n\nStack Trace:\n" +
                        ex.StackTrace,
                    "Fatal Error");

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorReport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
            }
            finally
            {
                System.Environment.Exit(1);
            }
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.Exception;
                string LogFile = Logger.LogError(ex);

                MessageBox.Show(
                    "The application encountered a fatal error and must exit. This error has been logged and should be reported using the Error Report utility.\n\n" +
                        "Error:\n" +
                        ex.Message +
                        "\n\nStack Trace:\n" +
                        ex.StackTrace,
                    "Fatal Error");

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorReport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
            }
            finally
            {
                System.Environment.Exit(1);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                
                Exception ex = (Exception)e.ExceptionObject;
                string LogFile = Logger.LogError(ex);

                MessageBox.Show(
                    "The application encountered a fatal error and must exit. This error has been logged and should be reported using the Error Report utility.\n\n" +
                        "Error:\n" +
                        ex.Message +
                        "\n\nStack Trace:\n" +
                        ex.StackTrace,
                    "Fatal Error");

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorReport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
            }
            finally
            {
                System.Environment.Exit(1);
            }
        }
    }
}

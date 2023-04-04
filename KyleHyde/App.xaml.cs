using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public App() : base() {
            if (Debugger.IsAttached) {
                //Just setup handling clipboard issue
                Dispatcher.UnhandledException += (sender, args) => {
                    var comException = args.Exception as System.Runtime.InteropServices.COMException;
                    if (comException != null && comException.ErrorCode == -2147221040) {
                        //Clipboard fault
                        args.Handled = true;
                    }
                };
            } else {
                //Setup exception handling rather than closing rudely
                AppDomain.CurrentDomain.UnhandledException += (sender, args) => ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");
                TaskScheduler.UnobservedTaskException += (sender, args) => {
                    ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");
                    args.SetObserved();
                };

                Dispatcher.UnhandledException += (sender, args) => {
                    var comException = args.Exception as System.Runtime.InteropServices.COMException;
                    args.Handled = true;
                    if (comException != null && comException.ErrorCode == -2147221040) {
                        //Clipboard fault
                    } else
                        ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException");
                };
            }
        }

        public static void ShowUnhandledExceptionFromSrc(Exception e, string source) {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                new WindowException(e, source).Show();
            });
        }

        void ShowUnhandledException(Exception e, string unhandledExceptionType) {
            new WindowException(e, unhandledExceptionType).Show(); //Removed: , Debugger.IsAttached
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            new MainWindow2().Show();
        }
    }
}

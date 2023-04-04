using System;
using System.Windows;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for WindowException.xaml
    /// </summary>
    public partial class WindowException : Window {

        public Exception Exception { get; private set; }
        public string ExceptionSource { get; private set; }
        public string DetailsText { get; private set; }

        public WindowException() {
            InitializeComponent();
        }

        public WindowException(Exception ex, string source) { //, bool allowContinue
            Exception = ex;
            ExceptionSource = source;
            DetailsText = ExceptionSource + "\r\n" + ex.ToString();
            this.DataContext = this;

            InitializeComponent();

            this.Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " Exception";
            //btnContinue.Visibility = (allowContinue ? Visibility.Visible : Visibility.Collapsed);
        }

        private void OnExitAppClick(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private void OnContinueAppClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}

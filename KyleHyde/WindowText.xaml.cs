using System.Windows;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for WindowText.xaml
    /// </summary>
    public partial class WindowText : Window {
        public WindowText(string input="") {
            InitializeComponent();

            textBox1.Text = input;
        }
    }
}

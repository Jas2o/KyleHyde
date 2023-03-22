using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

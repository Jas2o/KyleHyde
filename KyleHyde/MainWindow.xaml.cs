using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        Formats.HotelDusk.Loader loaderHD = new Formats.HotelDusk.Loader();
        Formats.LastWindow.Loader loaderLW = new Formats.LastWindow.Loader();

        public MainWindow() {
            InitializeComponent();
        }

        private void HotelDuskDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // Note that you can have more than one file.

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    loaderHD.Open(file);
                }
            }
        }

        private void LastWindowDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // Note that you can have more than one file.

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    loaderLW.Open(file);
                }
            }
        }
    }
}

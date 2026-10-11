using System.Windows;

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

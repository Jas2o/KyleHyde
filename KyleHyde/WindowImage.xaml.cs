using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for WindowImage.xaml
    /// </summary>
    public partial class WindowImage : Window {

        private string name;
        private Bitmap bmp;

        public WindowImage() {
            InitializeComponent();
        }

        public WindowImage(Bitmap bmp, string name) {
            InitializeComponent();
            
            Title += " - " + name;
            this.name = name;
            this.bmp = bmp;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            //Convert Bitmap to BitmapImage to display in WPF
            using (MemoryStream stream = new MemoryStream()) {
                bmp.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                imageBox.Source = bitmap;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            Directory.CreateDirectory(@".\Export");
            string outfile = @".\Export\" + name + ".png";
            bmp.Save(outfile);
            Process.Start("explorer.exe", "/select, \"" + outfile + "\"");
        }
    }
}

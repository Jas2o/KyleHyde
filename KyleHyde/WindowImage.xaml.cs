using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
    /// Interaction logic for WindowImage.xaml
    /// </summary>
    public partial class WindowImage : Window {

        private Bitmap bmp;

        public WindowImage() {
            InitializeComponent();
        }

        public WindowImage(Bitmap bmp) {
            InitializeComponent();
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
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for WindowImageAnimated.xaml
    /// </summary>
    public partial class WindowImageAnimated : Window {

        private string name;
        private Bitmap[] bmps;
        private BitmapImage[] bitmapImages;
        private int indexLast;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public WindowImageAnimated() {
            InitializeComponent();
        }

        public WindowImageAnimated(Bitmap[] bmps, string name) {
            InitializeComponent();

            Title += " - " + name;
            this.name = name;
            this.bmps = bmps;
            bitmapImages = new BitmapImage[bmps.Length];
            indexLast = 0;

            for(int i = 0; i < bmps.Length; i++) {
                using (MemoryStream stream = new MemoryStream()) {
                    bmps[i].Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    bitmapImages[i] = new BitmapImage();
                    bitmapImages[i].BeginInit();
                    bitmapImages[i].StreamSource = stream;
                    bitmapImages[i].CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImages[i].EndInit();
                    bitmapImages[i].Freeze();
                }
            }

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e) {
            if (indexLast >= bitmapImages.Length)
                indexLast = 0;
            imageBox.Source = bitmapImages[indexLast++];
        }

        private void btnSaveFrames_Click(object sender, RoutedEventArgs e) {
            Directory.CreateDirectory(@".\Export");
            for (int i = 0; i < bmps.Length; i++) {
                string outfile = @".\Export\" + name + "_" + i + ".png";
                bmps[i].Save(outfile);
                if(i == 0)
                    Process.Start("explorer.exe", "/select, \"" + outfile + "\"");
            }
        }
    }
}

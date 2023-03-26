using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for WindowImageAnimated.xaml
    /// </summary>
    public partial class WindowImageAnimated : Window {

        private Bitmap[] bmps;
        private BitmapImage[] bitmapImages;
        private int indexLast;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public WindowImageAnimated() {
            InitializeComponent();
        }

        public WindowImageAnimated(Bitmap[] bmps) {
            InitializeComponent();

            this.bmps = bmps;
            bitmapImages = new BitmapImage[bmps.Length];
            indexLast = 0;

            // Create the output folder if it does not exist
            string outputFolderPath = "output";
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            for (int i = 0; i < bmps.Length; i++)
            {
                string folderName = "output";
                string fileName = System.IO.Path.Combine(folderName, $"frame_{i}.png");
                Directory.CreateDirectory(folderName);

                using (FileStream stream = File.Create(fileName)) {
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
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e) {
            if (indexLast >= bitmapImages.Length)
                indexLast = 0;
            imageBox.Source = bitmapImages[indexLast++];
        }
    }
}

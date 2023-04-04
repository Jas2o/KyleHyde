using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window {

        GameTools.Loader loader;

        private FileList dataview;
        private Bitmap[] bmps;
        private BitmapImage[] bitmapImages;
        private int indexLast;
        private DispatcherTimer dispatcherTimer;

        public MainWindow2() {
            dataview = new FileList();
            this.DataContext = dataview;
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
        }

        private void BtnLoadHotelDusk_Click(object sender, RoutedEventArgs e) {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault()) {
                dataview.Clear();
                loader = new Formats.HotelDusk.Loader();

                string folder = dialog.SelectedPath;

                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files) {
                    dataview.Add(file, folder);
                }
            }
        }

        private void BtnLoadLastWindow_Click(object sender, RoutedEventArgs e) {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault()) {
                dataview.Clear();
                loader = new Formats.LastWindow.Loader();

                string folder = dialog.SelectedPath;

                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files) {
                    dataview.Add(file, folder);
                }
            }
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            Filter();
        }

        private void chkHidePacked_Changed(object sender, RoutedEventArgs e) {
            Filter();
        }

        private void dataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            dispatcherTimer.Stop();
            FileListItem item = dataGrid.SelectedItem as FileListItem;
            if (item == null)
                return;

            object result = loader.Open2(item.File);
            if (result is GameTools.GTFS)
                hexEditor.Stream = (result as GameTools.GTFS).GetStream();
            else
                hexEditor.Stream = item.GTFS.GetStream();
            //--
            
            if (result == null) {
                //Debug.WriteLine(item.File);
                tabControl.SelectedItem = tabHex;
            } else if (result is Exception) {
                //MessageBox.Show((result as Exception).Message, "Exception");
                txtText.Text = (result as Exception).Message;
                //tabControl.SelectedItem = tabHex;
                tabControl.SelectedItem = tabText;
            } else if (result is GameTools.GTFS) {
                tabControl.SelectedItem = tabHex;
            } else if (result is KyleHyde.Formats.HotelDusk.FRM) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.HotelDusk.FRM).bitmap };
                DisplayBitmap();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.HotelDusk.ANM) {
                bmps = (result as KyleHyde.Formats.HotelDusk.ANM).BitmapsBlended();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.HotelDusk.WPFBIN) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.HotelDusk.WPFBIN).bitmap };
                DisplayBitmap();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.BPG) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.LastWindow.BPG).bitmap };
                DisplayBitmap();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.EBP) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.LastWindow.EBP).bitmap };
                DisplayBitmap();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.LWBRA) {
                bmps = (result as KyleHyde.Formats.LastWindow.LWBRA).GetBitmaps();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            }
        }

        private void Filter() {
            ListCollectionView collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            if ((bool)chkHidePacked.IsChecked) {
                collectionView.Filter = new Predicate<object>(x =>
                    ((FileListItem)x).Ext != loader.PackExtension &&
                    ((FileListItem)x).Relative.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                );
            } else {
                collectionView.Filter = new Predicate<object>(x =>
                    ((FileListItem)x).Relative.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                );
                //collectionView.Filter = null;
            }
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e) {
            if (indexLast >= bitmapImages.Length)
                indexLast = 0;
            imageBox.Source = bitmapImages[indexLast++];
        }

        private void DisplayBitmap() {
            if (bmps.Length > 0 && bmps[0] != null)
                using (MemoryStream stream = new MemoryStream()) {
                    bmps[0].Save(stream, ImageFormat.Png);
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

        private void DisplayBitmaps() {
            bitmapImages = new BitmapImage[bmps.Length];
            indexLast = 0;

            for (int i = 0; i < bmps.Length; i++) {
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

            dispatcherTimer.Start();
        }

        private void btnImageBgLight_Click(object sender, RoutedEventArgs e) {
            brdImage.Background = null;
        }

        private void btnImageBgDark_Click(object sender, RoutedEventArgs e) {
            brdImage.Background = System.Windows.Media.Brushes.Gray;
        }

        private void btnImageSave_Click(object sender, RoutedEventArgs e) {
            FileListItem item = dataGrid.SelectedItem as FileListItem;
            if (item == null)
                return;
            string fileNameNoExt = Path.GetFileNameWithoutExtension(item.File);

            Directory.CreateDirectory(@".\Export");
            if (bmps.Length == 1) {
                string outfile = @".\Export\" + fileNameNoExt + ".png";
                bmps[0].Save(outfile);
                Process.Start("explorer.exe", "/select, \"" + outfile + "\"");
            } else if (bmps.Length > 1) {
                for (int i = 0; i < bmps.Length; i++) {
                    string outfile = @".\Export\" + fileNameNoExt + "_" + i + ".png";
                    bmps[i].Save(outfile);
                    if (i == 0)
                        Process.Start("explorer.exe", "/select, \"" + outfile + "\"");
                }
            }
        }

        private void btnImageSpeed50_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
        }
        private void btnImageSpeed100_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }
        private void btnImageSpeed150_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
        }

        private void btnImagePlayPause_Click(object sender, RoutedEventArgs e) {
            if(dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();
            else
                dispatcherTimer.Start();
        }

        private void btnOld_Click(object sender, RoutedEventArgs e) {
            new MainWindow().Show();
        }

        private void btnHexSave_Click(object sender, RoutedEventArgs e) {
            FileListItem item = dataGrid.SelectedItem as FileListItem;
            if (item == null)
                return;
            string fileNameNoExt = Path.GetFileNameWithoutExtension(item.File);

            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            dialog.DefaultExt = "bin";
            dialog.FileName = fileNameNoExt + "_out";

            if ((bool)dialog.ShowDialog(this)) {
                byte[] bytes = hexEditor.GetAllBytes();
                File.WriteAllBytes(dialog.FileName, bytes);
            }
        }
    }
}

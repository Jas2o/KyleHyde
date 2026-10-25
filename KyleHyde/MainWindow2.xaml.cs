using GameTools;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KyleHyde {
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window {

        GameTools.Loader loader;

        private FileList dataview;

        private Bitmap[] bmps;
        private Bitmap[] bmpsMTC;
        private Bitmap[] bmpsComposite;
        private BitmapImage[] bitmapImages;
        private BitmapImage[] bitmapImagesMTC;
        private BitmapImage[] bitmapImagesComposite;
        private int indexLast, indexLimit;
        private DispatcherTimer dispatcherTimer;

        public MainWindow2() {
            dataview = new FileList();
            this.DataContext = dataview;
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            chkMTCUse.IsChecked = App.Settings.WithMTC;
            chkShowLayers.IsChecked = App.Settings.ShowSeparateLayers;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Settings.WithMTC = (bool)chkMTCUse.IsChecked;
            App.Settings.ShowSeparateLayers = (bool)chkShowLayers.IsChecked;
            App.Settings.Save();
        }

        private void BtnLoadHotelDusk_Click(object sender, RoutedEventArgs e) {
            string basePath = string.Empty;
            string[] files = [];

            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift)) {
                var dialog = new VistaOpenFileDialog();
                dialog.InitialDirectory = App.Settings.DirHotelDusk;
                if (dialog.ShowDialog(this).GetValueOrDefault()) {
                    basePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    files = new string[] { dialog.FileName };
                }
            } else {
                var dialog = new VistaFolderBrowserDialog();
                dialog.SelectedPath = App.Settings.DirHotelDusk;
                if (dialog.ShowDialog(this).GetValueOrDefault()) {
                    basePath = dialog.SelectedPath;
                    App.Settings.DirHotelDusk = basePath;
                    files = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);
                }
            }

            if (files.Length == 0)
                return;

            dataview.Clear();
            loader = new Formats.HotelDusk.Loader();
            foreach (string file in files) {
                FileListItem item = dataview.Add(file, basePath);

                if(file.EndsWith(loader.PackExtension)) {
                    List<GameTools.Pack> packFiles = Formats.HotelDusk.WPF.Open(file, false);
                    foreach(Pack packFile in packFiles) {
                        FileListItem vitem = dataview.AddVirtual(item, packFile);
                    }
                }
            }
        }

        private void BtnLoadLastWindow_Click(object sender, RoutedEventArgs e) {
            var dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = App.Settings.DirLastWindow;
            if (dialog.ShowDialog(this).GetValueOrDefault()) {
                dataview.Clear();
                loader = new Formats.LastWindow.Loader();

                string folder = dialog.SelectedPath;
                App.Settings.DirLastWindow = folder;

                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files) {
                    FileListItem item = dataview.Add(file, folder);

                    if (file.EndsWith(loader.PackExtension)) {
                        List<GameTools.Pack> packFiles = Formats.LastWindow.LWPack.OpenPack(file, false, "");
                        foreach (Pack packFile in packFiles) {
                            FileListItem vitem = dataview.AddVirtual(item, packFile);
                        }
                    }
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

            object result = loader.Open2(item);
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
            } else if (result is KyleHyde.Formats.HotelDusk.TXT) {
                hexEditor.Stream = (result as KyleHyde.Formats.HotelDusk.TXT).GTFS.GetStream();
                txtText.Text = (result as KyleHyde.Formats.HotelDusk.TXT).Text;
                tabControl.SelectedItem = tabText;
            } else if (result is KyleHyde.Formats.HotelDusk.FRM) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.HotelDusk.FRM).bitmap };
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.HotelDusk.ANM) {
                bmps = (result as KyleHyde.Formats.HotelDusk.ANM).BitmapsBlended();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.HotelDusk.WPFBIN) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.HotelDusk.WPFBIN).bitmap };
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.BPG) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.LastWindow.BPG).bitmap };
                hexEditor.Stream = (result as KyleHyde.Formats.LastWindow.BPG).GTFS.GetStream();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.EBP) {
                bmps = new Bitmap[] { (result as KyleHyde.Formats.LastWindow.EBP).bitmap };
                hexEditor.Stream = (result as KyleHyde.Formats.LastWindow.EBP).GTFS.GetStream();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.LWBRA) {
                bmps = (result as KyleHyde.Formats.LastWindow.LWBRA).GetBitmaps();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            } else if (result is KyleHyde.Formats.LastWindow.MTC) {
                bmps = (result as KyleHyde.Formats.LastWindow.MTC).GetBitmaps();
                DisplayBitmaps();
                tabControl.SelectedItem = tabImage;
            }
        }

        private void cmbMTCSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            dispatcherTimer.Stop();
            FileListItem item = cmbMTCSelect.SelectedItem as FileListItem;
            if (item == null)
                return;

            object result = loader.Open2(item);
            if (result is KyleHyde.Formats.LastWindow.MTC) {
                bmpsMTC = (result as KyleHyde.Formats.LastWindow.MTC).GetBitmaps();
                DisplayBitmapsWithMTC();
                //tabControl.SelectedItem = tabImage;
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
            if (indexLast >= indexLimit)
                indexLast = 0;

            if (bitmapImages != null && indexLast < bitmapImages.Length)
                imageBox.Source = bitmapImages[indexLast];

            if (bitmapImagesComposite != null && indexLast < bitmapImagesComposite.Length)
                imageBoxComposite.Source = bitmapImagesComposite[indexLast];

            if (bitmapImagesMTC != null && indexLast < bitmapImagesMTC.Length)
                imageBoxMTC.Source = bitmapImagesMTC[indexLast];

            indexLast++;
        }

        /*
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
        */

        private void DisplayBitmaps() {
            indexLimit = bmps.Length - 1;

            bitmapImages = new BitmapImage[bmps.Length];
            bitmapImagesMTC = new BitmapImage[0];
            bitmapImagesComposite = new BitmapImage[0];
            bmpsComposite = new Bitmap[0];
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

            if (bitmapImages.Length == 0) 
                imageBox.Source = null;
            if(bitmapImagesComposite.Length == 0)
                imageBoxComposite.Source = null;
            if (bitmapImagesMTC.Length == 0)
                imageBoxMTC.Source = null;

            dispatcherTimer.Start();
        }

        private void DisplayBitmapsWithMTC() {
            if (bmps == null || bmps[0].Width != bmpsMTC[0].Width || bmps[0].Height != bmpsMTC[0].Height) {
                cmbMTCSelect.SelectedIndex = 0;
                return;
            }
            indexLimit = Math.Max(bmps.Length - 1, bmpsMTC.Length - 1);

            indexLast = 0;
            bitmapImagesMTC = new BitmapImage[bmpsMTC.Length];
            bitmapImagesComposite = new BitmapImage[bmpsMTC.Length];
            bmpsComposite = new Bitmap[bmpsMTC.Length];

            for (int i = 0; i < indexLimit; i++) {
                int indexBase = Math.Min(i, bmps.Length - 1);
                using (MemoryStream stream = new MemoryStream()) {
                    bmpsMTC[i].Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    bitmapImagesMTC[i] = new BitmapImage();
                    bitmapImagesMTC[i].BeginInit();
                    bitmapImagesMTC[i].StreamSource = stream;
                    bitmapImagesMTC[i].CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImagesMTC[i].EndInit();
                    bitmapImagesMTC[i].Freeze();
                }

                bmpsComposite[i] = new Bitmap(bmps[indexBase].Width, bmps[indexBase].Height);
                for (int y = 0; y < bmps[indexBase].Height; y++) {
                    for (int x = 0; x < bmps[indexBase].Width; x++) {
                        Color lowerC = bmps[indexBase].GetPixel(x, y);
                        Color upperC = bmpsMTC[i].GetPixel(x, y);
                        Color newC = Color.FromArgb(lowerC.A,
                            lowerC.R * upperC.R / 255,
                            lowerC.G * upperC.G / 255,
                            lowerC.B * upperC.B / 255);
                        bmpsComposite[i].SetPixel(x, y, newC);
                    }
                }

                using (MemoryStream stream = new MemoryStream()) {
                    bmpsComposite[i].Save(stream, ImageFormat.Png);
                    stream.Position = 0;

                    bitmapImagesComposite[i] = new BitmapImage();
                    bitmapImagesComposite[i].BeginInit();
                    bitmapImagesComposite[i].StreamSource = stream;
                    bitmapImagesComposite[i].CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImagesComposite[i].EndInit();
                    bitmapImagesComposite[i].Freeze();
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

        private void btnImageBgGreen_Click(object sender, RoutedEventArgs e) {
            brdImage.Background = System.Windows.Media.Brushes.Chartreuse;
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
                //Process.Start("explorer.exe", "/select, \"" + outfile + "\"");
            } else if (bmps.Length > 1) {
                for (int i = 0; i < bmps.Length; i++) {
                    string outfile = @".\Export\" + fileNameNoExt + "_" + i + ".png";
                    bmps[i].Save(outfile);
                    //if (i == 0) Process.Start("explorer.exe", "/select, \"" + outfile + "\"");
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
        private void btnImageSpeed200_Click(object sender, RoutedEventArgs e) {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
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

        private void btnCompare_Click(object sender, RoutedEventArgs e) {
            WindowCompare winCompare = new WindowCompare();
            winCompare.Show();
        }

        private void btnOpenExplorerToExport_Click(object sender, RoutedEventArgs e) {
            string dir = Path.GetFullPath(@".\Export\");
            Process.Start("explorer.exe", "\"" + dir + "\"");
        }
    }
}

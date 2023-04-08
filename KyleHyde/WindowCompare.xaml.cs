using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// Interaction logic for WindowCompare.xaml
    /// </summary>
    public partial class WindowCompare : Window {

        private FileList dataviewTopL;
        private FileList dataviewBottomR;
        private ColorCompareList dataviewCompare;
        private Bitmap bmpL;
        private Bitmap bmpR;

        public WindowCompare() {
            InitializeComponent();
            dataviewTopL = new FileList();
            dataviewBottomR = new FileList();
            dataviewCompare = new ColorCompareList();
            listTopL.DataContext = dataviewTopL;
            listBottomR.DataContext = dataviewBottomR;
            dataCompare.DataContext = dataviewCompare;
        }

        private void btnTopL_Click(object sender, RoutedEventArgs e) {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault()) {
                dataviewTopL.Clear();
                bool alsoAddToBottom = (dataviewBottomR.ListDataRaw.Count == 0);

                string folder = dialog.SelectedPath;

                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files) {
                    dataviewTopL.Add(file, folder);
                    if(alsoAddToBottom)
                        dataviewBottomR.Add(file, folder);
                }
            }
        }

        private void btnBottomR_Click(object sender, RoutedEventArgs e) {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault()) {
                dataviewBottomR.Clear();
                bool alsoAddToTop = (dataviewTopL.ListDataRaw.Count == 0);

                string folder = dialog.SelectedPath;

                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files) {
                    dataviewBottomR.Add(file, folder);
                    if (alsoAddToTop)
                        dataviewTopL.Add(file, folder);
                }
            }
        }

        private void listTopL_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            FileListItem selected = (FileListItem)listTopL.SelectedValue;
            if (selected == null)
                return;

            BitmapImage bitmap = new BitmapImage(new Uri(selected.File));
            bmpL = (Bitmap)Bitmap.FromFile(selected.File);
            imageBoxL.Source = bitmap;

            DoCompare();
        }

        private void listBottomR_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            FileListItem selected = (FileListItem)listBottomR.SelectedValue;
            if (selected == null)
                return;

            BitmapImage bitmap = new BitmapImage(new Uri(selected.File));
            bmpR = (Bitmap)Bitmap.FromFile(selected.File);
            imageBoxR.Source = bitmap;

            DoCompare();
        }

        private void DoCompare() {
            if (bmpL == null || bmpR == null)
                return;

            Bitmap compare = new Bitmap(bmpL.Width, bmpL.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dataviewCompare.Clear();

            int minWidth = Math.Min(bmpL.Width, bmpR.Width);
            int minHeight = Math.Min(bmpL.Height, bmpR.Height);

            int offX = (int)numX.Value;
            int offY = (int)numY.Value;

            try {
                for (int x = 0; x < minWidth; x++) {
                    for (int y = 0; y < minHeight; y++) {
                    
                            var left = bmpL.GetPixel(x, y);
                            var right = bmpR.GetPixel(x + offX, y + offY);

                            if (left.A == 0 && right.A == 0)
                                continue;
                            if (left != right) {
                                compare.SetPixel(x, y, System.Drawing.Color.Red);
                                dataviewCompare.Add(x, y, left, right);
                            }
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
            }

            using (MemoryStream stream = new MemoryStream()) {
                compare.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                imageBoxD.Source = bitmap;
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e) {
            DoCompare();
        }
    }
}

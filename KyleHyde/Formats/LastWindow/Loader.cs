using GameTools;
using KyleHyde.Formats.LastWindow;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace KyleHyde.Formats.LastWindow {
    public class Loader : GameTools.Loader {

        //openFileDialog.FileName = "";
        //openFileDialog.Filter = "All Last Window|*.pack;*.bra;*.bin;*.bpg;*.ebp|All Files (*.*)|*.*";

        public Loader() {
            PackExtension = ".pack";
        }

        public override void Open(string filePath, bool export = false, bool useGTFSView = false) {
            string fileName = Path.GetFileName(filePath);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);

            string[] filenameParts = fileName.Split('.');
            Array.Reverse(filenameParts);

            if (filenameParts[0].ToUpper() == "BPG") {
                GTFS fs = Decompress(filePath);
                fs.WriteBytesToFile("GT-KH-ZL.out");

                BPG bpg = new BPG(fs, fileName.ToLower());
                new WindowImage(bpg.bitmap, fileNameNoExt).Show();

                //if(useGTFSView) new GTFSView(fs).Show();
            } else if (filenameParts[0].ToUpper() == "EBP") {
                GTFS fs = Decompress(filePath);
                fs.WriteBytesToFile(filePath + ".gtbin");

                EBP ebp = new EBP(fs);

                if(ebp.bitmap != null)
                    new WindowImage(ebp.bitmap, fileNameNoExt).Show();

            } else if (filenameParts[0].ToUpper() == "BIN") {
                GTFS fs = Decompress(filePath);

                if (export)
                    fs.WriteBytesToFile("GT-KH-BIN.out");
                else {
                    MessageBoxResult res = MessageBox.Show("Write out to GT-KH-BIN.out file?", "KyleHyde", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) {
                        fs.WriteBytesToFile("GT-KH-BIN.out");
                        Process.Start("explorer.exe", "/select, \"GT-KH-BIN.out\"");
                    }
                }
            } else if (filenameParts[0].ToUpper() == "BRA") {
                LWBRA bra = new LWBRA(filePath);
                Bitmap[] bitmaps = bra.GetBitmaps();

                if(export) {
                    int n = 0;
                    foreach(Bitmap bitmap in bitmaps)
                        bitmap.Save("test-dec-" + (n++) + ".png");
                }

                new WindowImageAnimated(bitmaps, fileNameNoExt).Show();

                //string inputtext = LWBRA.Open(filePath, export, useGTFSView);
                //new FormText(inputtext).Show();
            } else if (filenameParts[0].ToUpper() == "PACK") {
                LWPack.OpenPack(filePath, fileName);
            } else {
                MessageBox.Show("Unexpected file extension: " + filenameParts[0]);
            }
        }

        public override void OpenAllPaks(List<string> dirfiles) {
            foreach(string file in dirfiles) {
                LWPack.OpenPack(file, Path.GetFileName(file));
            }
        }

        private static GTFS Decompress(string file) {
            //First 4 bytes is uncompressed size
            //Next 2 bytes should be 78 9C

            GTFS fs = null;

            FileStream fsComp = new FileStream(file, FileMode.Open);
            int uncomLen = GT.ReadInt32(fsComp, 4, false);
            fsComp.Position += 2;
            using (DeflateStream decompressionStream = new DeflateStream(fsComp, CompressionMode.Decompress)) {
                byte[] raw = new byte[uncomLen];
                decompressionStream.Read(raw, 0, uncomLen);
                fs = new GTFS(raw);
            }
            fsComp.Close();

            return fs;
        }

        public override void MassConvert(List<string> dirfiles) {
            throw new NotImplementedException();
        }
    }
}

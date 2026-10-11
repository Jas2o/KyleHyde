using GameTools;
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

        public override void Open(string filePath, bool export = false) {
            string fileName = Path.GetFileName(filePath);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);

            string[] filenameParts = fileName.Split('.');
            Array.Reverse(filenameParts);

            if (filenameParts[0].ToUpper() == "BPG") {
                GTFS fs = Decompress(filePath);
                fs.WriteBytesToFile("GT-KH-ZL.out");

                BPG bpg = new BPG(fs, fileName.ToLower());
                new WindowImage(bpg.bitmap, fileNameNoExt).Show();
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
            } else if (filenameParts[0].ToUpper() == "IFB") {
                GTFS fs = Decompress(filePath);

                if (export)
                    fs.WriteBytesToFile("GT-KH-IFB.out");
                else {
                    MessageBoxResult res = MessageBox.Show("Write out to GT-KH-IFB.out file?", "KyleHyde", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) {
                        fs.WriteBytesToFile("GT-KH-IFB.out");
                        Process.Start("explorer.exe", "/select, \"GT-KH-IFB.out\"");
                    }
                }
            } else if (filenameParts[0].ToUpper() == "IBA") {
                GTFS fs = Decompress(filePath);

                if (export)
                    fs.WriteBytesToFile("GT-KH-IBA.out");
                else {
                    MessageBoxResult res = MessageBox.Show("Write out to GT-KH-IBA.out file?", "KyleHyde", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes) {
                        fs.WriteBytesToFile("GT-KH-IBA.out");
                        Process.Start("explorer.exe", "/select, \"GT-KH-IBA.out\"");
                    }
                }
            } else if (filenameParts[0].ToUpper() == "BRA") {
                GTFS fs = new GTFS(filePath, true);
                LWBRA bra = new LWBRA(fs);
                Bitmap[] bitmaps = bra.GetBitmaps();

                if(export) {
                    int n = 0;
                    foreach(Bitmap bitmap in bitmaps)
                        bitmap.Save("test-dec-" + (n++) + ".png");
                }

                new WindowImageAnimated(bitmaps, fileNameNoExt).Show();

                //string inputtext = LWBRA.Open(filePath, export);
                //new FormText(inputtext).Show();
            } else if (filenameParts[0].ToUpper() == "MTC") {
                GTFS fs = new GTFS(filePath, true);
                MTC mtc = new MTC(fs);
                Bitmap[] bitmaps = mtc.GetBitmaps();
                new WindowImageAnimated(bitmaps, fileNameNoExt).Show();

            } else if (filenameParts[0].ToUpper() == "PACK") {
                LWPack.OpenPack(filePath, true, fileName);
            } else {
                MessageBox.Show("Unexpected file extension: " + filenameParts[0]);
            }
        }

        public override void OpenAllPaks(List<string> dirfiles) {
            foreach(string file in dirfiles) {
                LWPack.OpenPack(file, true, Path.GetFileName(file));
            }
        }

        private static GTFS Decompress(string file) {
            //First 4 bytes is uncompressed size
            //Next 2 bytes should be 78 9C (zlib header)

            GTFS fs = null;

            FileStream fsComp = new FileStream(file, FileMode.Open);
            int uncomLen = GT.ReadInt32(fsComp, 4, false);
            fsComp.Position += 2;

            byte[] raw = new byte[uncomLen];
            using (MemoryStream decompressedStream = new MemoryStream()) {
                using (DeflateStream decompressionStream = new DeflateStream(fsComp, CompressionMode.Decompress)) {
                    decompressionStream.CopyTo(decompressedStream);
                }
                raw = decompressedStream.ToArray();
                fs = new GTFS(raw);
            }

            fsComp.Close();

            return fs;
        }

        private static GTFS Decompress(GTFS fs_comp) {
            //First 4 bytes is uncompressed size
            //Next 2 bytes should be 78 9C (zlib header)

            GTFS fs_decomp = null;
            int uncomLen = GT.ReadInt32(fs_comp, 4, false);

            byte[] raw = new byte[uncomLen];
            using (MemoryStream decompressedStream = new MemoryStream()) {
                MemoryStream msStream = fs_comp.GetStream();
                msStream.Position = 6;
                using (DeflateStream decompressionStream = new DeflateStream(msStream, CompressionMode.Decompress)) {
                    decompressionStream.CopyTo(decompressedStream);
                }
                raw = decompressedStream.ToArray();
                fs_decomp = new GTFS(raw);
            }

            return fs_decomp;
        }

        public override void MassConvert(List<string> dirfiles) {
            throw new NotImplementedException();
        }

        public override object Open2(FileListItem fileItem) {
            string fileName = Path.GetFileName(fileItem.File);
            string[] filenameParts = fileName.Split('.');
            Array.Reverse(filenameParts);

            try {
                GTFS fs = fileItem.GTFS;
                if (fs == null)
                    return null; //This can happen from setting MTC to blank

                fs.PrepareByteStream();
                fs.Position = 0;

                string extUpper = filenameParts[0].ToUpper();
                switch (extUpper) {
                    case "BIN":
                    case "IBA":
                    case "IFB":
                        fs = Decompress(fileItem.GTFS);
                        return fs;

                    case "BPG":
                        fs = Decompress(fileItem.GTFS);
                        BPG bpg = new BPG(fs, fileName.ToLower());
                        return bpg;

                    case "EBP":
                        fs = Decompress(fileItem.GTFS);
                        EBP ebp = new EBP(fs);
                        return ebp;

                    case "BRA":
                        LWBRA bra = new LWBRA(fileItem.GTFS);
                        return bra;

                    case "MTC":
                        MTC mtc = new MTC(fs);
                        return mtc;
                }
            } catch(Exception ex) {
                return ex;
            }

            return null;
        }
    }
}

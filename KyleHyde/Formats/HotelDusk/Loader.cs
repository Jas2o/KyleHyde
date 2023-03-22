using GameTools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace KyleHyde.Formats.HotelDusk {
    class Loader : GameTools.Loader {

        //filePath = "";
        //openFileDialog.Filter = "All Hotel Dusk|*.anm;*.frm;*.wpf;*.bin;*.txt;*.dtx|All Files (*.*)|*.*";

        public override void Open(string filePath, bool export = false, bool useGTFSView = false) {
            string fileName = Path.GetFileName(filePath);

            string[] filenameParts = fileName.Split('.');
            Array.Reverse(filenameParts);

            if (filenameParts[0].ToUpper() == "FRM") {
                GTFS fs = new GTFS(filePath);
                FRM frm = new FRM(fs);

                new WindowImage(frm.bitmap).Show();
            } else if (filenameParts[0].ToUpper() == "ANM") {
                GTFS fs = new GTFS(filePath);
                ANM anm = new ANM(fs);

                new WindowImageAnimated(anm.BitmapsBlended()).Show();

            } else if (filenameParts[0].ToUpper() == "WPF") {
                //WPF.Open(filePath);
                //throw new NotImplementedException();
                MessageBox.Show(".wpf not supported");
            } else if (filenameParts[0].ToUpper() == "WPFBIN" || filenameParts[0].ToUpper() == "BIN") {
                GTFS fs = new GTFS(filePath);

                byte[] magic = GT.ReadBytes(fs, 4, false);
                fs.Position = 0;
                if(magic.SequenceEqual(LRIM.Magic)) {
                    LRIM lrim = new LRIM(fs);
                } else {
                    WPFBIN wpfbin = new WPFBIN(fs);
                    new WindowImage(wpfbin.bitmap).Show();
                }
            } else if (filenameParts[0].ToUpper() == "DTX") {
                GTFS fs = new GTFS(filePath);
                WPFBIN wpfbin = new WPFBIN(fs);
                new WindowImage(wpfbin.bitmap).Show();
                //GTFS compressed = new GTFS(filePath);
                //GTFS decompressed = Decompress.ToGTFS(compressed);
                //decompressed.WriteBytesToFile("GT-KH-DecompDTX.gtbin");
            } else if (filenameParts[0].ToUpper() == "TXT") {
                GTFS compressed = new GTFS(filePath);
                GTFS decompressed = Decompress.ToGTFS(compressed);
                //decompressed.WriteBytesToFile("GT-KH-DecompTXT.gtbin");

                byte[] test = GT.ReadBytes(decompressed, 4, false);
                decompressed.Position = 0;

                if (test[0] == 0 || test[1] == 0 || test[2] == 0 || test[3] == 3) {
                    int total = GT.ReadInt32(decompressed, 4, false);
                    for(int i = 0; i < total; i++) {
                        int offset = GT.ReadInt32(decompressed, 4, false);
                    }

                    long baseOffset = decompressed.Position;

                    string input = GT.ReadASCII(decompressed, (int)(decompressed.Length - 1 - baseOffset), false);
                    input = input.Replace((char)0x00, (char)0x20);

                    new WindowText(input).Show();

                } else {
                    string input = GT.ReadASCII(decompressed, decompressed.Length - 1, false);
                    input = input.Replace("\n", "\r\n");

                    new WindowText(input).Show();
                }
            } else {
                Debug.WriteLine("Unexpected file extension: " + filenameParts[0]);
            }
        }

        public override void OpenAllPaks(List<string> dirfiles) {
            throw new NotImplementedException();
        }

        public override void MassConvert(List<string> dirfiles) {
            throw new NotImplementedException();
        }
    }
}

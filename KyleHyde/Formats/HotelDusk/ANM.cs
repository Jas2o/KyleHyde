using GameTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace KyleHyde.Formats.HotelDusk {
    class ANM {

        GTFS fs;
        bool flip;
        public string FileName, SafeFileName;
        private List<Pack> listPackFrames;
        public List<FRM> listFrames;
        public MTC mtc;

        public int Height, Width;

        public ANM(GTFS fs) {
            flip = false;
            this.fs = fs;
            FileName = fs.FilePath;
            SafeFileName = Path.GetFileName(FileName);
            listPackFrames = new List<Pack>();
            listFrames = new List<FRM>();

            //--

            uint unk1 = GT.ReadUInt32(fs, 4, flip);
            uint numFrames = GT.ReadUInt32(fs, 4, flip);
            uint numFramesHeaderLen = GT.ReadUInt32(fs, 4, flip);
            uint unk2 = GT.ReadUInt32(fs, 4, flip);

            Height = GT.ReadUInt16(fs, 2, flip);
            Width = GT.ReadUInt16(fs, 2, flip);
            uint unk3 = GT.ReadUInt32(fs, 4, flip);
            uint unk4 = GT.ReadUInt32(fs, 4, flip);
            uint unk5 = GT.ReadUInt32(fs, 4, flip);

            for (int i = 0; i < numFrames; i++) {
                uint frameOffset = GT.ReadUInt32(fs, 4, flip);
                uint frameLen = GT.ReadUInt32(fs, 4, flip);
                uint frameUnk = GT.ReadUInt32(fs, 4, flip);
                uint framePad = GT.ReadUInt32(fs, 4, flip);

                string name = "Frame " + i + ".frm";
                listPackFrames.Add(new Pack(name, frameOffset, frameLen));
            }

            Bitmap lastBitmap = null;
            foreach(Pack pack in listPackFrames) {
                FRM frm = new FRM(fs, pack, Height, Width, lastBitmap);
                listFrames.Add(frm);

                lastBitmap = frm.bitmap;
            }

            //--

            string filemtc = FileName.Replace(".anm", "m_.mtc");
            if (File.Exists(filemtc)) {
                mtc = new MTC(new GTFS(filemtc));
                Console.WriteLine();
            }
        }

        public Bitmap[] BitmapsMTC() {
            int i = 0;
            Bitmap[] bmps = new Bitmap[mtc.numFrames];
            foreach (MTC.MTCFrame frame in mtc.listFrames) {
                bmps[i++] = Tools.ResizeImage(frame.bitmap, Width, Height);
            }
            return bmps;
        }

        public Bitmap[] Bitmaps() {
            int i = 0;
            Bitmap[] bmps = new Bitmap[listFrames.Count];
            foreach (FRM frame in listFrames) {
                bmps[i++] = frame.bitmap;
            }
            return bmps;
        }

        public Bitmap[] BitmapsBlended() {
            if (mtc == null)
                return Bitmaps();

            Bitmap[] blended = new Bitmap[listFrames.Count];
            for (int i = 0; i < listFrames.Count; i++) {
                Bitmap b = new Bitmap(listFrames[i].bitmap);
                Bitmap m = Tools.ResizeImage(mtc.listFrames[i].bitmap, Width, Height);

                blended[i] = Tools.BlendImage(b, m);
            }
            return blended;
        }

        public void DumpPackFramesCompressed() {
            //This was Stage 1 in working out the format, it should no longer be required.

            string extract_path = @".\GT-HotelDusk\Extract";

            string toFolder = SafeFileName.Replace('.', '_');

            if (!Directory.Exists(extract_path))
                Directory.CreateDirectory(extract_path);

            if (!Directory.Exists(extract_path + "\\" + toFolder))
                Directory.CreateDirectory(extract_path + "\\" + toFolder);

            foreach (Pack frame in listPackFrames) {
                string newfile = extract_path + "\\" + toFolder + "\\" + frame.Filename;
                GT.WriteSubFile(fs, newfile, frame.Size, frame.Offset);
            }
        }
    }
}
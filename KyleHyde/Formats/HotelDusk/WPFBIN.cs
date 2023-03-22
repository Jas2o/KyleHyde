using GameTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHyde.Formats.HotelDusk {
    class WPFBIN {

        public Bitmap bitmap;

        public WPFBIN(GTFS fs) {
            int header = GT.ReadInt32(fs, 4, false);

            if (header == 0) {
                fs.Seek(16, SeekOrigin.Begin);
            } else if (header == 14302482) {
                //123DDA00
                fs.Seek(32, SeekOrigin.Begin);
            } else if (header == 31079698) {
                //123DDA01
                fs = Decompress.ToGTFS(fs);
                //fs.WriteBytesToFile("GT-KH-Decomp.gtbin");

                header = GT.ReadInt32(fs, 4, false);

                if (header == 0) {
                    fs.Seek(16, SeekOrigin.Begin);
                } else {
                    Console.WriteLine();
                }
            } else
                throw new Exception();

            bool flip = false;

            int height = GT.ReadUInt16(fs, 2, flip);
            int width = GT.ReadUInt16(fs, 2, flip);

            int heightConfirm = GT.ReadUInt16(fs, 2, flip);
            int widthConfirm = GT.ReadUInt16(fs, 2, flip);
            int four = GT.ReadUInt16(fs, 2, flip);

            if (height != heightConfirm || width != widthConfirm)
                throw new Exception();

            //if (four != 4)
                //Console.WriteLine("Wasn't 4, may be a problem?");

            int paletteLen = GT.ReadUInt16(fs, 2, flip);
            int thirtytwo = GT.ReadUInt16(fs, 4, flip);

            if (thirtytwo != 32)
                throw new Exception();

            int headerLen = (int)fs.Position;
            paletteLen = paletteLen * 2;

            long offsetStart = headerLen + paletteLen; //560
            long offsetLast;

            fs.Position = offsetStart;

            if (width <= 0 || height <= 0)
                throw new Exception();

            bitmap = new Bitmap(width, height);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int look = fs.ReadByte();
                    offsetLast = fs.Position;

                    fs.Seek(look * 2 + headerLen, SeekOrigin.Begin); //+48 for header
                    byte left = (byte)fs.ReadByte();
                    byte right = (byte)fs.ReadByte();
                    //ushort palette = (ushort)(left | right << 8);
                    ushort palette = (ushort)(right | left << 8);

                    bitmap.SetPixel(x, height - y - 1, FRM.Palette2Color(palette));

                    //bmp.SetPixel(x, y, Color.FromArgb(look, look, look));

                    fs.Seek(offsetLast, SeekOrigin.Begin);
                }
            }
        }

    }
}

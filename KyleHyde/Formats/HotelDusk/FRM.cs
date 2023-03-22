using GameTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace KyleHyde.Formats.HotelDusk {
    class FRM {

        public int Width, Height;
        private byte[] rawbytes;
        public Bitmap bitmap;

        public FRM(GTFS fs) {
            Width = 192;
            Height = 256;

            //--

            bool flip = false;
            int zero = GT.ReadInt32(fs, 4, flip);
            int len = GT.ReadInt32(fs, 4, flip);
            int paletteLen = GT.ReadInt32(fs, 4, flip);
            int next = GT.ReadInt32(fs, 4, flip);

            long offsetPalette = 16 + len;

            List<byte> listBytes = new List<byte>();

            while (fs.Position - 16 < len) {
                byte first = GT.ReadByte(fs);

                if (first < 0x40) { //0x7F for Br_bracelet_.anm
                    for (int i = 0; i < first; i++) {
                        byte b = GT.ReadByte(fs);
                        listBytes.Add(b);
                    }
                } else if (first == 0x40) {
                    throw new Exception();
                } else if (first < 0x80) {
                    int repeatlen = first - 0x40;
                    byte second = GT.ReadByte(fs);
                    for (int i = 0; i < repeatlen; i++) {
                        listBytes.Add(second);
                    }
                } else if (first == 0x80) {
                    throw new Exception();
                } else {
                    int gap = first - 0x80;
                    for (int i = 0; i < gap; i++) {
                        listBytes.Add(0xFF);
                    }
                }

            }

            rawbytes = listBytes.ToArray();

            if (rawbytes.Length != 49152)
                Console.WriteLine();

            File.WriteAllBytes("GT-KY-FRM.bin", rawbytes);

            bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int k = 0;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    int b = 255 - rawbytes[k++];

                    if(b > 32) {
                        Color c = Color.FromArgb(b, b, b);
                        bitmap.SetPixel(x, Height - 1 - y, c);
                    } else {
                        fs.Position = offsetPalette + (62 - b*2);

                        byte left = (byte)fs.ReadByte();
                        byte right = (byte)fs.ReadByte();

                        ushort palette = (ushort)(right | left << 8);

                        Color c = Palette2Color(palette);
                        //c = Color.FromArgb(255 - (left3 * 36), c);
                        bitmap.SetPixel(x, Height - 1 - y, c);
                    }
                }
            }
        }

        public FRM(GTFS fs, Pack pack, int height, int width, Bitmap lastBitmap=null) {
            Height = height;
            Width = width;

            //--

            fs.Position = pack.Offset;

            bool flip = false;
            int zero = GT.ReadInt32(fs, 4, flip);
            int len = GT.ReadInt32(fs, 4, flip);
            int paletteLen = GT.ReadInt32(fs, 4, flip);
            int next = GT.ReadInt32(fs, 4, flip);

            long offsetPalette = pack.Offset + len + 16;

            List<byte> listBytes = new List<byte>();

            while (fs.Position < offsetPalette) { //- 16 < pack.Offset + len
                byte first = GT.ReadByte(fs);

                if (first < 0x40) { //0x7F for Br_bracelet_.anm
                    for (int i = 0; i < first; i++) {
                        byte b = GT.ReadByte(fs);
                        listBytes.Add(b);
                    }
                } else if (first == 0x40) {
                    throw new Exception();
                } else if (first < 0x80) {
                    int repeatlen = first - 0x40;
                    byte second = GT.ReadByte(fs);
                    for (int i = 0; i < repeatlen; i++) {
                        listBytes.Add(second);
                    }
                } else if (first == 0x80) {
                    throw new Exception();
                } else {
                    int gap = first - 0x80;
                    for (int i = 0; i < gap; i++) {
                        listBytes.Add(0xFF);
                    }
                }

            }

            rawbytes = listBytes.ToArray();

            if (rawbytes.Length != 49152)
                Console.WriteLine();

            //File.WriteAllBytes("GT-KY-FRM.bin", rawbytes);

            if (lastBitmap != null)
                bitmap = new Bitmap(lastBitmap);
            else
                bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int k = 0;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    int b = 255 - rawbytes[k++];

                    if(b == 0) {
                        //Do nothing
                    } else if (b > 32) {
                        Color c = Color.FromArgb(b, b, b);
                        bitmap.SetPixel(x, Height - 1 - y, c);
                    } else {
                        fs.Position = offsetPalette + (62 - b * 2);

                        byte left = (byte)fs.ReadByte();
                        byte right = (byte)fs.ReadByte();

                        ushort palette = (ushort)(right | left << 8);

                        Color c = Palette2Color(palette);
                        //c = Color.FromArgb(255 - (left3 * 36), c);
                        bitmap.SetPixel(x, Height - 1 - y, c);
                    }
                }
            }
        }

        public static Color Palette2Color(ushort palette) {
            ushort hex = Tools.SwapBytes(palette);
            int R = (hex & 0x1F) * 8;
            int G = (hex >> 5 & 0x1F) * 8;
            int B = (hex >> 10 & 0x1F) * 8;

            //int A = (hex >> 15 & 0x01) * 255;
            //return Color.FromArgb(255-A, R, G, B);

            return Color.FromArgb(R, G, B);
        }

        public static Color Palette2Color(byte left, byte right) {
            return Palette2Color((ushort)(right | left << 8));
        }
    }
}

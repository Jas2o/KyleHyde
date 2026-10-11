using GameTools;
using KyleHyde.Formats.HotelDusk;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace KyleHyde.Formats.LastWindow {
    class MTC {

        public List<MTCFrame> listFrames;
        public int numFrames;
        public int mtcHeight, mtcWidth;
        public int anmWidth, anmHeight;
        public int anmUnknown; //Speed or loop?

        public MTC(GTFS fs) {
            numFrames = GT.ReadInt32(fs, 4);
            anmWidth = GT.ReadInt32(fs, 4);
            anmHeight = GT.ReadInt32(fs, 4);
            anmUnknown = GT.ReadInt32(fs, 4);
            fs.Position += 16; //Zeroes

            mtcWidth = 25;
            mtcHeight = (int)(fs.Length - 32) / numFrames / 25 / 2;

            listFrames = new List<MTCFrame>();
            for (int i = 0; i < numFrames; i++)
                listFrames.Add(new MTCFrame(fs, mtcWidth, mtcHeight));
        }

        public Bitmap[] GetBitmaps()
        {
            Bitmap[] bitmaps = new Bitmap[numFrames];
            for (int i = 0; i < numFrames; i++) {
                bitmaps[i] = listFrames[i].bitmap;
                continue;
                
                Bitmap newImage = new Bitmap(anmWidth, anmHeight);
                using (Graphics gr = Graphics.FromImage(newImage)) {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(listFrames[i].bitmap, new Rectangle(0, 0, anmWidth, anmHeight));
                }
                bitmaps[i] = newImage;
            }
            return bitmaps;
        }

        public class MTCFrame {
            //public Color[] pixels;
            public Bitmap bitmap;

            public MTCFrame(GTFS fs, int mtcWidth, int mtcHeight) {
                //pixels = new Color[mtcWidth * mtcHeight];
                bitmap = new Bitmap(mtcWidth, mtcHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int i = 0;
                for (int y = 0; y < mtcHeight; y++) {
                    for (int x = 0; x < mtcWidth; x++) {
                        byte left = (byte)fs.ReadByte();
                        byte right = (byte)fs.ReadByte();

                        ushort palette = (ushort)(right | left << 8);
                        Color c = FRM.Palette2Color(palette);

                        //if (right >= 0x80)
                        //Console.WriteLine();
                        //Currently not handling the transparency

                        c = Color.FromArgb(255, c);

                        //pixels[i++] = c;
                        bitmap.SetPixel(x, y, c);
                    }
                }
            }
        }
    }
}

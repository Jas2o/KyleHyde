using GameTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHyde.Formats.HotelDusk {
    class MTC {

        public List<MTCFrame> listFrames;
        public int mtcHeight, mtcWidth, numFrames, anmWidth, anmHeight;

        public MTC(GTFS fs) {
            mtcHeight = 33;
            mtcWidth = 17;

            //--

            bool flip = false;
            numFrames = GT.ReadInt32(fs, 4, flip);
            anmWidth = GT.ReadInt16(fs, 2, flip);
            anmHeight = GT.ReadInt16(fs, 2, flip);
            fs.Position += 24;

            listFrames = new List<MTCFrame>();
            for(int i = 0; i < numFrames; i++)
                listFrames.Add(new MTCFrame(fs, mtcWidth, mtcHeight));
        }

        public class MTCFrame {
            //public Color[] pixels;
            public Bitmap bitmap;

            public MTCFrame(GTFS fs, int mtcWidth, int mtcHeight) {
                //pixels = new Color[mtcWidth * mtcHeight];
                bitmap = new Bitmap(mtcWidth, mtcHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int i = 0;
                for(int y = 0; y < mtcHeight; y++) {
                    for(int x = 0; x < mtcWidth; x++) {
                        byte left = (byte)fs.ReadByte();
                        byte right = (byte)fs.ReadByte();

                        ushort palette = (ushort)(right | left << 8);
                        Color c = FRM.Palette2Color(palette);

                        if(c.R != c.G && c.G != c.B)
                            c = Color.FromArgb(150, c);
                        else
                            c = Color.FromArgb(0, c);

                        //pixels[i++] = c;
                        bitmap.SetPixel(x, y, c);
                    }
                }
            }
        }
    }
}

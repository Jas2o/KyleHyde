using GameTools;
using System;
using System.Drawing;

namespace KyleHyde.Formats.LastWindow
{
    class EBP
    {

        public int Width, Height;
        public Bitmap bitmap;

        public EBP(GTFS fs)
        {
            bool flip = false;

            Width = GT.ReadInt32(fs, 4, flip);
            Height = GT.ReadInt32(fs, 4, flip);
            int flag = GT.ReadInt32(fs, 4, flip);

            bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Color[] palette;

            if (flag == 4)
            {
                //Unsure
                int numPal = 16;
                palette = new Color[numPal];
                for (int i = 0; i < numPal; i++)
                {
                    byte left = GT.ReadByte(fs);
                    byte right = GT.ReadByte(fs);
                    palette[i++] = HotelDusk.FRM.Palette2Color(left, left);
                    palette[i] = HotelDusk.FRM.Palette2Color(right, right);
                }

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        byte two = GT.ReadByte(fs);
                        byte left = (byte)(two >> 4);
                        byte right = (byte)(two & 0xF);

                        bitmap.SetPixel(x++, y, palette[left]);
                        bitmap.SetPixel(x, y, palette[right]);
                    }
                }

            }
            else if (flag == 8)
            {
                //BGRA8888
                int numPal = 256;
                palette = new Color[numPal];
                for (int i = 0; i < numPal; i++)
                {
                    byte red = GT.ReadByte(fs);
                    byte green = GT.ReadByte(fs);
                    byte blue = GT.ReadByte(fs);
                    byte alpha = GT.ReadByte(fs);
                    palette[i] = Color.FromArgb(255 - alpha, red, green, blue);
                }

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        byte lookup = GT.ReadByte(fs);
                        bitmap.SetPixel(x, y, palette[lookup]);
                    }
                }
            }
            else if (flag == 53)
            {
                //RBGA1555
                int numPal = 32;
                palette = new Color[numPal];
                for (int i = 0; i < numPal; i++)
                {
                    byte left = GT.ReadByte(fs);
                    byte right = GT.ReadByte(fs);
                    palette[i] = HotelDusk.FRM.Palette2Color(left, right);
                }

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        byte lookup = GT.ReadByte(fs);
                        if (lookup >= numPal)
                            lookup = (byte)(numPal - 1);
                        bitmap.SetPixel(x, y, palette[lookup]);
                    }
                }
            }
            else if (flag == 17476)
            {
                //RGBA1555
                int palLen = GT.ReadInt32(fs, 4, flip);
                int imgSmall = GT.ReadInt32(fs, 4, flip);
                int imgLarge = GT.ReadInt32(fs, 4, flip);

                int numPal = palLen / 2;
                palette = new Color[numPal];
                for (int i = 0; i < numPal; i++)
                {
                    byte left = GT.ReadByte(fs);
                    byte right = GT.ReadByte(fs);
                    palette[i] = HotelDusk.FRM.Palette2Color(left, right);
                }

                //This doesn't seem to be working at all, compression or the width/height are wrong?

                //First image should be the largest
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        byte lookup = GT.ReadByte(fs);
                        bitmap.SetPixel(x, y, palette[lookup]);
                    }
                }

                //Ignore the last image?
            }
            else
            {
                throw new Exception();
            }
        }

    }
}

using GameTools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace KyleHyde.Formats.LastWindow
{
    class LWBRF
    {

        protected static int numTilesX = 16;
        protected static int numTilesY = 12;

        protected static int tileHeight = 16;
        protected static int tileWidth = 16;

        public int Number;
        protected int Height, Width;
        protected bool[,] coord;
        public GTFSW decFrame;

        public LWBRF()
        {
        }

        public LWBRF(int Number, int Height, int Width, GTFSW fs, int decBufferLength, int offset, int coLength, int comBufferLen)
        {
            this.Number = Number;
            this.Height = Height;
            this.Width = Width;

            decFrame = new GTFSW(new byte[decBufferLength]);
            fs.Position = offset;

            coord = new bool[numTilesX, numTilesY];
            for (int k = 0; k < coLength; k += 2)
            {
                int x = GT.ReadByte(fs);
                int y = GT.ReadByte(fs);
                coord[x, y] = true;
            }

            byte[] frameData = new byte[comBufferLen];
            fs.Read(frameData, 0, comBufferLen);
            GTFSW comFrame = new GTFSW(frameData);

            bool newFirst = true;
            byte controlByte = 0x00;
            while (comFrame.Position < comFrame.Length)
            {
                byte mask = GT.ReadByte(comFrame);
                System.Collections.BitArray ba = new System.Collections.BitArray(new byte[] { mask });

                int start = 7;
                if (newFirst)
                {
                    newFirst = false;
                    start = 3;
                    controlByte = (byte)(mask & 0xF0);
                }

                for (int k = start; k >= 0; k--)
                {
                    if (comFrame.Position >= comFrame.Length)
                        break;

                    if (ba[k])
                    {
                        byte single = GT.ReadByte(comFrame);
                        decFrame.WriteByte(single);
                    }
                    else
                    {
                        byte doubleA = GT.ReadByte(comFrame);
                        byte doubleB = GT.ReadByte(comFrame);

                        if (doubleA == 0xFF && doubleB == 0xFF)
                        {
                            newFirst = true;
                            k = 0;
                            continue;
                        }

                        int length = 0;
                        int lookAtOffset = 0;

                        if (controlByte == 0x80)
                        {
                            length = ((doubleB & 0xF0) >> 4) + 2;
                            lookAtOffset = (short)(0xF000 + ((doubleB & 0x0F) << 8) + doubleA);
                        }
                        else if (controlByte == 0x40)
                        {
                            length = ((doubleB & 0xF8) >> 3) + 2;
                            lookAtOffset = (short)(0xF800 + ((doubleB & 0x07) << 8) + doubleA);
                        }
                        else if (controlByte == 0xC0)
                        {
                            length = ((doubleB & 0xE0) >> 5) + 2;
                            lookAtOffset = (short)(0xE000 + ((doubleB & 0x1F) << 8) + doubleA);
                        }
                        else if (controlByte == 0x00)
                        {
                            length = ((doubleB & 0xC0) >> 6) + 2;
                            lookAtOffset = (short)(0xC000 + ((doubleB & 0x3F) << 8) + doubleA);
                        }
                        else
                        {
                            Console.WriteLine();
                        }

                        if (lookAtOffset > 0)
                        {
                            for (int repeat = 0; repeat < length; repeat++)
                            {
                                byte b = GT.ReadByteAt(decFrame, lookAtOffset);
                                decFrame.WriteByte(b);
                            }
                        }
                        else if (decFrame.Position + lookAtOffset < 0)
                        {
                            //There's only one of these in lookpocketbook
                            for (int repeat = 0; repeat < length; repeat++)
                                decFrame.WriteByte(0x00);
                        }
                        else
                        {
                            // This is fairly good for the first half
                            for (int repeat = 0; repeat < length; repeat++)
                            {
                                byte b = GT.ReadByteAt(decFrame, decFrame.Position + lookAtOffset);
                                decFrame.WriteByte(b);
                            }
                        }
                    }
                }
            }
        }

        public Bitmap GenerateImage(Color[] palette)
        {
            decFrame.Position = 0;

            Bitmap bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < numTilesX; x++)
            {
                for (int y = 0; y < numTilesY; y++)
                {
                    if (coord[x, y])
                    {
                        for (int tx = 0; tx < tileWidth; tx++)
                        {
                            for (int ty = 0; ty < tileHeight; ty++)
                            {
                                byte lookup = (byte)decFrame.ReadByte();
                                int alpha = 255;
                                if (palette.Length != 256) {
                                    alpha = (byte)((lookup & 0xE0)+((lookup & 0xE0) >> 3) + ((lookup & 0xE0) >> 6));
                                    if (alpha > 255)
                                        alpha = 255;
                                    lookup = (byte)(lookup & 0x1F);
                                }

                                Color cBase = palette[lookup];
                                Color c = Color.FromArgb(alpha, cBase.R, cBase.G, cBase.B);
                                bitmap.SetPixel(y * tileHeight + ty, x * tileWidth + tx, c);
                            }
                        }
                    }
                }
            }

            return bitmap;
        }

    }
}

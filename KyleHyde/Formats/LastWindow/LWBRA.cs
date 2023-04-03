using GameTools;
using System.Drawing;

namespace KyleHyde.Formats.LastWindow
{
    class LWBRA
    {

        public int NumFrames;
        public int Unknown1;
        public int TotalFrameSize; //Can be less than Width * Height
        public int Unknown2;
        public int Width;
        public int Height;
        public Color[] palette;

        private int[] offsetCoTable;
        private int[] lenCoTable;
        private int[] lenBuffer;

        private LWBRF[] frames;

        /*
        public static string Open(string filepath, bool export = false) {
            / *
            if (export) {
                GT.WriteSubFile(fs, "test-pal.bin", paletteLen); //RGBA1555
                fs.Position = 28;

                fsFrame.WriteBytesToFile("test-" + i + ".brf");
            }
            * /
            LWBRA bra = new LWBRA(filepath);

            return "Done";
        }
        */

        public LWBRA(string filepath)
        {
            GTFSW fs = new GTFSW(filepath);

            NumFrames = GT.ReadInt32(fs);
            Unknown1 = GT.ReadInt32(fs); //Largest lenBuffer?
            int paletteLen = GT.ReadInt32(fs);
            TotalFrameSize = GT.ReadInt32(fs);
            Unknown2 = GT.ReadInt32(fs); //LookPocketBook=5, DoorA=9
            Width = GT.ReadInt32(fs);
            Height = GT.ReadInt32(fs);

            int paletteNum = paletteLen / 2;
            palette = new Color[paletteNum];
            for (int i = 0; i < paletteNum; i++)
            {
                byte left = GT.ReadByte(fs);
                byte right = GT.ReadByte(fs);
                palette[i] = HotelDusk.FRM.Palette2Color(left, right);
            }

            offsetCoTable = new int[NumFrames];
            lenCoTable = new int[NumFrames];
            lenBuffer = new int[NumFrames];
            for (int i = 0; i < NumFrames; i++)
            {
                offsetCoTable[i] = GT.ReadInt32(fs);
                lenCoTable[i] = GT.ReadInt32(fs);
                lenBuffer[i] = GT.ReadInt32(fs);
            }

            frames = new LWBRF[NumFrames];
            for (int i = 0; i < NumFrames; i++)
            {
                frames[i] = new LWBRF(i, Height, Width, fs, TotalFrameSize * 2, offsetCoTable[i], lenCoTable[i], lenBuffer[i]);
            }
        }

        public Bitmap[] GetBitmaps()
        {
            Bitmap[] bitmaps = new Bitmap[NumFrames];
            for (int i = 0; i < NumFrames; i++)
                bitmaps[i] = frames[i].GenerateImage(palette);
            return bitmaps;
        }

    }
}

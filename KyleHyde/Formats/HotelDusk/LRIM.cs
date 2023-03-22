using GameTools;
using System;
using System.Collections.Generic;
using System.IO;

namespace KyleHyde.Formats.HotelDusk {

    class LRIM {

        public static readonly byte[] Magic = new byte[]{ 0x4C, 0x52, 0x49, 0x4D };

        GTFS fs;
        bool flip;
        public string FileName, SafeFileName;
        private List<Pack> listPackFrames;

        public LRIM(GTFS fs) {
            flip = false;
            this.fs = fs;
            FileName = fs.FilePath;
            SafeFileName = Path.GetFileName(FileName);
            listPackFrames = new List<Pack>();

            //--

            int magicheader = GT.ReadInt32(fs, 4, flip);
            int num = GT.ReadInt32(fs, 4, flip);
            int sixteen = GT.ReadInt32(fs, 4, flip);
            int unk1 = GT.ReadInt32(fs, 4, flip);

            int[] offset = new int[num];
            for(int i = 0; i < num; i++) {
                offset[i] = GT.ReadInt32(fs, 4, flip);
            }

            int[] size = new int[num];
            for (int i = 0; i < num; i++) {
                size[i] = GT.ReadInt32(fs, 4, flip);

                string name = "LRIM " + i + ".lrim";
                listPackFrames.Add(new Pack(name, offset[i], size[i]));
            }

            foreach(Pack pack in listPackFrames) {
                pack.WriteOut(fs, "GT-HD-LRIM-" + SafeFileName.Replace(".bin", ""));
            }

            Console.WriteLine("Offsets: " + string.Join(", ", offset));
            Console.WriteLine("Sizes: " + string.Join(", ", size));

            Console.WriteLine();
        }

    }
}

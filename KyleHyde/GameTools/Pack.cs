using System;
using System.IO;

namespace GameTools {
    public class Pack {
        public string Filename;
        public long Offset;
        public long Size;

        public byte[] Data;

        public Pack(string filename, long offset, long size) {
            this.Filename = filename;
            this.Offset = offset;
            this.Size = size;
        }

        public void WriteOut(GTFS fs, string outdir) {
            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);

            Filename = Filename.Replace('/', '_'); //From old PAK4

            if (Offset < 0)
                throw new Exception(); //Probably invalid for GT.WriteSubFile, use the other one.

            string newfile = outdir + "\\" + Filename;
            GT.WriteSubFile(fs, newfile, Size, Offset);
        }
    }
}

using GameTools;
using System.IO;
using System.Collections.Generic;

namespace KyleHyde.Formats.LastWindow
{
    static class LWPack
    {
        public static string extract_path = @".\GT-LastWindow\Extract";

        public static List<Pack> OpenPack(string file, bool doExtract, string safefile)
        {
            bool flip = true;
            GTFS fs = new GTFS(file);

            uint nothing = GT.ReadUInt32(fs, 4, flip);
            uint numFiles = GT.ReadUInt32(fs, 4, flip);
            uint offset = GT.ReadUInt32(fs, 4, flip); // + 4 I guess?

            uint unknown = GT.ReadUInt32(fs, 4, flip);

            List<Pack> listPack = new List<Pack>();

            long nextOffset = offset + 4;
            for (int i = 0; i < numFiles; i++)
            {
                byte nameLen = GT.ReadByte(fs);
                byte[] nameB = GT.ReadTextArray(fs, nameLen, false);
                string name = System.Text.Encoding.Default.GetString(nameB);
                int fileLen = GT.ReadInt32(fs, 4, flip);

                listPack.Add(new Pack(name, nextOffset, fileLen));
                nextOffset += fileLen;
            }

            if (doExtract) {
                string toFolder = safefile.Replace('.', '_');

                if (!Directory.Exists(extract_path))
                    Directory.CreateDirectory(extract_path);

                if (!Directory.Exists(extract_path + "\\" + toFolder))
                    Directory.CreateDirectory(extract_path + "\\" + toFolder);

                foreach (Pack pack in listPack) {
                    string newfile = extract_path + "\\" + toFolder + "\\" + pack.Filename;
                    GT.WriteSubFile(fs, newfile, (int)pack.Size);
                }
            }

            return listPack;
        }

    }
}

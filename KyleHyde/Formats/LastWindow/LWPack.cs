using GameTools;
using System.IO;
using System.Collections.Generic;

namespace KyleHyde.Formats.LastWindow
{
    static class LWPack
    {
        public static string extract_path = @".\GT-LastWindow\Extract";

        public static void OpenPack(string file, string safefile)
        {
            bool flip = true;
            GTFS fs = new GTFS(file);

            uint nothing = GT.ReadUInt32(fs, 4, flip);
            uint numFiles = GT.ReadUInt32(fs, 4, flip);
            uint offset = GT.ReadUInt32(fs, 4, flip); // + 4 I guess?

            uint unknown = GT.ReadUInt32(fs, 4, flip);

            List<Pack> listPack = new List<Pack>();

            for (int i = 0; i < numFiles; i++)
            {
                byte nameLen = GT.ReadByte(fs);
                string name = GT.ReadASCII(fs, nameLen, false);
                int fileLen = GT.ReadInt32(fs, 4, flip);

                listPack.Add(new Pack(name, -1, fileLen));
            }

            string toFolder = safefile.Replace('.', '_');

            if (!Directory.Exists(extract_path))
                Directory.CreateDirectory(extract_path);

            if (!Directory.Exists(extract_path + "\\" + toFolder))
                Directory.CreateDirectory(extract_path + "\\" + toFolder);

            foreach (Pack pack in listPack)
            {
                string newfile = extract_path + "\\" + toFolder + "\\" + pack.Filename;
                GT.WriteSubFile(fs, newfile, (int)pack.Size);
            }
        }

    }
}

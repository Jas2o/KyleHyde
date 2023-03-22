using GameTools;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KyleHyde.Formats.HotelDusk {
    static class WPF {
        public static string extract_path = @".\Extract";

        public static void Open(string file) {
            bool flip = false;
            GTFS fs = new GTFS(file);

            List<Pack> listPack = new List<Pack>();

            while (fs.Position < fs.Length - 40) {
                byte[] nameArray = new byte[24];
                for (int k = 0; k < 24; k++)
                    nameArray[k] = (byte)fs.ReadByte();

                long fileLen = GT.ReadUInt32(fs, 4, flip);
                long nextFileOffset = GT.ReadUInt32(fs, 4, flip);
                long currentOffset = fs.Position;

                string name = Encoding.Default.GetString(nameArray);
                name = name.Substring(1).Replace("\0", "").Replace(".bin", ".wpfbin");

                listPack.Add(new Pack(name, currentOffset, fileLen));

                fs.Seek(nextFileOffset, SeekOrigin.Begin);
            }

            string fileName = Path.GetFileName(file);
            string toFolder = fileName.Replace('.', '_');

            if (!Directory.Exists(extract_path))
                Directory.CreateDirectory(extract_path);

            if (!Directory.Exists(extract_path + "\\" + toFolder))
                Directory.CreateDirectory(extract_path + "\\" + toFolder);

            foreach (Pack pack in listPack) {
                string newfile = extract_path + "\\" + toFolder + "\\" + pack.Filename;
                GT.WriteSubFile(fs, newfile, pack.Size, pack.Offset);
            }
        }
    }
}

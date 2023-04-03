using System;
using System.IO;
using System.Text;

namespace GameTools {
    public static class GT {

        public static bool EnableGTFSView = false;

        public static byte ReadByte(GTFS fs) {
            if (EnableGTFSView)
                fs.AddTrack(fs.Position, 1);

            return (byte)fs.ReadByte();
        }

        public static byte ReadByteAt(GTFS fs, long offset) {
            if (EnableGTFSView)
                fs.AddTrack(fs.Position, 1);

            long remember = fs.Position;
            fs.Position = offset;
            byte b = (byte)fs.ReadByte();
            fs.Position = remember;

            return b;
        }

        public static string ReadASCIItoNull(GTFS fs, long offset, bool flip = false, byte terminator = 0x00) {
            long previous = fs.Position;

            int end = 0;
            fs.Position = offset;
            for (int k = 0; k < 100; k++) {
                byte test = (byte)fs.ReadByte();
                if (test == terminator) {
                    end = k;
                    break;
                }
            }

            if (end == 0)
                throw new Exception();

            fs.Position = previous;

            byte[] bString = ReadBytes(fs, offset, end, flip);

            return Encoding.ASCII.GetString(bString);
        }

        public static string ReadASCIINulltoNull(GTFS fs, long offset, bool flip, byte terminator = 0x00) {
            long previous = fs.Position;

            int start = 0;
            fs.Position = offset;
            for (int k = 0; k < 100; k++) {
                byte test = (byte)fs.ReadByte();
                if (test != terminator) {
                    start = k;
                    break;
                }
            }

            int end = 0;
            fs.Position = offset + start;
            for (int k = start; k < 100; k++) {
                byte test = (byte)fs.ReadByte();
                if (test == terminator) {
                    end = k;
                    break;
                }
            }

            if (end == 0)
                throw new Exception();

            fs.Position = previous;

            byte[] bString = ReadBytes(fs, offset + start, end, flip);

            return Encoding.ASCII.GetString(bString);
        }

        public static string ReadASCII(GTFS fs, long offset, int length, bool flip = false) {
            return Encoding.ASCII.GetString(ReadBytes(fs, offset, length, flip));
        }
        public static string ReadASCII(GTFS fs, int length, bool flip = false) {
            return Encoding.ASCII.GetString(ReadBytes(fs, length, flip));
        }

        public static int ReadInt16(GTFS fs, int length, bool flip = false) {
            byte[] bytes = ReadBytes(fs, length, flip);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static uint ReadUInt32(GTFS fs, long offset, int length, bool flip = false) {
            byte[] bytes = ReadBytes(fs, offset, length, flip);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static int ReadInt32(FileStream fs, int length, bool flip = false) {
            byte[] bytes = new byte[length];
            fs.Read(bytes, (int)fs.Position, length);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static int ReadInt32(GTFS fs, int length = 4, bool flip = false) {
            byte[] bytes = ReadBytes(fs, length, flip);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint ReadUInt32(GTFS fs, int length = 4, bool flip = false) {
            byte[] bytes = ReadBytes(fs, length, flip);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static ushort ReadUInt16(GTFS fs, int length, bool flip = false) {
            byte[] bytes = ReadBytes(fs, length, flip);

            return BitConverter.ToUInt16(bytes, 0);
        }

        public static float ReadFloat(GTFS fs, int length, bool flip = false) {
            byte[] bytes = ReadBytes(fs, length, flip);

            return BitConverter.ToSingle(bytes, 0);
        }

        public static byte[] ReadBytes(GTFS fs, int length, bool flip = false) {
            if (length < 0)
                throw new Exception();

            byte[] bytes = new byte[length];
            fs.Read(bytes, 0, length);

            //for (int i = 0; i < length i++)
                //bytes[i] = (byte)fs.ReadByte();

            if (EnableGTFSView)
                fs.AddTrack(fs.Position-length, length);

            if (flip)
                Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] ReadBytes(GTFS fs, long offset, int length, bool flip = false) {
            byte[] bytes = new byte[length];

            long original = fs.Position;
            fs.Position = offset;

            //for (int i = 0; i < length; i++)
            //bytes[i] = (byte)fs.ReadByte();

            fs.Read(bytes, 0, length);

            if (EnableGTFSView)
                fs.AddTrack(offset, length);

            fs.Position = original;

            if (flip)
                Array.Reverse(bytes);

            return bytes;
        }

        public static string ByteArrayToString(byte[] ba, string join = "") {
            //http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", join);
        }

        public static void WriteSubFile(GTFS fs, string newfile, long length, long offset) {
            long last = fs.Position;

            FileStream nf = File.Create(newfile);
            byte[] buffer = new byte[length];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, buffer.Length);
            nf.Write(buffer, 0, buffer.Length);
            nf.Close();

            fs.Position = last;
        }

        public static void WriteSubFile(GTFS fs, string newfile, long length) {
            FileStream nf = File.Create(newfile);
            byte[] buffer = new byte[length];
            fs.Read(buffer, 0, buffer.Length);
            nf.Write(buffer, 0, buffer.Length);
            nf.Close();
        }
    }
}

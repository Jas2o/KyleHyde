using GameTools;
using System.Collections;

namespace KyleHyde.Formats.HotelDusk {
    class Decompress {
        // V1

        //https://www.gamefaqs.com/ds/933043-hotel-dusk-room-215/faqs/46878?print=1

        public static GTFS ToGTFS(GTFS fs) {
            fs.Position = 0;

            byte[] header = GT.ReadBytes(fs, 4, false);
            int sizeun = GT.ReadInt32(fs, 4, false);
            int sizeco = GT.ReadInt32(fs, 4, false);
            int zero = GT.ReadInt32(fs, 4, false);

            byte[] uncompressed = new byte[sizeun];
            byte[] expanded = new byte[sizeun]; // i.e. doesn't fill in the gaps
            int pos = 0;

            while (fs.Position < sizeco + 16) {
                byte input = GT.ReadByte(fs);
                BitArray bits = new BitArray(new byte[] { input });

                for (int i = 0; i < 8; i++) {
                    if (fs.Position >= sizeco + 16)
                        break;

                    if (bits[i]) {
                        byte b = GT.ReadByte(fs);
                        uncompressed[pos] = b;
                        expanded[pos] = b;
                        pos++;
                    } else {
                        byte[] bOff = GT.ReadBytes(fs, 2, false);
                        int len = 4 + GT.ReadByte(fs);
                        int offset = (bOff[1] << 8) + bOff[0];

                        int posHi = hibit(pos - 259);
                        int offHi = hibit(offset);
                        if (pos < 0x10000) {
                            int signed = (short)offset;
                            if(signed < 0 && signed + 259 >= 0)
                                offset = signed;
                        } else if (posHi >= 0x20000 && offHi < posHi) {
                            if(offset + posHi < pos)
                                offset += posHi;
                            else if (offset + 0x10000 < pos)
                                offset += 0x10000;
                        } else if (posHi >= 0x10000 && offHi < posHi && offset + posHi < pos)
                            offset += posHi;
                        offset += 259;

                        if (offset < 0 || offset + len >= uncompressed.Length) {
                            //Console.WriteLine("Shouldn't happen: " + GT.ByteArrayToString(bOff, " ") + " (" + offset + ") at pos: " + pos);
                            for (int x = 0; x < len; x++)
                                uncompressed[pos + x] = 0;
                        } else {
                            for (int x = 0; x < len; x++)
                                uncompressed[pos + x] = uncompressed[offset + x];
                        }
                        #region Expanded
                        expanded[pos + 0] = bOff[0];
                        expanded[pos + 1] = bOff[1];
                        expanded[pos + 2] = (byte)(len - 4);

                        for (int g = 3; g < len; g++)
                            expanded[pos + g] = 0x00;
                        #endregion

                        pos += len;
                    }
                }
            }

            //new FormHexCompare(expanded, uncompressed).Show();

            return new GTFS(uncompressed);
        }

        private static int hibit(int n) {
            //http://stackoverflow.com/questions/53161/find-the-highest-order-bit-in-c
            n |= (n >> 1);
            n |= (n >> 2);
            n |= (n >> 4);
            n |= (n >> 8);
            n |= (n >> 16);
            return n - (n >> 1);
        }

    }
}

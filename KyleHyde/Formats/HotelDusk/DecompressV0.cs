using GameTools;
using System;
using System.Collections;

namespace KyleHyde.Formats.HotelDusk {
    class DecompressV0 {

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

            byte boff1max = 0x62;

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
                        int offsetSIGNED = GT.ReadInt16(fs, 2, false);
                        fs.Position -= 2;
                        int offset = GT.ReadUInt16(fs, 2, false);
                        fs.Position -= 2;
                        byte[] bOff = GT.ReadBytes(fs, 2, false);

                        int len = 4 + GT.ReadByte(fs);

                        /*
                        //START DEBUG
                        uncompressed[pos + 0] = bOff[0];
                        uncompressed[pos + 1] = bOff[1];
                        uncompressed[pos + 2] = (byte)(len - 4);

                        for(int g = 3; g < len; g++) {
                            uncompressed[pos + g] = 0xF5;
                        }
                        //END DEBUG
                        */

                        /*if (bOff[1] == 0xFE || bOff[1] == 0xFF) {
                            Console.WriteLine("0 Problem " + GT.ByteArrayToString(bOff, " ") + " (Signed: " + offsetSIGNED + ") at " + pos + " (for len: " + len + ")");
                        } else {*/
                        if (pos >= 196608) {
                            // Shouldn't happen
                            throw new Exception();
                        }

                        if (pos >= 131072 && bOff[1] <= boff1max) {
                            offsetSIGNED += 131072 + 259;
                            if (offsetSIGNED >= 0 && offsetSIGNED < pos) {
                                for (int x = 0; x < len; x++)
                                    uncompressed[pos + x] = uncompressed[offsetSIGNED + x];
                            } else {
                                Console.WriteLine("1 Problem " + GT.ByteArrayToString(bOff, " ") + " (Signed: " + offsetSIGNED + ") at " + pos + " (for len: " + len + ")");
                                //throw new Exception();
                                for (int x = 0; x < len; x++)
                                    uncompressed[pos + x] = 0x00;
                            }
                        } else if (pos >= 65536 && bOff[1] <= boff1max) {

                            if(offsetSIGNED < 0)
                                offsetSIGNED = (bOff[1] << 8) + bOff[0] + 259;
                            else
                                offsetSIGNED += 65536;

                            if (offsetSIGNED >= 0 && offsetSIGNED < pos) {
                                for (int x = 0; x < len; x++)
                                    uncompressed[pos + x] = uncompressed[offsetSIGNED + x];
                            } //else
                                //throw new Exception();

                        } else {
                            offset += 259;

                            if (bOff[1] == 0xFF) {
                                //Can't use offsetSIGNED
                                for (int x = 0; x < len; x++)
                                    uncompressed[pos + x] = uncompressed[offset + x];
                            } else {
                                if (offsetSIGNED + 259 >= 0 && offsetSIGNED + 259 < pos) {
                                    offsetSIGNED += 259;
                                    for (int x = 0; x < len; x++)
                                        uncompressed[pos + x] = uncompressed[offsetSIGNED + x];
                                } else if (offset > pos) {
                                    Console.WriteLine("3 Problem " + GT.ByteArrayToString(bOff, " ") + " (Signed: " + offsetSIGNED + ") at " + pos + " (for len: " + len + ")");
                                } else if (offset >= sizeun) {
                                    uncompressed[pos + 0] = bOff[0];
                                    uncompressed[pos + 1] = bOff[1];
                                    uncompressed[pos + 2] = (byte)(len - 4);

                                    for (int g = 3; g < len; g++) {
                                        uncompressed[pos + g] = 0xF5;
                                    }
                                } else {
                                    for (int x = 0; x < len; x++)
                                        uncompressed[pos + x] = uncompressed[offset + x];
                                }
                            }
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

    }
}

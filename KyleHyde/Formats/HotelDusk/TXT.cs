using GameTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KyleHyde.Formats.HotelDusk {
    public class TXT {
        public GTFS GTFS { get; private set; }
        private byte[] Raw;
        public string Text;

        public TXT(GTFS fs) {
            GTFS = fs;

            Encoding enc = Encoding.GetEncoding("Windows-1252"); //1252, or maybe iso-8859-1/28591
            if (fs.FilePath.Contains("jp", StringComparison.InvariantCultureIgnoreCase))
                enc = Encoding.GetEncoding("Shift_JIS"); //932
            else if (fs.FilePath.Contains("_frn", StringComparison.InvariantCultureIgnoreCase) && false) { //Future Korean override
                enc = Encoding.GetEncoding("ks_c_5601-1987"); //It's not this
            }

            bool test0003 = false;
            if(fs.Length > 4) {
                byte[] test = GT.ReadBytes(fs, 4, false);
                fs.Position = 0;

                if (test[0] == 0 || test[1] == 0 || test[2] == 0 || test[3] == 3) {
                    test0003 = true;
                    System.Diagnostics.Debug.WriteLine("0003");

                    int total = GT.ReadInt32(fs, 4, false);
                    for (int i = 0; i < total; i++) {
                        int offset = GT.ReadInt32(fs, 4, false);
                    }

                    long baseOffset = fs.Position;

                    int len = (int)(fs.Length - 1 - baseOffset);
                    Raw = new byte[len];
                    fs.Read(Raw, 0, len);
                }
            }

            if (Raw == null) {
                if (fs.Length == 0)
                    Raw = [];
                else
                    Raw = GT.ReadTextArray(fs, fs.Length, false);
            }

            //File.WriteAllBytes("KH_TXT_out.bin", Raw);

            byte[] inputC = Encoding.Convert(enc, Encoding.Unicode, Raw);
            string input = Encoding.Unicode.GetString(inputC);

            if (test0003) {
                input = input.Replace((char)0x00, '\n');
                input = input.Replace("%n0", "\n");

                if (false) {
                    input = input.Replace("%c0", "<white>");
                    input = input.Replace("%c1", "<blue>");
                    input = input.Replace("%c2", "<orange>");
                }
            }

            Text = input;

            /*
             * %c0 reset to white?
             * %c2 blue text
             * %c1 orange text
             * 
             * %r102_09 change right background to same name
             * %p0000#00 where # is a person number? 1=Hyde, 2=Dunning, 7=Helen
             */
        }
        }
    }
}
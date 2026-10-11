using GameTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KyleHyde.Formats.HotelDusk {
    public class TXT {
        public GTFS GTFS { get; private set; }
        public string Text;

        public TXT(GTFS fs) {
            GTFS = fs;

            Encoding encEN = Encoding.GetEncoding(28591); //28591 = iso-8859-1 = Western European (ISO)
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            Text = encEN.GetString(buffer);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameTools {
    public class GTFSW : GTFS {
        //Writable

        public GTFSW(string path, bool readAllBytes) : base(path, readAllBytes) { }
        public GTFSW(string path) : base(path) { }
        public GTFSW(byte[] source) : base(source) { }

        public void Modify(long offset, byte value) {
            bytes[offset] = value;
        }

        public void Modify(long offset, byte[] values) {
            values.CopyTo(this.bytes, offset);
            //for (int i = 0; i < values.Length; i++) {
                //bytes[offset + i] = values[i];
            //}
        }

        public void Modify(long offset, GTFSW source) {
            source.bytes.CopyTo(this.bytes, offset);
        }

        public void WriteByte(byte b) {
            this.bytes[this.Position] = b;
            this.Position++;
        }

        /*
        //Use GT.ReadByteAt
        public byte GetByte(long position) {
            return this.bytes[position];
        }
        */

    }
}

namespace GameTools {
    public class Track {
        public long ID, Offset, Length;

        public Track(long id, long offset, long length) {
            ID = id;
            Offset = offset;
            Length = length;
        }

        public override string ToString() {
            return ID + ": " + Offset + " (" + Length + ")";
        }
    }
}
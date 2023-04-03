using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameTools {
    public class GTFS {

        protected string file;
        protected byte[] bytes;
        protected long nextTrackID;
        protected List<Track> _listTrack;

        public string FilePath {  get { return file; } }
        public long Position;
        public int Length { get { return bytes.Length; } }

        public GTFS(string path, bool readAllBytes) {
            file = path;
            if(readAllBytes)
                bytes = File.ReadAllBytes(path);
            Position = 0;

            nextTrackID = 0;
            _listTrack = new List<Track>();
        }

        public GTFS(string path) : this(path, true) { }

        public GTFS(byte[] source) {
            file = "GTFSBytes.null";
            bytes = source;
            Position = 0;

            nextTrackID = 0;
            _listTrack = new List<Track>();
        }

        public void PrepareByteStream() {
            if(bytes == null)
                bytes = File.ReadAllBytes(file);
        }

        public virtual byte ReadByte() {
            return bytes[Position++];
        }

        public void AddTrack(long offset, long length) {
            _listTrack.Add(new Track(nextTrackID++, offset, length));
        }

        public List<Track> GetTracking() {
            return new List<Track>(_listTrack);
        }

        public List<Track> GetTrackingSections() {
            List<Track> sections = new List<Track>();
            List<Track> gaps = GetTrackingGaps();

            long previousID = 0;
            long previousOffset = 0;
            for (int i = 0; i < gaps.Count; i++) {
                if (previousOffset < gaps[i].Offset)
                    sections.Add(new Track(previousID++, previousOffset, gaps[i].Offset - previousOffset));
                previousOffset = gaps[i].Offset + gaps[i].Length;
            }

            return sections;
        }

        public List<Track> GetTrackingGaps() {
            List<Track> gaps = new List<Track>();
            List<Track> listordered = _listTrack.OrderBy(o => o.Offset).ToList();

            long previousID = 0;
            long previousOffset = 0;
            for(int i = 0; i < listordered.Count; i++) {
                if (previousOffset < listordered[i].Offset)
                    gaps.Add(new Track(previousID++, previousOffset, listordered[i].Offset - previousOffset));
                previousOffset = listordered[i].Offset + listordered[i].Length;
            }

            // Gap at the end
            if (previousOffset < this.Length)
                gaps.Add(new Track(previousID++, previousOffset, this.Length - previousOffset));

            return gaps;
        }

        public void Seek(long offset, SeekOrigin origin) {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else if (origin == SeekOrigin.End)
                Position = Length - offset;
        }

        public virtual void Read(byte[] buffer, int bufferoffset, int length) {
            for (int i = 0; i < length; i++)
                buffer[bufferoffset + i] = bytes[Position++];
        }

        public void WriteBytesToFile(string filename) {
            File.WriteAllBytes(filename, bytes);
        }

        public MemoryStream GetStream() {
            PrepareByteStream();
            return new MemoryStream(bytes);
        }
    }
}

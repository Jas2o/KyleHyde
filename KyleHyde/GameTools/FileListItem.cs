using System.IO;

namespace GameTools {
    public class FileListItem {

        public enum FileKind {
            Null,
            Real,
            Virtual
        }

        public string File { get; private set; }
        public string Relative { get; private set; }
        public string Ext { get; private set; }

        public GTFS GTFS { get; private set; }
        public FileKind Kind { get; private set; }

        public FileListItem(string file, string relativeTo) {
            File = file;

            if(File == "") {
                Kind = FileKind.Null;
                Relative = "";
                Ext = "";
            }
            else
            {
                Kind = FileKind.Real;
                Relative = Path.GetRelativePath(relativeTo, file);
                Ext = Path.GetExtension(file);

                GTFS = new GTFS(file, false);
            }
        }

        public FileListItem(string filename, FileListItem parent, long offset, long size) {
            Kind = FileKind.Virtual;
            File = filename;
            Relative = parent.Relative + "\\" + filename;
            Ext = Path.GetExtension(filename);
            GTFS = parent.GTFS.GetSub(offset, size);
        }
    }
}

using GameTools;
using System.IO;

namespace KyleHyde {
    public class FileListItem {

        public string File { get; private set; }
        public string Relative { get; private set; }
        public string Ext { get; private set; }

        public GTFS GTFS { get; private set; }

        public FileListItem(string file, string relativeTo) {
            File = file;

            Relative = Path.GetRelativePath(relativeTo, file);
            Ext = Path.GetExtension(file);

            GTFS = new GTFS(file, false);
        }
    }
}

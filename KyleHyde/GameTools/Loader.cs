using Microsoft.Win32;
using System.Collections.Generic;

namespace GameTools {
    public abstract class Loader {
        public string PackExtension;
        public string MassExtension;

        public abstract void Open(string filePath, bool export = false, bool useGTFSView = false);

        public abstract void OpenAllPaks(List<string> dirfiles);
        public abstract void MassConvert(List<string> dirfiles);
    }
}
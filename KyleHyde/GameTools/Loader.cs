using Microsoft.Win32;
using System.Collections.Generic;

namespace GameTools {
    public abstract class Loader {
        public string PackExtension;
        public string MassExtension;

        public abstract void Open(string filePath, bool export = false);
        public abstract object Open2(string filePath);

        public abstract void OpenAllPaks(List<string> dirfiles);
        public abstract void MassConvert(List<string> dirfiles);
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHyde {
    public class FileList {

        public List<FileListItem> ListDataRaw { get; set; }
        public ObservableCollection<FileListItem> ListData { get; set; }

        public FileList() {
            ListDataRaw = new List<FileListItem>();
            ListData = new ObservableCollection<FileListItem>();
        }

        public void Clear() {
            ListDataRaw.Clear();
            ListData.Clear();
        }

        public FileListItem Add(string file, string relativeTo) {
            FileListItem di = new FileListItem(file, relativeTo);
            App.Current.Dispatcher.Invoke(new Action(() => {
                ListDataRaw.Add(di);
                ListData.Add(di);
            }));
            return di;
        }

    }
}

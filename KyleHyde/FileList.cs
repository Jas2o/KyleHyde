using GameTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KyleHyde {
    public class FileList {

        public List<FileListItem> ListDataRaw { get; set; }
        public ObservableCollection<FileListItem> ListData { get; set; }
        public ObservableCollection<FileListItem> ListDataMTC { get; set; }

        public FileList() {
            ListDataRaw = new List<FileListItem>();
            ListData = new ObservableCollection<FileListItem>();
            ListDataMTC = new ObservableCollection<FileListItem>();
            ListDataMTC.Add(new FileListItem("", ""));
        }

        public void Clear() {
            ListDataRaw.Clear();
            ListData.Clear();
            ListDataMTC.Clear();
            ListDataMTC.Add(new FileListItem("", ""));
        }

        public FileListItem Add(string file, string relativeTo) {
            FileListItem di = new FileListItem(file, relativeTo);
            App.Current.Dispatcher.Invoke(new Action(() => {
                ListDataRaw.Add(di);
                ListData.Add(di);

                if (file.IndexOf(".mtc", StringComparison.OrdinalIgnoreCase) >= 0)
                    ListDataMTC.Add(di);
            }));
            return di;
        }

        public FileListItem AddVirtual(FileListItem parent, Pack packFile) {
            FileListItem di = new FileListItem(packFile.Filename, parent, packFile.Offset, packFile.Size);
            App.Current.Dispatcher.Invoke(new Action(() => {
                ListDataRaw.Add(di);
                ListData.Add(di);

                if (packFile.Filename.IndexOf(".mtc", StringComparison.OrdinalIgnoreCase) >= 0)
                    ListDataMTC.Add(di);
            }));
            return di;
        }
    }
}

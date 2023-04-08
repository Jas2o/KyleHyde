using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHyde {
    public class ColorCompareList {

        public List<ColorCompareListItem> ListDataRaw { get; set; }
        public ObservableCollection<ColorCompareListItem> ListData { get; set; }

        public ColorCompareList() {
            ListDataRaw = new List<ColorCompareListItem>();
            ListData = new ObservableCollection<ColorCompareListItem>();
        }

        public void Clear() {
            ListDataRaw.Clear();
            ListData.Clear();
        }

        public ColorCompareListItem Add(int x, int y, System.Drawing.Color left, System.Drawing.Color right) {
            ColorCompareListItem di = new ColorCompareListItem(x, y, left, right);
            App.Current.Dispatcher.Invoke(new Action(() => {
                ListDataRaw.Add(di);
                ListData.Add(di);
            }));
            return di;
        }

    }
}

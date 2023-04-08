using GameTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHyde
{
    public class ColorCompareListItem
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int LA { get; private set; }
        public int LR { get; private set; }
        public int LG { get; private set; }
        public int LB { get; private set; }
        public int RA { get; private set; }
        public int RR { get; private set; }
        public int RG { get; private set; }
        public int RB { get; private set; }

        public ColorCompareListItem(int x, int y, int la, int lr, int lg, int lb, int ra, int rr, int rg, int rb) {
            X = x;
            Y = y;
            LA = la;
            LR = lr;
            LG = lg;
            LB = lb;
            RA = ra;
            RR = rr;
            RG = rg;
            RB = rb;
        }

        public ColorCompareListItem(int x, int y, System.Drawing.Color left, System.Drawing.Color right) {
            X = x;
            Y = y;
            LA = left.A;
            LR = left.R;
            LG = left.G;
            LB = left.B;
            RA = right.A;
            RR = right.R;
            RG = right.G;
            RB = right.B;
        }
    }
}

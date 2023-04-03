using GameTools;
using System.Collections.Generic;
using System.Drawing;

namespace KyleHyde.Formats.LastWindow
{
    public class BPG
    {

        public int Width, Height;
        public Bitmap bitmap;

        private static Dictionary<string, BpgProfile> profiles = new Dictionary<string, BpgProfile>() {
            { "map_icon.bpg", new BpgProfile(32, 0, 8) },
            { "my_icon.bpg", new BpgProfile(64, 0, 8) },
            { "my_icon_80.bpg", new BpgProfile(64, 0, 8) },
            { "person_icon.bpg", new BpgProfile(32, 0, 8) },
            { "person_icon_80.bpg", new BpgProfile(32, 0, 8) },
            { "spot.bpg", new BpgProfile(64, 0, 8) },
            { "spot_a.bpg", new BpgProfile(64, 0, 8) },
            { "touch.bpg", new BpgProfile(16, 0, 8) },
            { "two_icon_80.bpg", new BpgProfile(64, 0, 8) },
            { "r114_opendoor.bpg", new BpgProfile(16, 0, 8) },

        };

        public BPG(GTFS fs, string name)
        {

            bool flip = false;
            byte[] magic = GT.ReadBytes(fs, 4, flip); //BPG1
            int paletteNum = GT.ReadInt16(fs, 2, flip);
            int unknown2 = GT.ReadInt16(fs, 2, flip); //Should be 8 ?
            Width = GT.ReadInt16(fs, 2, flip);
            Height = GT.ReadInt16(fs, 2, flip);

            int tileWidth = GT.ReadInt16(fs, 2, flip);
            int tileHeight = GT.ReadInt16(fs, 2, flip);

            if (profiles.ContainsKey(name))
            {
                BpgProfile profile = profiles[name];

                if (profile.Width > 0) Width = profile.Width;
                if (profile.Height > 0) Height = profile.Height;
                if (profile.TileSize > 0) { tileWidth = profile.TileSize; tileHeight = profile.TileSize; }
            }

            if(Width == 0 || tileWidth == 0 || Height == 0 || tileHeight == 0)
                throw new System.Exception("Cannot divide by zero");

            int numTilesX = Width / tileWidth;
            int numTilesY = Height / tileHeight;

            Color[] palette = new Color[paletteNum];
            for (int i = 0; i < paletteNum; i++)
            {
                byte left = GT.ReadByte(fs);
                byte right = GT.ReadByte(fs);
                palette[i] = HotelDusk.FRM.Palette2Color(left, right);
            }

            bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int y = 0; y < numTilesY; y++)
            {
                for (int x = 0; x < numTilesX; x++)
                {
                    for (int ty = 0; ty < tileHeight; ty++)
                    {
                        for (int tx = 0; tx < tileWidth; tx++)
                        {
                            byte lookup = GT.ReadByte(fs);
                            bitmap.SetPixel(x * tileWidth + tx, y * tileHeight + ty, palette[lookup]);
                        }
                    }
                }
            }

        }

    }

    public class BpgProfile
    {
        public int Width;
        public int Height;
        public int TileSize;

        public BpgProfile(int Width, int Height, int TileSize)
        {
            this.Width = Width;
            this.Height = Height;
            this.TileSize = TileSize;
        }
    }
}

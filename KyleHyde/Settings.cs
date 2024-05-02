using Nucs.JsonSettings;

namespace KyleHyde
{
    public class Settings : JsonSettings {
        public override string FileName { get; set; } = "KyleHyde-config.json";
        public Settings() { }
        public Settings(string fileName) : base(fileName) { }

        public string DirHotelDusk { get; set; } = "";
        public string DirLastWindow { get; set; } = "";
        public bool WithMTC { get; set; } = false;
        public bool ShowSeparateLayers { get; set; } = false;
    }
}

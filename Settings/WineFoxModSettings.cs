namespace STS2_WineFox.Settings
{
    public sealed class WineFoxModSettings
    {
        public bool EventsEnabled { get; set; } = true;
        public bool PotionsEnabled { get; set; } = true;
        public bool FoodEnabled { get; set; } = true;
        public bool PublicRelicsEnabled { get; set; } = true;

        public bool FlatCardArtEnabled { get; set; }

        public string StandeeVariant { get; set; } = WineFoxStandeeVariants.New;

        public int MaterialHudOffsetX { get; set; }

        public int MaterialHudOffsetY { get; set; }

        public Dictionary<string, bool> FlatCardArtOverrides { get; set; } = [];
    }
}

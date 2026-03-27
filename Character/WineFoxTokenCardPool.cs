using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    public class WineFoxTokenCardPool : TypeListCardPoolModel
    {
        public override string Title => $"{Const.EnergyColorName} token";

        public override string EnergyColorName => "colorless";
        public override string CardFrameMaterialPath => "card_frame_colorless";

        public override Color DeckEntryCardColor => Colors.White;
        public override bool IsColorless => true;
    }
}

using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    public class MagicWineFoxCardPool : TypeListCardPoolModel
    {
        public override string Title => Const.EnergyColorName;

        public override string EnergyColorName => Const.EnergyColorName;
        public override string? BigEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string? TextEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string CardFrameMaterialPath => "card_frame_orange";

        public override Color DeckEntryCardColor => new("b66bff");
        public override Color EnergyOutlineColor => new("5f2f91");
        public override bool IsColorless => false;
    }
}

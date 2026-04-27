using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    /// <summary>
    ///     Shared pool for craft-table products and backpack craft upgrades (shown separately in the card library
    ///     compendium from other WineFox tokens).
    /// </summary>
    [RegisterSharedCardPool]
    public class WineFoxCraftingCardPool : TypeListCardPoolModel
    {
        public override string Title => $"{Const.EnergyColorName} craft";

        public override string EnergyColorName => "colorless";
        public override string CardFrameMaterialPath => "card_frame_colorless";

        public override Color DeckEntryCardColor => Colors.White;
        public override bool IsColorless => true;
    }
}

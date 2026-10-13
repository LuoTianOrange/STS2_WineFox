using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Character
{
    /// <summary>
    ///     魔法酒狐的法术修正符专用池。
    /// <para>
    ///         图鉴筛选器由 <c>Main.Initialize</c> 通过
    ///         <c>RegisterCardLibraryCompendiumSharedPoolFilter</c> 注册，不依赖此处的 <c>EnergyColorName</c>。
    /// </para>
    /// </summary>
    [RegisterSharedCardPool]
    public class MagicWineFoxSpellModifierCardPool : TypeListCardPoolModel
    {
        public override string Title => $"{Const.MagicEnergyColorName} spell modifier";

        public override string EnergyColorName => "colorless";

        public override string CardFrameMaterialPath => "card_frame_colorless";

        public override Color DeckEntryCardColor => new("b66bff");

        public override bool IsColorless => true;

        public override Material? PoolFrameMaterial =>
            MaterialUtils.CreateUnmodulatedHsvShaderMaterial();
    }
}

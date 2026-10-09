using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Character
{
    /// <summary>
    ///     魔法酒狐（MagicWineFox）专属的衍生牌池。
    /// <para>
    /// 存放法杖/法术体系里不进正常卡池、只由效果生成的牌，
    ///         例如【双重释放符】（由起始遗物【狐火杖】在战斗开始时预装）。
    /// </para>
    /// <para>
    ///         与 <c>WineFoxTokenCardPool</c>（普通酒狐的衍生池）分开，两个角色互不混池。
    /// </para>
    /// </summary>
    [RegisterSharedCardPool]
    public class MagicWineFoxTokenCardPool : TypeListCardPoolModel
    {
        public override string Title => $"{Const.EnergyColorName} magic token";

        public override string EnergyColorName => "colorless";
        public override string CardFrameMaterialPath => "card_frame_colorless";

        public override Color DeckEntryCardColor => new("b66bff");
        public override bool IsColorless => true;

    /// <summary>
        ///     同 <see cref="MagicWineFoxCardPool" />：法术令牌的边框贴图也不接受卡池色调制。
    /// </summary>
        public override Material? PoolFrameMaterial =>
            MaterialUtils.CreateUnmodulatedHsvShaderMaterial();
    }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     压缩施法：本回合牺牲法术槽，换取法杖内法术的伤害与防御提升。
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class CompressionCasting() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("SlotLoss", 2m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCompressionCasting);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var owner = Owner;
            if (owner?.Creature == null)
                return;

            var power = await PowerCmd.Apply<CompressionCastingPower>(
                choiceContext,
                owner.Creature,
                1m,
                owner.Creature,
                this);

            if (power != null)
                await power.ReduceSlots((int)DynamicVars["SlotLoss"].BaseValue);

            // 加成已经生效，但法杖里的法术数值需要主动刷新才会把 +4 算进卡面。
            MagicWineFoxSpellCmd.RefreshWandCardValues(MagicWineFoxSpellCmd.GetPower(owner));
        }

        protected override void OnUpgrade()
        {
            DynamicVars["SlotLoss"].UpgradeValueBy(-1m);
        }
    }
}

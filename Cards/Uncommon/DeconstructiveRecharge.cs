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
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class DeconstructiveRecharge() : WineFoxCard(
        0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Gain", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDeconstructiveRecharge);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var owner = Owner;
            if (owner?.Creature is not { } creature)
                return;

            var slots = await MagicWineFoxSpellCmd.EnsurePower(owner, choiceContext);
            slots?.AddSlotCapacityOffset(MagicWineFoxSpellSlotPower.TemporaryLossSource, -1);

            await PowerCmd.Apply<TemporarySlotLossPower>(
                choiceContext, creature, 1m, creature, this);

            await PlayerCmd.GainEnergy(DynamicVars["Gain"].BaseValue, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Gain"].UpgradeValueBy(1m);
        }
    }
}

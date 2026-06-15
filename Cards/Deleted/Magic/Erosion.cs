/*
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Erosion() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Stacks", 5m, card => WineFoxCardVarFactory.ChantScaledAmount(card, "Stacks")),
            WineFoxCardVarFactory.PowerAmountVar<ChantPower>(1m),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DemisePower>(),
            HoverTipFactory.FromPower<ChantPower>(),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardErosion);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var stacks = WineFoxCardVarFactory.ChantScaledAmount(this, "Stacks");
            await PowerCmd.Apply<DemisePower>(play.Target,
                stacks,
                owner,
                this);

            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                var disintegration = enemy.Powers.OfType<DemisePower>().FirstOrDefault();
                if (disintegration == null || disintegration.Amount <= 0m) continue;

                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    disintegration.Amount,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);
            }

            await PowerCmd.Apply<ChantPower>(owner, DynamicVars["ChantPower"].BaseValue, owner, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Stacks"].UpgradeValueBy(4m);
        }
    }
}
*/

namespace STS2_WineFox.Cards.Deleted
{
}

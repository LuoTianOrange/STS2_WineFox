using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class RecordPlayer() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("RitualPower", 2m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRecordPlayer);

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<DiamondPower>().Any(p => (decimal)p.Amount >= 1);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (play.IsFirstInSeries && !MaterialCmd.IsFreePlay(play))
            {
                var diamondPower = Owner.Creature.Powers
                    .OfType<DiamondPower>()
                    .FirstOrDefault(p => (decimal)p.Amount >= 1);
                if (diamondPower == null) return;
            }

            await MaterialCmd.LoseMaterial<DiamondPower>(this, 1m, play);
            await PowerCmd.Apply<RitualPower>(
                Owner.Creature, DynamicVars["RitualPower"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["RitualPower"].UpgradeValueBy(1m);
        }
    }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Deleted
{
    // [RegisterCard(typeof(WineFoxCardPool))]
    public class RecordPlayer() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1), new("RitualPower", 2m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRecordPlayer);

        protected override bool IsPlayable
        {
            get
            {
                var creature = Owner?.Creature;
                if (creature == null) return false;
                return creature.Powers.OfType<DiamondPower>().Any(p => (decimal)p.Amount >= 1);
            }
        }

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<RitualPower>()];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

            if (play.IsFirstInSeries && !MaterialCmd.IsFreePlay(play))
            {
                var hasDiamond = Owner.Creature.Powers
                    .OfType<DiamondPower>()
                    .Any(p => (decimal)p.Amount >= 1);
                if (!hasDiamond) return;
            }

            await MaterialCmd.LoseMaterial<DiamondPower>(this, 1m, play);
            await PowerCmd.Apply<RitualPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["RitualPower"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["RitualPower"].UpgradeValueBy(1m);
        }
    }
}

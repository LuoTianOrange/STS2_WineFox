using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class FoxBite() : WineFoxCard(
        2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Debuff", 4m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardFoxBite);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var owner = Owner?.Creature;
            if (owner == null) return;

            var target = play.Target;

            var amount = DynamicVars["Debuff"].BaseValue;

            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, amount, owner, this);

            await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, amount, owner, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Debuff"].UpgradeValueBy(3m);
        }
    }
}

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class ImprovisedWeapon() : WineFoxCard(
        1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(10m, ValueProp.Move)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardImprovisedWeapon);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            var owner = Owner;
            var handCards = PileType.Hand.GetPile(owner).Cards;
            if (handCards.Count == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_IMPROVISED_WEAPON_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);
            var selected = await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this);

            foreach (var card in selected)
                await CardPileCmd.Add(card, PileType.Exhaust);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4m);
        }
    }
}

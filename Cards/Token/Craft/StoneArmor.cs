using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.Craft
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class StoneArmor() : WineFoxCard(
        0, CardType.Power, CardRarity.Token, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Armor", 7m), new("StoneArmor", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardStoneArmor);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["Armor"].BaseValue, Owner.Creature, this);

            await PowerCmd.Apply<StoneArmorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["StoneArmor"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Armor"].UpgradeValueBy(2m);
        }
    }
}

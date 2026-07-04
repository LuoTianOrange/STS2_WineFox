using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Event
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class CraftingStorage() : WineFoxCard(
        1, CardType.Power, CardRarity.Event, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.CraftKeyword];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCraftingStorage);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            await PowerCmd.Apply<CraftingStoragePower>(new ThrowingPlayerChoiceContext(), creature, 1m, creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

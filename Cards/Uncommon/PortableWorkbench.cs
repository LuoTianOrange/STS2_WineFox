using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class PortableWorkbench() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), ICraftingCard
    {
        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(7m, ValueProp.Move)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.CraftKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPortableWorkbench);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

            await CraftCmd.CraftIntoHand(choiceContext, this);

            await PowerCmd.Apply<PortableWorkbenchPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m); // 7 → 10
        }
    }
}

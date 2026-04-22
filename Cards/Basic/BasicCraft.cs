using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Ancient;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    [RegisterCard(typeof(WineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(WineFox))]
    [RegisterArchaicToothTranscendence(typeof(Forging))]
    public class BasicCraft() : WineFoxCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self),
        ICraftingCard
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft, WineFoxKeywords.Material];

        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(5, ValueProp.Move)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBasicCraft);

        protected override bool IsPlayable =>
            CraftCmd.CanCraftAny(Owner.Creature);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            await CraftCmd.CraftIntoHand(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

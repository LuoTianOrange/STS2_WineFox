using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    [RegisterCard(typeof(WineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(WineFox), 4)]
    public class WineFoxDefend() : WineFoxCard(1,
        CardType.Skill, CardRarity.Basic,
        TargetType.Self)
    {
        public override bool GainsBlock => true;
        protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
        protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWineFoxDefend);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(3m);
        }
    }
}

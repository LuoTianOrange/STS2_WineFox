/*
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class ManaSurge() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("ChantPower", 2m, card =>
            {
                var raw = WineFoxCardVarFactory.ChantScaledAmount(card, "ChantPower");
                var cap = card?.IsUpgraded == true ? 6m : 4m;
                return Math.Min(cap, raw);
            }),
            new CardsVar(2),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChantPower>()];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, WineFoxKeywords.MagicKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardManaSurge);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;
            var chantGain = WineFoxCardVarFactory.ChantScaledAmount(this, "ChantPower");
            var cap = IsUpgraded ? 6m : 4m;
            chantGain = Math.Min(cap, chantGain);

            await PowerCmd.Apply<ChantPower>(choiceContext, creature, chantGain, creature, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
*/

namespace STS2_WineFox.Cards.Deleted.Magic
{
}

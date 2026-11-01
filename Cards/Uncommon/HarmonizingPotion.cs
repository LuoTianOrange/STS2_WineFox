using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class HarmonizingPotion() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IMagicWineFoxSpellCard
    {
        public override SpellTraits Traits =>
            new()
            {
                AttackSpell = false,
                SupportSpell = true,
                TargetSelf = true,
                UsesLimit = 3
            };

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            .. base.CanonicalKeywords,
            CardKeyword.Exhaust,
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHarmonizingPotion);

        public override string SpellIconPath => Const.Paths.SpellIconHarmonizingPotion;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<MegaCrit.Sts2.Core.Models.Cards.Wish>(),
        ];

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature?.CombatState is not { } combatState)
                return;

            var wish = combatState.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Wish>(owner);

            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(wish, PileType.Hand, owner));
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
            await ConsumeUseAfterLoad(choiceContext);
        }

        protected override void OnUpgrade()
        {
        }
    }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class SuppliesReplenishment() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self), IMagicWineFoxSpellCard
    {
        public override SpellTraits Traits =>
            new()
            {
                AttackSpell = false,
                SupportSpell = true,
                TargetSelf = true,
                UsesLimit = 3
            };

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSuppliesReplenishment);

        public override string SpellIconPath => Const.Paths.SpellIconSuppliesReplenishment;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner)
                return;

            await CardPileCmd.Draw(context.ChoiceContext, 1, owner);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
            await ConsumeUseAfterLoad(choiceContext);

            if (IsUpgraded && Owner is { } owner)
                await CardPileCmd.Draw(choiceContext, 1, owner);
        }

        protected override void OnUpgrade()
        {
        }
    }
}

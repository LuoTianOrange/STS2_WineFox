using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class RuneSiphon() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Copies", 1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRuneSiphon);

        public override string SpellIconPath => Const.Paths.SpellIconRuneSiphon;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true, SupportSpell = true };

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<EnergyNextTurnPower>(),
        ];

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);

            if (Owner is not { } owner || owner.Creature is not { } creature)
                return;

            await PowerCmd.Apply<EnergyNextTurnPower>(
                choiceContext,
                creature,
                1m,
                creature,
                this);

            var copies = (int)DynamicVars["Copies"].BaseValue;
            for (var i = 0; i < copies; i++)
                await SpellModifierFactory.ChooseOneOfRandomToHand(
                    choiceContext, owner, Rarity, exactRarity: Rarity);

            await CardPileCmd.Draw(choiceContext, 1, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Copies"].UpgradeValueBy(1m);
        }
    }
}

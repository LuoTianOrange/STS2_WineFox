using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
            new("Copies", 2m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRuneSiphon);

        public override string SpellIconPath => Const.Paths.SpellIconRuneSiphon;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true, SupportSpell = true };

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner)
                return;

            await CardPileCmd.Draw(context.ChoiceContext, 1, owner);

            MagicWineFoxSpellCmd.GetPower(owner)?.QueueSpellModifier(
                new(
                    PendingSpellModifierMode.RandomChoice,
                    Rarity,
                    (int)DynamicVars["Copies"].BaseValue));
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Copies"].UpgradeValueBy(1m);
        }
    }
}

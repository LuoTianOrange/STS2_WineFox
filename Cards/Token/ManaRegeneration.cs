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

namespace STS2_WineFox.Cards.Token
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class ManaRegeneration() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(1)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            [.. base.CanonicalKeywords, CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardManaRegeneration);

        public override string SpellIconPath => Const.Paths.SpellIconManaRegeneration;

        public override SpellTraits Traits =>
            new() { AttackSpell = false, SupportSpell = true, TargetSelf = true };

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner)
                return;

            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, owner);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
        }
    }
}

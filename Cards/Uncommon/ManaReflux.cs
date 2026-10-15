using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ManaReflux() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Bonus", 0m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardManaReflux);

        public override string SpellIconPath => Const.Paths.SpellIconManaReflux;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true, SupportSpell = true };

        public Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner)
                return Task.CompletedTask;

            var power = MagicWineFoxSpellCmd.GetPower(owner);
            if (power == null)
                return Task.CompletedTask;

            power.ManaRefluxPending = true;
            power.ManaRefluxBonus = (int)DynamicVars["Bonus"].BaseValue;

            return Task.CompletedTask;
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Bonus"].UpgradeValueBy(1m);
        }
    }
}

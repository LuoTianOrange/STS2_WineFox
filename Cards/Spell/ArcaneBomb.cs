using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ArcaneBomb() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardArcaneBomb);

        public override string SpellIconPath => Const.Paths.SpellIconArcaneBomb;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }
        
        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature?.CombatState is not { } combatState)
                return;

            await DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                .FromCard(context.SourceCard, null)
                .TargetingAllOpponents(combatState)
                .Execute(context.ChoiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}

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
    public class EmergencyProcedure() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(7m, ValueProp.Move),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEmergencyProcedure);

        public override string SpellIconPath => Const.Paths.SpellIconEmergencyProcedure;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            await DealSpellDamage(context, DynamicVars.Damage.BaseValue);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);

            if (Owner is not { } owner)
                return;

            await CardPileCmd.Draw(choiceContext, 1, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}

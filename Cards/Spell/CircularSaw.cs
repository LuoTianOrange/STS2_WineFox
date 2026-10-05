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
    public class CircularSaw() : MagicWineFoxSpellCard(
        2, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(20m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        public override string SpellIconPath => Const.Paths.SpellIconCircularSaw;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            if (owner?.Creature == null)
                return;

            var commands = await DealSpellDamage(context, DynamicVars.Damage.BaseValue);

            var overkill = commands
                .SelectMany(command => command.Results)
                .SelectMany(hits => hits)
                .Sum(result => result.OverkillDamage);

            Main.Logger.Info($"[CircularSaw] 结算完成，溢出伤害={overkill}");

            if (overkill <= 0m)
                return;

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                owner.Creature,
                overkill,
                ValueProp.Unpowered,
                null,
                null);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(6m);
        }
    }
}

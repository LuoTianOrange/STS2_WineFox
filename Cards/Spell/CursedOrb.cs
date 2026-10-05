using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class CursedOrb() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(2m, ValueProp.Move),
            new("Calamity", 13m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCursedOrb);

        public override string SpellIconPath => Const.Paths.SpellIconCursedOrb;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;
        
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DoomPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }
        
        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            if (owner == null)
                return;

            await DealSpellDamage(context, DynamicVars.Damage.BaseValue);

            // 灾厄的施加目标跟随伤害目标：带【穿刺魔弹】这类全体修正时对全体生效。
            var targets = new List<Creature>();
            if (context.TargetsAllEnemies)
            {
                if (owner.Creature?.CombatState is { } combatState)
                    targets.AddRange(combatState.HittableEnemies);
            }
            else if (ResolveSpellTarget(context) is { } single)
            {
                targets.Add(single);
            }

            foreach (var enemy in targets)
            {
                await PowerCmd.Apply<DoomPower>(
                    new ThrowingPlayerChoiceContext(),
                    enemy,
                    DynamicVars["Calamity"].BaseValue,
                    owner.Creature,
                    context.SourceCard);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Calamity"].BaseValue += 4m;
        }
    }
}

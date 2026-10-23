using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class PureShield() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(14m, ValueProp.Move),
            new("BlockLossPerModifier", 4m),
        ];

        public override bool GainsBlock => true;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPureShield);

        public override string SpellIconPath => Const.Paths.SpellIconPureShield;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true };

        public override decimal PreviewBlock
        {
            get
            {
                var penalty = DynamicVars["BlockLossPerModifier"].BaseValue * WandModifierCount;
                var afterPenalty = Math.Max(0m, DynamicVars.Block.BaseValue - penalty);

                return CombatState == null
                    ? afterPenalty
                    : MagicBlock.Resolve(this, afterPenalty);
            }
        }

        private int WandModifierCount
        {
            get
            {
                var slots = Owner?.Creature?.CombatState == null
                    ? null
                    : MagicWineFoxSpellCmd.GetPower(Owner);

                return slots?.Slots.Count(slot =>
                    slot?.Card is IMagicWineFoxSpellModifierCard) ?? 0;
            }
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner?.Creature;
            if (owner == null)
                return;

            var penalty = DynamicVars["BlockLossPerModifier"].BaseValue * context.WandModifierCount;
            var afterPenalty = Math.Max(0m, DynamicVars.Block.BaseValue - penalty);
            var block = context.BlockWithModifiers(afterPenalty);

            if (block <= 0m)
                return;

            await CreatureCmd.GainBlock(owner, block, ValueProp.Unpowered, null);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(4);
        }
    }
}

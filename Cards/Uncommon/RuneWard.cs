using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class RuneWard() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(5m, ValueProp.Move),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRuneWard);

        public override string SpellIconPath => Const.Paths.SpellIconRuneWard;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true };

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature is not { } creature)
                return;

            await CreatureCmd.GainBlock(
                creature, context.BlockWithModifiers(DynamicVars.Block.BaseValue), ValueProp.Move, null);

            MagicWineFoxSpellCmd.GetPower(owner)?.QueueSpellModifier(
                new(PendingSpellModifierMode.RandomChoiceWithDiscount, Rarity));
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m);
        }
    }
}

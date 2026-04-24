using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Bucket() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(8m, ValueProp.Move), new IntVar("Weak", 2m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<WeakPower>()];
        
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBucket);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;

            if (creature.CombatState is not { } combatState) return;

            var blockAmount = DynamicVars.Block;
            var weakAmount = DynamicVars["Weak"].BaseValue;

            await CreatureCmd.GainBlock(creature, blockAmount, play);

            await PowerCmd.Apply<BlockNextTurnPower>(new ThrowingPlayerChoiceContext(), creature, blockAmount.BaseValue, creature, this);

            foreach (var ally in combatState.GetTeammatesOf(creature).Where(c => c.IsAlive))
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), ally, weakAmount, creature, this);

            foreach (var enemy in combatState.Enemies.Where(e => e.IsAlive))
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), enemy, weakAmount, creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m); // 8 → 11
        }
    }
}

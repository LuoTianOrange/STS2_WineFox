using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class SpiritFoxForm() : WineFoxCard(
        3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Slow", 1m)];


        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ArtifactPower>(),
            HoverTipFactory.FromPower<SlowPower>(),
        ];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSpiritFoxForm);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            if (creature.CombatState is not { } combatState) return;

            foreach (var enemy in combatState.Enemies.Where(e => e.IsAlive))
            {
                var artifact = enemy.Powers.OfType<ArtifactPower>().FirstOrDefault();
                if (artifact != null)
                    await PowerCmd.Remove(artifact);

                await PowerCmd.Apply<SlowPower>(enemy, DynamicVars["Slow"].BaseValue, creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}


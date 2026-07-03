using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BladeMaster() : WineFoxCard(
        0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(8m),
            new ExtraDamageVar(8m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(PlayedSwordTypeCount),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.SwordKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBladeMaster);

        protected override bool ShouldGlowGoldInternal => GetPlayedSwordTypeCount() > 0;

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (Owner.Creature.CombatState is not { } combatState) return;

            await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                .FromCard(this, play)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(2m); // 8 -> 10
            DynamicVars.ExtraDamage.UpgradeValueBy(2m); // 8 -> 10
        }

        private static decimal PlayedSwordTypeCount(CardModel card, Creature? _)
        {
            return card is BladeMaster bladeMaster ? bladeMaster.GetPlayedSwordTypeCount() : 0m;
        }

        private int GetPlayedSwordTypeCount()
        {
            if (Owner.Creature.CombatState == null)
                return 0;

            return CombatManager.Instance.History.CardPlaysStarted
                .Where(entry => entry.Actor == Owner.Creature
                                && entry.CardPlay.IsFirstInSeries
                                && IsSwordCard(entry.CardPlay.Card))
                .Select(entry => entry.CardPlay.Card.GetType())
                .Distinct()
                .Count();
        }

        private static bool IsSwordCard(CardModel card)
        {
            return card.IsSword() && card is not BladeMaster;
        }
    }
}

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Token.Craft;
using STS2_WineFox.Cards.Token.HellGift;
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
            [new DamageVar(60m, ValueProp.Move)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBladeMaster);

        protected override bool IsPlayable =>
            Owner.Creature.CombatState != null && GetPlayedSwordTypeCount() >= 4;

        protected override bool ShouldGlowGoldInternal => GetPlayedSwordTypeCount() >= 4;

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (Owner.Creature.CombatState is not { } combatState) return;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(15m); // 60 -> 75
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
            return card is WoodenSword or StoneSword or IronSword or DiamondSword or GoldenSword or NetheriteSword;
        }
    }
}




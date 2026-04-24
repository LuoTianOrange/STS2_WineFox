using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MaidSupport() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.AllAllies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Armor", 5m), new IntVar("Thorns", 2m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<PlatingPower>(),HoverTipFactory.FromPower<ThornsPower>()];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMaidSupport);

        public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            if (owner.Creature.CombatState is not { } combatState) return;

            var armorAmount = DynamicVars["Armor"].BaseValue;
            var thornsAmount = DynamicVars["Thorns"].BaseValue;

            var teammates = combatState.GetTeammatesOf(owner.Creature)
                .Where(c => c is { IsAlive: true, IsPlayer: true });

            foreach (var teammate in teammates)
            {
                await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), teammate, armorAmount, owner.Creature, this);
                await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), teammate, thornsAmount, owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Armor"].UpgradeValueBy(1m);
            DynamicVars["Thorns"].UpgradeValueBy(1m);
        }
    }
}

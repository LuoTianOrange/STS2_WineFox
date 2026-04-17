using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     石化术 - 1 cost Skill Uncommon.
    ///     获得 N 个圆石。使所有敌人失去等同于 N 的生命。
    ///     升级：N 由 4 变为 6。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class PetrificationSpell() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Stones", 4m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPetrificationSpell);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var stones = DynamicVars["Stones"].BaseValue;

            // Gain stones first (without stress multiplier so the HP loss is predictable)
            await MaterialCmd.GainMaterial<StonePower>(owner, stones, sourceCard: this, applyStress: false);

            // Enemies lose HP equal to stones gained from this card
            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    stones,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Stones"].UpgradeValueBy(2m);
        }
    }
}


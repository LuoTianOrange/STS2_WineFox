using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     石化术 - 1 cost Skill Uncommon.
    ///     获得 4+Chant 个圆石。所有敌人失去等同于你本回合获得圆石数量的生命。获得 1 点吟唱。
    ///     升级：基础圆石由 4 变为 6。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class PetrificationSpell() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Stones", 4m, card => WineFoxCardVarFactory.ChantScaledAmount(card, "Stones")),
            WineFoxCardVarFactory.PowerAmountVar<ChantPower>(1m),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.StoneKeyword, WineFoxKeywords.MagicKeyword];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChantPower>()];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPetrificationSpell);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var stones = WineFoxCardVarFactory.ChantScaledAmount(this, "Stones");
            await MaterialCmd.GainMaterial<StonePower>(this, stones);

            var gainedThisTurnByType = CraftCmd.GetMaterialGainedAmountsByTypeThisTurn(owner);
            var damageAmount = gainedThisTurnByType.TryGetValue(typeof(StonePower), out var gainedStonesThisTurn)
                ? gainedStonesThisTurn
                : 0m;

            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    damageAmount,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);
            }

            await PowerCmd.Apply<ChantPower>(owner, DynamicVars["ChantPower"].BaseValue, owner, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Stones"].UpgradeValueBy(2m);
        }
    }
}

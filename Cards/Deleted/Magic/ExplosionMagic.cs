/*
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     爆裂魔法 - 2 cost Skill Rare.
    ///     给予所有敌人 6（受吟唱影响）层烧伤 2 次。失去 3 点吟唱。
    ///     升级：变为 3 次。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class ExplosionMagic() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Burn", 6m, card => WineFoxCardVarFactory.ChantScaledAmount(card, "Burn")),
            ModCardVars.Int("Hits", 2m),
            new PowerVar<ChantPower>(-3m)
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BurningPower>(),
            HoverTipFactory.FromPower<ChantPower>(),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardExplosionMagic);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var burn = WineFoxCardVarFactory.ChantScaledAmount(this, "Burn");
            var hits = (int)DynamicVars["Hits"].BaseValue;
            for (var i = 0; i < hits; i++)
            {
                foreach (var enemy in combatState.HittableEnemies.ToList())
                {
                    await PowerCmd.Apply<BurningPower>(
                        new ThrowingPlayerChoiceContext(),
                        enemy,
                        burn,
                        owner,
                        this);
                }
            }

            await PowerCmd.Apply<ChantPower>(new ThrowingPlayerChoiceContext(), owner, DynamicVars["ChantPower"].BaseValue, owner, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Hits"].UpgradeValueBy(1m);
        }
    }
}
*/

namespace STS2_WineFox.Cards.Deleted.Magic
{
}

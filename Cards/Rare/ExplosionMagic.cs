using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     爆裂魔法 - 2 cost Skill Rare.
    ///     给予所有敌人 12 层灼烧。失去 3 敏捷。
    ///     升级：变为 15 层灼烧。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class ExplosionMagic() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Burn", 12m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BurningPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardExplosionMagic);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var burn = DynamicVars["Burn"].BaseValue;
            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                await PowerCmd.Apply<BurningPower>(enemy, burn, owner, this);
            }

            await PowerCmd.Apply<DexterityPower>(owner, -3m, owner, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Burn"].UpgradeValueBy(3m);
        }
    }
}

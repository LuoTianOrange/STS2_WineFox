using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     爆裂魔法 - 2 cost Skill Rare.
    ///     所有敌人失去 32 点生命。失去 3 敏捷。
    ///     升级：变为 40 点。
    /// </summary>
    public class ExplosionMagic() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Damage", 32m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DexterityPower>(),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardExplosionMagic);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var damage = DynamicVars["Damage"].BaseValue;
            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    damage,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);
            }

            await PowerCmd.Apply<DexterityPower>(owner, -3m, owner, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Damage"].UpgradeValueBy(8m);
        }
    }
}


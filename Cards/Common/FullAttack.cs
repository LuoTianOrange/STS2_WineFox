using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class FullAttack() : WineFoxCard(2, CardType.Attack, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(4m, ValueProp.Move)];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardFullAttack,
            Const.Paths.CardFullAttack);

        protected override bool IsPlayable
        {
            get
            {
                var wood = Owner.Creature.Powers.OfType<WoodPower>().FirstOrDefault()?.Amount ?? 0;
                var stone = Owner.Creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
                return wood + stone > 0;
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            // 记录当前资源数量
            var woodPower = owner.Powers.OfType<WoodPower>().FirstOrDefault();
            var stonePower = owner.Powers.OfType<StonePower>().FirstOrDefault();
            var woodAmount = woodPower?.Amount ?? 0;
            var stoneAmount = stonePower?.Amount ?? 0;
            var totalHits = woodAmount + stoneAmount;

            if (totalHits <= 0) return;

            // 消耗所有资源（applier 传 null，跳过 HandCrank 倍增钩子）
            if (woodPower != null && woodAmount > 0)
                await PowerCmd.ModifyAmount(woodPower, -(decimal)woodAmount, null, this);
            if (stonePower != null && stoneAmount > 0)
                await PowerCmd.ModifyAmount(stonePower, -(decimal)stoneAmount, null, this);

            // 每层资源对随机一名存活敌人造成伤害
            for (var i = 0; i < totalHits; i++)
            {
                var enemies = combatState.Enemies
                    .Where(e => e.IsAlive)
                    .ToList();

                if (enemies.Count == 0) break;

                var target = enemies[Random.Shared.Next(enemies.Count)];
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}

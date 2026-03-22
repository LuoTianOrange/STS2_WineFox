using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class MechanicalDrill() : WineFoxCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Iron, WineFoxKeywords.Stress];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Iron", 2m)];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardMechanicalDrill,
            Const.Paths.CardMechanicalDrill);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            // 在改变状态前先判断是否有应力
            var stressPower = Owner.Creature.Powers
                .OfType<StressPower>()
                .FirstOrDefault(p => (decimal)p.Amount > 0);

            // 获得 2 铁
            await WineFoxActions.GainMaterial<IronPower>(this, DynamicVars["Iron"].BaseValue);

            // 有应力则返还 2 费用
            if (stressPower != null)
            {
                Owner.PlayerCombatState?.Energy += 2;
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Iron"].UpgradeValueBy(1m); // 2 → 3
        }
    }
}
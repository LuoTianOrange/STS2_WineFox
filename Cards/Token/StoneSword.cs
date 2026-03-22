using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;          // ← StrengthPower
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class StoneSword() : WineFoxCard(
        0, CardType.Power, CardRarity.Token, TargetType.Self,
        showInCardLibrary: false, autoAdd: false)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Strength", 4m)];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardStoneSword,
            Const.Paths.CardStoneSword);
        
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Strength];
        
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            // ① 原生力量 +4（与游戏所有力量相关联动自动生效）
            await PowerCmd.Apply<StrengthPower>(
                Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);

            // ② 挂上倒计时（2 回合后各扣 2 力量）
            await PowerCmd.Apply<StoneSwordPower>(
                Owner.Creature, 2m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            // Token 卡无升级
        }
    }
}
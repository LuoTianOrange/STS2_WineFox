using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps; // ← StrengthPower
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class StoneSword() : WineFoxCard(
        0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy,
        showInCardLibrary: false, autoAdd: false)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(6m, ValueProp.Move), new("Strength", 4m)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardStoneSword,
            Const.Paths.CardStoneSword);
        
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Strength];
        
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var target = play.Target
                         ?? Owner.Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive);

            if (target != null)
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);

            await PowerCmd.Apply<StrengthPower>(
                Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<StoneSwordPower>(
                Owner.Creature, 2m, Owner.Creature, this);
        }


        protected override void OnUpgrade()
        {
            // Token 卡无升级
        }
    }
}
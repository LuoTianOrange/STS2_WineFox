using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon;

public class CrushingWheel() : WineFoxCard(
    3, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    // 已追踪进费用的合成次数（用于回合结束后撤销）
    private int _craftCountTracked;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(21m, ValueProp.Move)];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCrushingWheel);

    // 回合开始：撤销上一回合的降费，重置追踪值
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner?.Creature) return Task.CompletedTask;

        if (_craftCountTracked > 0)
            EnergyCost.AddThisCombat(_craftCountTracked); // 撤销降费

        _craftCountTracked = 0;
        // 如无任何 WineFoxPower 存活，此处兜底重置计数器
        WineFoxActions.MaterialConsumeCountThisTurn = 0;
        return Task.CompletedTask;
    }

    // 每次有卡打出后：检测是否有新的合成发生，有则降费
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return Task.CompletedTask;

        var newCrafts = WineFoxActions.MaterialConsumeCountThisTurn - _craftCountTracked;
        if (newCrafts <= 0) return Task.CompletedTask;

        EnergyCost.AddThisCombat(-newCrafts);
        _craftCountTracked = WineFoxActions.MaterialConsumeCountThisTurn;
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (Owner.Creature.CombatState is not { } combatState) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}

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
    protected override IEnumerable<string> RegisteredKeywordIds =>
        [WineFoxKeywords.Material];
    
    private int _craftCountTracked;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(21m, ValueProp.Move)];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCrushingWheel);
    
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner?.Creature) return Task.CompletedTask;

        if (_craftCountTracked > 0)
            EnergyCost.AddThisCombat(_craftCountTracked);

        _craftCountTracked = 0;

        WineFoxActions.MaterialConsumeCountThisTurn = 0;
        return Task.CompletedTask;
    }
    
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

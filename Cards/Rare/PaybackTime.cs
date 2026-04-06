using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare;

public class PaybackTime() : WineFoxCard(
    3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m,ValueProp.Move),
        new IntVar("BonusDamage", 2m),
        ModCardVars.Computed("TotalDamage", 2m, CalcTotalDamage),
    ];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPaybackTime);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

        var creature = Owner.Creature;
        if (creature.CombatState is null) return;

        var gained = CraftCmd.GetMaterialGainedAmountThisCombat(creature);
        var damage = DynamicVars.Damage.BaseValue + DynamicVars["BonusDamage"].BaseValue * gained;

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    private static decimal CalcTotalDamage(CardModel? card)
    {
        if (card == null) return 0m;
        if (!card.DynamicVars.TryGetValue("Damage", out var dmg)) return 0m;
        if (!card.DynamicVars.TryGetValue("BonusDamage", out var bonus)) return 0m;
        var creature = card._owner?.Creature;
        if (creature == null) return dmg.BaseValue;
        var gained = CraftCmd.GetMaterialGainedAmountThisCombat(creature);
        return dmg.BaseValue + bonus.BaseValue * gained;
    }
}

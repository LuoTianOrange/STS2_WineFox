using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common;

public class LightAssault() : WineFoxCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<string> RegisteredKeywordIds =>
        [WineFoxKeywords.Material];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ModCardVars.Computed("Damage", 14m, CalcDamage)
    ];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLightAssault);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

        var totalMaterials = Owner.Creature.Powers
            .OfType<MaterialPower>()
            .Sum(p => (decimal)p.Amount);
        var damage = Math.Max(0m, DynamicVars["Damage"].BaseValue - totalMaterials);

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Damage"].UpgradeValueBy(4m);
    }

    private static decimal CalcDamage(CardModel? card)
    {
        if (card == null) return 14m;
        if (!card.DynamicVars.TryGetValue("Damage", out var dynamicVar)) return 14m;

        var creature = card._owner?.Creature;
        if (creature == null) return dynamicVar.BaseValue;

        var totalMaterials = creature.Powers
            .OfType<MaterialPower>()
            .Sum(p => (decimal)p.Amount);

        return Math.Max(0m, dynamicVar.BaseValue - totalMaterials);
    }
}
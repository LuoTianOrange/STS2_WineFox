using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare;

public class MaidSupport() : WineFoxCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [ new("Armor", 5m), new IntVar("Thorns", 2m) ];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMaidSupport);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var owner = Owner;
        if (owner.Creature.CombatState is not { } combatState) return;

        var armorAmount = DynamicVars["Armor"].BaseValue;
        var thornsAmount = DynamicVars["Thorns"].BaseValue;

        var teammates = combatState.GetTeammatesOf(owner.Creature)
            .Where(c => c.IsAlive && c.IsPlayer);

        foreach (var teammate in teammates)
        {
            await PowerCmd.Apply<PlatingPower>(teammate, armorAmount, owner.Creature, this);
            await PowerCmd.Apply<ThornsPower>(teammate, thornsAmount, owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Armor"].UpgradeValueBy(1m);
        DynamicVars["Thorns"].UpgradeValueBy(1m);
    }
}
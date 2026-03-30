using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Enchantments;

public class FireAspect : WineFoxEnchantmentsPool
{
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Attack;

    // 如果你想在卡面显示数量可以改为 true 并提供 CanonicalVars
    public override bool ShowAmount => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card != Card)
            return;

        var ownerCreature = Owner;
        if (ownerCreature == null)
            return;

        const decimal burnStacks = 5m;

        var explicitTarget = cardPlay.Target;
        if (explicitTarget != null)
        {
            await PowerCmd.Apply<BurningPower>(explicitTarget, burnStacks, ownerCreature, this);
        }
        else
        {
            var combatState = ownerCreature.CombatState;
            if (combatState != null)
            {
                var enemies = combatState.HittableEnemies;
                if (enemies != null)
                {
                    foreach (var enemy in enemies)
                        await PowerCmd.Apply<BurningPower>(enemy, burnStacks, ownerCreature, this);
                }
            }
        }
    }
}
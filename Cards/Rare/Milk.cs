using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare;

public class Milk() : WineFoxCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMilk);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cardSource = this;
        var owner = cardSource.Owner;
        var creature = owner?.Creature;
        if (creature == null) return;

        if (owner == null) 
            return;
        
        await CreatureCmd.TriggerAnim(creature, "Cast", owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_flying_slash");

        var powersSnapshot = creature.Powers.ToList();

        foreach (var power in powersSnapshot)
        {

            if (IsUpgraded)
            {
                if (power.Type == PowerType.Debuff)
                {
                    await PowerCmd.Remove(power);
                }
            }
            else
            {
                // 未升级：移除 Buff 与 Debuff
                if (power.Type == PowerType.Debuff || power.Type == PowerType.Buff)
                {
                    await PowerCmd.Remove(power);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级仅改变 IsUpgraded 语义（无额外字段）
        // 若需显示变化（文本/提示），请更新本地化字符串
    }
}
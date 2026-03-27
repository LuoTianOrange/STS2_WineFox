using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Ancient;

public class NetheritePickaxe() : WineFoxCard(
    1, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNetheritePickaxe);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var creature = Owner?.Creature;
        if (creature == null)
            return;
        
        var appliedPower = await PowerCmd.Apply<NetheritePickaxePower>(creature, 1m, creature, this);
        if (appliedPower != null)
        {
            appliedPower.ExcludeCard(this);
        }

    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}

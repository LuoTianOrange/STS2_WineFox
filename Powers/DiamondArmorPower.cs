using System.Linq;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers;

public class DiamondArmorPower : WineFoxPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondArmorPowerIcon);

    public override bool ShouldClearBlock(Creature creature)
    {
        if (!ReferenceEquals(Owner, creature))
            return true;

        var plating = Owner.Powers.OfType<PlatingPower>().FirstOrDefault();
        var platingAmount = plating?.Amount ?? 0m;

        return platingAmount <= 0m;
    }

    public override Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (!ReferenceEquals(this, preventer))
            return Task.CompletedTask;
        Flash();
        return Task.CompletedTask;
    }
}
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using STS2_WineFox.Settings;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    [RegisterSharedRelicPool]
    public sealed class WineFoxSharedRelicPool : TypeListRelicPoolModel
    {
        public override string EnergyColorName => Const.EnergyColorName;
        public override string? BigEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string? TextEnergyIconPath => Const.Paths.EnergyIconCake;
        public override Color LabOutlineColor => WineFox.Color;

        public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
        {
            return WineFoxRuntimeSettings.PublicRelicsEnabled
                ? base.GetUnlockedRelics(unlockState)
                : [];
        }
    }
}

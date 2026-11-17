using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_WineFox.Settings;

namespace STS2_WineFox.Character
{
    public partial class WineFoxCreatureVisuals : NCreatureVisuals
    {
        public override void _Ready()
        {
            base._Ready();
            WineFoxStandeeSync.TryEnsurePublished();
            WineFoxStandee.ApplyVariantIfNeeded(this);
        }
    }
}

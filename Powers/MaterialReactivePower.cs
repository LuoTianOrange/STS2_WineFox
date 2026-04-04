using STS2_WineFox.Commands;

namespace STS2_WineFox.Powers
{
    public abstract class MaterialReactivePower : WineFoxPower, IMaterialEventListener
    {
        public virtual Task BeforeMaterialGain(MaterialGainEvent evt)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterMaterialGain(MaterialGainEvent evt)
        {
            return Task.CompletedTask;
        }

        public virtual Task BeforeMaterialConsume(MaterialConsumeEvent evt)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterMaterialConsume(MaterialConsumeEvent evt)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterMaterialResolved(MaterialResolvedEvent evt)
        {
            return Task.CompletedTask;
        }
    }
}

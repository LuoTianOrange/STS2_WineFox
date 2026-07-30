using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Relics
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class WineFoxPublicRelicAttribute : Attribute
    {
        public static bool IsDefined(RelicModel relic)
        {
            return relic.GetType().IsDefined(typeof(WineFoxPublicRelicAttribute), false);
        }
    }
}

using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace STS2_WineFox.Relics.Backpack
{
    public sealed class SophisticatedBackpackDescriptionVar(Func<string> descriptionFactory)
        : StringVar(SophisticatedBackpackEffects.DescriptionVar)
    {
        public void Refresh()
        {
            StringValue = descriptionFactory();
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(StringValue)
                ? descriptionFactory()
                : StringValue;
        }
    }
}

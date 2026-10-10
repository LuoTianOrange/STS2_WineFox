using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    /// <summary>
    ///     让法杖内法术的卡面数值显示Power + 法术修正符的合成值。
    ///     <para>
    ///         卡面描述由 <c>CardModel.GetDescriptionForPile</c> 渲染，数值从卡牌自己的
    ///         动态变量取（<c>{Damage:diff()}</c>，参数必须是变量对象，塞裸数字会打印占位符）。
    ///         因此这里在渲染前把 <c>Damage</c> 变量临时换成合成值，渲染后立刻还原：
    ///         既让卡面显示合成值，又完全不改动真实结算用的数值。
    ///     </para>
    ///     <para>
    ///         与 RitsuLib 的 <c>CardModelCapabilityPatches.DescriptionPatch</c> 同一目标；
    ///         它用 Transpiler，这里用 Prefix/Postfix + <c>__state</c> 完成同样的事。
    ///     </para>
    /// </summary>
    internal sealed class SpellCardDescriptionPreviewPatch : IPatchMethod
    {
        public static string PatchId => "winefox_spell_card_description_preview";
        public static string Description => "Swap winefox spell card damage to the modifier-adjusted value while rendering its description";
        public static bool IsCritical => false;

        /// <summary>
        ///     <c>GetDescriptionForPile</c> 有重载，只给方法名会因歧义导致补丁失败，
        ///     所以两个重载都显式指定参数类型：
        ///     <list type="number">
        ///         <item><c>(PileType, Creature)</c>——公开重载。</item>
        ///         <item>
        ///             <c>(PileType, DescriptionPreviewType, Creature)</c>——卡牌实际渲染走的那个；
        ///             <c>DescriptionPreviewType</c> 是 <see cref="CardModel" /> 的私有嵌套枚举，
        ///             因此只能反射取（RitsuLib 的 <c>CardDescriptionPatchTarget</c> 同样如此）。
        ///             取不到时由 <c>ignoreIfMissing</c> 安全跳过。
        ///         </item>
        ///     </list>
        /// </summary>
        public static ModPatchTarget[] GetTargets()
        {
            var targets = new List<ModPatchTarget>
            {
                new(typeof(CardModel), nameof(CardModel.GetDescriptionForPile),
                    [typeof(PileType), typeof(Creature)], true),
            };

            var previewType = typeof(CardModel)
                .GetNestedType("DescriptionPreviewType", BindingFlags.NonPublic);

            if (previewType != null)
            {
                targets.Add(new ModPatchTarget(
                    typeof(CardModel),
                    nameof(CardModel.GetDescriptionForPile),
                    [typeof(PileType), previewType, typeof(Creature)],
                    true));
            }

            return [.. targets];
        }

        /// <summary>
        ///     Harmony 只认一个名为 <c>__state</c> 的状态参数，多写一个（如 <c>__blockState</c>）
        ///     会被当成原方法参数去匹配，导致 "Parameter ... does not contain a valid index" 补丁失败。
        ///     因此把伤害与格挡两个还原值打包进同一个 <c>__state</c>。
        /// </summary>
        public static void Prefix(CardModel __instance, out (decimal Damage, decimal Block) __state)
        {
            __state = (-1m, -1m);

            if (__instance is not MagicWineFoxSpellCard spell)
                return;

            var damage = -1m;
            var block = -1m;

            if (spell.PreviewDamageOverride is { } value)
                damage = spell.BeginDisplayOverride(value);

            // 只有带格挡变量的法术才做格挡覆盖：否则 DynamicVars["Block"] 会抛异常。
            if (spell.HasBlockVar && spell.PreviewBlockOverride is { } blockValue)
                block = spell.BeginBlockDisplayOverride(blockValue);

            __state = (damage, block);
        }

        public static void Postfix(CardModel __instance, (decimal Damage, decimal Block) __state)
        {
            if (__instance is not MagicWineFoxSpellCard spell)
                return;

            if (__state.Damage >= 0m)
                spell.EndDisplayOverride(__state.Damage);

            if (__state.Block >= 0m)
                spell.EndBlockDisplayOverride(__state.Block);
        }
    }
}

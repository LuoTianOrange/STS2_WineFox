namespace STS2_WineFox.Cards
{
    /// <summary>
    ///     标记接口：所有会触发 <c>CraftCmd</c> 分支（即从合成表里取材料产出卡）的卡 / Power 都应该实现它。
    ///     用于让顶栏合成表按钮在非 WineFox 角色持有合成类卡片时也能显示。
    /// </summary>
    /// <remarks>
    ///     契约只是一个纯粹的 marker —— 不需要任何额外成员。被标记的类型一般满足：
    ///     <list type="bullet">
    ///         <item>卡效果在 <c>OnPlay</c> / 派生命令中调用 <c>CraftCmd.CraftIntoHand(...)</c></item>
    ///         <item>或是会随回合触发合成效果的 <c>Power</c>（如 <see cref="Powers.AutoCrafterPower" />）</item>
    ///     </list>
    ///     跨 Mod 合作时，其他 Mod 只要实现本接口即可让自己的合成卡被合成表按钮识别。
    /// </remarks>
    public interface ICraftingCard
    {
    }
}

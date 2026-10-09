using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards
{
    public abstract class WineFoxCard(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true) : ModCardTemplate(baseCost, type, rarity, target, showInCardLibrary)
    {
        public sealed override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (Owner == player)
                CraftCmd.ObserveTurnStarted(choiceContext, player);

            return Task.CompletedTask;
        }

        protected override void AddExtraArgsToDescription(LocString description)
        {
            base.AddExtraArgsToDescription(description);
            var suffix = "";
            if (Pile == null
                && CraftRecipeRegistry.TryGetRecipe(GetType(), out var recipe)
                && recipe.Costs.Length > 0)
                suffix = CraftRecipeFormatting.ToPreviewSuffix(recipe);

            description.Add("CraftRecipeSuffix", suffix);
        }

        protected static CardAssetProfile Art(string portraitPath)
        {
            return new(portraitPath, portraitPath);
        }

        /// <summary>
        ///     魔法酒狐的卡牌边框。基类由两个角色共用，所以按卡池判定；
        ///     卡牌级覆盖优先于卡池级的 <c>CardFrameMaterialPath</c>。
        ///     原版只按卡牌类型区分边框贴图（与稀有度无关），故这里同样按类型分发。
        /// </summary>
        public override string? CustomFramePath => IsMagicWineFox ? FramePathFor(Type) : null;

        /// <summary>
        ///     魔法酒狐的肖像框（卡图外圈那道边框）。
        ///     <para>
        ///         判据与边框一致。素材先复用 plaque 那三张，确认位置后再另出专用图。
        ///     </para>
        /// </summary>
        public override string? CustomPortraitBorderPath =>
            IsMagicWineFox ? PortraitBorderPathFor(Type) : null;

        /// <summary>
        ///     魔法酒狐的标题横幅。判据与边框一致：基类由两个角色共用，所以按卡池判定。
        /// </summary>
        public override string? CustomBannerTexturePath =>
            IsMagicWineFox ? Const.Paths.CardFrameMagicBanner : null;

        private bool IsMagicWineFox
        {
            get
            {
                try
                {
                    return Pool is MagicWineFoxCardPool or MagicWineFoxTokenCardPool;
                }
                catch (InvalidProgramException)
                {
                    return false;
                }
            }
        }

        private static string FramePathFor(CardType type)
        {
            return type switch
            {
                CardType.Attack => Const.Paths.CardFrameMagicAttack,
                CardType.Power => Const.Paths.CardFrameMagicPower,
                _ => Const.Paths.CardFrameMagicSkill,
            };
        }

        /// <summary>
        ///     肖像框素材的类型映射。
        /// </summary>
        private static string PortraitBorderPathFor(CardType type)
        {
            return type switch
            {
                CardType.Attack => Const.Paths.CardPlaqueMagicAttack,
                CardType.Power => Const.Paths.CardPlaqueMagicPower,
                _ => Const.Paths.CardPlaqueMagicSkill,
            };
        }
    }
}

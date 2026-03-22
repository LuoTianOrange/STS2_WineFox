using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    public class BaseCraft() : WineFoxCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardBaseCraft,
            Const.Paths.CardBaseCraft
        );

        // 至少有一个配方可以合成时才可打出
        protected override bool IsPlayable =>
            CraftRecipeRegistry.All.Any(r => r.CanCraft(Owner.Creature));

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (Owner.Creature.CombatState is not { } combatState)
                return;

            // 只保留当前材料能满足的配方
            var craftable = CraftRecipeRegistry.All
                .Where(r => r.CanCraft(Owner.Creature))
                .ToList();

            // 按配方创建对应的卡牌供选择
            var options = craftable
                .Select(r => r.Factory(combatState, Owner))
                .ToList();

            var prefs = new CardSelectorPrefs(
                new LocString("cards", "STS2_WINE_FOX_CHOOSE_CRAFT"),
                1);

            await choiceContext.SignalPlayerChoiceBegun(PlayerChoiceOptions.None);
            NPlayerHand.Instance?.CancelAllCardPlay();

            var screen = NSimpleCardSelectScreen.Create(options, prefs);
            NOverlayStack.Instance!.Push(screen);

            var chosen = (await screen.CardsSelected()).FirstOrDefault();

            await choiceContext.SignalPlayerChoiceEnded();

            if (chosen == null) return;

            // 找到选中卡对应的配方，消耗对应材料
            var chosenIndex = options.IndexOf(chosen);
            await craftable[chosenIndex].ConsumeMaterials(this);

            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, false);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

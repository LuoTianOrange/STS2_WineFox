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
using STS2_WineFox.Cards.Token;
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

        protected override bool IsPlayable
        {
            get
            {
                var woodAmount = Owner.Creature.Powers.OfType<WoodPower>()
                    .FirstOrDefault()?.Amount ?? 0m;
                var stoneAmount = Owner.Creature.Powers.OfType<StonePower>()
                    .FirstOrDefault()?.Amount ?? 0m;

                return woodAmount >= 2m && stoneAmount >= 2m;
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await WineFoxActions.LostMaterial<WoodPower, StonePower>(this, 2m, 2m);

            if (Owner.Creature.CombatState is not { } combatState)
                return;

            var options = new List<CardModel>
            {
                combatState.CreateCard<StonePickaxe>(Owner),
                combatState.CreateCard<StoneSword>(Owner),
                combatState.CreateCard<IronArmor>(Owner)
            };

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

            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, false);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

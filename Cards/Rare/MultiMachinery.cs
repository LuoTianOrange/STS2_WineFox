using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2_WineFox.Cards.Token.MultiMachinery;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MultiMachinery() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.StressKeyword];

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<StressPower>().Any(p => p.Amount >= 2m);

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMultiMachinery);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<FineProcessing>(IsUpgraded),
            HoverTipFactory.FromCard<RawMaterialProcessing>(IsUpgraded),
            HoverTipFactory.FromCard<ScrapRecycling>(IsUpgraded),
            HoverTipFactory.FromCard<Reforge>(IsUpgraded),
        ];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;
            var combatState = creature.CombatState;
            if (combatState == null) return;

            if (!await StressCmd.ConsumeOne(creature, this)) return;
            if (!await StressCmd.ConsumeOne(creature, this)) return;

            List<CardModel> options =
            [
                combatState.CreateCard<FineProcessing>(owner),
                combatState.CreateCard<RawMaterialProcessing>(owner),
                combatState.CreateCard<ScrapRecycling>(owner),
                combatState.CreateCard<Reforge>(owner),
            ];

            if (IsUpgraded)
                CardCmd.Upgrade(options, CardPreviewStyle.None);

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_MULTI_MACHINERY_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);
            var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, owner, prefs))
                .FirstOrDefault();

            if (chosen is IMultiMachineryChoice choice)
                await choice.Apply(choiceContext, this);

            await Task.Yield();
            foreach (var option in options.Where(option => !ReferenceEquals(option, chosen)))
                option.RemoveFromState();
        }

        protected override void OnUpgrade()
        {
        }
    }
}

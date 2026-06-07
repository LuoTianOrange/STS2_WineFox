using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2_WineFox.Character;
using STS2_WineFox.Utils;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class PressWToThink() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPressWToThink);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Replay", 1m), new IntVar("Exhaust", 1m)];

        private void PlayPonderFX()
        {
            var nCreature = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
            if (nCreature != null)
            {
                var fxPos = nCreature.VfxSpawnPosition + new Vector2(0f, -150f);
                VFXUtil.PlaySimpleBack(Const.Paths.HoldWToPonderVfx, fxPos, 5f);
                VFXUtil.PlaySFXSimple(Const.Audio.IdleSet1);
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var drawPileCards = PileType.Draw.GetPile(owner).Cards;
            var topCards = drawPileCards.Take(3).ToList();

            if (topCards.Count == 0) return;

            var promptLocKey = new LocString("cards", "STS2_WINE_FOX_CARD_PRESS_W_TO_THINK_CHOOSE");
            var prefs = new CardSelectorPrefs(promptLocKey, 1);

            

            IReadOnlyList<CardModel> options = topCards;
            var selectedList = await CardSelectCmd.FromSimpleGrid(choiceContext, options, owner, prefs);
            var chosen = selectedList.FirstOrDefault();
            PlayPonderFX(); //在选牌后播放特效，以免被挡住
            if (chosen == null)
                return;

            chosen.BaseReplayCount += DynamicVars["Replay"].IntValue;
            chosen.AddKeyword(CardKeyword.Exhaust);
            CardCmd.Preview(chosen);
            await CardPileCmd.Add(chosen, PileType.Hand);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

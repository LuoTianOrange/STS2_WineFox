using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class RuneRubbing() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Copies", 1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRuneRubbing);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var owner = Owner;
            if (owner == null)
                return;

            var slots = MagicWineFoxSpellCmd.GetPower(owner);
            var template = slots?.Slots
                .Where(s => s != null)
                .Select(s => s!.Card)
                .FirstOrDefault(c => c is IMagicWineFoxSpellModifierCard);
            if (template == null)
                return;

            var copies = (int)DynamicVars["Copies"].BaseValue;
            for (var i = 0; i < copies; i++)
            {
                var result = await CardPileCmd.AddGeneratedCardToCombat(
                    template.CreateClone(), PileType.Draw, owner, CardPilePosition.Top);
                CardCmd.PreviewCardPileAdd(result);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Copies"].UpgradeValueBy(1m);
        }
    }
}

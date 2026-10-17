using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class UnleashingPotential() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Slots", 2m),
            new CardsVar(2)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardUnleashingPotential);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner || owner.Creature is not { } creature)
                return;

            var gained = (int)DynamicVars["Slots"].BaseValue;

            await MagicWineFoxSpellCmd.EnsurePower(owner, choiceContext);

            await PowerCmd.Apply<TemporarySlotGainPower>(
                choiceContext,
                creature,
                gained,
                creature,
                this);

            await DrawSpells(choiceContext, owner, DynamicVars.Cards.IntValue);
        }

        private static async Task DrawSpells(PlayerChoiceContext choiceContext, Player owner, int count)
        {
            var drawPile = PileType.Draw.GetPile(owner);

            for (var i = 0; i < count; i++)
            {
                var spell = drawPile.Cards.FirstOrDefault(
                    card => card is MagicWineFoxSpellCard { IsLoadable: true });

                if (spell == null)
                    return;

                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(spell, PileType.Hand));
            }
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}

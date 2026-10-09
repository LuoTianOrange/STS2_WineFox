using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(MagicWineFox), 1)]
    public class QuickReload() : WineFoxCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuickReload);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner)
                return;

            await DrawSpells(choiceContext, owner, (int)DynamicVars.Cards.BaseValue);
        }
        
        private static async Task DrawSpells(
            PlayerChoiceContext choiceContext,
            Player owner,
            int count)
        {
            for (var i = 0; i < count; i++)
            {
                var spells = SpellsInDrawPile(owner);

                // 抽牌堆里没有法术牌：弃牌堆里还有就洗牌，否则退出。
                if (spells.Count == 0)
                {
                    if (!SpellsInDiscardPile(owner))
                        return;

                    await CardPileCmd.Shuffle(choiceContext, owner);
                    spells = SpellsInDrawPile(owner);

                    if (spells.Count == 0)
                        return;
                }

                var index = owner.RunState.Rng.CombatCardGeneration.NextInt(spells.Count);
                var spell = spells[index];
                spells.Remove(spell);

                await CardPileCmd.Add(spell, PileType.Hand, CardPilePosition.Top, null, false);
            }
        }

        private static List<CardModel> SpellsInDrawPile(Player owner)
        {
            return
            [
                .. PileType.Draw.GetPile(owner).Cards
                    .Where(card => card is Spell.MagicWineFoxSpellCard)
            ];
        }

        private static bool SpellsInDiscardPile(Player owner)
        {
            return PileType.Discard.GetPile(owner).Cards
                .Any(card => card is Spell.MagicWineFoxSpellCard);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}

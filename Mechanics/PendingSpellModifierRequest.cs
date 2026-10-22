using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Spell;

namespace STS2_WineFox.Mechanics
{
    public enum PendingSpellModifierMode
    {
        FromPool = 0,
        RandomChoice = 1,
        RandomChoiceWithDiscount = 2
    }

    /// <summary>
    ///     装填时登记的「选择法术修正」请求，实际选择推迟到下一个己方回合开始时结算。
    /// </summary>
    public sealed record PendingSpellModifierRequest(
        PendingSpellModifierMode Mode,
        CardRarity MaxRarity,
        int OptionCount = 3)
    {
        public async Task Resolve(PlayerChoiceContext choiceContext, Player owner)
        {
            switch (Mode)
            {
                case PendingSpellModifierMode.FromPool:
                    await SpellModifierFactory.ChooseOneFromPoolToHand(choiceContext, owner, MaxRarity);
                    break;

                case PendingSpellModifierMode.RandomChoice:
                    await SpellModifierFactory.ChooseOneOfRandomToHand(
                        choiceContext, owner, MaxRarity, OptionCount);
                    break;

                case PendingSpellModifierMode.RandomChoiceWithDiscount:
                    var modifier = await SpellModifierFactory.ChooseOneOfRandom(
                        choiceContext, owner, MaxRarity, OptionCount);

                    if (modifier == null)
                        return;

                    modifier.EnergyCost.AddThisCombat(-1);
                    CardCmd.PreviewCardPileAdd(
                        await CardPileCmd.AddGeneratedCardToCombat(modifier, PileType.Hand, owner));
                    break;
            }
        }
    }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     异界跨越 - 1 cost Power Uncommon.
    ///     每回合开始时，将你每张手牌的一张虚无消耗复制品加入到你的手牌。
    ///     升级：附加固有。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class OtherworldCrossing() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardOtherworldCrossing);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<OtherworldCrossingPower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}


using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     解放 - 2 cost Power Rare.
    ///     At end of turn, trigger each Retained card in hand
    ///     (auto-play a copy; original card keeps Retain and stays in hand).
    ///     升级：变为1费。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Liberation() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLiberation);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromKeyword(CardKeyword.Retain)];
        
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<LiberationPower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}


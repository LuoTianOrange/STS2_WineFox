using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ArcaneCapacitor() : WineFoxCard(
        2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardArcaneCapacitor);

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.ChargeKeyword];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature is not { } creature)
                return;

            var power = await PowerCmd.Apply<ArcaneCapacitorPower>(
                choiceContext, creature, 1m, creature, this);

            power?.Configure(IsUpgraded);
        }
    }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MagicOverloaded() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicOverloaded);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ChantPower>(),
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
        ];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            var chantPower = creature.Powers.OfType<ChantPower>().FirstOrDefault();
            var chantAmount = chantPower?.Amount ?? 0m;

            if (chantPower != null)
                await PowerCmd.Remove(chantPower);

            var convertedAmount = Math.Max(0m, chantAmount + (IsUpgraded ? 1m : 0m));
            if (convertedAmount <= 0m)
                return;

            await PowerCmd.Apply<StrengthPower>(creature, convertedAmount, creature, this);
            await PowerCmd.Apply<DexterityPower>(creature, convertedAmount, creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

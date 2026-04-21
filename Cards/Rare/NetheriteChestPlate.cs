using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NetheriteChestPlate() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Plating", 7m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<PlatingPower>()];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNetheriteChestPlate);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            await PowerCmd.Apply<PlatingPower>(creature, DynamicVars["Plating"].BaseValue, creature, this);
            await PowerCmd.Apply<NetheriteChestPlatePower>(creature, 1m, creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Plating"].UpgradeValueBy(2m);
        }
    }
}

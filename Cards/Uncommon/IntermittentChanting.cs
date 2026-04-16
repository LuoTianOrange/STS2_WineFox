using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     间隙咏唱 - 1 cost Power Uncommon.
    ///     获得 TrackingPower（每当你使敌人失去生命时，敌人获得 1 点格挡，你获得 2 点格挡）。
    ///     升级：你获得 3 点格挡。
    /// </summary>
    public class IntermittentChanting() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Block", 2m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardIntermittentChanting);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            await PowerCmd.Apply<TrackingPower>(creature, DynamicVars["Block"].BaseValue, creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(1m);
        }
    }
}


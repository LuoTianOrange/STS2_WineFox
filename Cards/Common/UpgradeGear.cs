using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class UpgradeGear() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(8m, ValueProp.Move),
            new IntVar("Count", 2m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardUpgradeGear);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;

            await CreatureCmd.GainBlock(creature, DynamicVars.Block, play);

            var drawPile = PileType.Draw.GetPile(owner).Cards;
            if (drawPile.Count == 0) return;

            var count = Math.Min(DynamicVars["Count"].IntValue, drawPile.Count);
            var rng = owner.RunState.Rng.CombatCardGeneration;
            var shuffled = drawPile.OrderBy(_ => rng.NextInt(0, int.MaxValue)).ToList();

            for (var i = 0; i < count; i++)
                CardCmd.Upgrade(shuffled[i]);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m);    // 8 → 11
            DynamicVars["Count"].UpgradeValueBy(1m);  // 2 → 3
        }
    }
}

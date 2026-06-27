using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class AnchorCore : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.AnchorCoreRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<ArtifactPower>(1m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ArtifactPower>()];

        public override async Task BeforeCombatStart()
        {
            Flash();
            await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
                DynamicVars["ArtifactPower"].BaseValue, Owner.Creature, null);
        }
    }
}

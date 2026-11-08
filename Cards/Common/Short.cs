using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class Short() : WineFoxCard(0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Charge", 3m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardShort);

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Exhaust
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ChargePower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature is not { } creature)
                return;

            var slot = creature.Powers.OfType<MagicWineFoxSpellSlotPower>().FirstOrDefault();
            var loaded = slot?.LoadedCount ?? 0;
            if (loaded <= 0)
                return;

            await ChargePower.Gain(
                choiceContext,
                creature,
                loaded * DynamicVars["Charge"].BaseValue,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Charge"].UpgradeValueBy(1m);
        }
    }
}

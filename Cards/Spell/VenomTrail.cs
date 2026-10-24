using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class VenomTrail() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Ratio", 25m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardVenomTrail);

        public override string SpellIconPath => Const.Paths.SpellIconVenomTrail;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<PoisonPower>()
        ];

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MarkDamageToPoison(DynamicVars["Ratio"].BaseValue / 100m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Ratio"].UpgradeValueBy(15m);
        }
    }
}

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Cards.WineFoxCard;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class Battery() : WineFoxCardBase(
        1, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<ChargePower>(3m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBattery);

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            [.. base.CanonicalKeywords, CardKeyword.Retain, CardKeyword.Exhaust];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChargePower>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature is not { } caster)
                return;

            await ChargePower.Gain(choiceContext, caster, DynamicVars["ChargePower"].BaseValue, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}

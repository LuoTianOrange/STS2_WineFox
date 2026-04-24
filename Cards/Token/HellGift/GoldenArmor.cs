using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.HellGift
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class GoldenArmor() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Buffer", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGoldenArmor);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["Buffer"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
            await PowerCmd.Apply<GoldenArmorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Buffer"].UpgradeValueBy(1m);
        }
    }
}

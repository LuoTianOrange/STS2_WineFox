using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.HellGift
{
    public class GoldenArmor() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new ("Buffer",1m)];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGoldenArmor);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<BufferPower>(Owner.Creature, DynamicVars["Buffer"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<ArtifactPower>(Owner.Creature, 1m, Owner.Creature, this);
            await PowerCmd.Apply<GoldenArmorPower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Buffer"].UpgradeValueBy(1m);
        }
    }
}


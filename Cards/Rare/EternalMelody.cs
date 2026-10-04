using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EternalMelody() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("MaxHpLoss", 4m)
        ];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardEternalMelody,
            Const.Paths.CardEternalMelody,
            FrameMaterialPath: Const.Paths.CardEternalMelodyCosmicStarsFrameMat);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await PowerCmd.Apply<EternalMelodyPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars["MaxHpLoss"].BaseValue,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MaxHpLoss"].UpgradeValueBy(2m);
        }
    }
}

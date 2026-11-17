using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EternalMelody() : WineFoxCardBase(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("MaxHpLoss", 3m),
        ];

        public override CardAssetProfile AssetProfile
        {
            get
            {
                var portrait = MagicCardArt.Resolve(Const.Paths.CardEternalMelody);

                return new(portrait, portrait,
                    FrameMaterialPath: Const.Paths.CardEternalMelodyCosmicStarsFrameMat);
            }
        }

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

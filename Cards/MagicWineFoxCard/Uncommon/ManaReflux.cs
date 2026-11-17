using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ManaReflux() : WineFoxCardBase(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Bonus", 0m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardManaReflux);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner)
                return;

            await ManaRefluxPower.RegisterAsync(
                choiceContext,
                owner.Creature,
                (int)DynamicVars["Bonus"].BaseValue,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Bonus"].UpgradeValueBy(1m);
        }
    }
}

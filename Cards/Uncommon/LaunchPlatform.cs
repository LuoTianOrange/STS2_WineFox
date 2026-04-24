using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     弹射置物台 - 1 cost Power Uncommon.
    ///     每回合第一次消耗应力时，获得1费。
    ///     升级：变为0费。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class LaunchPlatform() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLaunchPlatform);

        protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1)
        ];
        
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];
        
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<LaunchPlatformPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}


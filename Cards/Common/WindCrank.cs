using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class WindCrank() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<StressPower>(3m)];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWindCrank);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            await PowerCmd.Apply<StressPower>(Owner.Creature, DynamicVars["StressPower"].BaseValue, Owner.Creature,
                this);
            PlayerCmd.EndTurn(Owner, false);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["StressPower"].UpgradeValueBy(1m);
        }
    }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class CyclicProcess() : WineFoxCard(
        1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), ICraftingCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(2m, ValueProp.Move),
            new IntVar("Hits", 3m),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.CraftKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCyclicProcess);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars["Hits"].IntValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await CraftCmd.CraftIntoHand(choiceContext, this);
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Random);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Hits"].UpgradeValueBy(1m); // 3 -> 4
        }
    }
}



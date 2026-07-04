using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class AccumulatingGrudges() : WineFoxCard(
        2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(5m, ValueProp.Move),
            ModCardVars.Computed("TotalHits", 1m, card =>
            {
                var creature = card?._owner?.Creature;
                return creature == null ? 1m : 1 + CraftCmd.GetCraftCountThisCombat(creature);
            }),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.CraftKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAccumulatingGrudges);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var craftCount = CraftCmd.GetCraftCountThisCombat(Owner.Creature);
            var hits = 1 + craftCount;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(hits)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m); // 5 → 7
        }
    }
}

/*
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class TriuneBlessing() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.AllAllies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("MaxConsume", 2m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<FocusPower>(),
            HoverTipFactory.FromPower<ChantPower>(),
        ];

        public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardTriuneBlessing);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var maxConsume = DynamicVars["MaxConsume"].BaseValue;
            var availableChant = Math.Max(0m, owner.GetPowerAmount<ChantPower>());
            var consumed = Math.Min(maxConsume, availableChant);

            if (consumed > 0m)
                await PowerCmd.Apply<ChantPower>(choiceContext, owner, -consumed, owner, this);

            var strengthDexterity = 1m + consumed;
            var focus = 1m + Math.Floor(consumed / 2m);

            var allies = combatState
                .GetTeammatesOf(owner)
                .Where(c => c is { IsAlive: true, IsPlayer: true });

            foreach (var ally in allies)
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), ally, strengthDexterity, owner, this);
                await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), ally, strengthDexterity, owner, this);
                await PowerCmd.Apply<FocusPower>(new ThrowingPlayerChoiceContext(), ally, focus, owner, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MaxConsume"].UpgradeValueBy(2m);
        }
    }
}
*/

namespace STS2_WineFox.Cards.Deleted.Magic
{
}

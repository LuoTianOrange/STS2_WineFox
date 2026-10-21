using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class OverloadCasting() : WineFoxCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<SpellOverload>(IsUpgraded)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardOverloadCasting);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner.Creature?.CombatState is not { } combatState)
                return;

            var overload = combatState.CreateCard<SpellOverload>(Owner);

            if (IsUpgraded && !overload.IsUpgraded)
                CardCmd.Upgrade(overload);

            await MagicWineFoxSpellCmd.Load(choiceContext, overload, play);

            await MagicWineFoxSpellCmd.CastAll(choiceContext, Owner, null, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}

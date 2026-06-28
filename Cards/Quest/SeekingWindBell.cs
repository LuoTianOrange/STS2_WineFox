using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Quest
{
    [RegisterCard(typeof(QuestCardPool))]
    public sealed class SeekingWindBell : WineFoxCard
    {
        private int _targetActIndex = -1;

        public override int MaxUpgradeLevel => 0;
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSeekingWindBell);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1)];

        [SavedProperty]
        public int TargetActIndex
        {
            get => _targetActIndex;
            set
            {
                AssertMutable();
                _targetActIndex = value;
            }
        }

        public SeekingWindBell()
            : base(1, CardType.Quest, CardRarity.Quest, TargetType.None)
        {
        }

        public override void AfterCreated()
        {
            TargetActIndex = Owner.RunState.CurrentActIndex + 1;
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
}

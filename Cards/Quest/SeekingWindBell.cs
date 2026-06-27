using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
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
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSeekingWindBell);

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
            : base(-1, CardType.Quest, CardRarity.Quest, TargetType.Self)
        {
        }

        public override void AfterCreated()
        {
            TargetActIndex = Owner.RunState.CurrentActIndex + 1;
        }
    }
}

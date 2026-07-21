using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2_WineFox.Character;
using STS2_WineFox.Potions;
using STS2RitsuLib;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Lifecycle;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class OilyBeanCurd : WineFoxRelic
    {
        private const int Threshold = 5;
        private const int HpGain = 2;
        private const float ScaleGain = 0.05f;
        private const float MaxScale = 5f;

        public override RelicRarity Rarity => RelicRarity.Common;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.OilyBeanCurdRelicIcon);
        public override bool ShowCounter => true;
        public override int DisplayAmount => TotalFoodsEaten;

        private int _foodsEaten;
        private int _totalFoodsEaten;

        [SavedProperty]
        public int FoodsEaten
        {
            get => _foodsEaten;
            set
            {
                AssertMutable();
                _foodsEaten = value;
            }
        }

        [SavedProperty]
        public int TotalFoodsEaten
        {
            get => _totalFoodsEaten;
            set
            {
                AssertMutable();
                _totalFoodsEaten = value;
                InvokeDisplayAmountChanged();
            }
        }

        private static IDisposable? _lifecycleSubscription;

        static OilyBeanCurd()
        {
            _lifecycleSubscription = RitsuLibFramework.SubscribeLifecycle<PotionUsedEvent>(OnPotionUsedGlobally);
        }

        private static void OnPotionUsedGlobally(PotionUsedEvent evt)
        {
            if (evt.Potion is not SellableToMerchantPotionModel)
                return;

            var owner = evt.Potion.Owner;
            var relic = owner?.Relics.OfType<OilyBeanCurd>().FirstOrDefault();
            if (relic == null)
                return;

            _ = relic.OnFoodConsumed();
        }

        private async Task OnFoodConsumed()
        {
            TotalFoodsEaten++;

            var newCount = _foodsEaten + 1;
            FoodsEaten = newCount;

            if (newCount < Threshold)
                return;

            FoodsEaten = 0;
            Flash();
            await CreatureCmd.GainMaxHp(Owner.Creature, HpGain);
            Grow();
        }

        public override Task AfterRoomEntered(AbstractRoom room)
        {
            Grow();
            return Task.CompletedTask;
        }

        private void Grow()
        {
            var node = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
            if (node == null)
                return;

            var growthCount = TotalFoodsEaten / Threshold;
            var nextScale = Math.Min(MaxScale, 1f + growthCount * ScaleGain);
            node.ScaleTo(nextScale, 0.0);
        }
    }
}

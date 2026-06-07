using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Enchantments;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Events
{
    [RegisterActEvent(typeof(Overgrowth))]
    [RegisterActEvent(typeof(Underdocks))]
    public sealed class DesertPyramid : ModEventTemplate
    {
        public override EventAssetProfile AssetProfile => new(InitialPortraitPath: Const.Paths.EventDesertPyramid);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new HpLossVar(6m)];

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return
            [
                new(this, ThoroughExploration, InitialOptionKey("THOROUGH_EXPLORATION")),
                new(this, Leave, InitialOptionKey("LEAVE")),
            ];
        }

        private IReadOnlyList<EventOption> ChestRewards()
        {
            return
            [
                new(this, Chest1, ModOptionKey("2", "CHEST1")),
                new(this, Chest2, ModOptionKey("2", "CHEST2"), HoverTipFactory.FromEnchantment<FireAspect>()),
                new(this, Chest3, ModOptionKey("2", "CHEST3"), HoverTipFactory.FromEnchantment<SweepingEdge>()),
            ];
        }

        private async Task Chest1()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            owner.PopulateRelicGrabBagIfNecessary(owner.PlayerRng.Rewards);

            var characterRelicIds = owner.Character.RelicPool.AllRelics
                .Where(r => r.IsAllowed(owner.RunState))
                .Where(r => r.Rarity is RelicRarity.Common or RelicRarity.Uncommon or RelicRarity.Rare)
                .Select(r => r.Id)
                .Distinct()
                .ToList();

            Shuffle(characterRelicIds);

            RelicModel? relic = null;
            foreach (var relicId in characterRelicIds)
            {
                relic = TryPullAnyRewardRarity(r => r.Id == relicId);
                if (relic != null)
                    break;
            }

            relic ??= TryPullAnyRewardRarity(RelicGrabBagPullCompat.PassAllRelics);

            relic ??= RelicFactory.PullNextRelicFromFront(owner);
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CHEST1FINISH.description"));
            await RelicCmd.Obtain(relic.ToMutable(), owner);
            return;

            RelicModel? TryPullAnyRewardRarity(Func<RelicModel, bool> filter)
            {
                var rewardRarities = new List<RelicRarity>
                {
                    RelicRarity.Common,
                    RelicRarity.Uncommon,
                    RelicRarity.Rare,
                };

                Shuffle(rewardRarities);

                foreach (var rarity in rewardRarities)
                {
                    var pulled = TryPull(filter, rarity);
                    if (pulled != null)
                        return pulled;
                }

                return null;
            }

            RelicModel? TryPull(Func<RelicModel, bool> filter, RelicRarity r)
            {
                var pulled = RelicGrabBagPullCompat.PullFromFront(owner.RelicGrabBag, r, filter, owner.RunState);
                if (pulled != null)
                    owner.RunState.SharedRelicGrabBag.Remove(pulled);
                return pulled;
            }

            void Shuffle<T>(IList<T> list)
            {
                for (var i = list.Count - 1; i > 0; i--)
                {
                    var j = owner.PlayerRng.Rewards.NextInt(0, i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }



        private async Task Chest2()
        {
            ArgumentNullException.ThrowIfNull(Owner);

            var enchantment = ModelDb.Enchantment<FireAspect>();
            var card = (await CardSelectCmd.FromDeckForEnchantment(
                Owner,
                enchantment,
                1,
                c => enchantment.CanEnchant(c),
                new(CardSelectorPrefs.EnchantSelectionPrompt, 1)
            )).FirstOrDefault();

            if (card != null)
            {
                CardCmd.Enchant<FireAspect>(card, 1m);

                var vfx = NCardEnchantVfx.Create(card);
                if (vfx != null)
                    NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }

            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CHEST2FINISH.description"));
        }

        private async Task Chest3()
        {
            ArgumentNullException.ThrowIfNull(Owner);

            var enchantment = ModelDb.Enchantment<SweepingEdge>();
            var card = (await CardSelectCmd.FromDeckForEnchantment(
                Owner,
                enchantment,
                1,
                c => enchantment.CanEnchant(c),
                new(CardSelectorPrefs.EnchantSelectionPrompt, 1)
            )).FirstOrDefault();

            if (card != null)
            {
                CardCmd.Enchant<SweepingEdge>(card, 1m);

                var vfx = NCardEnchantVfx.Create(card);
                if (vfx != null)
                    NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }

            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CHEST2FINISH.description"));
        }

        private async Task ThoroughExploration()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.HpLoss.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            SetEventState(L10NLookup($"{Id.Entry}.pages.CHEST_REWARDS.description"), ChestRewards());
        }

        private Task Leave()
        {
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.LEAVE.description"));
            return Task.CompletedTask;
        }
    }
}

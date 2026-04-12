using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Enchantments;
using STS2_WineFox.Relics;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Events
{
    public sealed class DesertPyramid : ModEventTemplate
    {
        public override EventAssetProfile AssetProfile => new(InitialPortraitPath:Const.Paths.EventDesertPyramid);
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
                new(this, Chest2, ModOptionKey("2", "CHEST2")),
                new(this, Chest3, ModOptionKey("2", "CHEST3")),
            ];
        }

        private async Task Chest1()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            await RelicCmd.Obtain<SophisticatedBackpack>(Owner);
            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CHEST1.description"));
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
                new (CardSelectorPrefs.EnchantSelectionPrompt, 1)
            )).FirstOrDefault();

            if (card != null)
            {
                CardCmd.Enchant<FireAspect>(card, 1m);
                
                var vfx = NCardEnchantVfx.Create(card);
                if (vfx != null)
                    NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }

            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CHEST2.description"));
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
                new (CardSelectorPrefs.EnchantSelectionPrompt, 1)
            )).FirstOrDefault();

            if (card != null)
            {
                CardCmd.Enchant<SweepingEdge>(card, 1m);
                
                var vfx = NCardEnchantVfx.Create(card);
                if (vfx != null)
                    NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }

            SetEventFinished(L10NLookup($"{Id.Entry}.pages.CHEST3.description"));
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

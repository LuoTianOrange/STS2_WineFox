using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    public abstract class SellableToMerchantPotionModel : PotionModel, IModPotionAssetOverrides
    {
        protected abstract int SellGold { get; }

        /// <summary>
        ///     Whether this potion can be used outside combat (excluding merchant selling).
        ///     Defaults to true for food.
        /// </summary>
        public virtual bool CanBeUsedOutOfCombat => true;

        public override PotionUsage Usage => PotionUsage.AnyTime;

        public override TargetType TargetType =>
            Owner.RunState.CurrentRoom is MerchantRoom
                ? TargetType.TargetedNoCreature
                : CombatManager.Instance.IsInProgress
                    ? CombatTargetType
                    : TargetType.Self;

        protected virtual TargetType CombatTargetType => TargetType.AnyPlayer;

        public override bool PassesCustomUsabilityCheck =>
            CombatManager.Instance.IsInProgress
            || Owner.RunState.CurrentRoom is MerchantRoom
            || CanBeUsedOutOfCombat;

        protected sealed override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            if (Owner.RunState.CurrentRoom is MerchantRoom)
            {
                ShowPotionVfx(NRun.Instance?.MerchantRoom?.MerchantButton);
                await PlayerCmd.GainGold(SellGold, Owner);
                return;
            }

            if (CombatManager.Instance.IsInProgress)
            {
                await OnUseInCombat(choiceContext, target);
                return;
            }

            await OnUseOutOfCombat(choiceContext);
        }

        public virtual PotionAssetProfile AssetProfile => PotionAssetProfile.Empty;
        public virtual string? CustomImagePath => AssetProfile.ImagePath;
        public virtual string? CustomOutlinePath => AssetProfile.OutlinePath;

        protected static PotionAssetProfile Art(string imagePath, string? outlinePath = null)
        {
            return new(imagePath, outlinePath ?? imagePath);
        }

        protected abstract Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target);

        protected static Creature GetCombatTarget(Creature? target)
        {
            PotionModel.AssertValidForTargetedPotion(target);
            return target!;
        }

        /// <summary>
        ///     Called when this potion is used outside combat (excluding merchant selling).
        ///     Food should override this method and avoid applying special powers.
        /// </summary>
        protected virtual Task OnUseOutOfCombat(PlayerChoiceContext choiceContext) => Task.CompletedTask;

        public override Task AfterRoomEntered(AbstractRoom room)
        {
            if (room is not RestSiteRoom)
                return Task.CompletedTask;

            if (this is not ICampfireTransformPotion { CampfireTransformResult: { } result })
                return Task.CompletedTask;

            var owner = Owner;
            var slotIndex = owner.GetPotionSlotIndex(this);
            if (slotIndex < 0)
                return Task.CompletedTask;

            var newPotion = result.ToMutable();
            owner.DiscardPotionInternal(this);
            owner.AddPotionInternal(newPotion, slotIndex);
            return Task.CompletedTask;
        }

        private static void ShowPotionVfx(NMerchantButton? merchantButton)
        {
            if (TestMode.IsOn || merchantButton == null)
                return;

            var scenePath = SceneHelper.GetScenePath("vfx/vfx_slime_impact");
            var vfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>();
            merchantButton.GetParent().AddChildSafely(vfx);
            vfx.GlobalPosition = merchantButton.GlobalPosition;
        }
    }
}

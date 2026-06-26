using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class TurtleMasterPotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Rare;
        public override TargetType TargetType => TargetType.AnyPlayer;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.TurtleMasterPotion);
        protected override Color PotionParticleColor => new("2e7d32");
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TurtleMasterPower>(4m), new IntVar("Duration", TurtleMasterPower.Duration)];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [new HoverTip(
            ModelDb.Power<TurtleMasterPower>(),
            new LocString("powers", "STS2_WINE_FOX_POWER_TURTLE_MASTER_POWER.potionDescription").GetFormattedText(),
            false)];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            PotionModel.AssertValidForTargetedPotion(target);
            ShowPotionHitVfx(target);
            var existing = target.Powers.OfType<TurtleMasterPower>().FirstOrDefault();
            if (existing != null)
            {
                existing.RemainingTurns += (int)DynamicVars["Duration"].BaseValue;
                return;
            }

            await PowerCmd.Apply<TurtleMasterPower>(choiceContext, target, DynamicVars["TurtleMasterPower"].BaseValue, Owner.Creature, null);
        }
    }
}

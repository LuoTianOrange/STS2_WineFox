using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class HarmingPotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Common;
        public override TargetType TargetType => TargetType.AllEnemies;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.HarmingPotion);
        protected override Color PotionParticleColor => new("b71c1c");
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Unpowered)];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            var targets = GetEnemyPotionTargets(target);
            ShowEnemyPotionHitVfx(target);
            await CreatureCmd.Damage(choiceContext, targets, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, null, null);
        }
    }
}

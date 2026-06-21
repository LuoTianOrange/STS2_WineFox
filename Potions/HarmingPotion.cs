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
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Unpowered)];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, null);
        }
    }
}

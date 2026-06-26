using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class PoisonPotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Common;
        public override TargetType TargetType => TargetType.AllEnemies;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.PoisonPotion);
        protected override Color PotionParticleColor => new("4e9331");
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PoisonPower>(4m)];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            var targets = GetEnemyPotionTargets(target);
            ShowEnemyPotionHitVfx(target);
            await PowerCmd.Apply<PoisonPower>(choiceContext, targets, DynamicVars.Poison.BaseValue, Owner.Creature, null);
        }
    }
}

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class WindChargedPotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Uncommon;
        public override TargetType TargetType => TargetType.AnyEnemy;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.WindChargedPotion);
        protected override Color PotionParticleColor => new("b8fff7");
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WindChargedPower>(1m)];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<WindChargedPower>()];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            PotionModel.AssertValidForTargetedPotion(target);
            ShowEnemyPotionHitVfx(target);
            await PowerCmd.Apply<WindChargedPower>(choiceContext, target, DynamicVars["WindChargedPower"].BaseValue, Owner.Creature, null);
        }
    }
}

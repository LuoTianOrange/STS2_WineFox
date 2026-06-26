using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class FireResistancePotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Uncommon;
        public override TargetType TargetType => TargetType.AnyPlayer;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.FireResistancePotion);
        protected override Color PotionParticleColor => new("e49a3a");
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ArtifactPower>(1m)];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<ArtifactPower>()];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            PotionModel.AssertValidForTargetedPotion(target);
            ShowPotionHitVfx(target);
            await PowerCmd.Apply<ArtifactPower>(choiceContext, target, DynamicVars["ArtifactPower"].BaseValue, Owner.Creature, null);
        }
    }
}

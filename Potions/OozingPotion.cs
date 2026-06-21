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
    public sealed class OozingPotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Uncommon;
        public override TargetType TargetType => TargetType.AnyEnemy;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.OozingPotion);
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OozingPower>(1m)];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<OozingPower>()];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            PotionModel.AssertValidForTargetedPotion(target);
            await PowerCmd.Apply<OozingPower>(choiceContext, target, DynamicVars["OozingPower"].BaseValue, Owner.Creature, null);
        }
    }
}

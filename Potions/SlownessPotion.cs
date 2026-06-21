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
    public sealed class SlownessPotion : WineFoxPotion
    {
        public override PotionRarity Rarity => PotionRarity.Rare;
        public override TargetType TargetType => TargetType.AnyEnemy;
        public override PotionAssetProfile AssetProfile => Art(Const.Paths.SlownessPotion);
        protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SlowPower>(1m)];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<SlowPower>()];

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            PotionModel.AssertValidForTargetedPotion(target);
            await PowerCmd.Apply<SlowPower>(choiceContext, target, DynamicVars["SlowPower"].BaseValue, Owner.Creature, null);
        }
    }
}

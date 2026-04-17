using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    // [RegisterPotion(typeof(WineFoxPotionPool))]
    public class GoldenApple : PotionModel
    {
        public override PotionRarity Rarity => PotionRarity.Uncommon;
        public override PotionUsage Usage => PotionUsage.AnyTime;
        public override TargetType TargetType => TargetType.AnyPlayer;
        public override bool CanBeGeneratedInCombat => false;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            new DynamicVar[] { new HealVar(10m), new PowerVar<RegenPower>(6m), new GoldVar(64) };

        protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
        {
            var potion = this;
        }
    }
}

// 文件：Potions/GoldenApple.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

using MegaCrit.Sts2.Core.Models.Powers;

namespace STS2_WineFox.Potions
{
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

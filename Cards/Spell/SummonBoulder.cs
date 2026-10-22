using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class SummonBoulder() : MagicWineFoxSpellCard(
        2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy), IMagicWineFoxSpellCard
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSummonBoulder);

        public override string SpellIconPath => Const.Paths.SpellIconSummonBoulder;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<GiantRock>(IsUpgraded)
        ];

        public override SpellTraits Traits => new();

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature?.CombatState is not { } combatState)
                return;

            var rock = combatState.CreateCard<GiantRock>(owner);

            var enemies = combatState.HittableEnemies;

            var target = context.Target is { IsAlive: true } candidate && enemies.Contains(candidate)
                ? candidate
                : enemies.FirstOrDefault();

            if (target == null)
                return;
            
            rock.AddKeyword(CardKeyword.Exhaust);

            if (IsUpgraded && !rock.IsUpgraded)
                CardCmd.Upgrade(rock);

            await CardCmd.AutoPlay(context.ChoiceContext, rock, target);
        }

        protected override void OnUpgrade()
        {
        }
    }
}

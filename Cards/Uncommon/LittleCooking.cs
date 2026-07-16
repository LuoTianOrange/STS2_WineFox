using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Potions;
using STS2_WineFox.Settings;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class LittleCooking() : WineFoxCard(
        2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            [CardKeyword.Exhaust, WineFoxKeywords.CookedFoodKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLittleCooking);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (!WineFoxRuntimeSettings.FoodEnabled)
                return;

            var owner = Owner;
            var rng = owner.RunState.Rng.CombatPotionGeneration;

            var cookedFoods = ModelDb
                .PotionPool<WineFoxFoodPotionPool>()
                .GetUnlockedPotions(owner.UnlockState)
                .OfType<ICampfireTransformPotion>()
                .Select(p => p.CampfireTransformResult)
                .Distinct()
                .ToList();

            if (cookedFoods.Count == 0) return;

            var count = Math.Min(2, cookedFoods.Count);
            var shuffled = cookedFoods.OrderBy(_ => rng.NextInt(0, int.MaxValue)).ToList();

            for (var i = 0; i < count; i++)
            {
                if (!owner.HasOpenPotionSlots)
                    break;

                var potion = shuffled[i].ToMutable();
                if (!(await PotionCmd.TryToProcure(potion, owner)).success)
                    break;
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 2 → 1
        }
    }
}

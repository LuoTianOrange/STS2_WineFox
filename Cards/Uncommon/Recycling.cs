using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Recycling() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Count", 3m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            WineFoxKeywords.MaterialKeyword,
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRecycling);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var handCards = PileType.Hand.GetPile(owner).Cards
                .Where(c => !ReferenceEquals(c, this))
                .ToList();

            if (handCards.Count > 0)
            {
                var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_RECYCLING_CHOOSE");
                var prefs = new CardSelectorPrefs(prompt, 1);
                var selected = (await CardSelectCmd.FromHand(choiceContext, owner, prefs,
                        c => handCards.Contains(c), this))
                    .FirstOrDefault();

                if (selected != null)
                    await CardPileCmd.Add(selected, PileType.Exhaust);
            }

            var stressConsumed = await StressCmd.ConsumeOne(owner.Creature, this);
            var mult = stressConsumed ? 2m : 1m;

            var rng = owner.RunState.Rng.CombatCardGeneration;
            var count = DynamicVars["Count"].IntValue;
            var gains = new List<(Type Type, decimal Amount)>();
            for (var i = 0; i < count; i++)
            {
                var roll = rng.NextInt(0, 100); // 0–99
                var type = roll switch
                {
                    < 30 => typeof(WoodPower),
                    < 60 => typeof(StonePower),
                    < 90 => typeof(IronPower),
                    _ => typeof(DiamondPower),
                };
                gains.Add((type, 1m * mult));
            }

            await MaterialCmd.GainMaterials(this, gains, applyStress: false);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Count"].UpgradeValueBy(1m); // 3 → 4
        }
    }
}

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Token.HandCrank;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    [RegisterCharacterStarterRelic(typeof(WineFox))]
    [RegisterTouchOfOrobasRefinement(typeof(Deployer))]
    public class HandCrank : WineFoxRelic
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.HandCrankRelicIcon);
        public override RelicRarity Rarity => RelicRarity.Starter;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Stress", 1m),
            new("Wood", 1m),
            new("Stone", 3m)
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (Owner.Creature.CombatState?.RoundNumber > 1) return;

            var combatState = Owner.Creature.CombatState;
            if (combatState == null) return;

            var options = CreateOptions(combatState);

            Flash();

            var chosen = await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                options,
                Owner);

            if (chosen == null) return;
            await ((IDirectApply)chosen).Apply();
        }

        protected virtual List<CardModel> CreateOptions(ICombatState combatState)
        {
            return
            [
                combatState.CreateCard(ModelDb.Card<ObtainStress>(), Owner),
                combatState.CreateCard(ModelDb.Card<ObtainMaterials>(), Owner)
            ];
        }
    }
}

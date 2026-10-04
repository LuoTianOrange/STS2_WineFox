using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(MagicWineFoxRelicPool))]
    [RegisterCharacterStarterRelic(typeof(MagicWineFox))]
    public class MagicWineFoxWand : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.ApprenticeStaffIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("SlotCapacity", MagicWineFoxSpellCmd.DefaultCapacity),
            new("CastCount", MagicWineFoxSpellCmd.DefaultCastCount),
            new("PreloadSigils", 1m),
            new("LoadDiscount", MagicWineFoxSpellCmd.FirstLoadDiscount)
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (player.PlayerCombatState?.TurnNumber != 1) return;

            Flash();

            var slots = (int)DynamicVars["SlotCapacity"].BaseValue;
            var casts = (int)DynamicVars["CastCount"].BaseValue;

            var power = await MagicWineFoxSpellCmd.EnsurePower(Owner, slots, casts);
            if (power == null) return;
            
            var sigils = (int)DynamicVars["PreloadSigils"].BaseValue;
            for (var i = 0; i < sigils; i++)
                await SendSigilToHand(Owner);
        }

        private static async Task<bool> SendSigilToHand(Player owner)
        {
            if (owner.Creature?.CombatState is not { } combatState)
                return false;

            var sigil = combatState.CreateCard<DoubleReleaseSigil>(owner);
            var instance = await CardPileCmd.AddGeneratedCardToCombat(sigil, PileType.Hand, owner);
            CardCmd.PreviewCardPileAdd(instance);
            return true;
        }
        
        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (Owner == null) return;
            if (side != Owner.Creature.Side) return;

            await MagicWineFoxSpellCmd.CastAll(choiceContext, Owner, null, null);
        }
    }
}

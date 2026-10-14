using Godot;
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
    [RegisterTouchOfOrobasRefinement(typeof(MagicWineFoxMasterWand))]
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

            var slots = (int)DynamicVars["SlotCapacity"].BaseValue;
            var casts = (int)DynamicVars["CastCount"].BaseValue;

            var power = await MagicWineFoxSpellCmd.EnsurePower(Owner, choiceContext, slots, casts);
            if (power == null) return;

            MagicWineFoxSpellCmd.RefreshWandCardValues(power);


            if (player.PlayerCombatState?.TurnNumber != 1) return;

            Flash();

            var stale = power.LoadedCount;
            if (stale > 0)
                power.ClearSlots();

            var sigils = (int)DynamicVars["PreloadSigils"].BaseValue;
            for (var i = 0; i < sigils; i++)
                await MagicWineFoxSpellCmd.GiveDoubleReleaseSigil(Owner);
        }

        // 回合结束的释放触发已移到 MagicWineFoxSpellSlotPower.AfterSideTurnEnd：
        // 触发条件从「持有本遗物」改为「拥有法术槽能力」，跨角色装填法术时才会一并释放。
    }
}

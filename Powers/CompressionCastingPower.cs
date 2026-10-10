using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     压缩施法：本回合减少法术槽，法杖内法术的伤害与防御提高。
    /// </summary>
    [RegisterPower]
    public class CompressionCastingPower : WineFoxPower, IMagicDamageModifier, IMagicBlockModifier
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.CompressionCastingPowerIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars => [new("Bonus", 4m)];

        private decimal Bonus =>
            DynamicVars.TryGetValue("Bonus", out var bonus) ? bonus.BaseValue : 0m;

        public decimal ModifyMagicDamageAdditive(
            Creature? target,
            decimal baseAmount,
            Creature dealer,
            CardModel cardSource)
        {
            if (dealer != Owner || cardSource is not MagicWineFoxSpellCard)
                return 0m;

            return Bonus;
        }

        public decimal ModifyMagicBlockAdditive(
            Creature defender,
            decimal baseAmount,
            CardModel cardSource)
        {
            if (defender != Owner || cardSource is not MagicWineFoxSpellCard)
                return 0m;

            return Bonus;
        }

        public async Task ReduceSlots(int count)
        {
            if (Owner.Player is not { } player)
                return;

            var slots = await MagicWineFoxSpellCmd.EnsurePower(player);
            slots?.ApplySlotCapacityOffset(-count);
        }

        public override Task AfterRemoved(Creature oldOwner)
        {
            if (oldOwner.Player is { } player)
                MagicWineFoxSpellCmd.GetPower(player)?.ClearSlotCapacityOffset();

            return Task.CompletedTask;
        }

        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side)
                return;

            await PowerCmd.Remove(this);
        }
    }
}

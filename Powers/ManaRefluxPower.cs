using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     下回合开始时按本回合释放的法术次数提供能量。
    ///     次数由回合结束释放流程写入，能力自身只负责下回合的结算。
    /// </summary>
    [RegisterPower]
    public class ManaRefluxPower : WineFoxPower
    {
        private int _armed;

        public int Bonus { get; set; }

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SpellIconManaReflux);

        public void Arm(int spellsCast)
        {
            _armed = Math.Max(_armed, spellsCast);
        }

        public static async Task RegisterAsync(
            PlayerChoiceContext choiceContext,
            Creature creature,
            int bonus,
            CardModel? cardSource)
        {
            var existing = creature.Powers.OfType<ManaRefluxPower>().FirstOrDefault();

            if (existing != null)
            {
                existing.Bonus = Math.Max(existing.Bonus, bonus);
                return;
            }

            var created = await PowerCmd.Apply<ManaRefluxPower>(
                choiceContext, creature, 1m, creature, cardSource);

            if (created != null)
                created.Bonus = bonus;
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            var total = _armed > 0 ? _armed + Bonus : Bonus;

            if (total > 0)
            {
                await PlayerCmd.GainEnergy(total, player);
                Flash();
            }

            await PowerCmd.Remove(this);
        }
    }
}

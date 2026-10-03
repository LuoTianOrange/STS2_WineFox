using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    /// <summary>
    ///     狐火杖 —— 魔法酒狐的起始法杖遗物（法杖构筑版 v0.2.6）。
    ///     <para>
    ///         法杖是**遗物**而不是卡牌：不进牌池、不能被商店买到。
    ///         战斗开始时建立槽位容器，并按遗物效果装载 1 张【双重释放符】。
    ///     </para>
    ///     <para>
    ///         初始面板：槽位 3 / 释放轮数 1。整局**只能升级一次**，二选一：
    ///         「扩张」槽位 3→4，或「速铸」释放轮数 1→2。升级入口见 <c>WandUpgradeCmd</c>（待实现），
    ///         本类只负责战斗内面板的建立。
    ///     </para>
    /// </summary>
    [RegisterRelic(typeof(MagicWineFoxRelicPool))]
    [RegisterCharacterStarterRelic(typeof(MagicWineFox))]
    public class MagicWineFoxWand : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.HandCrankRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("SlotCapacity", MagicWineFoxSpellCmd.DefaultCapacity),
            new("CastCount", MagicWineFoxSpellCmd.DefaultCastCount),
            new("PreloadSigils", 1m)
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

            // 遗物效果：战斗开始时白送 N 张【双重释放符】，直接装填进槽位。
            var sigils = (int)DynamicVars["PreloadSigils"].BaseValue;
            for (var i = 0; i < sigils; i++)
            {
                if (!await PreloadSigil(Owner))
                    break;
            }
        }

        /// <summary>
        ///     释放：回合结束时自动触发，按装填顺序释放槽内法术。
        ///     放在遗物侧而不是 Power 侧——Power 的 Owner 是 Creature，拿不到 PlayerCombatState。
        /// </summary>
        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (Owner == null) return;
            if (side != Owner.Creature.Side) return;

            await MagicWineFoxSpellCmd.CastAll(choiceContext, Owner, null, null);
        }

        /// <summary>
        ///     把一张【双重释放符】直接塞进法杖（不经过手牌、不支付费用）。
        /// </summary>
        private static async Task<bool> PreloadSigil(Player owner)
        {
            var power = MagicWineFoxSpellCmd.GetPower(owner);
            if (power == null) return false;

            var canonical = ModelDb.Card<DoubleReleaseSigil>();
            if (canonical == null) return false;

            var clone = canonical.ToMutable();
            clone.Owner = owner;

            var snapshot = new Mechanics.MagicWineFoxSpellSlotSnapshot(clone, null, IsModifier: true);
            return power.TryLoad(snapshot);
        }
    }
}

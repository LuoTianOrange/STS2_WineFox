using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Temp
{
    /// <summary>
    ///     临时占位卡基类：0 费，「获得 1 点能量，抽 1 张牌」。
    ///     <para>
    ///         **用途**：商店需要凑齐「2 张攻击 + 2 张技能 + 1 张能力且互不重复」，
    ///         而魔狐卡池缺少足够的「类型 × 稀有度」组合（尤其是能力牌一张都没有），
    ///         导致商店取不到牌而黑屏。这里补齐 3 类型 × 3 稀有度 = 9 张。
    ///     </para>
    ///     <para>
    ///         正式卡牌做出来后，**整体替换本文件**即可（含四语言条目）。
    ///     </para>
    /// </summary>
    public abstract class TempEnergyDrawCard(int baseCost, CardType type, CardRarity rarity)
        : WineFoxCard(baseCost, type, rarity, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(1),
            new CardsVar(1)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        protected override void OnUpgrade()
        {
            // 临时占位卡：不提供升级效果。
        }
    }

    // ── 攻击牌 ──────────────────────────────────────────────
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempAttackCommon() : TempEnergyDrawCard(0, CardType.Attack, CardRarity.Common) { }

    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempAttackUncommon() : TempEnergyDrawCard(0, CardType.Attack, CardRarity.Uncommon) { }

    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempAttackRare() : TempEnergyDrawCard(0, CardType.Attack, CardRarity.Rare) { }

    // ── 技能牌 ──────────────────────────────────────────────
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempSkillCommon() : TempEnergyDrawCard(0, CardType.Skill, CardRarity.Common) { }

    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempSkillUncommon() : TempEnergyDrawCard(0, CardType.Skill, CardRarity.Uncommon) { }

    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempSkillRare() : TempEnergyDrawCard(0, CardType.Skill, CardRarity.Rare) { }

    // ── 能力牌 ──────────────────────────────────────────────
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempPowerCommon() : TempEnergyDrawCard(0, CardType.Power, CardRarity.Common) { }

    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempPowerUncommon() : TempEnergyDrawCard(0, CardType.Power, CardRarity.Uncommon) { }

    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class TempPowerRare() : TempEnergyDrawCard(0, CardType.Power, CardRarity.Rare) { }
}

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2_WineFox.Character;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Content
{
    /// <summary>
    ///     Generated placeholders so unfinished lists / rewards can still resolve stable ModelIds without extra CLR types.
    /// </summary>
    internal static class WineFoxPlaceholders
    {
        internal static void Register(ModContentPackContext ctx)
        {
            var content = ctx.Content;

            content.RegisterPlaceholderCard<WineFoxCardPool>("wf_placeholder_attack",
                new(
                    1,
                    CardType.Attack,
                    CardRarity.Common,
                    TargetType.AnyEnemy));

            content.RegisterPlaceholderCard<WineFoxCardPool>("wf_placeholder_skill",
                new(
                    1,
                    CardType.Skill,
                    CardRarity.Common));

            content.RegisterPlaceholderCard<WineFoxCardPool>("wf_placeholder_power",
                new(
                    1,
                    CardType.Power,
                    CardRarity.Uncommon));

            content.RegisterPlaceholderCard<WineFoxTokenCardPool>("wf_placeholder_token",
                new(
                    0));

            content.RegisterPlaceholderRelic<WineFoxRelicPool>("wf_placeholder_relic_common",
                new(RelicRarity.Common));

            content.RegisterPlaceholderRelic<WineFoxRelicPool>("wf_placeholder_relic_rare",
                new(RelicRarity.Rare));

            content.RegisterPlaceholderPotion<WineFoxPotionPool>("wf_placeholder_potion",
                new(
                    PotionRarity.Common));
        }
    }
}

using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2RitsuLib.Models.Capabilities;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     为法术卡牌贡献一张属性提示，排布参考 Noita 的法术详情框。
    /// </summary>
    public sealed class SpellAttributeTipCapability : CardCapability, ICardHoverTipContributor
    {
        public const string TipId = "sts2_winefox_spell_attribute_tip";

        private const char FullWidthSpace = '\u3000';

        private const int IconSize = 24;

        public IEnumerable<IHoverTip> GetHoverTips(CardModel card)
        {
            if (card is not MagicWineFoxSpellCard spell)
                return [];

            var tip = new HoverTip(
                new("cards", spell.Id.Entry + ".title"),
                BuildText(spell),
                LoadIcon(spell))
            {
                Id = TipId,
            };

            return [tip];
        }

        private static string BuildText(MagicWineFoxSpellCard spell)
        {
            var traits = spell.Traits;

            var rows = new List<(string Icon, string Label, string Value)>
            {
                (Const.Paths.SpellAttrTypeIcon, Text("TYPE"), TypeText(spell)),
                (Const.Paths.SpellAttrCostIcon, Text("COST"),
                    spell.EnergyCost.Canonical.ToString()),
            };

            if (spell.PreviewDamage > 0m)
            {
                var damage = spell.PreviewDamage * Math.Max(1, traits.HitCount);
                rows.Add((Const.Paths.SpellAttrDamageIcon, Text("DAMAGE"), damage.ToString("0")));
            }

            rows.Add((Const.Paths.SpellAttrHitsIcon, Text("HITS"), traits.HitCount.ToString()));
            rows.Add((Const.Paths.SpellAttrTargetIcon, Text("TARGET"), TargetText(traits)));

            if (traits.BounceTargets > 0)
                rows.Add((Const.Paths.SpellAttrBounceIcon, Text("BOUNCE"), traits.BounceTargets.ToString()));

            rows.Add((Const.Paths.SpellAttrUsesIcon, Text("USES"),
                spell.IsUnlimitedUses ? "∞" : spell.UsesRemaining.ToString()));

            if (!traits.Loadable)
                rows.Add((string.Empty, Text("LOADABLE"), Text("NO")));

            var width = rows.Max(row => row.Label.Length);

            return string.Join("\n", rows.Select(row => Format(row, width)));
        }

        private static string Format((string Icon, string Label, string Value) row, int width)
        {
            var pad = new string(FullWidthSpace, Math.Max(1, width - row.Label.Length + 1));
            var prefix = string.IsNullOrEmpty(row.Icon)
                ? string.Empty
                : $"[img={IconSize}x{IconSize}]{row.Icon}[/img] ";

            return prefix + row.Label + pad + row.Value;
        }

        private static Texture2D? LoadIcon(MagicWineFoxSpellCard spell)
        {
            return string.IsNullOrEmpty(spell.SpellIconPath)
                ? null
                : ResourceLoader.Load<Texture2D>(spell.SpellIconPath);
        }

        private static string TypeText(MagicWineFoxSpellCard spell)
        {
            if (spell is IMagicWineFoxSpellModifierCard)
                return Text("MODIFIER");

            var traits = spell.Traits;

            if (traits.AttackSpell)
                return Text("ATTACK");

            return traits.SupportSpell ? Text("SUPPORT") : Text("DEFENSE");
        }

        private static string TargetText(SpellTraits traits)
        {
            if (traits.TargetSelf)
                return Text("SELF");

            if (traits.AreaOfEffect)
                return Text("ALL");

            return traits.RandomTarget ? Text("RANDOM") : Text("SINGLE");
        }

        private static string Text(string suffix)
        {
            return new LocString("powers", $"STS2_WINE_FOX_SPELL_ATTR_{suffix}").GetFormattedText();
        }
    }
}

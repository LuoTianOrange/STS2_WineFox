using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token;

public class Shield() : WineFoxCard(
    0, CardType.Skill, CardRarity.Token, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("Dex", 10m), new("Block", 10m)];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardShield);

    protected override bool IsPlayable =>
        !Owner.Creature.Powers.OfType<ShieldCooldownPower>().Any();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var creature = Owner.Creature;
        var target = play.Target ?? creature;

        if (!play.IsAutoPlay)
        {
            var cooldown = IsUpgraded ? 1m : 2m;
            await PowerCmd.Apply<ShieldCooldownPower>(creature, cooldown, creature, this);
        }

        await PowerCmd.Apply<ShieldDexPower>(target, DynamicVars["Dex"].BaseValue, creature, this);
        if (IsUpgraded)
            await CreatureCmd.GainBlock(target, DynamicVars["Block"].BaseValue, ValueProp.Move, null);
    }

    protected override void OnUpgrade()
    {

    }
}

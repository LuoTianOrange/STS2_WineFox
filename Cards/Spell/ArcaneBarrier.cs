using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ArcaneBarrier() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(8m, ValueProp.Move)
        ];

        public override bool GainsBlock => true;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardArcaneBarrier);

        public override string SpellIconPath => Const.Paths.SpellIconArcaneBarrier;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner?.Creature;
            if (owner == null)
                return;

            await CreatureCmd.GainBlock(
                owner,
                context.DamageWithModifiers(DynamicVars.Block.BaseValue),
                ValueProp.Unpowered,
                null);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m);
        }
    }
}

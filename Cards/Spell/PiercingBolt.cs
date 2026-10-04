using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class PiercingBolt() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("DamageReduction", 20m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        public override string SpellIconPath => Const.Paths.SpellIconPiercingBolt;
        
        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            var reduction = DynamicVars["DamageReduction"].BaseValue;
            modifiers.MarkTargetsAllEnemies();
            modifiers.MultiplyDamage(1m - reduction / 100m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        { }
    }
}

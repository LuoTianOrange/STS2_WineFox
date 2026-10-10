using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     法术余烬：法杖中的攻击型法术被释放时，给所有敌人叠加灼烧。
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class SpellAshes() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
        
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Burn", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSpellAshes);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await PowerCmd.Apply<SpellAshesPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars["Burn"].BaseValue,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Burn"].UpgradeValueBy(1m);
        }
    }
}

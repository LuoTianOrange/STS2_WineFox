using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     奥术屏障 —— 1 费防御法术（白/防御），获得 6 点格挡（升级 9）。
    ///     <para>
    ///         仅通过不带「装填」关键字来表达。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(MagicWineFox), 1)]
    public class ArcaneBarrier() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(6m, ValueProp.Move)
        ];

        public override bool GainsBlock => true;

        /// <summary>防御类法术不参与装填。</summary>
        public override bool IsLoadable => false;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicWineFoxDefend);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m);
        }
    }
}

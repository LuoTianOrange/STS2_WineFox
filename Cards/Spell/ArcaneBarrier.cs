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
    /// <summary>
    ///     奥术屏障 —— 1 费防御法术（白/防御），获得 6 点格挡（升级 9）。
    ///     <para>
    ///         可装填：装填后带「装填」+「释放」关键字，在回合结束的释放阶段结算格挡。
    ///         因此它和其他法术一样有分支：直接打出立即获得格挡，装填则延后到释放时生效。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(MagicWineFox), 1)]
    public class ArcaneBarrier() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Basic, TargetType.Self), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(6m, ValueProp.Move)
        ];

        public override bool GainsBlock => true;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicWineFoxDefend);

        public override string SpellIconPath => Const.Paths.SpellIconArcaneBarrier;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        /// <summary>释放阶段结算：获得格挡。防御法术无目标，直接作用于自身。</summary>
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

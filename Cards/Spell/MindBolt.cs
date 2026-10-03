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
    ///     灵能弹 —— 0 费攻击法术（无色/起始弹药）。
    ///     <para>
    ///         0 费让序列不必占用能量预算，是「不启动慢」的第四道保险：
    ///         抽到它就能免费往法杖里塞一发弹药。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(MagicWineFox), 1)]
    public class MindBolt() : MagicWineFoxSpellCard(
        0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(4m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Target == null) return;

            await DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                .FromCard(context.SourceCard, null)
                .Targeting(context.Target)
                .Execute(context.ChoiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}
